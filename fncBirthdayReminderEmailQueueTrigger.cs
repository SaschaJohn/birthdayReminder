using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace MnM.Birthday
{
    public class fncBirthdayReminderEmailQueueTrigger
    {

        [FunctionName("fncBirthdayReminderEmailQueueTrigger")]
        public async Task Run([QueueTrigger("qu0email", Connection = "sac0birthdayreminder_STORAGE")] BirthdayListItem birthdayListItem,
        [SendGrid(ApiKey = "sendgridKey")] IAsyncCollector<SendGridMessage> messageCollector, ILogger log)
        {
            log.LogInformation($"Telegram Queue trigger function processed: {birthdayListItem}");

            var message = new SendGridMessage();
            message.AddTo(getSetting("mailTo"));
            message.AddContent("text/html", birthdayListItem.getSummary());
            message.SetFrom(new EmailAddress(getSetting("mailFrom")));
            message.SetSubject(birthdayListItem.getSummary());
            message.AddAttachment(System.Guid.NewGuid() + ".ics", Convert.ToBase64String(Encoding.UTF8.GetBytes(birthdayListItem.getICS())));

            await messageCollector.AddAsync(message);

        }

        private String getSetting(String parameterName)
        {
            return System.Environment.GetEnvironmentVariable($"{parameterName}");
        }

    }


}

