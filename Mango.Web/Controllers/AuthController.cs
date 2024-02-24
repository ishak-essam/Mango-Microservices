using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService,ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }
        [HttpGet]
        public IActionResult Login ( )
        {
            LoginRequestDTO loginRequestDTO = new();
            return View (loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login ( LoginRequestDTO loginRequestDTO )
        {
            ResponseDTO responseDTO =await _authService.LoginAsync(loginRequestDTO);
            if ( responseDTO != null && responseDTO.IsSuccessful )
            {
                LoginResponseDTO loginResponseDTO=JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(responseDTO.Result));
                await SignIn (loginResponseDTO);
                _tokenProvider.SetToken (loginResponseDTO.Token);
                return RedirectToAction ("Index", "Home");
            }
            else
            {
                TempData [ "error" ] = responseDTO.Message;
                return View (loginRequestDTO);
            }
        }



        [HttpGet]
        public IActionResult Register ( )
        {

            var roleList=new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin },
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer },
            };
            ViewBag.RoleList = roleList;
            return View ();
        }

        [HttpPost]
        public async Task<IActionResult> Register ( RegistrationRequestDTO registrationRequestDTO)
        {
            ResponseDTO responseDTO =await _authService.RegisterAsync(registrationRequestDTO);
            ResponseDTO assignRole;
            if ( responseDTO != null && responseDTO.IsSuccessful )
            {
                if ( string.IsNullOrEmpty (registrationRequestDTO.Role) )
                {
                    registrationRequestDTO.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignAsync (registrationRequestDTO);
                if ( assignRole != null && assignRole.IsSuccessful )
                {
                    TempData [ "success" ] = "Registerion Successfully";
                    return RedirectToAction (nameof (Login));
                }

            }
            else {
                    TempData [ "error" ] = responseDTO.Message;
            }
            var roleList=new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin },
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer },
            };
            ViewBag.RoleList = roleList;
            return View (registrationRequestDTO);
        }


        public async Task<IActionResult> Logout ( )
        {
            await HttpContext.SignOutAsync ();
            _tokenProvider.ClearToken ();
            return RedirectToAction ("Index","Home");
        }
        private async Task SignIn (LoginResponseDTO login )
        {
            var handler=new JwtSecurityTokenHandler();
            var jwt= handler.ReadJwtToken(login.Token);
            var identity=new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim (new Claim (JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault (ele => ele.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim (new Claim (JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault (ele => ele.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim (new Claim (JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault (ele => ele.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim (new Claim (ClaimTypes.Name, jwt.Claims.FirstOrDefault (ele => ele.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim (new Claim (ClaimTypes.Role, jwt.Claims.FirstOrDefault (ele => ele.Type == "role").Value));
            var principle=new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync (CookieAuthenticationDefaults.AuthenticationScheme,principle);
        }
    }

}
