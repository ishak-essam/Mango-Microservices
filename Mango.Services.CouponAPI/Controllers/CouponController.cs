using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Service.CouponAPI.Controllers
{

    [Route ("api/coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private  AppDbContext _appDbContext;
        private  IMapper _mapper;
        private  ResponseDTO responseDTO;

        public CouponController ( AppDbContext appDbContext, IMapper mapper )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            responseDTO = new ResponseDTO ();
        }
        [HttpGet]
        //[Authorize (Roles = "ADMIN")]

        public ResponseDTO GetAll ( )
        {
            try
            {
                IEnumerable<Coupon> couponsList=_appDbContext.Coupons.ToList();
                responseDTO.Result = _mapper.Map<IEnumerable<Coupon>> (couponsList);
            }
            catch ( Exception ex )
            {
                responseDTO.Message = ex.Message;
                responseDTO.IsSuccessful = false;
            }
            return responseDTO;
        }
        [HttpGet]
        [Route ("{id:int}")]
        public ResponseDTO GetId ( int id )
        {
            try
            {
                Coupon couponsList=_appDbContext.Coupons.First(ele=>ele.CouponId==id);

                responseDTO.Result = _mapper.Map<Coupon> (couponsList);

            }
            catch ( Exception ex )
            {
                responseDTO.Message = ex.Message;
                responseDTO.IsSuccessful = false;
            }
            return responseDTO;

        }


        [HttpGet]
        [Route ("GetByCode/{code}")]
        public ResponseDTO GetByCode ( string code )
        {
            try
            {
                Coupon couponsList=_appDbContext.Coupons.First(ele=>ele.CouponCode.ToLower()==code.ToLower());

                responseDTO.Result = _mapper.Map<Coupon> (couponsList);

            }
            catch ( Exception ex )
            {
                responseDTO.Message = ex.Message;
                responseDTO.IsSuccessful = false;
            }
            return responseDTO;

        }



        [HttpPost]
        [Authorize (Roles = "ADMIN")]
        public ResponseDTO Post ( [FromBody] CouponDTO couponDTO )
        {
            try
            {
                Coupon coupon=_mapper.Map<Coupon>(couponDTO);
                _appDbContext.Add (coupon);
                _appDbContext.SaveChanges ();

               Stripe.StripeConfiguration.ApiKey = "sk_test_51OTOjfCaFpg3jez0QCI9TwQGwHrR9TPr3hRc1DWWZjcyRPKimekPDwNVpFnt9G0j6vxzmGW93se4n4s8mQXhkmX900DV26dgmi";
                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(couponDTO.DiscountAmount*100),
                    Name = couponDTO.CouponCode,
                    Currency="usd",
                    Id=couponDTO.CouponCode,
                };
                var service = new Stripe.CouponService();
                service.Create (options);
                responseDTO.Result = _mapper.Map<CouponDTO> (coupon); ;

            }
            catch ( Exception ex )
            {
                responseDTO.Message = ex.Message;
                responseDTO.IsSuccessful = false;
            }
            return responseDTO;

        }


        [HttpPut]
        [Authorize (Roles = "ADMIN")]
        public ResponseDTO Put ( [FromBody] CouponDTO couponDTO )
        {
            try
            {
                Coupon coupon=_mapper.Map<Coupon>(couponDTO);
                _appDbContext.Update (coupon);
                _appDbContext.SaveChanges ();
                responseDTO.Result = _mapper.Map<CouponDTO> (coupon); ;

            }
            catch ( Exception ex )
            {
                responseDTO.Message = ex.Message;
                responseDTO.IsSuccessful = false;
            }
            return responseDTO;

        }

        [HttpDelete]
        [Route ("{id:int}")]
        [Authorize (Roles = "ADMIN")]
        public ResponseDTO Delete ( int id )
        {
            try
            {
                Coupon coupon=_appDbContext.Coupons.First(ele=>ele.CouponId==id);
                _appDbContext.Remove (coupon);
                _appDbContext.SaveChanges ();
                responseDTO.Result = _mapper.Map<CouponDTO> (coupon); ;
            }
            catch ( Exception ex )
            {
                responseDTO.Message = ex.Message;
                responseDTO.IsSuccessful = false;
            }
            return responseDTO;

        }



    }
}
