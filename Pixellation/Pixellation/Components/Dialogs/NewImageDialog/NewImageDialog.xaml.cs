using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pixellation.Components.Dialogs.NewImageDialog
{
    /// <summary>
    /// Interaction logic for NewImageDialog.xaml
    /// </summary>
    public partial class NewImageDialog : Window
    {
        public NewImageDialog()
        {
            InitializeComponent();
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public string Answer
        {
            get { return txtWidth.Text + ";" + txtHeight.Text; }
        }
    }
}
