using Microsoft.Win32;
using Pixellation.Components.Dialogs.NewImageDialog;
using Pixellation.Components.Editor;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static Pixellation.Utils.ExtensionMethods;

namespace Pixellation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Drawing.Color ChosenColour
        {
            get { return (System.Drawing.Color)GetValue(ChosenColourProperty); }
            set { SetValue(ChosenColourProperty, value); }
        }
        public static readonly DependencyProperty ChosenColourProperty =
         DependencyProperty.Register("ChosenColour", typeof(System.Drawing.Color), typeof(MainWindow), new FrameworkPropertyMetadata(
            System.Drawing.Color.Black));

        public MainWindow()
        {
            InitializeComponent();
            InitStatusBarTexts();
        }

        private void InitStatusBarTexts()
        {
            statWidth.Text = "Width: 0px";
            statHeight.Text = "Width: 0px";
            statZoom.Text = "Zoom: 100%";
            statZoomedWidth.Text = "Zoomed Width: 0px";
            statZoomedHeight.Text = "Zoomed Width: 0px";
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
                WriteableBitmap wrBitmap = canvasImage.GetBitmap();
                SaveBitmapSourceToFile(fileName, wrBitmap);
            }
        }

        private void SaveBitmapSourceToFile(string fileName, BitmapSource image)
        {
            if (fileName != string.Empty)
            {
                string extension = fileName.Split('.')[^1];
                // Saving
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
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
        }

        private void CommonCommandBinding_False(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void CommonCommandBinding_True(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void UpdatePreview(object sender, MouseButtonEventArgs e)
        {
            var bitmap = canvasImage.GetBitmap().ToBitmap();
            var b = bitmap.ToBitmapSource();
            preview.Source = b;
        }

        private void NewImage(object sender, RoutedEventArgs e)
        {
            NewImageDialog newImgDialog = new NewImageDialog();
            if (newImgDialog.ShowDialog() == true)
            {
                // Get Data
                var widthHeight = newImgDialog.Answer;
                int width = Int32.Parse(widthHeight.Split(';')[0]);
                int height = Int32.Parse(widthHeight.Split(';')[1]);
                // New Image
                canvasImage = new PixelEditor(width, height, 2);
                canvasImage.InvalidateVisual();
            }
        }

        private void ZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var a = ChosenColour;
            if (canvasImage != null)
            {
                int newZoom = (int)e.NewValue;

                canvasImage.UpdateMagnification(newZoom);

                // TODO: scroll when magnified
                canvasImage.RenderTransformOrigin = new Point(canvasImage.ActualWidth, canvasImage.ActualHeight);
                canvasImage.HorizontalAlignment = HorizontalAlignment.Center;
                canvasImage.VerticalAlignment = VerticalAlignment.Center;

                canvasImage.InvalidateVisual();

                canvasScroll.InvalidateScrollInfo();
                canvasScroll.InvalidateVisual();
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            checkboardBg.Width = paintSurface.ActualWidth;
            checkboardBg.Height = paintSurface.ActualHeight;
            checkboardBg.Measure(new Size(paintSurface.ActualWidth, paintSurface.ActualHeight));
            checkboardBg.InvalidateMeasure();
        }
    }
}