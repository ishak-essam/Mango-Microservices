using static Mango.Web.Utility.SD;

namespace Mango.Web.Models
{
    public class RequestDTO
    {
        public ApiType ApiType { get; set; } = ApiType.Get;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
        public ContentType contentType { get; set; } = ContentType.Json;
	}
}
