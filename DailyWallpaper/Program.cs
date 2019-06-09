using System;
using System.Configuration;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var source = ConfigurationManager.AppSettings["WallpaperSource"].ToString();
            string url = string.Empty;
            switch (source)
            {
                case "NASA":
                    url = await ImageSource.GetNasaApodImageUrlAsync();
                    break;
                case "BING":
                    url = await ImageSource.GetBingImageUrlAsync();
                    break;
                case "UNSPLASH":
                    url = await ImageSource.GetUnsplashRandomPhotoDownloadUrlAsync();
                    break;
            }
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Uri imageUrl = new Uri(url, UriKind.Absolute);
                await Wallpaper.SetWallpaperAsync(imageUrl, Wallpaper.Style.Fill);
            }
        }
    }
}
