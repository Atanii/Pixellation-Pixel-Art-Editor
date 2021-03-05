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

            // Signals to PixelEditor if mouse left the container element.
            canvasScroll.MouseMove += (sender, mouseArgs) =>
            {
                var p = mouseArgs.GetPosition(canvasScroll);
                canvasImage.MouseLeftContainerElement(
                    !(p.X >= 0 && p.Y >= 0 && p.X < canvasScroll.ActualWidth && p.Y < canvasScroll.ActualHeight),
                    mouseArgs
                );
            };
        }

        /// <summary>
        /// Opens a project or an image after asking for location (and for saving unsaved changes if there is any).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Open(object sender, RoutedEventArgs e) => OpenProjectOrImage();

        public async void OpenProjectOrImage(string filePath = "")
        {
            if (!AskAndTrySaveUnsavedChanges())
            {
                return;
            }

            if (filePath == string.Empty)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = Res.OpenFileFilter
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            string extension = filePath.GetExtension();
            if (extension == Res.ExtensionForProjectFilePackage)
            {
                // Getting Project
                var data = await _pm.LoadProject(filePath, canvasImage);
                if (data.Value != null)
                {
                    ProjectTitle = data.Key;
                    ThereAreUnsavedChanges = false;
                    _caretaker.ClearAll();

                    canvasImage.NewProject(data.Value);
                }
            }
            else
            {
                // Getting Bitmap
                var wrbmp = _pm.LoadImage(filePath);
                if (wrbmp != null)
                {
                    ProjectTitle = filePath.GetFileNameWithoutExtension();
                    ThereAreUnsavedChanges = false;
                    _caretaker.ClearAll();
                    _pm.Reset();

                    canvasImage.NewProject(wrbmp);
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
            SaveProject(false);
        }

        /// <summary>
        /// Saves the project after choosing the location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>True if project is successfully saved, otherwise false.</returns>
        private void Menu_SaveProjectAs(object sender, RoutedEventArgs e)
        {
            SaveProject(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="saveAs"></param>
        /// <returns></returns>
        private bool SaveProject(bool saveAs = false)
        {
            if (!saveAs && _pm.AlreadySaved)
            {
                var res = _pm.SaveProject(canvasImage.Frames, ProjectTitle);

                if (res)
                {
                    ThereAreUnsavedChanges = false;
                    return true;
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
                    var res = _pm.SaveProject(canvasImage.Frames, ProjectTitle, fileName);

                    if (res)
                    {
                        ProjectTitle = ProjectTitle == Res.DefaultProjectTitle ? fileName.GetFileNameWithoutExtension() : ProjectTitle;
                        ThereAreUnsavedChanges = false;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Ask for a location before save if there are unsaved changes and save.
        /// </summary>
        /// <returns>True if save if successful, false otherwise.</returns>
        private bool AskAndTrySaveUnsavedChanges()
        {
            if (ThereAreUnsavedChanges)
            {
                var res = MBox.Question(Properties.Messages.WouldYouLikeToSave, Properties.Messages.WarningUnsavedChanges);

                if (res == MessageBoxResult.Cancel)
                {
                    return false;
                }

                if (res == MessageBoxResult.Yes)
                {
                    if (!SaveProject())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Opens a <see cref="NewProjectDialog"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_NewImage(object sender, RoutedEventArgs e)
        {
            if (!AskAndTrySaveUnsavedChanges())
            {
                return;
            }

            var newImgDialog = new StringTripleDialog("New Project", "Project Name", "Width", "Height");
            if (newImgDialog.ShowDialog() == true)
            {
                // Get Data
                var nameAndSize = newImgDialog.Answer;

                ProjectTitle = nameAndSize.Split(';')[0];

                var w = int.Parse(nameAndSize.Split(';')[1]);
                var h = int.Parse(nameAndSize.Split(';')[2]);

                _pm.Reset();
                _caretaker.ClearAll();
                ThereAreUnsavedChanges = false;

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

        /// <summary>
        /// Closes program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
        }

        /// <summary>
        /// Ask for location then exports the project into the chosen format.
        /// </summary>
        /// <param name="mode">Export format.</param>
        private void ExportAsImage(ExportModes mode)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = Res.ExportFilter,
                DefaultExt = (mode > ExportModes.SPRITESHEET_ALL_FRAME) ? "gif" : "png"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                _pm.ExportAsImage(fileName, canvasImage, mode);
            }
        }

        /// <summary>
        /// Exports the project into a spritesheet.
        /// </summary>
        /// <param name="mode"></param>
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
                    Filter = Res.ExportFilter,
                    DefaultExt = "png"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    _pm.ExportAsImage(fileName, canvasImage, mode, rows, cols);
                }
            }
        }

        /// <summary>
        /// Opens a <see cref="SaveFileDialog"/> for choosing location and name for the exported file then exports the selected layer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_ExportSelectedLayer(object sender, RoutedEventArgs e)
        {
            ExportAsImage(ExportModes.LAYER);
        }

        /// <summary>
        /// Opens a <see cref="SaveFileDialog"/> for choosing location and name for the exported file then exports the selected frame.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_ExportSelectedFrame(object sender, RoutedEventArgs e)
        {
            ExportAsImage(ExportModes.FRAME);
        }

        /// <summary>
        /// Opens a <see cref="SaveFileDialog"/> for choosing location and name for the exported file then exports all frames.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_ExportAllFrames(object sender, RoutedEventArgs e)
        {
            ExportAsImage(ExportModes.FRAME_ALL);
        }

        /// <summary>
        /// Opens a <see cref="SaveFileDialog"/> for choosing location and name for the exported file then exports the selected frame into a spritesheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_SpritesheetFromFrame(object sender, RoutedEventArgs e)
        {
            ExportToSpritesheet(ExportModes.SPRITESHEET_FRAME);
        }

        /// <summary>
        /// Opens a <see cref="SaveFileDialog"/> for choosing location and name for the exported file then exports all the frames into a spritesheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_SpritesheetFromAllFrames(object sender, RoutedEventArgs e)
        {
            ExportToSpritesheet(ExportModes.SPRITESHEET_ALL_FRAME);
        }

        /// <summary>
        /// Opens a <see cref="SaveFileDialog"/> for choosing location and name for the exported file then exports the selected frame into a gif.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_GifFromFrame(object sender, RoutedEventArgs e)
        {
            ExportAsImage(ExportModes.GIF_FRAME);
        }

        /// <summary>
        /// Opens a <see cref="SaveFileDialog"/> for choosing location and name for the exported file then exports the frames into a gif.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_GifFromAllFrames(object sender, RoutedEventArgs e)
        {
            ExportAsImage(ExportModes.GIF_ALL_FRAMES);
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
                SaveProject();
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
            e.Cancel = !AskAndTrySaveUnsavedChanges();
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