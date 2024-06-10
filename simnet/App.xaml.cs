using log4net.Config;
using log4net;
using System.IO;
using System.Reflection;
using System.Windows;

namespace simnet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string logDir = @"C:\Logs";
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // Your other startup code here
            var logger = LogManager.GetLogger(typeof(App));
            logger.Info("Application started");
        }
    }
}
