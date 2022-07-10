using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WallpaperChanger
{
	internal class WallpaperFetcher
	{
		public delegate void SelectedResolutionChangedHandler(WallpaperResolution resolution);
		public event SelectedResolutionChangedHandler ResolutionChangedHandler;

		private WallpaperResolution resolution = WallpaperResolution.FHD;

		private readonly Dictionary<WallpaperResolution, string> RESOLUTIONS = new Dictionary<WallpaperResolution, string>()
		{
			{ WallpaperResolution.SD, "1366x768" },
			{ WallpaperResolution.FHD, "1920x1080" },
			{ WallpaperResolution.UHD, "UHD" }
		};

		class ImageData
		{
			public string Urlbase { get; set; }
		}

		class ApiResult
		{
			public IList<ImageData> Images { get; set; }
		}

		private readonly string BING_WALLPAPER_API = "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US";
		private readonly string BING_IMAGE_URL_FORMAT = "https://www.bing.com/{0}_{1}.jpg";

		public WallpaperFetcher()
		{
			resolution = WallpaperResolution.FHD;
		}

		public WallpaperFetcher(WallpaperResolution resolution)
		{
			this.resolution = resolution;
		}

		public void SetResolution(WallpaperResolution newResolution)
		{
			resolution = newResolution;
			if (ResolutionChangedHandler != null)
			{
				ResolutionChangedHandler(newResolution);
			}
		}

		// returns a stream to file
		public async Task<Stream> DownloadWallpaper()
		{
			Stream imageStream = null;
			HttpClient client = new HttpClient();

			try
			{
				HttpResponseMessage responseApi = await client.GetAsync(BING_WALLPAPER_API);
				if (responseApi.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception("Failed to contact Bing API. Status code: " + responseApi.StatusCode);
				}

				string resultContent = await responseApi.Content.ReadAsStringAsync();

				ApiResult r = JsonConvert.DeserializeObject<ApiResult>(resultContent);
				if (r != null)
				{
					string fullImageUrl = String.Format(BING_IMAGE_URL_FORMAT, r.Images.First().Urlbase, RESOLUTIONS[resolution]);
					HttpResponseMessage responseImage = await client.GetAsync(fullImageUrl);
					if (responseImage.StatusCode != HttpStatusCode.OK)
					{
						throw new Exception("Failed to get wallpaper from Bing API. Status code: " + responseImage.StatusCode);
					}

					imageStream = await responseImage.Content.ReadAsStreamAsync();
				}
			}
			catch(Exception ex)
			{
				throw new Exception("Failed to contact Bing API: " + ex.Message);
			}
			
			return imageStream;
		}
	}
}
