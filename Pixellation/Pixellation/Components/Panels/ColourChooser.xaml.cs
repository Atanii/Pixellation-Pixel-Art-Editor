using Pixellation.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    using Color = System.Drawing.Color;
    using Rectangle = System.Windows.Shapes.Rectangle;

    /// <summary>
    /// Interaction logic for ColourPicker.xaml
    /// </summary>
    public partial class ColourChooser : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Event used for one- and twoway databinding.
        /// Marks change regarding one of the properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private Color _primaryColor;

        public Color PrimaryColor
        {
            get => _primaryColor;
            set
            {
                _primaryColor = value;
                Refresh();
                SetHueFrom(value);
                OnPropertyChanged();
            }
        }

        private Color _secondaryColor;

        public Color SecondaryColor
        {
            get => _secondaryColor;
            set
            {
                _secondaryColor = value;
                Refresh();
                SetHueFrom(value);
                OnPropertyChanged();
            }
        }

        private Color PrimaryColorLocal
        {
            get => PrimaryColor;
            set
            {
                _primaryColor = value;
                Refresh();
                OnPropertyChanged(nameof(PrimaryColor));
            }
        }

        private Color SecondaryColorLocal
        {
            get => SecondaryColor;
            set
            {
                _secondaryColor = value;
                Refresh();
                OnPropertyChanged(nameof(SecondaryColor));
            }
        }

        public ColourChooser()
        {
            InitializeComponent();

            PrimaryColorLocal = Properties.Settings.Default.DefaultPrimaryColor;
            SecondaryColorLocal = Properties.Settings.Default.DefaultSecondaryColor;

            Resources["HueColor"] = Properties.Settings.Default.DefaultHueColor.ToMediaColor();

            SetCcRectanglesFill();
            SetRGBATxtInputs();
        }

        private void Refresh()
        {
            SetCcRectanglesFill();
            SetRGBATxtInputs();
        }

        private void SetHueFrom(Color color)
        {
            var hsl = ColorUtils.ToHSL(color.R, color.G, color.B);
            hsl.S = 1;
            Resources["HueColor"] = ColorUtils.ToRGB(hsl);
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
                    PrimaryColorLocal = colour;
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                var colour = GetPixelColor(colourGradientCanvas, e.GetPosition(colourGradientCanvas));
                if (colour != null)
                {
                    SecondaryColorLocal = colour;
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

        private void ColourGradientCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            SetChosenColourFromMousePosition(e);
        }

        private void ColourWheelVisualHue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetHueColourFromMousePosition(e.GetPosition(colourGradientHue));
        }

        private void Sc_TextInput(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(scR.Text, out int R) &&
                int.TryParse(scG.Text, out int G) &&
                int.TryParse(scB.Text, out int B) &&
                int.TryParse(scA.Text, out int A) &&
                R <= 255 && G <= 255 && B <= 255 && A <= 255 &&
                R >= 0 && G >= 0 && B >= 0 && A >= 0)
            {
                PrimaryColorLocal = Color.FromArgb(A, R, G, B);
            }
        }

        private void Sc2_TextInput(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(scR2.Text, out int R) &&
                int.TryParse(scG2.Text, out int G) &&
                int.TryParse(scB2.Text, out int B) &&
                int.TryParse(scA2.Text, out int A) &&
                R <= 255 && G <= 255 && B <= 255 && A <= 255 &&
                R >= 0 && G >= 0 && B >= 0 && A >= 0)
            {
                SecondaryColorLocal = Color.FromArgb(A, R, G, B);
            }
        }

        private void BtnSwapColors_Click(object sender, RoutedEventArgs e)
        {
            var tmp = SecondaryColorLocal;
            SecondaryColorLocal = PrimaryColorLocal;
            PrimaryColorLocal = tmp;
        }

        /// <summary>
        /// Used as change notification for one- and twoway binding with <see cref="DependencyProperty"/> objects.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}