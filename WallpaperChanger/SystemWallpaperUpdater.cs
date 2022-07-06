using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperChanger
{
	internal static class SystemWallpaperUpdater
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		private const int SPI_SETDESKWALLPAPER = 20;
		private const int SPIF_UPDATEINIFILE = 0x01;
		private const int SPIF_SENDWININICHANGE = 0x02;

		public static void UpdateSystemWallpaper(string image)
		{
			SystemParametersInfo(SPI_SETDESKWALLPAPER,
			0,
			image,
			SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
		}
	}
}
