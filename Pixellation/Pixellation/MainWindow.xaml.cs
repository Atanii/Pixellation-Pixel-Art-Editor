using Microsoft.Win32;
using Pixellation.Components.Dialogs;
using Pixellation.Components.Dialogs.AboutDialog;
using Pixellation.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Pixellation
{
    using Res = Properties.Resources;

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

        /// <summary>
        /// Template for the title including a conditionally shown asterisk indicating unsaved changes.
        /// </summary>
        private string TitleStringTemplate => $"{ProgramTitle} - {ProjectTitle} {(ThereAreUnsavedChanges ? " (*)" : "")}";

        /// <summary>
        /// Marks if there are changes in the project that can be saved.
        /// </summary>
        public bool ThereAreUnsavedChanges
        {
            get => _thereAreUnsavedChanges;
            set
            {
                _thereAreUnsavedChanges = value;
                Title = TitleStringTemplate;
            }
        }

        /// <summary>
        /// Title of the program.
        /// </summary>
        private string ProgramTitle
        {
            get => _programTitle;
            set
            {
                _programTitle = value;
                Title = TitleStringTemplate;
            }
        }

        /// <summary>
        /// Title of the currently opened project.
        /// </summary>
        private string ProjectTitle
        {
            get => _projectTitle;
            set
            {
                _projectTitle = value;
                Title = TitleStringTemplate;
            }
        }

        /// <summary>
        /// Title shown on the window of the program.
        /// </summary>
        public new string Title
        {
            get => base.Title;
            private set
            {
                base.Title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Constructor for <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Proxying event to PixelEditor instance
            GetWindow(this).KeyDown += canvasImage.OnKeyDown;

            // Setting titles
            ProgramTitle = Res.Title;
            ProjectTitle = Res.DefaultProjectTitle;

            // To know when there is a new change that can be saved
            _caretaker.OnNewUndoAdded += (d, e) => { ThereAreUnsavedChanges = true; };
            _caretaker.OnNewRedoAdded += (d, e) => { ThereAreUnsavedChanges = true; };

            // Automaximize at start
            WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// Opens a project or an image after asking for location (and for saving unsaved changes if there is any).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Menu_Open(object sender, RoutedEventArgs e)
        {
            if (ThereAreUnsavedChanges)
            {
                var closeWindowDialog = new UnsavedChangesDialog("You should save before opening another image or project!");
                if (closeWindowDialog.ShowDialog() == true)
                {
                    var ans = closeWindowDialog.Answer;

                    if (ans == UnsavedChangesDialog.CloseDialogAnswer.SAVE)
                    {
                        if (!SaveProject(this, new RoutedEventArgs()))
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = Res.OpenFileFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                string extension = fileName.Split('.')[^1];
                if (extension == Res.ExtensionForProjectFilePackage)
                {
                    // Getting Project
                    var data = await _pm.LoadProject(fileName, canvasImage);
                    if (data.Value != null)
                    {
                        ProjectTitle = Res.Title + " - " + data.Key;
                        ThereAreUnsavedChanges = false;
                        _caretaker.ClearAll();
                        _pm.Reset();

                        canvasImage.NewProject(data.Value);
                    }
                }
                else
                {
                    // Getting Bitmap
                    var wrbmp = _pm.LoadImage(fileName);
                    if (wrbmp != null)
                    {
                        ProjectTitle = Res.Title + " - " + fileName.Split('.')[0].Split('\\')[^1];
                        ThereAreUnsavedChanges = false;
                        _caretaker.ClearAll();
                        _pm.Reset();

                        canvasImage.NewProject(wrbmp);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the project after choosing the location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>True if project is successfully saved, otherwise false.</returns>
        private void Menu_SaveProject(object sender, RoutedEventArgs e)
        {
            SaveProject(sender, e);
        }

        private bool SaveProject(object sender, RoutedEventArgs e)
        {
            if (_pm.AlreadySaved)
            {
                var res = _pm.SaveProject(canvasImage.Frames);

                if (res)
                {
                    ProjectTitle = Res.Title + " - " + _pm.PreviousFullPath.Split('.')[0].Split('\\')[^1];
                    ThereAreUnsavedChanges = false;
                }
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = Res.SaveFileFilter
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    var res = _pm.SaveProject(canvasImage.Frames, fileName);

                    if (res)
                    {
                        ProjectTitle = Res.Title + " - " + fileName.Split('.')[0].Split('\\')[^1];
                        ThereAreUnsavedChanges = false;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Opens a <see cref="NewProjectDialog"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_NewImage(object sender, RoutedEventArgs e)
        {
            if (ThereAreUnsavedChanges)
            {
                var closeWindowDialog = new UnsavedChangesDialog("You should save before editing a new image!");
                if (closeWindowDialog.ShowDialog() == true)
                {
                    var ans = closeWindowDialog.Answer;

                    if (ans == UnsavedChangesDialog.CloseDialogAnswer.SAVE)
                    {
                        if (!SaveProject(this, new RoutedEventArgs()))
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            var newImgDialog = new DualInputDialog("New Project", "Width", "Height");
            if (newImgDialog.ShowDialog() == true)
            {
                // Get Data
                var widthHeight = newImgDialog.Answer;
                var w = int.Parse(widthHeight.Split(';')[0]);
                var h = int.Parse(widthHeight.Split(';')[1]);

                _pm.Reset();
                _caretaker.ClearAll();
                ThereAreUnsavedChanges = false;
                ProjectTitle = Res.DefaultProjectTitle;

                // New Project
                canvasImage.NewProject(w, h);
            }
        }

        /// <summary>
        /// Opens <see cref="AboutDialog"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_About(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }

        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
        }

        private void ExportAsImage(ExportModes mode)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = Res.ExportFilter
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                _pm.ExportAsImage(fileName, canvasImage, mode);
            }
        }

        private void ExportToSpritesheet(ExportModes mode)
        {
            if (mode < ExportModes.SPRITESHEET_FRAME)
            {
                return;
            }
            var newImgDialog = new DualInputDialog("Export To Spritesheet", "Rows", "Cols");
            if (newImgDialog.ShowDialog() == true)
            {
                // Get Data
                var rowsCols = newImgDialog.Answer;
                var rows = int.Parse(rowsCols.Split(';')[0]);
                var cols = int.Parse(rowsCols.Split(';')[1]);

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = Res.ExportFilter
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    _pm.ExportAsImage(fileName, canvasImage, mode, rows, cols);
                }
            }
        }

        /// <summary>
        /// Opens a <see cref="SaveFileDialog"/> for choosing location and name for the exported file then exports the project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_ExportSelectedLayer(object sender, RoutedEventArgs e)
        {
            ExportAsImage(ExportModes.LAYER);
        }

        private void Menu_ExportSelectedFrame(object sender, RoutedEventArgs e)
        {
            ExportAsImage(ExportModes.FRAME);
        }

        private void Menu_ExportAllFrames(object sender, RoutedEventArgs e)
        {
            ExportAsImage(ExportModes.FRAME_ALL);
        }

        private void Menu_SpritesheetFromFrame(object sender, RoutedEventArgs e)
        {   
            ExportToSpritesheet(ExportModes.SPRITESHEET_FRAME);
        }

        private void Menu_SpritesheetFromAllFrames(object sender, RoutedEventArgs e)
        {
            ExportToSpritesheet(ExportModes.SPRITESHEET_ALL_FRAME);
        }

        /// <summary>
        /// Overrides <see cref="OnKeyDown(KeyEventArgs)"/> for handling hotkeys for save, undo, ...
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// Called when the user wants to exit the program.
        /// Shows a warning and asks if the user'd like to save unsaved changes (if there is any).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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