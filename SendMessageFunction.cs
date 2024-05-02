using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebChatServer
{
    public static class SendMessageFunction
    {
        [FunctionName("SendMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "sendMessage")] HttpRequest req,
            ILogger log,
            [SignalR(HubName = "myChatRoom")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var receivedContent = await new StreamReader(req.Body).ReadToEndAsync();
            var m = JsonConvert.DeserializeObject<Message>(receivedContent);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] {m}
                });

            return new OkResult();
        }
    }

    internal class Message
    {
        public string Text { get; set; }
    }
}
