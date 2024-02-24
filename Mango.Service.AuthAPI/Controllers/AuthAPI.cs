using Mango.MessageBus;
using Mango.Service.AuthAPI.Models.Dto;
using Mango.Service.AuthAPI.Service.IService;
using Mango.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Mango.Service.AuthAPI.Controllers
{
    [Route ("api/auth")]
    [ApiController]
    public class AuthAPI : ControllerBase
    {
        private readonly IAuth _auth;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        protected  ResponseDTO _responseDTO;
        public AuthAPI(IAuth auth, IMessageBus messageBus, IConfiguration configuration )
        {
            _auth = auth;
            _configuration = configuration;
            _messageBus = messageBus;
            _responseDTO = new();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register ( [FromBody] RegistrationRequestDTO registrationRequestDTO ) {
            var errorMessage=await _auth.Register(registrationRequestDTO);
            if ( !string.IsNullOrEmpty (errorMessage) )
            {
                _responseDTO.IsSuccessful = false;
                _responseDTO.Message = errorMessage;
                return BadRequest(_responseDTO);
            }
            _messageBus.PublishMessage (registrationRequestDTO.Email,_configuration.GetValue<string>("TopicsAndQueueNames:RegisterUserQueue"));
            return Ok (_responseDTO);
        }
        [HttpPost ("login")]
        public async Task<IActionResult> Login ([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginresponse= await _auth.Login(loginRequestDTO);
            if ( loginresponse.User == null ) { 
                _responseDTO.IsSuccessful=false;
                _responseDTO.Message = "UserName Or Password is valid";
                return BadRequest (_responseDTO);
            }
            _responseDTO.Result = loginresponse;
            return Ok (_responseDTO);

        }

        [HttpPost ("assignRole")]
        public async Task<IActionResult> AssignRole ( [FromBody] RegistrationRequestDTO registrationRequestDTO )
        {
            var AssignRole= await _auth.AssignRole(registrationRequestDTO.Email,registrationRequestDTO.Role.ToUpper() );
            if ( !AssignRole )
            {
                _responseDTO.IsSuccessful = false;
                _responseDTO.Message = "Error Encoutend ";
                return BadRequest (_responseDTO);
            }
            _responseDTO.Result = AssignRole;
            return Ok (_responseDTO);

        }

    }
}
