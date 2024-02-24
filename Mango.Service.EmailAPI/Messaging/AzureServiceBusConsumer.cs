using Azure.Messaging.ServiceBus;
using Mango.Service.EmailAPI.Message;
using Mango.Service.EmailAPI.Models.Dto;
using Mango.Service.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Service.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string _serviceBuseConnectionString;
        private readonly string _emailQueue;
        private readonly string _RegisterUserQueue;
        private readonly string orderCreated_Topic;
        private readonly string _orderCreated_Subscription;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly ServiceBusProcessor _emailserviceBusProcessor;
        private readonly ServiceBusProcessor _busProcessor;
        private readonly ServiceBusProcessor _RegisterUserQueueProcessor;

        public AzureServiceBusConsumer(IConfiguration  configuration, EmailService emailService )
        {
            _configuration = configuration;
            _emailService = emailService;
            _serviceBuseConnectionString = _configuration.GetValue<string> ("ServiceBusConnectionStrings");
            _emailQueue = _configuration.GetValue<string> ("TopicsAndQueueNames:EmailShoppingCartQueue");
            _RegisterUserQueue = _configuration.GetValue<string> ("TopicsAndQueueNames:RegisterUserQueue");
            orderCreated_Topic = _configuration.GetValue<string> ("TopicsAndQueueNames:ordercreated");
            _orderCreated_Subscription = _configuration.GetValue<string> ("TopicsAndQueueNames:orderCreatedRewardUpdate");
            var client = new ServiceBusClient(_serviceBuseConnectionString);
            _busProcessor = client.CreateProcessor (_emailQueue);
            _emailserviceBusProcessor = client.CreateProcessor (orderCreated_Topic,_orderCreated_Subscription);
            _RegisterUserQueueProcessor = client.CreateProcessor (_RegisterUserQueue);
        }

        public async Task Start ( )
        {
            _busProcessor.ProcessMessageAsync += OnEmailCartRequestRecived;
            _busProcessor.ProcessErrorAsync+= ErrorHandler;
            await _busProcessor.StartProcessingAsync ();
            
            _RegisterUserQueueProcessor.ProcessMessageAsync += OnUserRequestRecived;
            _RegisterUserQueueProcessor.ProcessErrorAsync += ErrorHandler;
            await _RegisterUserQueueProcessor.StartProcessingAsync ();

            _emailserviceBusProcessor.ProcessMessageAsync += OnOrderRecivedPlaced;
            _emailserviceBusProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailserviceBusProcessor.StartProcessingAsync ();
        }

        private async Task OnUserRequestRecived ( ProcessMessageEventArgs args )
        {
            //this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string objMessage = JsonConvert.DeserializeObject<string>(body);
            try
            {
                //TODO - try to log email
                await _emailService.RegisterUserEmailAndLog(objMessage);
                await args.CompleteMessageAsync (args.Message);
            }
            catch ( Exception ex )
            {
                throw;
            }
        }

        private async Task OnEmailCartRequestRecived ( ProcessMessageEventArgs args )
        {
            //this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                //TODO - try to log email
                await _emailService.EmailCartAndLog (objMessage);
                await args.CompleteMessageAsync (args.Message);
            }
            catch ( Exception ex )
            {
                throw;
            }
        }
        private async Task OnOrderRecivedPlaced ( ProcessMessageEventArgs args )
        {
            //this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardMessage objMessage = JsonConvert.DeserializeObject<RewardMessage>(body);
            try
            {
                //TODO - try to log email
                await _emailService.LogOrderPlaced(objMessage);
                await args.CompleteMessageAsync (args.Message);
            }
            catch ( Exception ex )
            {
                throw;
            }
        }

        private Task ErrorHandler ( ProcessErrorEventArgs args )
        {
           Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        public async Task Stop ( )
        {
            await _busProcessor.StopProcessingAsync ();
            await _busProcessor.DisposeAsync ();

            await _RegisterUserQueueProcessor.StopProcessingAsync ();
            await _RegisterUserQueueProcessor.DisposeAsync ();

            await _emailserviceBusProcessor.StopProcessingAsync ();
            await _emailserviceBusProcessor.DisposeAsync ();
        }
    }
}
