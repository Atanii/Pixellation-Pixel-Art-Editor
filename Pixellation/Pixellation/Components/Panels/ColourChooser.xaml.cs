using Pixellation.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Tools
{
    using Rectangle = System.Windows.Shapes.Rectangle;
    using Color = System.Drawing.Color;

    /// <summary>
    /// Interaction logic for ColourPicker.xaml
    /// </summary>
    public partial class ColourChooser : UserControl
    {   
        public Color ChosenColour
        {
            get { return (Color)GetValue(ChosenColourProperty); }
            set
            {
                SetValue(ChosenColourProperty, value);
                SetCcRectangleFill();
                SetRGBATxtInputs();
            }
        }

        public static readonly DependencyProperty ChosenColourProperty =
         DependencyProperty.Register("ChosenColour", typeof(Color), typeof(ColourChooser), new FrameworkPropertyMetadata(
            Color.Black, (d, e) => { RaiseChosenColourPropertyChangeEventHandlerEvent?.Invoke(default, EventArgs.Empty); }));

        private delegate void ChosenColourPropertyChangeEventHandler(object sender, EventArgs args);

        private static event ChosenColourPropertyChangeEventHandler RaiseChosenColourPropertyChangeEventHandlerEvent;

        public ColourChooser()
        {
            InitializeComponent();
            Init();
            SetRGBATxtInputs();
        }

        private void Init()
        {
            RaiseChosenColourPropertyChangeEventHandlerEvent += (s, a) => { SetCcRectangleFill(); };
        }

        private Color GetPixelColor(Rectangle cvs, Point mousePos)
        {
            var source = PresentationSource.FromVisual(cvs);
            Point ptDpi;
            if (source != null)
            {
                ptDpi = new Point(
                    96d * source.CompositionTarget.TransformToDevice.M11,
                    96d * source.CompositionTarget.TransformToDevice.M22
                );
            }
            else
            {
                ptDpi = new Point(96d, 96d); // Default for most monitors.
            }

            var srcSize = VisualTreeHelper.GetDescendantBounds(cvs).Size;

            // Viewbox uses [0; 1] so we normalize the Rect with respect to the visual's size
            var percentSrcRec = new Rect(
                mousePos.X / srcSize.Width, mousePos.Y / srcSize.Height,
                1 / srcSize.Width, 1 / srcSize.Height
            );

            // Generalized for monitors with different dpi
            var bmpOut = new RenderTargetBitmap(
                (int)(ptDpi.X / 96d),
                (int)(ptDpi.Y / 96d),
                ptDpi.X, ptDpi.Y, PixelFormats.Default
            );

            var dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawRectangle(
                    new VisualBrush { Visual = cvs, Viewbox = percentSrcRec },
                    null, // No Pen
                    new Rect(0, 0, 1d, 1d)
                );
            }
            bmpOut.Render(dv);

            var bytes = new byte[4];
            int iStride = 4; // = 4 * bmpOut.Width (for 32 bit graphics with 4 bytes per pixel -- 4 * 8 bits per byte = 32)
            bmpOut.CopyPixels(bytes, iStride, 0);

            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        private void SetChosenColourFromMousePosition(Point mousePos)
        {
            var colour = GetPixelColor(colourGradientCanvas, mousePos);
            if (colour != null)
            {
                ChosenColour = colour;
            }
        }

        private void SetHueColourFromMousePosition(Point mousePos)
        {
            var colour = GetPixelColor(colourGradientHue, mousePos);
            if (colour != null)
            {
                Resources["CurrentColor"] = colour.ToMediaColor();
            }
        }

        private void SetCcRectangleFill()
        {
            ccRectangle.Fill = new SolidColorBrush(ChosenColour.ToMediaColor());
        }

        private void SetRGBATxtInputs()
        {
            scR.Text = ChosenColour.R.ToString();
            scG.Text = ChosenColour.G.ToString();
            scB.Text = ChosenColour.B.ToString();
            scA.Text = ChosenColour.A.ToString();
        }

        private void ColourWheelVisual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChosenColourFromMousePosition(e.GetPosition(colourGradientCanvas));
        }

        private void ColourWheelVisualHue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetHueColourFromMousePosition(e.GetPosition(colourGradientHue));
        }

        private void ColourWheelVisual_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetChosenColourFromMousePosition(e.GetPosition(colourGradientCanvas));
            }
        }

        public void SetHueColor(Color c)
        {
            if (c != null)
            {
                Resources["CurrentColor"] = c.ToMediaColor();
            }
        }

        public void SetHueColor(System.Windows.Media.Color c)
        {
            if (c != null)
            {
                Resources["CurrentColor"] = c;
            }
        }

        private void sc_TextInput(object sender, KeyEventArgs e)
        {
            if (int.TryParse(scR.Text, out int R) &&
                int.TryParse(scG.Text, out int G) &&
                int.TryParse(scB.Text, out int B) &&
                int.TryParse(scA.Text, out int A) &&
                R <= 255 && G <= 255 && B <= 255 && A <= 255 &&
                R >= 0 && G >= 0 && B >= 0 && A >= 0)
            {
                ChosenColour = Color.FromArgb(A, R, G, B);
            }
        }
    }
}