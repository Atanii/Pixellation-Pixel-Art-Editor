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
        #region Fields and properties.

        /// <summary>
        /// Event used for one- and twoway databinding.
        /// Marks change regarding one of the properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Primary color.
        /// </summary>
        private Color _primaryColor;

        /// <summary>
        /// PrimaryColor used with left-click.
        /// </summary>
        public Color PrimaryColor
        {
            get => _primaryColor;
            set
            {
                _primaryColor = value;
                Refresh();
                SetHueFrom(value);
                SetSliderValues();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Secondary color.
        /// </summary>
        private Color _secondaryColor;

        /// <summary>
        /// SecondaryColor used with right-click.
        /// </summary>
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

        /// <summary>
        /// PrimaryColor proxy-property for locally made changes (picking color, setting sliders, ...).
        /// </summary>
        private Color PrimaryColorLocal
        {
            get => PrimaryColor;
            set
            {
                _primaryColor = value;
                Refresh();
                // Proxying notification for property PrimaryColor.
                OnPropertyChanged(nameof(PrimaryColor));
            }
        }

        /// <summary>
        /// SecondaryColor proxy-property for locally made changes (picking color, setting sliders, ...).
        /// </summary>
        private Color SecondaryColorLocal
        {
            get => SecondaryColor;
            set
            {
                _secondaryColor = value;
                Refresh();
                // Proxying notification for property SecondaryColor.
                OnPropertyChanged(nameof(SecondaryColor));
            }
        }

        #endregion Fields and properties.

        /// <summary>
        /// Inits the default colors and sets the ui elements according to the default colors.
        /// </summary>
        public ColourChooser()
        {
            InitializeComponent();

            PrimaryColorLocal = Properties.Settings.Default.DefaultPrimaryColor;
            SecondaryColorLocal = Properties.Settings.Default.DefaultSecondaryColor;

            Resources["HueColor"] = Properties.Settings.Default.DefaultHueColor.ToMediaColor();

            SetCcRectanglesFill();
            SetRGBATxtInputs();
            SetSliderValues();
        }

        /// <summary>
        /// Updates the two color indicating rectangle and the RGBA inputs and the color usersettings.
        /// </summary>
        private void Refresh()
        {
            // Updating default colors
            Properties.Settings.Default.DefaultPrimaryColor = PrimaryColor;
            Properties.Settings.Default.DefaultSecondaryColor = SecondaryColor;

            // Updating UI and inputs
            SetCcRectanglesFill();
            SetRGBATxtInputs();
        }

        /// <summary>
        /// Gets the color from the given colored rectangle based on the given mouse position.
        /// </summary>
        /// <param name="cvs">Rectangle, preferably the colorpicker rectangle.</param>
        /// <param name="mousePos">Mouseposition.</param>
        /// <returns></returns>
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

        #region Setters

        /// <summary>
        /// Sets the hue from a given <see cref="Color"/>.
        /// </summary>
        /// <param name="color"></param>
        private void SetHueFrom(Color color)
        {
            var hsl = ColorUtils.ToHSL(color.R, color.G, color.B);
            hsl.S = 1;
            var tmp = ColorUtils.ToRGB(hsl);
            Resources["HueColor"] = ColorUtils.ToRGB(hsl);
            Properties.Settings.Default.DefaultHueColor = tmp.ToDrawingColor();
        }

        /// <summary>
        /// Sets the color from the colorpicker based on the given mouse position.
        /// </summary>
        /// <param name="e"></param>
        private void SetChosenColourFromMousePosition(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var colour = GetPixelColor(colourGradientCanvas, e.GetPosition(colourGradientCanvas));
                if (colour != null)
                {
                    PrimaryColorLocal = colour;
                    SetSliderValues();
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

        /// <summary>
        /// Sets the hue from the hue-ribbon based on the given mouse position.
        /// </summary>
        /// <param name="mousePos"></param>
        private void SetHueColourFromMousePosition(Point mousePos)
        {
            var colour = GetPixelColor(colourGradientHue, mousePos);
            if (colour != null)
            {
                Resources["HueColor"] = colour.ToMediaColor();
                Properties.Settings.Default.DefaultHueColor = colour;
            }
        }

        /// <summary>
        /// Sets the fill colours of the two chosen color indicating rectangles.
        /// </summary>
        private void SetCcRectanglesFill()
        {
            Resources["PrimaryColor"] = PrimaryColor.ToMediaColor();
            Resources["SecondaryColor"] = SecondaryColor.ToMediaColor();
        }

        /// <summary>
        /// Updates the RGBA input fields from both <see cref="PrimaryColor"/> and <see cref="SecondaryColor"/>.
        /// </summary>
        private void SetRGBATxtInputs()
        {
            // PrimaryColor
            scR.Text = PrimaryColor.R.ToString();
            scG.Text = PrimaryColor.G.ToString();
            scB.Text = PrimaryColor.B.ToString();
            scA.Text = PrimaryColor.A.ToString();

            // SecondaryColor
            scR2.Text = SecondaryColor.R.ToString();
            scG2.Text = SecondaryColor.G.ToString();
            scB2.Text = SecondaryColor.B.ToString();
            scA2.Text = SecondaryColor.A.ToString();
        }

        /// <summary>
        /// Updates the HSL sliders from the <see cref="PrimaryColor"/>.
        /// </summary>
        private void SetSliderValues()
        {
            var tmp = ColorUtils.ToHSL(PrimaryColor.R, PrimaryColor.G, PrimaryColor.B);
            HueSlider.Value = tmp.H;
            SaturationSlider.Value = tmp.S;
            LuminanceSlider.Value = tmp.L;
            AlphaSlider.Value = PrimaryColor.A;
        }

        #endregion Setters

        #region Handling events

        /// <summary>
        /// Picks a color from the colorpicker after pressing the left mouse-button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColourWheelVisual_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetChosenColourFromMousePosition(e);
        }

        /// <summary>
        /// Updates color from colorpicker while moving the mouse if left mouse-button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColourGradientCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            SetChosenColourFromMousePosition(e);
        }

        /// <summary>
        /// Sets the hue from the clicked mouse position on the hue-ribbon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColourWheelVisualHue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetHueColourFromMousePosition(e.GetPosition(colourGradientHue));
        }

        /// <summary>
        /// Updates <see cref="PrimaryColor"/> from the RGBA input fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                SetSliderValues();
            }
        }

        /// <summary>
        /// Updates <see cref="SecondaryColor"/> from the RGBA input fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Swaps the values of <see cref="PrimaryColor"/> and <see cref="SecondaryColor"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSwapColors_Click(object sender, RoutedEventArgs e)
        {
            var tmp = SecondaryColorLocal;
            SecondaryColorLocal = PrimaryColorLocal;
            PrimaryColorLocal = tmp;
            SetSliderValues();
        }

        /// <summary>
        /// Sets the hue of the <see cref="PrimaryColor"/> with the new value from the slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var tmp = ColorUtils.ToHSL(PrimaryColorLocal.R, PrimaryColorLocal.G, PrimaryColorLocal.B);
            tmp.H = (float)e.NewValue;
            PrimaryColorLocal = ColorUtils.ToRGB(tmp).ToDrawingColor();
        }

        /// <summary>
        /// Sets the saturation of the <see cref="PrimaryColor"/> with the new value from the slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var tmp = ColorUtils.ToHSL(PrimaryColorLocal.R, PrimaryColorLocal.G, PrimaryColorLocal.B);
            tmp.S = (float)e.NewValue;
            PrimaryColorLocal = ColorUtils.ToRGB(tmp).ToDrawingColor();
        }

        /// <summary>
        /// Sets the luminance of the <see cref="PrimaryColor"/> with the new value from the slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LuminanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var tmp = ColorUtils.ToHSL(PrimaryColorLocal.R, PrimaryColorLocal.G, PrimaryColorLocal.B);
            tmp.L = (float)e.NewValue;
            PrimaryColorLocal = ColorUtils.ToRGB(tmp).ToDrawingColor();
        }

        /// <summary>
        /// Sets the alpha of the <see cref="PrimaryColor"/> with the new value from the slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var tmp = Color.FromArgb(
                (int)e.NewValue,
                PrimaryColorLocal.R,
                PrimaryColorLocal.G,
                PrimaryColorLocal.B
            );
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

        #endregion Handling events
    }
}