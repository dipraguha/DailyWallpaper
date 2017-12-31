using Newtonsoft.Json;
using System;

namespace DailyWallpaper.Models
{
    public class NasaApod
    {
        [JsonProperty("date")]
        public DateTime ImageDate { get; set; }

        [JsonProperty("explanation")]
        public string ImageDescription { get; set; }

        [JsonProperty("hdurl")]
        public string ImageHdUrl { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        [JsonProperty("service_version")]
        public string ServiceVersion { get; set; }

        [JsonProperty("title")]
        public string ImageTitle { get; set; }

        [JsonProperty("url")]
        public string ImageUrl { get; set; }
    }
}
