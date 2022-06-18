using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    public class Program
    {
        static async Task Main()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var source = configuration.GetSection("WallpaperSource").Value;
            string url = string.Empty;
            switch (source)
            {
                case "NASA":
                    url = await ImageSource.GetNasaApodImageUrlAsync(configuration);
                    break;
                case "BING":
                    url = await ImageSource.GetBingImageUrlAsync(configuration);
                    break;
                case "UNSPLASH":
                    url = await ImageSource.GetUnsplashRandomPhotoDownloadUrlAsync(configuration);
                    break;
            }
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Uri imageUrl = new Uri(url, UriKind.Absolute);
                await Wallpaper.SetWallpaperAsync(configuration, imageUrl, Wallpaper.Style.Fill);
            }
        }
    }
}
