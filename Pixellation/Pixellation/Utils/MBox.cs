using System.Windows;

namespace Pixellation.Utils
{
    /// <summary>
    /// Helper class for showing different types of messageboxes.
    /// </summary>
    public static class MBox
    {
        /// <summary>
        /// Shows a warning with the given message.
        /// </summary>
        /// <param name="msg">Message to show.</param>
        public static void Warning(string msg)
        {
            MessageBox.Show(
                msg,
                Properties.Messages.CaptionWarning,
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        /// <summary>
        /// Shows an error with the given message.
        /// </summary>
        /// <param name="msg">Message to show.</param>
        public static void Error(string msg)
        {
            MessageBox.Show(
                msg,
                Properties.Messages.CaptionWarning,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        /// <summary>
        /// Shows an info messagebox with the given message.
        /// </summary>
        /// <param name="msg">Message to show.</param>
        public static void Info(string msg)
        {
            MessageBox.Show(
                msg,
                Properties.Messages.CaptionWarning,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Shows a question messagebox.
        /// </summary>
        /// <param name="question">Question to ask from the user.</param>
        /// <param name="title">Title of the messagebox.</param>
        /// <returns>Answer of the user.</returns>
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