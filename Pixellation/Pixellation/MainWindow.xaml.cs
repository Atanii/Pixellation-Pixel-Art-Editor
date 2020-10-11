using Microsoft.Win32;
using Pixellation.Components.Dialogs.NewImageDialog;
using Pixellation.Components.Editor;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Pixellation.Utils.HelperFunctions;

namespace Pixellation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point currentPoint = new Point();

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

        private void Canvas_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(paintSurface);
        }

        private void Canvas_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line();

                line.Stroke = SystemColors.WindowFrameBrush;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(paintSurface).X;
                line.Y2 = e.GetPosition(paintSurface).Y;

                currentPoint = e.GetPosition(paintSurface);

                paintSurface.Children.Add(line);
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
            WriteableBitmap wrBitmap = canvasImage.GetBitmap();
            var bitmap = BitmapFromWriteableBitmap(wrBitmap);
            var b = BitmapToBitmapSource(bitmap);
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

        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            checkboardBg.Width = paintSurface.ActualWidth;
            checkboardBg.Height = paintSurface.ActualHeight;
            checkboardBg.Measure(new Size(paintSurface.ActualWidth, paintSurface.ActualHeight));
            checkboardBg.InvalidateMeasure();
        }
    }
}