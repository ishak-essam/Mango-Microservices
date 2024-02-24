using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString="Endpoint=sb://mangowebisaac.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DV65J5iuqDEaSs5jE3LSKKtkhzicMJ0KU+ASbGCiGac=";
        public async Task PublishMessage ( object message, string topic_queueName )
        {
            await using var client=new ServiceBusClient(connectionString);
            ServiceBusSender serviceBusSender=client.CreateSender(topic_queueName);
            var JsonMessage =JsonConvert.SerializeObject(message);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonMessage)){ 
            CorrelationId  =Guid.NewGuid().ToString()};
            await serviceBusSender.SendMessageAsync (serviceBusMessage);
            await client.DisposeAsync();
        }
    }
}