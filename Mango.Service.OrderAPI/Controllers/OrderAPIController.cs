using AutoMapper;
using Azure;
using Mango.MessageBus;
using Mango.Service.OrderAPI.Models;
using Mango.Service.OrderAPI.Models.Dto;
using Mango.Service.OrderAPI.Services.IServices;
using Mango.Service.OrderAPI.Utility;
using Mango.Services.OrderAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Mango.Service.OrderAPI.Controllers
{
    [Route ("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDTO responseDTO;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        private IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderAPIController ( AppDbContext db,   IMapper mapper,
         IProductService productService, IMessageBus messageBus ,IConfiguration configuration)
        {
            _messageBus = messageBus;
            _configuration = configuration;
            _mapper = mapper;
            responseDTO=new ResponseDTO();
            _db=db;
            _productService = productService;
        }

        [Authorize]
        [HttpGet ("GetOrders")]
        public ResponseDTO? Get (string? userId="" )
        {
            try {
                IEnumerable<OrderHeader> orderHeaders;
                if ( User.IsInRole(SD.RoleAdmin) ) {
                    orderHeaders = _db.orderHeaders.Include (ele => ele.orderDetails).OrderByDescending (olo=>olo.OrderHeaderId).ToList();
                }
                else
                {
                    orderHeaders = _db.orderHeaders.Include (ele => ele.orderDetails).Where(ele=>ele.UserId==userId).OrderByDescending (olo => olo.OrderHeaderId).ToList ();
                }
                    responseDTO.Result =_mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);
            
            }
            catch(Exception ex) {
                responseDTO.IsSuccessful = false;
                responseDTO.Message = ex.Message.ToString ();
            }
            return responseDTO;
        }

        [Authorize]
        [HttpGet ("GetOrder/{id:int}")]
        public ResponseDTO? Get ( int id )
        {
            try {
                OrderHeader orderHeader=_db.orderHeaders.Include(ele=>ele.orderDetails).First(u=>u.OrderHeaderId==id);
                responseDTO.Result = _mapper.Map<OrderHeaderDto> (orderHeader);
                    }
            catch ( Exception ex )
            {
                responseDTO.IsSuccessful = false;
                responseDTO.Message = ex.Message.ToString ();
            }
            return responseDTO;
        }


        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder ( [FromBody] CartDto cartDto )
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.orderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>> (cartDto.CartDetails);
                orderHeaderDto.OrderTotal = Math.Round (orderHeaderDto.OrderTotal, 2);
                OrderHeader orderCreated = _db.orderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync ();
                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                responseDTO.Result = orderHeaderDto;
            }
            catch (Exception ex){
                responseDTO.Message = ex.Message.ToString();
                responseDTO.IsSuccessful =false;
            }
            return responseDTO;
        }

        [Authorize] 
        [HttpPost ("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession ( [FromBody] StripeRequestDto stripeRequestDto )
        {
            try
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl=stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };
                var DiscountObj=new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions{ 
                    Coupon=stripeRequestDto.OrderHeader.CouponCode
                    }
                };
                foreach (var item in stripeRequestDto.OrderHeader.orderDetails)
                {
                    var sessionLineItem=new SessionLineItemOptions
                    {
                        PriceData=new  SessionLineItemPriceDataOptions
                        {
                            UnitAmount=(long)(item.Price*100), //20.99 to 2099
                            Currency="usd",
                            ProductData=new SessionLineItemPriceDataProductDataOptions
                            {
                                Name=item.Product.Name
                            }
                        },
                        Quantity=item.Count
                    };


                    options.LineItems.Add (sessionLineItem);
                }
                if ( stripeRequestDto.OrderHeader.Discount > 0 ) {
                    options.Discounts = DiscountObj ;
                }
                var service = new SessionService();
                Session session=service.Create (options);
                stripeRequestDto.StripeSessionUrl =session.Url;
                OrderHeader orderHeader=_db.orderHeaders.First(i=>i.OrderHeaderId ==stripeRequestDto.OrderHeader.OrderHeaderId );
                orderHeader.StripeSessionId = session.Id;
                _db.SaveChanges ();
                responseDTO.Result = stripeRequestDto;
            }
            catch ( Exception ex )
            {
                responseDTO.IsSuccessful = false;
                responseDTO.Message = ex.Message.ToString ();
            }


            return responseDTO;
        }

        [Authorize]
        [HttpPost ("ValidateStripeSession")]
        public async Task<ResponseDTO> ValidateStripeSession ( [FromBody] int  OrderHeaderId  )
        {
            try
            {
                OrderHeader orderHeader =_db.orderHeaders.First(ele=>ele.OrderHeaderId==OrderHeaderId);
                var service =new SessionService();
                Session  session=service.Get(orderHeader.StripeSessionId);
                var paymentService =new PaymentIntentService();
                PaymentIntent paymentIntent =paymentService.Get(session.PaymentIntentId);
                if ( paymentIntent.Status == "succeeded" ) {
                    //then payment was succeeded
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved ;
                    _db.SaveChanges ();
                    RewardDTO rewardDTO=new()
                    {
                        OrderId=orderHeader.OrderHeaderId,
                        RewardActivity=Convert.ToInt32(orderHeader.OrderTotal)
                    ,
                        UserId=orderHeader.UserId
                    };
                    string topicName=_configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage (rewardDTO, topicName);
                    responseDTO.Result= _mapper.Map<OrderHeaderDto>(orderHeader);
                }
            }

            catch ( Exception ex )
            {
                responseDTO.IsSuccessful = false;
                responseDTO.Message = ex.Message.ToString ();
            }
            return responseDTO;
        }


        [Authorize]
        [HttpPost ("UpdateOrderStatus/{orderid:int}")]
        public async Task<ResponseDTO> UpdateOrderStatus ( int orderid ,[FromBody]  string status ) {
            try
            {
                OrderHeader orderHeader = _db.orderHeaders.First(u=>u.OrderHeaderId==orderid);
                if (orderHeader !=null)
                {
                    if ( status == SD.Status_Cancelled ) {
                        //refun
                        var options=new RefundCreateOptions{
                            Reason=RefundReasons.RequestedByCustomer,
                            PaymentIntent=orderHeader.PaymentIntentId
                        };
                        var service =new RefundService();
                        Refund refund=service.Create(options);
                    }
                        orderHeader.Status = status;
                    _db.SaveChanges ();
                }
            }
            catch ( Exception ex )
            {
                responseDTO.IsSuccessful = false;
                responseDTO.Message = ex.Message.ToString();

            }
            return responseDTO;
        }

    }
}
