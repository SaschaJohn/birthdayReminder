using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;

namespace MnM.Birthday
{
    public class fncBirthdayReminderHttpTrigger
    {
        [FunctionName("fncBirthdayReminderHttpTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "birthdayreminder", containerName: "birthdaylist", Connection = "CosmosDBConnection")] CosmosClient client,
            [Queue("qu0telegram"), StorageAccount("sac0birthdayreminder_STORAGE")] ICollector<BirthdayListItem> telegramMsg,
            [Queue("qu0email"), StorageAccount("sac0birthdayreminder_STORAGE")] ICollector<BirthdayListItem> emailMsg,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            await writeCosmosEntriesToQueue(client, log, telegramMsg, emailMsg);
            return new OkObjectResult("");
        }

        public async Task writeCosmosEntriesToQueue(CosmosClient client, ILogger log, ICollector<BirthdayListItem> telegramMsg, ICollector<BirthdayListItem> emailMsg)
        {
            Container container = client.GetDatabase("birthdayreminder").GetContainer("birthdaylist");
            var searchterm = DateTime.Now.ToString("MM-dd"); //09-29
            log.LogInformation($"Searching for: {searchterm}");

            QueryDefinition queryDefinition = new QueryDefinition(
                "SELECT * FROM c WHERE c.key = @searchterm")
                .WithParameter("@searchterm", searchterm);
            using (FeedIterator<BirthdayListItem> resultSet = container.GetItemQueryIterator<BirthdayListItem>(queryDefinition))
            {
                while (resultSet.HasMoreResults)
                {

                    FeedResponse<BirthdayListItem> response = await resultSet.ReadNextAsync();
                    foreach (BirthdayListItem person in response)
                    {
                        log.LogInformation(person.Name);
                        telegramMsg.Add(person);
                        emailMsg.Add(person);
                    }

                }
            }
        }

    }
}
