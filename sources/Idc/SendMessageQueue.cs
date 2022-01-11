using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Queues;
using System.Collections.Generic;

namespace sendmessagequeue
{
    public static class SendMessageQueue
    {
        [FunctionName("SendMessageQueue")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string inputContainer = data.inputContainer;
            string outputContainer = data.outputContainer;
            string filePath = data.filePath;
            Dictionary<string, string> mess = new Dictionary<string, string>();
            mess.Add("inputContainer", inputContainer);
            mess.Add("outputContainer", outputContainer);
            mess.Add("filePath", filePath);
            string json = JsonConvert.SerializeObject(mess, Formatting.Indented);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
            QueueClient queueClient = new QueueClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"),Environment.GetEnvironmentVariable("QueueName"));
            queueClient.SendMessage(System.Convert.ToBase64String(plainTextBytes));

            return new OkObjectResult("Done");
        }
    }
}
