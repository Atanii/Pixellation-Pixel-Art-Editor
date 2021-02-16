using Microsoft.Win32;
using Pixellation.Components.Dialogs.AboutDialog;
using Pixellation.Components.Dialogs.NewImageDialog;
using Pixellation.Models;
using Pixellation.Properties;
using Pixellation.Utils.FilePackage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace Pixellation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

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
                    var fpr = new FilePackageReader(fileName);
                    var data = await fpr.LoadProjectModel();

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
                    BitmapImage bitmap = new BitmapImage(new Uri(fileName, UriKind.Absolute));
                    WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);
                    canvasImage.NewImage(writeableBitmap);
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

                var filePaths = new List<string>();

                // Saving LayerModels
                var layersPath = Properties.Resources.PackageContentFileNameForLayers + "." + Properties.Resources.ExtensionForLayersFile;
                var formatter = new BinaryFormatter();
                var stream = new FileStream(layersPath, FileMode.Create, FileAccess.Write);
                formatter.Serialize(stream, canvasImage.GetLayerModels());
                stream.Close();
                filePaths.Add(layersPath);

                // Saving Metadata
                var fpmd = new FilePackageMetadata
                {
                    Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    SaveDate = DateTime.Now.ToString()
                };

                var metaDataPath = Properties.Resources.PackageContentFileNameForMetaData + "." + Properties.Resources.ExtensionForDataFile;
                string jsonString = JsonSerializer.Serialize(fpmd);
                File.WriteAllText(metaDataPath, jsonString);
                filePaths.Add(metaDataPath);

                // Saving Project Data
                var fppd = new ProjectDataModel
                {
                    ProjectName = fileName.Split('.')[0].Split('\\')[^1]
                };

                var projectInfoPath = Properties.Resources.PackageContentFileNameForProjectData + "." + Properties.Resources.ExtensionForDataFile;
                jsonString = JsonSerializer.Serialize(fppd);
                File.WriteAllText(projectInfoPath, jsonString);
                filePaths.Add(projectInfoPath);

                // Packaging
                var fp = new FilePackage
                {
                    FilePath = fileName,
                    ContentFilePathList = filePaths
                };

                var fpwr = new FilePackageWriter(fp);
                fpwr.SaveProjectModel();

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

                // Getting Bitmap
                var wrBitmap = canvasImage.VisualAndLayerManager.GetAllMergedWriteableBitmap();
                SaveBitmapSourceToFile(fileName, wrBitmap);
            }
        }

        private void SaveBitmapSourceToFile(string fileName, BitmapSource image)
        {
            if (fileName != string.Empty)
            {
                string extension = fileName.Split('.')[^1];
                // Saving
                using FileStream fs = new FileStream(fileName, FileMode.Create);
                BitmapEncoder encoder;
                switch (extension.ToLower())
                {
                    case "png":
                        encoder = new PngBitmapEncoder();
                        break;

                    case "jpg":
                    case "jpeg":
                        encoder = new JpegBitmapEncoder();
                        break;

                    case "bmp":
                        encoder = new BmpBitmapEncoder();
                        break;

                    case "tiff":
                        encoder = new TiffBitmapEncoder();
                        break;

                    case "gif":
                        encoder = new GifBitmapEncoder();
                        break;

                    default:
                        MessageBox.Show($"Saving into (.{extension}) image format is not supported!", "Error");
                        return;
                }
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fs);
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
    }
}