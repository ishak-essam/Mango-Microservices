using Mango.Service.EmailAPI.Message;
using Mango.Service.EmailAPI.Models;
using Mango.Service.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Service.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService ( DbContextOptions<AppDbContext> dbOptions )
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog ( CartDto cartDto )
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine ("<br/>Cart Email Requested ");
            message.AppendLine ("<br/>Total " + cartDto.CartHeader.CartTotal);
            message.Append ("<br/>");
            message.Append ("<ul>");
            foreach ( var item in cartDto.CartDetails )
            {
                message.Append ("<li>");
                message.Append (item.Product.Name + " x " + item.Count);
                message.Append ("</li>");
            }
            message.Append ("</ul>");

            await LogAndEmail (message.ToString (), cartDto.CartHeader.Email);
        }

        public async Task LogOrderPlaced ( RewardMessage rewardMessage )
        {
            string message = "New Order Placed <br/> OrderId : " + rewardMessage.OrderId;
            await LogAndEmail (message, "isaacessam122@gmail.com");
        }

        public async Task RegisterUserEmailAndLog ( string email )
        {
            string message = "User Registeration Successful. <br/> Email : " + email;
            await LogAndEmail (message, "isaacessam122@gmail.com");
        }

        private async Task<bool> LogAndEmail ( string message, string email )
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var _db = new AppDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync (emailLog);
                await _db.SaveChangesAsync ();
                return true;
            }
            catch ( Exception ex )
            {
                return false;
            }
        }
    }
}