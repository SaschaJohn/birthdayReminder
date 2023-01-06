using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace MnM.Birthday
{
    public class fncBirthdayReminderTimerTrigger
    {
        [FunctionName("fncBirthdayReminderTimerTrigger")]
        public async Task<IActionResult> Run([TimerTrigger("0 0 14 * * *")] TimerInfo myTimer,
            [CosmosDB(databaseName: "birthdayreminder", containerName: "birthdaylist", Connection = "CosmosDBConnection")] CosmosClient client,
            [Queue("qu0telegram"), StorageAccount("sac0birthdayreminder_STORAGE")] ICollector<BirthdayListItem> telegramMsg,
            [Queue("qu0email"), StorageAccount("sac0birthdayreminder_STORAGE")] ICollector<BirthdayListItem> emailMsg,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await writeCosmosEntriesToQueueFromTimer(client, log, telegramMsg, emailMsg);
            return new OkObjectResult("");
        }

        public async Task writeCosmosEntriesToQueueFromTimer(CosmosClient client, ILogger log, ICollector<BirthdayListItem> telegramMsg, ICollector<BirthdayListItem> emailMsg)
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
