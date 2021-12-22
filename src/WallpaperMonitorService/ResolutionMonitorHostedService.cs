using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Wallpaper;

namespace WallpaperMonitorService
{
    public delegate void DisplayChange();
    internal class ResolutionMonitorHostedService : IHostedService
    {
        private readonly ILogger<ResolutionMonitorHostedService> _logger;
        private readonly WallpaperEngine _wallpaperEngine;
        private readonly IDisplaySettingsCache _displaySettingsCache;
        public ResolutionMonitorHostedService(ILogger<ResolutionMonitorHostedService> logger, WallpaperEngine wallpaperEngine, IDisplaySettingsCache displaySettingsCache)
        {
            _logger = logger;
            _wallpaperEngine = wallpaperEngine;
            _displaySettingsCache = displaySettingsCache;
        }

        public async void SetWallpaper()
        {
            try
            {
                await _wallpaperEngine.CreateRandomWallpaper();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error setting wallpaper.", ex);
            }
        }

        private  void HandleDisplayChange(object obj, EventArgs e)
        {
            try
            {
                _logger.LogInformation("Display settings changed, set new wallpapers");
                if (_displaySettingsCache.HasChanged(Screen.AllScreens))
                {
                     _wallpaperEngine.CreateRandomWallpaper().RunSynchronously();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error setting wallpaper.", ex);
            }
            
            

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            SystemEvents.DisplaySettingsChanged += HandleDisplayChange;
            

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            SystemEvents.DisplaySettingsChanged -= HandleDisplayChange;
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");

            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");

            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");

            // Perform post-stopped activities here
        }
    }
}
