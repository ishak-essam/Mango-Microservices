using Mango.Service.AuthAPI.Models;
using Mango.Service.AuthAPI.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Service.AuthAPI.Service
{
    public class JwtTokenGeneratorService : IJwtTokenGenerator
    {

        private readonly JwtOptions _jwtOptions;

        public JwtTokenGeneratorService( IConfiguration configuration,  IOptions< JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            configuration = configuration;
        }
        public string GenerateToken ( ApplicationUser applicationUser, IEnumerable<string> roles )
        {
            var tokenhandler=new JwtSecurityTokenHandler();
            var key= Encoding.UTF8.GetBytes( _jwtOptions.Secret );
            //var claimsList=new []{
            //new Claim(JwtRegisteredClaimNames.Email,applicationUser.Email),
            //new Claim(JwtRegisteredClaimNames.Sub,applicationUser.Id),
            //new Claim(ClaimTypes.Role,roles.FirstOrDefault()),
            //new Claim(JwtRegisteredClaimNames.Name,applicationUser.UserName.ToString()),
            //};
            var claimsList=new List<Claim>{
            new Claim(JwtRegisteredClaimNames.Email,applicationUser.Email),
            new Claim(JwtRegisteredClaimNames.Sub,applicationUser.Id),
            new Claim(ClaimTypes.Role,roles.FirstOrDefault()),
            new Claim(JwtRegisteredClaimNames.Name,applicationUser.UserName.ToString()),
            };
            claimsList.AddRange(roles.Select(ele=>new Claim(ClaimTypes.Role,ele)));
            var tokenDescrptor=new SecurityTokenDescriptor{
                Audience=_jwtOptions.Audience,
                Issuer=_jwtOptions.Issuer,
                Subject= new ClaimsIdentity(claimsList),
                Expires =DateTime.Now.AddDays(7),
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token =tokenhandler.CreateToken(tokenDescrptor);
            return tokenhandler.WriteToken (token);
        }
    }
}




//var tokenhandler=new JwtSecurityTokenHandler();
//var key= Encoding.UTF8.GetBytes( _jwtOptions.Secret );
//var claimsList=new []{
//            new Claim(JwtRegisteredClaimNames.Email,applicationUser.Email),
//            new Claim(JwtRegisteredClaimNames.Sub,applicationUser.Id),
//            new Claim(ClaimTypes.Role,roles.FirstOrDefault()),
//            new Claim(JwtRegisteredClaimNames.Name,applicationUser.UserName.ToString()),
//            };
////claimsList.AddRange(roles.Select(ele=>new Claim(ClaimTypes.Role,ele)));
//var tokenDescrptor=new SecurityTokenDescriptor
//{
//    Audience=_jwtOptions.Audience,
//    Issuer=_jwtOptions.Issuer,
//    Claims= claimsList,
//    Expires =DateTime.Now.AddDays(7),
//    SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256)
//};
//var token =tokenhandler.CreateToken(tokenDescrptor);
//return tokenhandler.WriteToken (token);