using Azure.Messaging.ServiceBus;
using Mango.Service.RewardAPI.Message;
using Mango.Service.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Service.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string _serviceBuseConnectionString;
        private readonly string _OrderCreatedTopic;
        private readonly string _OrderCreatedSubscriptions;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;
        private readonly ServiceBusProcessor _rewardProcessor;
        private readonly ServiceBusProcessor _RegisterUserQueueProcessor;

        public AzureServiceBusConsumer(IConfiguration  configuration, RewardService rewardService )
        {
            _configuration = configuration;
             _rewardService = rewardService;
            _serviceBuseConnectionString = _configuration.GetValue<string> ("ServiceBusConnectionStrings");
            _OrderCreatedTopic = _configuration.GetValue<string> ("TopicsAndQueueNames:OrderCreatedTopic");
            _OrderCreatedSubscriptions = _configuration.GetValue<string> ("TopicsAndQueueNames:OrderCreated_Rewards_Subscription");
            var client = new ServiceBusClient(_serviceBuseConnectionString);
            _rewardProcessor = client.CreateProcessor (_OrderCreatedTopic, _OrderCreatedSubscriptions);
        }

        public async Task Start ( )
        {
            _rewardProcessor.ProcessMessageAsync += OnRewardRequestRecived;
            _rewardProcessor.ProcessErrorAsync+= ErrorHandler;
            await _rewardProcessor.StartProcessingAsync ();
            
            //_RegisterUserQueueProcessor.ProcessMessageAsync += OnRewardRequestRecived;
            //_RegisterUserQueueProcessor.ProcessErrorAsync += ErrorHandler;
            //await _RegisterUserQueueProcessor.StartProcessingAsync ();
        }


        private async Task OnRewardRequestRecived ( ProcessMessageEventArgs args )
        {
            //this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardMessage  rewardMessage=JsonConvert.DeserializeObject<RewardMessage>(body);
            try
            {
                //TODO - try to log email
                await _rewardService.UpdateReward (rewardMessage);
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
            await _rewardProcessor.StopProcessingAsync ();
            await _rewardProcessor.DisposeAsync ();
            await _RegisterUserQueueProcessor.StopProcessingAsync ();
            await _RegisterUserQueueProcessor.DisposeAsync ();
        }
    }
}
