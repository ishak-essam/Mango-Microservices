using AutoMapper;
using Mango.MessageBus;
using Mango.Service.CartAPI.Models;
using Mango.Service.CartAPI.Models.Dto;
using Mango.Service.CartAPI.Services.IServices;
using Mango.Services.CartAPI.Data;
using Mango.Services.CartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mango.Service.CartAPI.Controllers
{
    [Route ("api/cart")]
    [ApiController]

    public class CartController : ControllerBase
    {
        private  AppDbContext _db;
        private  IMapper _mapper;
        private  IProductService _productService;
        private  IMessageBus _IMessageBus;
        private readonly ICouponService _couponService;
        private readonly IConfiguration _configuration;
        private  ResponseDTO _responseDTO;
        public CartController ( AppDbContext appDbContext, IMapper mapper , IMessageBus IMessageBus, IProductService productService,ICouponService couponService,IConfiguration configuration)
        {
            _IMessageBus = IMessageBus;
            _db = appDbContext;
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
           _configuration = configuration;
            _responseDTO = new ResponseDTO ();
        }
        [HttpPost ("GetCart/{Userid}")]
        public async Task<ResponseDTO> GetCart ( string Userid ) {
            try
            {
                var ex=_db.CartHeader.First(x=>x.UserId==Userid);
                CartDto cart=new(){
                    CartHeader =_mapper.Map<CartHeaderDTO>(_db.CartHeader.First(x=>x.UserId==Userid)),
            };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>> (_db.CartDetail.Where(ele=>ele.cartHeadersId==cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDTO> productDTOs=await _productService.GetProducts( );
                foreach (var item in cart.CartDetails)
                {
                    item.Product=productDTOs.FirstOrDefault(x=>x.ProductId==item.ProductId);
                    cart.CartHeader.CartTotal += ( item.Count * item.Product.Price );
                }
                //Apply Coupon  if any
                if ( !string.IsNullOrEmpty (cart.CartHeader.CouponCode) )
                {
                    CouponDTO couponDTO=await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if (couponDTO !=null && cart.CartHeader.CartTotal> couponDTO.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= couponDTO.DiscountAmount;
                        cart.CartHeader.Discount  = couponDTO.DiscountAmount;
                    }
                }
                _responseDTO.Result = cart;
            }
            catch(Exception ex )
            {
                _responseDTO.IsSuccessful =false;
                _responseDTO.Message = ex.Message.ToString ();
            }
            return _responseDTO;
        }

        [HttpPost ("ApplyCoupon")]

        public async Task<object> ApplyCoupon ([FromBody] CartDto cartDto ) {
            try {
                var cartFromDb=await _db.CartHeader.FirstAsync(u=>u.UserId==cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeader.Update (cartFromDb);
               await _db.SaveChangesAsync();
                _responseDTO.Result = true;
            }
            catch (Exception ex) {

                _responseDTO.Message = ex.Message.ToString ();
                _responseDTO.IsSuccessful = false;
            }
            return _responseDTO;
        }


        [HttpPost ("RemoveCoupon")]

        public async Task<object> RemoveCoupon ( [FromBody] CartDto cartDto )
        {
            try
            {
                var cartFromDb=await _db.CartHeader.FirstAsync(u=>u.UserId==cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                _db.CartHeader.Update (cartFromDb);
                await _db.SaveChangesAsync ();
                _responseDTO.Result = true;
            }
            catch ( Exception ex )
            {

                _responseDTO.Message = ex.Message.ToString ();
                _responseDTO.IsSuccessful = false;
            }
            return _responseDTO;
        }
 
        [HttpPost ("UpsertCart")]
        public async Task<ResponseDTO> CartUpsert ( CartDto cartDto )
        {
            try
            {
                var cartHeaderFromDb=await _db.CartHeader.AsNoTracking().FirstOrDefaultAsync(x=>x.UserId==cartDto.CartHeader.UserId);
                if ( cartHeaderFromDb == null )
                {
                    //create header and details
                    CartHeader cartHeader=_mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.Add (cartHeader);
                    _db.SaveChanges ();
                    cartDto.CartDetails.First ().cartHeadersId = cartHeader.CartHeaderId;
                    _db.CartDetail.Add (_mapper.Map<CartDetails> (cartDto.CartDetails.First ()));
                    await _db.SaveChangesAsync ();
                }
                else
                {
                    //if header not null 
                    //check if details has same product
                    var cartDetailFromDb=await _db.CartDetail.AsNoTracking().FirstOrDefaultAsync(x=>x.ProductId==cartDto.CartDetails.First().ProductId&&x.cartHeadersId==cartHeaderFromDb.CartHeaderId);
                    if ( cartDetailFromDb == null )
                    { //create cart detail
                        //create header and details
                        cartDto.CartDetails.First ().cartHeadersId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetail.Add (_mapper.Map<CartDetails> (cartDto.CartDetails.First ()));
                        await _db.SaveChangesAsync ();
                  
                    }
                    else
                    {
                        //update count in cart detail
                        cartDto.CartDetails.First ().Count += cartDetailFromDb.Count;
                        cartDto.CartDetails.First ().cartHeadersId = cartDetailFromDb.cartHeadersId;
                        cartDto.CartDetails.First ().CartDetailId = cartDetailFromDb.CartDetailId;
                        _db.CartDetail.Update (_mapper.Map<CartDetails> (cartDto.CartDetails.First ()));
                        await _db.SaveChangesAsync ();
                    }
                    _responseDTO.Result = cartDto;
                }
            }
            catch ( Exception ex )
            {
                _responseDTO.Message = ex.Message.ToString();
                _responseDTO.IsSuccessful = true;

            }
            return _responseDTO;
        }

        [HttpPost ("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart ([FromBody ]int cartDeailId )
        {
            try
            {
                CartDetails cartDetail=_db.CartDetail.First(ele=>ele.CartDetailId==cartDeailId);
                int TotalCountofCartitem=_db.CartDetail.Where(ele=>ele.cartHeadersId==cartDetail.cartHeadersId).Count();

                _db.CartDetail.Remove (cartDetail);
                if ( TotalCountofCartitem == 1 ) {
                    var CartHeaderToRemove=_db.CartHeader.FirstOrDefault(ele=>ele.CartHeaderId==cartDetail.cartHeadersId);
                    _db.CartHeader.Remove (CartHeaderToRemove);
                }
                await _db.SaveChangesAsync ();
                _responseDTO.Result = true;
            }
            catch ( Exception ex )
            {
                _responseDTO.Message = ex.Message.ToString ();
                _responseDTO.IsSuccessful = true;

            }
            return _responseDTO;
        }

        [HttpPost ("EmailCartRequest")]

        public async Task<object> EmailCartRequest ( [FromBody] CartDto cartDto )
        {
            var exe=_configuration.GetValue<string> ("TopicsAndQueueNames:EmailShoppingCartQueue");
            try
            {
                await _IMessageBus.PublishMessage (cartDto, _configuration.GetValue<string> ("TopicsAndQueueNames:EmailShoppingCartQueue"));
                _responseDTO.Result = true;
            }
            catch ( Exception ex )
            {

                _responseDTO.Message = ex.Message.ToString ();
                _responseDTO.IsSuccessful = false;
            }
            return _responseDTO;
        }


    }
}
