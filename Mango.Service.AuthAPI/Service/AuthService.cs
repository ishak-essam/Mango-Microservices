using Mango.Service.AuthAPI.Models;
using Mango.Service.AuthAPI.Models.Dto;
using Mango.Service.AuthAPI.Service.IService;
using Mango.Services.AuthAPI.Data;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace Mango.Service.AuthAPI.Service
{
    public class AuthService : IAuth
    {
    
        private readonly AppDbContext _db;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly UserManager<ApplicationUser>    _userManager;
        private readonly RoleManager<IdentityRole> _roleManager ;
        public AuthService (  AppDbContext db,IJwtTokenGenerator
            jwtTokenGenerator,
          UserManager<ApplicationUser>    userManager,
          RoleManager<IdentityRole> roleManager )
        {
         
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole ( string email, string roleName )
        {
            var user=_db.ApplicationUser.FirstOrDefault(ele=>ele.UserName.ToLower()==email.ToLower());
            if ( user !=null ) {
                if ( !_roleManager.RoleExistsAsync (roleName).GetAwaiter().GetResult()) {
                    //create role if it doesn't exist
                    _roleManager.CreateAsync (new IdentityRole (roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync (user,roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login ( LoginRequestDTO requestDTO )
        {
            var user=_db.ApplicationUser.FirstOrDefault(ele=>ele.UserName.ToLower()==requestDTO.UserName.ToLower());
            bool  boolPassword = await _userManager.CheckPasswordAsync (user,requestDTO.Password);
            if ( user == null || boolPassword == false ) {
                return new LoginResponseDTO () { User = null, Token = "" };
            }
            var userRoles=await _userManager.GetRolesAsync(user);
          var token=  _jwtTokenGenerator.GenerateToken (user,userRoles);
          
            UserDTO userDTO=new()
            {
                ID=user.Id,
                Name=user.Name,
                PhoneNumber=user.PhoneNumber
            };
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                User=userDTO,
                Token=token
            };
            return loginResponseDTO;
        }

        public async Task<string> Register ( RegistrationRequestDTO requestDTO )
        {
            ApplicationUser user= new ()
            {
                UserName = requestDTO.Email,
                Email = requestDTO.Email,
                NormalizedEmail=requestDTO.Email.ToUpper(),
                Name = requestDTO.Name,
                PhoneNumber = requestDTO.PhoneNumber,

            };
            try {
                var result=await _userManager.CreateAsync(user,requestDTO.Password);
                if ( result.Succeeded )
                {
                    var UserToReturn=_db.ApplicationUser.First(ele=>ele.UserName==requestDTO.Email);
                    UserDTO userDTO=new()
                    {
                        ID=UserToReturn.Id,
                        Name=UserToReturn.Name,
                        PhoneNumber=UserToReturn.PhoneNumber
                    };
                    return "";
                }
                else return result.Errors.FirstOrDefault ().Description;
            }
            catch (Exception ex) {


            }
                return "Error executions";
        }
    }
}
