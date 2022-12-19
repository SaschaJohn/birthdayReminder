using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;


namespace MnM.Birthday
{
    public class fncBirthdayReminderTelegramQueueTrigger
    {
        static TelegramBotClient bot = new TelegramBotClient(getSetting("botCredentials"));
        static String chat_id = getSetting("chatid");

        [FunctionName("fncBirthdayReminderTelegramQueueTrigger")]
        public async Task Run([QueueTrigger("qu0telegram", Connection = "sac0birthdayreminder_STORAGE")] BirthdayListItem birthdayListItem, ILogger log)
        {
            log.LogInformation($"Telegram Queue trigger function processed: {birthdayListItem}");

            await sendMessage(chat_id, birthdayListItem);
            await sendFile(chat_id, birthdayListItem);
        }

        private async Task sendMessage(String chat_id, BirthdayListItem birthdayListItem)
        {
            await bot.SendTextMessageAsync(chat_id, birthdayListItem.getSummary());
        }

        private async Task sendFile(String chat_id, BirthdayListItem birthdayListItem)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(birthdayListItem.getICS());
            MemoryStream stream = new MemoryStream(byteArray);
            await fncBirthdayReminderTelegramQueueTrigger.bot.SendDocumentAsync(chat_id, new InputOnlineFile(stream, System.Guid.NewGuid() + ".ics"));
        }

        private static String getSetting(String parameterName)
        {
            return System.Environment.GetEnvironmentVariable($"{parameterName}");
        }



    }


}

