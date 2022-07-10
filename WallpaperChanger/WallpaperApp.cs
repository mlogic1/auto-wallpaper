using Microsoft.Win32;
using Newtonsoft.Json;

namespace WallpaperChanger
{
	internal class WallpaperApp : ApplicationContext
	{
		private NotifyIcon notifyIcon;  // TODO1: add this to fix the icon https://stackoverflow.com/questions/14723843/notifyicon-remains-in-tray-even-after-application-closing-but-disappears-on-mous
										// TODO2: icon size guideline https://stackoverflow.com/a/3531316
		private ToolStripMenuItem[] notifyIconToolStripMenuItems;
		private ToolStripMenuItem notifyIconAutoStartupItem;
		private WallpaperFetcher fetcher;
		private AppConfiguration appConfig = new AppConfiguration();
		private const string APP_DATA_DIR_NAME = "WallpaperChanger";
		private const string REGISTRY_STARTUP_ENTRY_NAME = "WallpaperChanger";
		private const string CACHE_DIR_NAME = "Cache";
		private const string CONFIG_FILE_NAME = "WallpaperChangerConfig.json";
		private const string LOG_FILE_NAME = "WallpaperChangerLog.txt";
		private string CONFIG_FILE_FULL_PATH = null;
		private string LOG_FILE_FULL_PATH = null;
		private DirectoryInfo cacheDirectory = null;
		private ILogger logger;
		// win32 wallpaper https://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net
		private System.Windows.Forms.Timer wallpaperTimer;
		private const int WALLPAPER_TIMER_INTERVAL = 300000;  // 5 min

		internal WallpaperApp()
		{	
			InitConfigurationAndLogging();
			InitTrayIcon();
			UpdateTrayIconResolutionCheckMarker(appConfig.SelectedResolution);
			fetcher = new WallpaperFetcher(appConfig.SelectedResolution);
			fetcher.ResolutionChangedHandler += OnSelectedResolutionUpdated;
			wallpaperTimer = new System.Windows.Forms.Timer();
			wallpaperTimer.Tick += OnCheckTime;
			wallpaperTimer.Interval = WALLPAPER_TIMER_INTERVAL;
			wallpaperTimer.Start();

			logger.Info("Application started");
		}

		private void InitConfigurationAndLogging()
		{
			string appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_DATA_DIR_NAME);
			try
			{
				var dirInfo = Directory.CreateDirectory(appDataDir);
				cacheDirectory = dirInfo.CreateSubdirectory(CACHE_DIR_NAME);
				
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unable to create config and log directory: " + appDataDir + "\r\nMore details: " + ex.Message, "Wallpaper changer was unable to start", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}

			LOG_FILE_FULL_PATH = Path.Combine(appDataDir, LOG_FILE_NAME);
			CONFIG_FILE_FULL_PATH = Path.Combine(appDataDir, CONFIG_FILE_NAME);

			try
			{
				logger = new FileLogger(LOG_FILE_FULL_PATH);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Wallpaper changer was unable to start", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}
			
			if (!File.Exists(CONFIG_FILE_FULL_PATH))
			{
				try
				{
					SaveConfiguration();
				}
				catch (Exception ex)
				{
					logger.Error("Unable to save default config file. Exception message: " + ex.Message);
					Application.Exit();
				}
			}
			else
			{
				try
				{
					string configContent = File.ReadAllText(CONFIG_FILE_FULL_PATH);
					appConfig = JsonConvert.DeserializeObject<AppConfiguration>(configContent);
					if (appConfig == null)
					{
						throw new Exception("Failed to deserialize App Configuration file");
					}
				}
				catch (Exception ex)
				{
					logger.Info("Unable to open existing config. Falling back to default config. Exception message: " + ex.Message);
				}
			}
		}

		private void SaveConfiguration()
		{
			try
			{
				string output = JsonConvert.SerializeObject(appConfig);
				File.WriteAllText(CONFIG_FILE_FULL_PATH, output);
			} 
			catch (Exception ex)
			{
				MessageBox.Show("Error: Wallpaper changer was unable to save configuration file: " + CONFIG_FILE_FULL_PATH + "\r\nMore details: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}
		}

		private void InitTrayIcon()
		{
			ContextMenuStrip notifyIconOptions = new ContextMenuStrip();
			notifyIconOptions.Items.AddRange(GenerateMenuItems());

			notifyIcon = new NotifyIcon();
			notifyIcon.Icon = Properties.Resources.appicon;
			notifyIcon.Visible = true;
			notifyIcon.Text = "Wallpaper updater";
			notifyIcon.ContextMenuStrip = notifyIconOptions;

			Application.ApplicationExit += (sender, e) =>	// properly hide icon when application exits so it doesnt end up stuck in system tray
			{
				notifyIcon.Visible = false;
				notifyIcon.Dispose();
			};
		}

		private ToolStripItem[] GenerateMenuItems()
		{
			List<ToolStripItem> items = new List<ToolStripItem>();
			ToolStripItem itemExit = new ToolStripMenuItem("Exit");
			ToolStripMenuItem itemMenuResolution = new ToolStripMenuItem("Resolution");
			ToolStripMenuItem itemMenuSettings = new ToolStripMenuItem("Settings");

			ToolStripMenuItem[] resolutionItems = new ToolStripMenuItem[]
			{
				new ToolStripMenuItem("SD 1366x768", null, MenuClickResolutionClick),
				new ToolStripMenuItem("FHD 1920x1080", null, MenuClickResolutionClick),
				new ToolStripMenuItem("UHD 3840x2160", null, MenuClickResolutionClick)
			};

			notifyIconToolStripMenuItems = resolutionItems;

			resolutionItems[0].Tag = WallpaperResolution.SD;
			resolutionItems[1].Tag = WallpaperResolution.FHD;
			resolutionItems[2].Tag = WallpaperResolution.UHD;
			
			itemMenuResolution.DropDownItems.AddRange(resolutionItems);

			// ToolStripDropDownMenu
			itemExit.Click += MenuClickExit;

			notifyIconAutoStartupItem = new ToolStripMenuItem("Auto startup", null, MenuClickToggleStartup);
			notifyIconAutoStartupItem.Checked = IsAutoStartupEnabled();

			ToolStripMenuItem[] settingsItems = new ToolStripMenuItem[]
			{
				notifyIconAutoStartupItem,
				new ToolStripMenuItem("About")	// TODO about menu
			};
			itemMenuSettings.DropDownItems.AddRange(settingsItems);

			items.Add(itemMenuSettings);
			items.Add(itemMenuResolution);
			items.Add(itemExit);
			
			return items.ToArray();
		}

		void OnSelectedResolutionUpdated(WallpaperResolution newResolution)
		{
			UpdateTrayIconResolutionCheckMarker(newResolution);
			SaveConfiguration();
		}

		private void UpdateTrayIconResolutionCheckMarker(WallpaperResolution newResolution)
		{
			notifyIconToolStripMenuItems[0].Checked = notifyIconToolStripMenuItems[1].Checked = notifyIconToolStripMenuItems[2].Checked = false;

			switch (newResolution)
			{
				case WallpaperResolution.SD:
					notifyIconToolStripMenuItems[0].Checked = true;
					break;

				case WallpaperResolution.FHD:
					notifyIconToolStripMenuItems[1].Checked = true;
					break;

				case WallpaperResolution.UHD:
					notifyIconToolStripMenuItems[2].Checked = true;
					break;

				default:
					// LOG missing resolution
					break;
			}
		}

		private void MenuClickResolutionClick(object? sender, EventArgs e)
		{
			try
			{
				WallpaperResolution resolution = (WallpaperResolution)((ToolStripMenuItem)sender).Tag;
				appConfig.SelectedResolution = resolution;
				fetcher.SetResolution(resolution);
				logger.Info("Updated selected resolution: " + resolution.ToString());
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message);
			}
		}

		private void MenuClickToggleStartup(object? sender, EventArgs e)
		{
			bool autoStartupEnabled = IsAutoStartupEnabled();
			UpdateAutoStartup(!autoStartupEnabled);
		}

		private bool IsAutoStartupEnabled()
		{
			RegistryKey? key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

			return key.GetValue(REGISTRY_STARTUP_ENTRY_NAME) != null;
		}

		private void UpdateAutoStartup(bool enabled)
		{
			RegistryKey? key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

			if (enabled)
			{
				if (key.GetValue(REGISTRY_STARTUP_ENTRY_NAME) == null)
				{					
					string fullPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
					key.SetValue(REGISTRY_STARTUP_ENTRY_NAME, fullPath);
				}
			}
			else
			{
				if (key.GetValue(REGISTRY_STARTUP_ENTRY_NAME) != null)
				{
					key.DeleteValue(REGISTRY_STARTUP_ENTRY_NAME);
				}
			}

			notifyIconAutoStartupItem.Checked = enabled;
		}

		private void MenuClickExit(object? sender, EventArgs e)
		{
			Application.Exit();
		}

		void OnCheckTime(object? sender, EventArgs e)
		{
			DateTime now = DateTime.Now;
			double diff = (now - appConfig.LastWallpaperDate).TotalHours;
			if (diff >= 24.0)
			{
				UpdateSystemWallpaper();
			}
		}

		async void UpdateSystemWallpaper()
		{
			try
			{
				Stream? imageStream = await fetcher.DownloadWallpaper();
				if (imageStream == null)
				{
					throw new Exception("Unable to get imagestream from wallpaper fetcher");
				}

				// clear cache
				IEnumerable<FileInfo> cacheFiles = cacheDirectory.EnumerateFiles();
				logger.Info(String.Format("Clearing cache, {0} files.", cacheFiles.Count()));
				foreach (FileInfo file in cacheFiles)
				{
					file.Delete();
				}

				// write wallpaper to disk
				string wallpaperID = Guid.NewGuid().ToString();
				string wallpaperDiskPath = Path.Combine(cacheDirectory.FullName, wallpaperID + ".jpg");
				logger.Info(String.Format("Writing wallpaper to cache directory: {0}", wallpaperID));
				using (var fileStream = File.Create(wallpaperDiskPath))
				{
					imageStream.Seek(0, SeekOrigin.Begin);
					imageStream.CopyTo(fileStream);
				}

				SystemWallpaperUpdater.UpdateSystemWallpaper(wallpaperDiskPath);
				appConfig.LastWallpaperDate = DateTime.Now; // TODO: this needs to be autosaved
				SaveConfiguration();
			}
			catch(Exception ex)
			{
				logger.Error(ex.Message);
			}
		}
	}
}
