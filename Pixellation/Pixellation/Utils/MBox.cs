using System.Windows;

namespace Pixellation.Utils
{
    public static class MBox
    {
        public static void Warning(string msg)
        {
            MessageBox.Show(
                msg,
                Properties.Messages.CaptionWarning,
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        public static void Error(string msg)
        {
            MessageBox.Show(
                msg,
                Properties.Messages.CaptionWarning,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        public static void Info(string msg)
        {
            MessageBox.Show(
                msg,
                Properties.Messages.CaptionWarning,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        public static MessageBoxResult Question(string question, string title)
        {
            return MessageBox.Show(
                question,
                title,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel
            );
        }
    }
}