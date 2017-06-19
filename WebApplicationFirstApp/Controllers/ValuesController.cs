using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WebApplicationFirstApp.Controllers
{
    public class ValuesController : ApiController
    {
        static CloudQueue cloudQueueOne;
        static CloudQueue cloudQueueTwo;

        // Connection to QueueOne and QueueTwo
        public static void ConnectToStorageQueue()
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=storagenumvem;AccountKey=JMGSFPJU6pWtFGktyE4hRf3xmqca+boDbakLU7BcGXAHLXv6rrEHKllUFat7VWlW3XuL2sBbRz1zjHRm7t74tQ==;EndpointSuffix=core.windows.net";
            CloudStorageAccount cloudStorageAccount;

            if (!CloudStorageAccount.TryParse(connectionString, out cloudStorageAccount))
            {
                Console.WriteLine("Expected connection string 'Azure Storage Account to be a valid Azure Storage Connection String.");
            }

            var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            cloudQueueOne = cloudQueueClient.GetQueueReference("queueone");
            cloudQueueTwo = cloudQueueClient.GetQueueReference("queuetwo");

            cloudQueueOne.CreateIfNotExists();
            cloudQueueTwo.CreateIfNotExists();
        }

        //PUT message in QueueOne
        private string PutMessageToQueueOne(String MessageText)
        {
            ConnectToStorageQueue();
            var message = new CloudQueueMessage(MessageText);
            cloudQueueOne.AddMessage(message);
            return "Message "+MessageText+" add in QueueOne";

        }

        //GET message from QueueTwo
        private string GetMessageFromQueueTwo()
        {
			ConnectToStorageQueue();
			CloudQueueMessage cloudQueueMessage = cloudQueueTwo.GetMessage();

            if (cloudQueueMessage == null)
            {
                return "The QueueTwo is Empty";
            }
            
            String message = cloudQueueMessage.AsString;
            cloudQueueTwo.DeleteMessage(cloudQueueMessage);
            return message;
        }
        
        // GET api/values
        public string Get()
        {
            return GetMessageFromQueueTwo();
        }

        // GET api/values/id
        public string Get(String id)
        {
            return PutMessageToQueueOne(id);
        }

        
    }
}
