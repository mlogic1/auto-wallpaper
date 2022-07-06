using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperChanger
{
	internal class AppConfiguration
	{
		public DateTime LastWallpaperDate { get; set; } = DateTime.MinValue;
		public WallpaperResolution SelectedResolution { get; set; } = WallpaperResolution.FHD;
	}
}