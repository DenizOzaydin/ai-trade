using MetuTrade.AdminUI.Services;
using MetuTrade.AdminUI.ViewModels.ChartData;
using MetuTrade.Business.Services;
using MetuTrade.Business.Settings;
using MetuTrade.Business.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MetuTrade.AdminUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private IHost _host { get; set; }

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.SetBasePath(Package.Current.InstalledLocation.Path);
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((builder, services) =>
                {
                    services.AddOptions();
                    services.AddHttpClient();

                    services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));

                    services.AddTransient<DownloadChartDataControlViewModel>();
                    services.AddScoped<AdminService>();
                    services.AddSingleton<AdminClient>();

                    services.AddHostedService<AppService>();
                }).Build();

            this.InitializeComponent();
        }

        public static T GetService<T>() where T : class
        {
            return (Current as App)!._host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            await _host.StartAsync();

            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
    }
}
