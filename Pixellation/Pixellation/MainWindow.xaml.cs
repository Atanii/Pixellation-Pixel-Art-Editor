using Microsoft.Win32;
using Pixellation.Components.Dialogs.AboutDialog;
using Pixellation.Components.Dialogs.NewImageDialog;
using Pixellation.Components.Editor;
using Pixellation.Properties;
using Pixellation.Utils;
using Pixellation.Utils.MementoPattern;
using System.Windows;
using System.Windows.Input;

namespace Pixellation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PersistenceManager pm = PersistenceManager.GetInstance();
        private readonly Caretaker<IEditorEventType> _mementoCaretaker = Caretaker<IEditorEventType>.GetInstance();

        public MainWindow()
        {
            InitializeComponent();
            GetWindow(this).KeyDown += canvasImage.OnKeyDown;
        }

        private async void Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = Properties.Resources.OpenFileFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                string extension = fileName.Split('.')[^1];
                if (extension == Properties.Resources.ExtensionForProjectFilePackage)
                {
                    // Getting Project

                    var data = await pm.LoadProject(fileName);

                    var width = Settings.Default.DefaultImageSize;
                    var height = Settings.Default.DefaultImageSize;
                    if (data.Layers.Count > 0)
                    {
                        width = data.Layers[0].Width;
                        height = data.Layers[0].Height;
                    }

                    canvasImage.NewImage(data.Layers, width, height);
                    Title = Properties.Resources.Title + " - " + data.ProjectData.ProjectName;
                }
                else
                {
                    // Getting Bitmap

                    var wrbmp = pm.LoadImage(fileName);
                    canvasImage.NewImage(wrbmp);
                }
            }
        }

        private void SaveProject(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = Properties.Resources.SaveFileFilter
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                pm.SaveProject(fileName, canvasImage);
                Title = Properties.Resources.Title + " - " + fileName.Split('.')[0].Split('\\')[^1];
            }
        }

        private void ExportAsImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = Properties.Resources.ExportFilter
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                pm.ExportAsImage(fileName, canvasImage);
            }
        }

        private void CommonCommandBinding_False(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void CommonCommandBinding_True(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenNewImageDialog(object sender, RoutedEventArgs e)
        {
            var newImgDialog = new NewImageDialog();
            if (newImgDialog.ShowDialog() == true)
            {
                // Get Data
                var widthHeight = newImgDialog.Answer;
                var w = int.Parse(widthHeight.Split(';')[0]);
                var h = int.Parse(widthHeight.Split(';')[1]);

                // New Image
                canvasImage.NewImage(w, h);
            }
        }

        private void OpenAboutDialog(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Z)
            {
                _mementoCaretaker.Undo();
            }
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Y)
            {
                _mementoCaretaker.Redo();
            }
        }
    }
}