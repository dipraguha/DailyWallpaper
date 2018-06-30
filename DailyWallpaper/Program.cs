using System;
using System.Configuration;

namespace DailyWallpaper
{
    public class Program
    {
        static void Main(string[] args)
        {
            var source = ConfigurationManager.AppSettings["WallpaperSource"].ToString();
            string url = string.Empty;
            switch (source)
            {
                case "NASA":
                    url = ImageSource.GetNasaApodImageUrl();
                    break;
                case "BING":
                    url = ImageSource.GetBingImageUrl();
                    break;
                case "UNSPLASH":
                    url = ImageSource.GetUnsplashRandomPhotoDownloadUrl();
                    break;
            }
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Uri imageUrl = new Uri(url, UriKind.Absolute);
                Wallpaper.SetWallpaper(imageUrl, Wallpaper.Style.Fill);
            }
        }
    }
}
