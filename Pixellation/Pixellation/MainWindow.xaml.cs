using Microsoft.Win32;
using Pixellation.Components.Dialogs;
using Pixellation.Components.Dialogs.AboutDialog;
using Pixellation.Properties;
using Pixellation.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Pixellation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly PersistenceManager _pm = PersistenceManager.GetInstance();
        private readonly PixellationCaretakerManager _caretaker = PixellationCaretakerManager.GetInstance();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _thereAreUnsavedChanges;
        private string _programTitle;
        private string _projectTitle;

        private string TitleStringTemplate => $"{ProgramTitle} - {ProjectTitle} {(ThereAreUnsavedChanges ? " (*)" : "")}";

        public bool ThereAreUnsavedChanges
        {
            get
            {
                return _thereAreUnsavedChanges;
            }
            set
            {
                _thereAreUnsavedChanges = value;
                Title = TitleStringTemplate;
            }
        }

        private string ProgramTitle
        {
            get
            {
                return _programTitle;
            }
            set
            {
                _programTitle = value;
                Title = TitleStringTemplate;
            }
        }

        private string ProjectTitle
        {
            get
            {
                return _projectTitle;
            }
            set
            {
                _projectTitle = value;
                Title = TitleStringTemplate;
            }
        }

        public new string Title
        {
            get
            {
                return base.Title;
            }
            private set
            {
                base.Title = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            GetWindow(this).KeyDown += canvasImage.OnKeyDown;

            ProgramTitle = Properties.Resources.Title;
            ProjectTitle = Properties.Resources.DefaultProjectTitle;

            _caretaker.OnNewUndoAdded += (d, e) => { ThereAreUnsavedChanges = true; };
            _caretaker.OnNewRedoAdded += (d, e) => { ThereAreUnsavedChanges = true; };

            WindowState = WindowState.Maximized;
        }

        private async void Open(object sender, RoutedEventArgs e)
        {
            if (ThereAreUnsavedChanges)
            {
                var closeWindowDialog = new UnsavedChangesDialog("You should save before opening another image or project!");
                if (closeWindowDialog.ShowDialog() == true)
                {
                    var ans = closeWindowDialog.Answer;

                    if (ans == UnsavedChangesDialog.CloseDialogAnswer.SAVE)
                    {
                        SaveProject(this, new RoutedEventArgs());
                    }
                }
                else
                {
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = Properties.Resources.OpenFileFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                string extension = fileName.Split('.')[^1];
                if (extension == Properties.Resources.ExtensionForProjectFilePackage)
                {
                    // Getting Project

                    var data = await _pm.LoadProject(fileName, canvasImage);

                    ProjectTitle = Properties.Resources.Title + " - " + data.Key;
                    ThereAreUnsavedChanges = false;
                    _caretaker.ClearAll();
                    _pm.Reset();

                    canvasImage.NewProject(data.Value);
                }
                else
                {
                    // Getting Bitmap

                    ProjectTitle = Properties.Resources.Title + " - " + fileName.Split('.')[0].Split('\\')[^1];
                    ThereAreUnsavedChanges = false;
                    _caretaker.ClearAll();
                    _pm.Reset();

                    var wrbmp = _pm.LoadImage(fileName);
                    canvasImage.NewProject(wrbmp);
                }
            }
        }

        private void SaveProject(object sender, RoutedEventArgs e)
        {
            if (_pm.AlreadySaved)
            {
                _pm.SaveProject(canvasImage.Frames);

                ProjectTitle = Properties.Resources.Title + " - " + _pm.PreviousFullPath.Split('.')[0].Split('\\')[^1];
                ThereAreUnsavedChanges = false;
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = Properties.Resources.SaveFileFilter
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    _pm.SaveProject(canvasImage.Frames, fileName);

                    ProjectTitle = Properties.Resources.Title + " - " + fileName.Split('.')[0].Split('\\')[^1];
                    ThereAreUnsavedChanges = false;
                }
            }
        }

        private void ExportAsImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = Properties.Resources.ExportFilter
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                _pm.ExportAsImage(fileName, canvasImage);
            }
        }

        private void OpenNewImageDialog(object sender, RoutedEventArgs e)
        {
            if (ThereAreUnsavedChanges)
            {
                var closeWindowDialog = new UnsavedChangesDialog("You should save before editing a new image!");
                if (closeWindowDialog.ShowDialog() == true)
                {
                    var ans = closeWindowDialog.Answer;

                    if (ans == UnsavedChangesDialog.CloseDialogAnswer.SAVE)
                    {
                        SaveProject(this, new RoutedEventArgs());
                    }
                }
                else
                {
                    return;
                }
            }

            var newImgDialog = new NewProjectDialog();
            if (newImgDialog.ShowDialog() == true)
            {
                // Get Data
                var widthHeight = newImgDialog.Answer;
                var w = int.Parse(widthHeight.Split(';')[0]);
                var h = int.Parse(widthHeight.Split(';')[1]);

                _pm.Reset();
                _caretaker.ClearAll();
                ThereAreUnsavedChanges = false;
                ProjectTitle = Properties.Resources.DefaultProjectTitle;

                // New Project
                canvasImage.NewProject(w, h);
            }
        }

        private void OpenAboutDialog(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S)
            {
                SaveProject(this, new RoutedEventArgs());
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Z)
            {
                _caretaker.Undo();
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Y)
            {
                _caretaker.Redo();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (ThereAreUnsavedChanges)
            {
                var closeWindowDialog = new UnsavedChangesDialog("Are you sure about closing the program?");
                if (closeWindowDialog.ShowDialog() == true)
                {
                    var ans = closeWindowDialog.Answer;

                    if (ans == UnsavedChangesDialog.CloseDialogAnswer.SAVE)
                    {
                        SaveProject(this, new RoutedEventArgs());
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Used as change notification for one- and twoway binding with <see cref="DependencyProperty"/> objects.
        /// </summary>
        /// <param name="name">Name of the caller property.</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}