using AutoMapper;
using Mango.Service.ProductAPI.Models;
using Mango.Service.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Service.ProductAPI.Controllers
{
    [Route ("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private  AppDbContext _appDbContext;
        private  IMapper _mapper;
        private  ResponseDTO responseDTO;
        public ProductController ( AppDbContext appDbContext, IMapper mapper )
        {

            _appDbContext = appDbContext;
            _mapper = mapper;
            responseDTO = new ResponseDTO ();
        }
        [HttpGet]
        public ResponseDTO GetAll ( )
        {
            try
            {
                IEnumerable<Product> couponsList=_appDbContext.Products.ToList();
                responseDTO.Result = _mapper.Map<IEnumerable<Product>> (couponsList);
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
                Product couponsList=_appDbContext.Products.First(ele=>ele.ProductId==id);

                responseDTO.Result = _mapper.Map<Product> (couponsList);

            }
            catch ( Exception ex )
            {
                responseDTO.Message = ex.Message;
                responseDTO.IsSuccessful = false;
            }
            return responseDTO;
        }

        [HttpGet]
        [Route ("GetByName/{Name}")]
        public ResponseDTO GetByCode ( string Name )
        {
            try
            {
                Product couponsList=_appDbContext.Products.First(ele=>ele.Name.ToLower()==Name.ToLower());
                responseDTO.Result = _mapper.Map<Product> (couponsList);

            }
            catch ( Exception ex )
            {
                responseDTO.Message = ex.Message;
                responseDTO.IsSuccessful = false;
            }
            return responseDTO;

        }


        [HttpPost]
        public ResponseDTO Post (  ProductDTO productDTO )
        {
            try
            {
                Product product=_mapper.Map<Product>(productDTO);
                _appDbContext.Add (product);
                _appDbContext.SaveChanges ();
				if ( productDTO.Image != null )
                {
                    string fileName= product.ProductId+ Path.GetExtension(productDTO.Image.FileName);
                    string filePath=@"wwwroot/ProductImages/"+fileName;
                    var filePathDirectory=Path.Combine(Directory.GetCurrentDirectory(),filePath);
                    using ( var fileStream = new FileStream (filePathDirectory, FileMode.Create) )
                    {
                        productDTO.Image.CopyTo (fileStream);
                    }
                    var baseUrl=$"{HttpContext.Request.Scheme }://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else {
                    product.ImageUrl = "https://placehold.co/600*400";
                }
                _appDbContext.Products.Update (product);
                _appDbContext.SaveChanges ();
                responseDTO.Result = _mapper.Map<ProductDTO> (product); ;

            }
            catch ( Exception ex )
            {
                responseDTO.Message = ex.Message;
                responseDTO.IsSuccessful = false;
            }
            return responseDTO;

        }



        [HttpPut]
        public ResponseDTO Put ( ProductDTO productDTO )
        {
            try
            {
                Product product=_mapper.Map<Product>(productDTO);
                if ( productDTO.Image != null )
                {
                    string fileName= product.ProductId+ Path.GetExtension(productDTO.Image.FileName);
                    string filePath=@"wwwroot/ProductImages/"+fileName;
                    var filePathDirectory=Path.Combine(Directory.GetCurrentDirectory(),filePath);
                    using ( var fileStream = new FileStream (filePathDirectory, FileMode.Create) )
                    {
                        productDTO.Image.CopyTo (fileStream);
                    }
                    var baseUrl=$"{HttpContext.Request.Scheme }://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600*400";
                }
                if ( !string.IsNullOrEmpty (product.ImageLocalPath) )
                {
                    var oldFileDirectory=Path.Combine(Directory.GetCurrentDirectory(),product.ImageLocalPath);
                    FileInfo  file=new FileInfo(oldFileDirectory);
                    if ( file.Exists )
                    {
                        file.Delete ();
                    }
                }
                _appDbContext.Update (product);
                _appDbContext.SaveChanges ();

                responseDTO.Result = _mapper.Map<ProductDTO> (product); ;

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
        public ResponseDTO Delete ( int id )
        {
            try
            {
                Product product=_appDbContext.Products.First(ele=>ele.ProductId==id);
                if ( !string.IsNullOrEmpty (product.ImageLocalPath) ) {
                    var oldFileDirectory=Path.Combine(Directory.GetCurrentDirectory(),product.ImageLocalPath);
                    FileInfo  file=new FileInfo(oldFileDirectory);
                    if ( file.Exists ) { 
                        file.Delete ();
                    }
                }
                _appDbContext.Remove (product);
                _appDbContext.SaveChanges ();
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
