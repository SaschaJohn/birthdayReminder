using Newtonsoft.Json;

namespace MnM.Birthday
{
    public class TelegramMessage
    {
        [JsonProperty("chat_id")]
        public string chat_id { get; set; }

        [JsonProperty("text")]
        public string text { get; set; }
    }

}