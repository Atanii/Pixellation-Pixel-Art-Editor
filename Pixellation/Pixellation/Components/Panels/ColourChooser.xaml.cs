using Pixellation.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Tools
{
    /// <summary>
    /// Interaction logic for ColourPicker.xaml
    /// </summary>
    public partial class ColourChooser : UserControl
    {
        private Bitmap colourWheel;

        public System.Drawing.Color ChosenColour
        {
            get { return (System.Drawing.Color)GetValue(ChosenColourProperty); }
            set
            {
                SetValue(ChosenColourProperty, value);
                SetCcRectangleFill();
            }
        }

        public static readonly DependencyProperty ChosenColourProperty =
         DependencyProperty.Register("ChosenColour", typeof(System.Drawing.Color), typeof(ColourChooser), new FrameworkPropertyMetadata(
            System.Drawing.Color.Black, (d, e) => { RaiseChosenColourPropertyChangeEventHandlerEvent?.Invoke(default, EventArgs.Empty); }));

        private delegate void ChosenColourPropertyChangeEventHandler(object sender, EventArgs args);

        private static event ChosenColourPropertyChangeEventHandler RaiseChosenColourPropertyChangeEventHandlerEvent;

        public ColourChooser()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {   
            RaiseChosenColourPropertyChangeEventHandlerEvent += (s, a) => { SetCcRectangleFill(); };
        }

        private void CanvasToBitmap() {
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)colourGradientCanvas.RenderSize.Width,
                (int)colourGradientCanvas.RenderSize.Height, 
                96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(colourGradientCanvas);

            var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, 180, 180));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));

            using Stream s = new MemoryStream();
            pngEncoder.Save(s);
            colourWheel = new Bitmap(s);
        }

        private void SetChosenColourFromMousePosition(System.Windows.Point mousePos)
        {
            if (colourWheel == null)
            {
                CanvasToBitmap();
            }
            var colour = colourWheel.GetPixel(
                (int)mousePos.X,
                (int)mousePos.Y
            );
            if (colour != null)
            {
                ChosenColour = colour;
            }
        }

        private void SetCcRectangleFill()
        {
            ccLabel.Content = ChosenColour.ToString();
            ccRectangle.Fill = new SolidColorBrush(ChosenColour.ToMediaColor());
        }

        private void ColourWheelVisual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChosenColourFromMousePosition(e.GetPosition(colourGradientCanvas));
        }

        private void ColourWheelVisual_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetChosenColourFromMousePosition(e.GetPosition(colourGradientCanvas));
            }
        }

        private void ccBrightness_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
        }
    }
}