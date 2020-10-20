using Pixellation.Utils;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            var temp = new Uri(@"pack://application:,,,/Resources/colourwheel_180x180.png");
            colourWheel = new Bitmap(Application.GetResourceStream(temp).Stream);

            RaiseChosenColourPropertyChangeEventHandlerEvent += (s, a) => { SetCcRectangleFill(); };
        }

        private void SetChosenColourFromMousePosition(System.Windows.Point mousePos)
        {
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
            SetChosenColourFromMousePosition(e.GetPosition(colourWheelVisual));
        }

        private void ColourWheelVisual_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetChosenColourFromMousePosition(e.GetPosition(colourWheelVisual));
            }
        }

        private void ccBrightness_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
        }
    }
}