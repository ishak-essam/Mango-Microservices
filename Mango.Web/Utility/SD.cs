namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string CartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }
        public static string RoleAdmin ="Admin";
        public static string RoleCustomer ="Customer";
        public static string TokenCookie ="JWTToken";
        public enum ApiType
        {
            Get,Post,Put,Delete
        }
        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";

        public enum ContentType
        {
            Json,
            MultipartFormData,
        }
    }
}
