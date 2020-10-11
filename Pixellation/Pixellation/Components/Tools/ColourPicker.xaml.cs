using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pixellation.Components.Tools
{
    /// <summary>
    /// Interaction logic for ColourPicker.xaml
    /// </summary>
    public partial class ColourPicker : UserControl
    {
        private readonly Bitmap colourWheel;
        public System.Drawing.Color ChosenColour { get; private set; }

        public ColourPicker()
        {
            InitializeComponent();
           // colourWheel = new Bitmap(@"/Resources/colourwheel_180x180.png");
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*System.Windows.Point mousePos = e.GetPosition(colourPickerWheel);
            System.Drawing.Color colour = colourWheel.GetPixel(
                (int) mousePos.X,
                (int) mousePos.Y
            );
            if (colour != null)
            {
                ChosenColour = colour;
                var cc = new System.Windows.Media.Color();
                cc.R = colour.R;
                cc.G = colour.G;
                cc.B = colour.B;
                cc.A = colour.A;
                ccLabel.Content = colour.ToString();
                ccRectangle.Fill = new System.Windows.Media.SolidColorBrush(cc);
            }*/

        }
    }
}
