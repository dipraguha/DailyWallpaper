﻿using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace DailyWallpaper
{
    public class Wallpaper
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style
        {
            Fill,
            Fit,
            Span,
            Stretch,
            Tile,
            Center
        }

        public static void SetWallpaper(Uri url, Style style)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = client.GetAsync(url).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (var stream = response.Content.ReadAsStreamAsync().Result)
                            {
                                using (var img = Image.FromStream(stream))
                                {
                                    if (img != null)
                                    {
                                        string tempPath = Path.Combine(Path.GetTempPath(), "Wallpaper.jpg");
                                        img.Save(tempPath, ImageFormat.Jpeg);

                                        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                                        switch (style)
                                        {
                                            case Style.Center:
                                                regKey.SetValue(@"WallpaperStyle", "0");
                                                regKey.SetValue(@"TileWallpaper", "0");
                                                break;
                                            case Style.Fill:
                                                regKey.SetValue(@"WallpaperStyle", "10");
                                                regKey.SetValue(@"TileWallpaper", "0");
                                                break;
                                            case Style.Fit:
                                                regKey.SetValue(@"WallpaperStyle", "6");
                                                regKey.SetValue(@"TileWallpaper", "0");
                                                break;
                                            case Style.Span:
                                                regKey.SetValue(@"WallpaperStyle", "22");
                                                regKey.SetValue(@"TileWallpaper", "0");
                                                break;
                                            case Style.Stretch:
                                                regKey.SetValue(@"WallpaperStyle", "2");
                                                regKey.SetValue(@"TileWallpaper", "0");
                                                break;
                                            case Style.Tile:
                                                regKey.SetValue(@"WallpaperStyle", "0");
                                                regKey.SetValue(@"TileWallpaper", "1");
                                                break;
                                            default:
                                                break;
                                        }

                                        int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                                        if (result == 0)
                                        {
                                            EventLogger.LogEvent("Setting wallpaper failed. SystemParametersInfo returned 0.");
                                        }
                                    }
                                    else
                                    {
                                        EventLogger.LogEvent("Setting wallpaper failed. Image is null.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                EventLogger.LogEvent(e.Message);
            }
        }
    }
}