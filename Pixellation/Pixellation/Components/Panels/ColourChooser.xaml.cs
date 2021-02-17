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
        public Color PrimaryColor
        {
            get { return (Color)GetValue(PrimaryColourProperty); }
            set
            {
                SetValue(PrimaryColourProperty, value);
                SetCcRectanglesFill();
                SetRGBATxtInputs();
            }
        }

        public static readonly DependencyProperty PrimaryColourProperty =
         DependencyProperty.Register("PrimaryColor", typeof(Color), typeof(ColourChooser), new FrameworkPropertyMetadata(
            Properties.Settings.Default.DefaultPrimaryColor, (d, e) => { RaiseChosenColourPropertyChangeEventHandlerEvent?.Invoke(default, EventArgs.Empty); }));

        public Color SecondaryColor
        {
            get { return (Color)GetValue(SecondaryColourProperty); }
            set
            {
                SetValue(SecondaryColourProperty, value);
                SetCcRectanglesFill();
                SetRGBATxtInputs();
            }
        }

        public static readonly DependencyProperty SecondaryColourProperty =
         DependencyProperty.Register("SecondaryColor", typeof(Color), typeof(ColourChooser), new FrameworkPropertyMetadata(
            Properties.Settings.Default.DefaultSecondaryColor, (d, e) => { RaiseChosenColourPropertyChangeEventHandlerEvent?.Invoke(default, EventArgs.Empty); }));

        private delegate void ChosenColourPropertyChangeEventHandler(object sender, EventArgs args);

        private static event ChosenColourPropertyChangeEventHandler RaiseChosenColourPropertyChangeEventHandlerEvent;

        public ColourChooser()
        {
            InitializeComponent();
            Resources["HueColor"] = Properties.Settings.Default.DefaultHueColor.ToMediaColor();
            SetCcRectanglesFill();
            RaiseChosenColourPropertyChangeEventHandlerEvent += (s, a) => {
                SetCcRectanglesFill();
                SetRGBATxtInputs();
            };
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

        private void SetChosenColourFromMousePosition(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var colour = GetPixelColor(colourGradientCanvas, e.GetPosition(colourGradientCanvas));
                if (colour != null)
                {
                    PrimaryColor = colour;
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                var colour = GetPixelColor(colourGradientCanvas, e.GetPosition(colourGradientCanvas));
                if (colour != null)
                {
                    SecondaryColor = colour;
                }
            }
        }

        private void SetHueColourFromMousePosition(Point mousePos)
        {
            var colour = GetPixelColor(colourGradientHue, mousePos);
            if (colour != null)
            {
                Resources["HueColor"] = colour.ToMediaColor();
            }
        }

        private void SetCcRectanglesFill()
        {
            Resources["PrimaryColor"] = PrimaryColor.ToMediaColor();
            Resources["SecondaryColor"] = SecondaryColor.ToMediaColor();
        }

        private void SetRGBATxtInputs()
        {
            scR.Text = PrimaryColor.R.ToString();
            scG.Text = PrimaryColor.G.ToString();
            scB.Text = PrimaryColor.B.ToString();
            scA.Text = PrimaryColor.A.ToString();

            scR2.Text = SecondaryColor.R.ToString();
            scG2.Text = SecondaryColor.G.ToString();
            scB2.Text = SecondaryColor.B.ToString();
            scA2.Text = SecondaryColor.A.ToString();
        }

        private void ColourWheelVisual_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetChosenColourFromMousePosition(e);
        }

        private void colourGradientCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            SetChosenColourFromMousePosition(e);
        }

        private void ColourWheelVisualHue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetHueColourFromMousePosition(e.GetPosition(colourGradientHue));
        }


        private void sc_TextInput(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(scR.Text, out int R) &&
                int.TryParse(scG.Text, out int G) &&
                int.TryParse(scB.Text, out int B) &&
                int.TryParse(scA.Text, out int A) &&
                R <= 255 && G <= 255 && B <= 255 && A <= 255 &&
                R >= 0 && G >= 0 && B >= 0 && A >= 0)
            {
                PrimaryColor = Color.FromArgb(A, R, G, B);
            }
        }

        private void sc2_TextInput(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(scR2.Text, out int R) &&
                int.TryParse(scG2.Text, out int G) &&
                int.TryParse(scB2.Text, out int B) &&
                int.TryParse(scA2.Text, out int A) &&
                R <= 255 && G <= 255 && B <= 255 && A <= 255 &&
                R >= 0 && G >= 0 && B >= 0 && A >= 0)
            {
                SecondaryColor = Color.FromArgb(A, R, G, B);
            }
        }

        private void btnSwapColors_Click(object sender, RoutedEventArgs e)
        {
            var tmp = SecondaryColor;
            SecondaryColor = PrimaryColor;
            PrimaryColor = tmp;
        }
    }
}