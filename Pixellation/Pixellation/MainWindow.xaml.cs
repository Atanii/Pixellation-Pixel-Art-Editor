using Microsoft.Win32;
using Pixellation.Components.Dialogs.AboutDialog;
using Pixellation.Components.Dialogs.NewImageDialog;
using System;
using System.IO;
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
        }

        private void OpenImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.bmp, *.jpg, *.png, *.tiff, *.gif)|*.bmp;*.jpg;*.png;*.tiff;*.gif"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                // Getting Bitmap
                BitmapImage bitmap = new BitmapImage(new Uri(fileName, UriKind.Absolute));
                WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);
                canvasImage.NewImage(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, (int)sliderZoom.Value, writeableBitmap);
            }
        }

        private void SaveAsImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Image Files (*.bmp, *.jpg, *.png, *.tiff, *.gif)|*.bmp;*.jpg;*.png;*.tiff;*.gif"
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
                int width = Int32.Parse(widthHeight.Split(';')[0]);
                int height = Int32.Parse(widthHeight.Split(';')[1]);
                // New Image
                canvasImage.NewImage(width, height, (int)sliderZoom.Value); // TODO: update UI binding
            }
        }

        private void OpenAboutDialog(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }

        private void ZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (canvasImage != null)
            {
                int newZoom = (int)e.NewValue;
                canvasImage.UpdateMagnification(newZoom);
                canvasImage.InvalidateMeasure();
            }
        }
    }
}