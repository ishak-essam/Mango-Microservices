using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;

namespace CourseUdemy.Extensions
{
    public static class IdentitiyServices
    {
        public static IServiceCollection AddIdetityServices ( this IServiceCollection services,
            IConfiguration config )
        {
            var settingsSection = config.GetSection("ApiSettings");
            var secret = settingsSection.GetValue<string>("Secret");
            var issuer = settingsSection.GetValue<string>("Issuer");
            var audience = settingsSection.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer (options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey (key),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateAudience = true
                };
            });
            return services;
        }
    }
}
