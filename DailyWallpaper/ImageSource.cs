using DailyWallpaper.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DailyWallpaper
{
    public class ImageSource
    {
        public static string GetNasaApodImageUrl()
        {
            string apiKey = ConfigurationManager.AppSettings["NasaApiKey"].ToString();
            string url = string.Format(Constants.NasaApodUrl, apiKey);

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    using (var response = client.GetAsync(url).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var data = response.Content.ReadAsAsync<NasaApod>().Result;
                            if (data.MediaType != "image")
                            {
                                EventLogger.LogEvent("NASA APOD API call did not return an image.");
                                return null;
                            }
                            return data.ImageHdUrl;
                        }
                        else
                        {
                            EventLogger.LogEvent("NASA APOD API call failed with response status code " + response.StatusCode.ToString());
                            return null;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                EventLogger.LogEvent(e.Message);
                return null;
            }

        }

        public static string GetBingImageUrl()
        {
            var url = Constants.BingImageUrl;
            var baseUrl = ConfigurationManager.AppSettings["BingBaseUrl"].ToString();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    using (var response = client.GetAsync(url).Result)
                    {
                        if(response.IsSuccessStatusCode)
                        {
                            var data = response.Content.ReadAsStringAsync().Result;
                            var urlPart = Convert.ToString(JsonConvert.DeserializeObject<dynamic>(data).images[0].url);
                            if (!Uri.IsWellFormedUriString(urlPart, UriKind.Relative))
                            {
                                EventLogger.LogEvent("Failed to retrieve well-formed Bing image URL");
                                return null;
                            }                            
                            return baseUrl + urlPart;
                        }
                        else
                        {
                            EventLogger.LogEvent("Failed to retrieve Bing image URL. Response Status Code: " + response.StatusCode.ToString());
                            return null;
                        }                        
                    }
                }
            }
            catch(Exception e)
            {
                EventLogger.LogEvent(e.Message);
                return null;
            }            
        }
    }
}
