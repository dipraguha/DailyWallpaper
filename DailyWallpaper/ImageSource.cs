using DailyWallpaper.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    public class ImageSource
    {
        public async static Task<string> GetNasaApodImageUrlAsync(IConfiguration configuration)
        {
            string apiKey = configuration.GetSection("NasaApiKey").Value;
            string url = string.Format(Utility.GetUrl(configuration), apiKey);

            try
            {
                using (var client = Utility.GetHttpClient(configuration))
                {
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    using (var response = await client.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsAsync<NasaApod>();
                            if (data.MediaType != "image")
                            {
                                EventLogger.LogEvent("NASA APOD API call did not return an image.");
                                return null;
                            }
                            return Utility.PrepareImageDownloadUrl(configuration, data.ImageHdUrl);
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

        public async static Task<string> GetBingImageUrlAsync(IConfiguration configuration)
        {
            var url = Utility.GetUrl(configuration);

            try
            {
                using (var client = Utility.GetHttpClient(configuration))
                {
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    using (var response = await client.GetAsync(url))
                    {
                        if(response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var urlPart = Convert.ToString(JsonConvert.DeserializeObject<dynamic>(data).images[0].url);
                            if (!Uri.IsWellFormedUriString(urlPart, UriKind.Relative))
                            {
                                EventLogger.LogEvent("Failed to retrieve well-formed Bing image URL");
                                return null;
                            }
                            return Utility.PrepareImageDownloadUrl(configuration, urlPart);
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

        public async static Task<string> GetUnsplashRandomPhotoDownloadUrlAsync(IConfiguration configuration)
        {            
            var appKey = configuration.GetSection("UnsplashAppKey").Value;
            var url = string.Format(Utility.GetUrl(configuration), appKey);

            try
            {
                using (var client = Utility.GetHttpClient(configuration))
                {
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    using (var response = await client.GetAsync(url))
                    {
                        if(response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();                            
                            var downloadImageUrl = Convert.ToString(JsonConvert.DeserializeObject<dynamic>(data).urls.full);
                            var downloadCounterUrl = Convert.ToString(JsonConvert.DeserializeObject<dynamic>(data).links.download_location);
                            if (!Uri.IsWellFormedUriString(downloadImageUrl, UriKind.Absolute))
                            {
                                EventLogger.LogEvent("Failed to retrieve well-formed Unsplash photo URL");
                                return null;
                            }

                            //API requirement: trigger download endpoint if setting the image as wallpaper etc.
                            await TriggerUnsplashImageDownloadCounterEndpointAsync(configuration, downloadCounterUrl, appKey);

                            return string.Format(Utility.PrepareImageDownloadUrl(configuration, downloadImageUrl), appKey);
                        }
                        else
                        {
                            EventLogger.LogEvent("Failed to retrieve Unsplash photo URL. Response Status Code: " + response.StatusCode.ToString());
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                EventLogger.LogEvent(e.Message);
                return null;
            }
        }

        private async static Task TriggerUnsplashImageDownloadCounterEndpointAsync(IConfiguration configuration, string url, string appKey)
        {
            try
            {
                using (var client = Utility.GetHttpClient(configuration))
                {
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    var downloadEndpointUrl = url + "?client_id=" + appKey;
                    using (var response = await client.GetAsync(downloadEndpointUrl))
                    {
                        if(!response.IsSuccessStatusCode)
                        {
                            EventLogger.LogEvent("Failed to trigger Unsplash photo download endpoint");                            
                        }
                    }
                }
            }
            catch(Exception e)
            {
                EventLogger.LogEvent(e.Message);
            }
        }
    }
}
