using System;
using Newtonsoft.Json;

namespace MnM.Birthday
{
    public class BirthdayListItem
    {

        static string icsSummary = "Geburtstag von {0}";
        static string icsFileContent =
@"BEGIN:VCALENDAR
BEGIN:VEVENT
DTSTART;TZID=Europe/Berlin:{0}
DTEND;TZID=Europe/Berlin:{0}
RRULE:FREQ=YEARLY;COUNT=10
SUMMARY:Geburtstag von {1} {2}
END:VEVENT
END:VCALENDAR";

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        public string getICS()
        {
            //TODO add person key + year
            var icsDate = DateTime.Now.ToString("yyyyMMdd"); //20221231
            return string.Format(icsFileContent, icsDate, this.Name, this.Year);
        }

        public string getSummary()
        {
            return string.Format(icsSummary, this.Name);
        }
    }

}