using System.IO;
using System.Windows;

namespace Pixellation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running.
            // Process command line args.
            bool startMinimized = false;
            string path = "";
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "/StartMinimized")
                {
                    startMinimized = true;
                }
                else if (File.Exists(e.Args[i]))
                {
                    path = e.Args[i];
                }
            }

            // Create main application window, starting minimized if specified, load project if path is provided.
            MainWindow mainWindow = new MainWindow();
            if (startMinimized)
            {
                mainWindow.WindowState = WindowState.Minimized;
            }
            if (path != string.Empty)
            {
                mainWindow.OpenProjectOrImage(path);
            }
            mainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Pixellation.Properties.Settings.Default.Save();
        }
    }
}