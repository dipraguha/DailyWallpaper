using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DailyWallpaper
{
    public class Utility
    {
        public static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            var source = ConfigurationManager.AppSettings["WallpaperSource"].ToString();
            switch (source)
            {
                case "NASA":
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    break;
                case "BING":
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    break;
                case "UNSPLASH":
                    client.DefaultRequestHeaders.Add("Accept-Version", "v1");
                    break;
            }
            return client;
        }

        public static string GetUrl()
        {
            var url = "";
            var source = ConfigurationManager.AppSettings["WallpaperSource"].ToString();
            switch (source)
            {
                case "NASA":
                    url = Constants.NasaApodUrl + "?api_key={0}";
                    break;
                case "BING":
                    url = Constants.BingImageUrl + "?format=js&idx=0&n=1";
                    break;
                case "UNSPLASH":
                    url = Constants.UnsplashRandomPhotoUrl + "?client_id={0}";
                    break;
            }
            return url;
        }

        public static string PrepareImageDownloadUrl(string url)
        {
            var preparedUrl = "";
            var source = ConfigurationManager.AppSettings["WallpaperSource"].ToString();            
            switch (source)
            {
                case "NASA":
                    preparedUrl = url;
                    break;
                case "BING":
                    preparedUrl = ConfigurationManager.AppSettings["BingBaseUrl"].ToString() + url;
                    break;
                case "UNSPLASH":
                    preparedUrl = url + "&client_id={0}";
                    break;
            }
            return preparedUrl;
        }
    }
}
