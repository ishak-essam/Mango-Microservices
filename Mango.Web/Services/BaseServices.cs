using Mango.Web.Models;
using Mango.Web.Services.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utility.SD;


namespace Mango.Web.Services
{
    public class BaseServices : IBaseServices
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseServices(IHttpClientFactory httpClientFactory ,ITokenProvider tokenProvider )
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDTO?> SendAsync ( RequestDTO requestDTO ,bool withBearer=true )
        {
            try {
                HttpClient client=_httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage message =new();
                //token 

                if ( requestDTO.contentType == ContentType.MultipartFormData )
                {
					message.Headers.Add ("Accept", "*/*");
                }
                else {
                    message.Headers.Add ("Accept", "application/json");
				}


				if ( withBearer )
                {

                    var token=_tokenProvider.GetToken();
                    message.Headers.Add ("Authorization", $"Bearer {token}");
                }
                message.RequestUri = new Uri (requestDTO.Url);

				if ( requestDTO.contentType == ContentType.MultipartFormData )
				{
                    var content  = new MultipartFormDataContent();
                    foreach (var prop in requestDTO.Data.GetType().GetProperties()) { 
                    var value=prop.GetValue (requestDTO.Data);
                        if ( value is FormFile )
                        {
                            var file =(FormFile)value;
                            if ( file != null )
                            {
                                content.Add (new StreamContent (file.OpenReadStream ()), prop.Name, file.FileName);
                            }
                        }

                        else {
                            content.Add (new StringContent (value == null ? "" : value.ToString ()), prop.Name);
                        }
                    }
                    message.Content = content;
				}
				else
				{
					if ( requestDTO.Data != null )
					{
					message.Content = new StringContent (JsonConvert.SerializeObject(requestDTO.Data),Encoding.UTF8, "application/json");
					}
				}
				
                    HttpResponseMessage? httpResponseMessage=null;
                    switch ( requestDTO.ApiType )
                    {
                        case ApiType.Post:
                            message.Method = HttpMethod.Post;
                            break;
                        case ApiType.Put:
                            message.Method = HttpMethod.Put;
                            break;
                        case ApiType.Delete:
                            message.Method = HttpMethod.Delete;
                            break;
                        default:
                            message.Method = HttpMethod.Get;
                            break;
                    }
                    httpResponseMessage = await client.SendAsync (message);
                    switch ( httpResponseMessage.StatusCode ) {
                        case HttpStatusCode.NotFound:
                            return new () { IsSuccessful = false, Message = "Not Found" };
                        case HttpStatusCode.Unauthorized:
                            return new () { IsSuccessful = false, Message = "Un authorized" };

                        case HttpStatusCode.Forbidden:
                            return new () { IsSuccessful = false, Message = "Forbidden" };

                        case HttpStatusCode.InternalServerError:
                            return new () { IsSuccessful = false, Message = "Internal Server Error" };
                        default:
                            var apicontent=await httpResponseMessage.Content.ReadAsStringAsync();
                            var ApiResponse=JsonConvert.DeserializeObject<ResponseDTO>(apicontent);
                            return ApiResponse;
                }
            }
			catch ( Exception ex )
			{
				var dto=new ResponseDTO
				{
					IsSuccessful = false,
					Message = ex.Message.ToString(),
				};
				return dto;
			}

		}
    }
}
