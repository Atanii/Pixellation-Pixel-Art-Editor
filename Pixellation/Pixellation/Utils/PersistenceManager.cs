using Pixellation.Components.Editor;
using Pixellation.FilePackaging;
using Pixellation.Interfaces;
using Pixellation.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Utils
{
    /// <summary>
    /// Class for managing loading and saving the project, loading from and exporting to images.
    /// </summary>
    internal class PersistenceManager
    {
        public readonly string MetadataFileName = Resources.PackageContentFileNameForMetaData + "." + Resources.ExtensionForDataFile;
        public readonly string ProjectDataFileName = Resources.PackageContentFileNameForProjectData + "." + Resources.ExtensionForDataFile;

        private string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private string CurrentSaveDate => DateTime.Now.ToString();

        public bool AlreadySaved { get; private set; }
        public string PreviousFullPath { get; private set; }

        private static PersistenceManager _instance;

        /// <summary>
        /// Gets a singleton instance.
        /// </summary>
        /// <returns>Singleton instance.</returns>
        public static PersistenceManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PersistenceManager();
            }
            return _instance;
        }

        private PersistenceManager()
        {
        }

        /// <summary>
        /// Gives a new <see cref="FilePackageMetadata"/> instance with up-to-date data.
        /// </summary>
        private FilePackageMetadata CurrentMetadata => new FilePackageMetadata
        {
            Version = CurrentVersion,
            SaveDate = CurrentSaveDate
        };

        /// <summary>
        /// Loads a Pixellation project from the given path.
        /// </summary>
        /// <param name="filePath">Path for saved project.</param>
        /// <param name="helper"><see cref="IDrawingHelper"/> for creating loaded list of <see cref="DrawingFrame"/>..</param>
        /// <returns>Project name as key and the list of saved <see cref="DrawingFrame"/> for the project.</returns>
        public async Task<KeyValuePair<string, List<DrawingFrame>>> LoadProject(string filePath, IDrawingHelper helper)
        {
            KeyValuePair<string, List<DrawingFrame>> res = new KeyValuePair<string, List<DrawingFrame>>();

            try
            {
                var fpr = new FilePackageReader(filePath);
                var data = await fpr.LoadProjectModel(MetadataFileName, ProjectDataFileName);
                res = ModelConverterExtensions.GetProjectData(data, helper);

                AlreadySaved = true;
                PreviousFullPath = filePath;
            }
            catch (Exception ex)
            {
                MBox.Error(string.Format(Messages.ErrorWhileLoadingProject, ex.Message));
            }

            return res;
        }

        /// <summary>
        /// Loads an image.
        /// </summary>
        /// <param name="filePath">Path for the image file.</param>
        /// <returns>Loaded image as a <see cref="WriteableBitmap"/>.</returns>
        public WriteableBitmap LoadImage(string filePath)
        {
            WriteableBitmap res = null;

            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                res = new WriteableBitmap(bitmap);
            }
            catch (Exception ex)
            {
                MBox.Error(string.Format(Messages.ErrorWhileLoadingImage, ex.Message));
            }

            return res;
        }

        /// <summary>
        /// Saves the frames into a project file setting filename (without extension) as the title of the project.
        /// </summary>
        /// <param name="frames">List of <see cref="DrawingFrame"/> to save.</param>
        /// <param name="filePath">Savepath.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool SaveProject(List<DrawingFrame> frames, string filePath = "")
        {
            if (filePath == "" && (!AlreadySaved || PreviousFullPath == ""))
            {
                return false;
            }
            else if (AlreadySaved && PreviousFullPath != "")
            {
                filePath = PreviousFullPath;
            }

            var filePaths = new List<string>();

            // Saving ProjectModel
            var pm = ModelConverterExtensions.MakeProjectModel(filePath.GetFileNameWithoutExtension(), frames);
            var projectInfoPath = Resources.PackageContentFileNameForProjectData + "." + Resources.ExtensionForDataFile;
            var jsonString = JsonSerializer.Serialize(pm);
            File.WriteAllText(projectInfoPath, jsonString);
            filePaths.Add(projectInfoPath);

            // Saving Metadata
            var fpmd = CurrentMetadata;
            var metaDataPath = Resources.PackageContentFileNameForMetaData + "." + Resources.ExtensionForDataFile;
            jsonString = JsonSerializer.Serialize(fpmd);
            File.WriteAllText(metaDataPath, jsonString);
            filePaths.Add(metaDataPath);

            // Packaging
            var fp = new FilePackage
            {
                FilePath = filePath,
                ContentFilePathList = filePaths
            };

            var fpwr = new FilePackageWriter(fp);

            try
            {
                fpwr.SaveProjectModel();

                AlreadySaved = true;
                PreviousFullPath = filePath;

                return true;
            }
            catch (Exception ex)
            {
                MBox.Error(string.Format(Messages.ErrorWhileSavingProject, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Resets the "already saved" state.
        /// </summary>
        public void Reset()
        {
            AlreadySaved = false;
            PreviousFullPath = "";
        }

        /// <summary>
        /// Export the edited project (or part of it) to image in the specified format.
        /// </summary>
        /// <param name="filePath">Savepath.</param>
        /// <param name="vm"><see cref="IFrameProvider"/> for getting layers and frames for saving.</param>
        /// <param name="exportMode">Form of export.</param>
        /// <param name="rows">Rows for the case of exporting to spritesheet.</param>
        /// <param name="cols">Cols for the case of exporting to spritesheet.</param>
        public void ExportAsImage(string filePath, IFrameProvider vm, ExportModes exportMode = ExportModes.FRAME, int rows = 0, int cols = 0)
        {
            try
            {
                WriteableBitmap bmp = null;
                IEnumerable<BitmapSource> frames = new List<BitmapSource>();

                switch (exportMode)
                {
                    case ExportModes.LAYER:
                        var index = vm.ActiveLayerIndex;
                        if (index != -1)
                        {
                            bmp = vm.Layers[index].GetWriteableBitmapWithAppliedOpacity();
                        }
                        break;

                    case ExportModes.FRAME:
                        bmp = MergeUtils.MergeAll(vm.Frames[vm.ActiveFrameIndex].Layers);
                        break;

                    case ExportModes.FRAME_ALL:
                        bmp = MergeUtils.MergeAll(vm.Frames);
                        break;

                    case ExportModes.SPRITESHEET_FRAME:
                        bmp = MergeUtils.GenerateSpriteSheetFromLayers(
                            vm.Frames[vm.ActiveFrameIndex].Layers, WriteableBitmapExtensions.BlendMode.Alpha,
                            rows, cols,
                            Colors.Transparent);
                        break;

                    case ExportModes.SPRITESHEET_ALL_FRAME:
                        bmp = MergeUtils.GenerateSpriteSheetFromFrames(
                            vm.Frames, WriteableBitmapExtensions.BlendMode.Alpha,
                            rows, cols,
                            Colors.Transparent);
                        break;

                    case ExportModes.GIF_ALL_FRAMES:
                        frames = vm.GetFramesAsWriteableBitmaps();
                        break;

                    case ExportModes.GIF_FRAME:
                        frames = vm.Frames[vm.ActiveFrameIndex].GetLayersAsBitmapSources();
                        break;

                    default:
                        break;
                }

                var extension = filePath.GetExtension();

                if (extension.ToLower() == "gif")
                {
                    SaveBitmapSourceToGif(filePath, frames);
                }
                else
                {
                    SaveBitmapSourceToFile(filePath, extension, bmp);
                }
            }
            catch (Exception ex)
            {
                MBox.Error(string.Format(Messages.ErrorWhileExportingImage, ex.Message));
            }
        }

        /// <summary>
        /// Saves the given <see cref="BitmapSource"/> to file.
        /// </summary>
        /// <param name="filePath">Savepath.</param>
        /// <param name="extension">Image format.</param>
        /// <param name="image">Actual image to save.</param>
        private void SaveBitmapSourceToFile(string filePath, string extension, BitmapSource image)
        {
            try
            {
                if (filePath != string.Empty)
                {
                    using FileStream fs = new FileStream(filePath, FileMode.Create);
                    BitmapEncoder encoder;

                    switch (extension.ToLower())
                    {
                        case "jpg":
                        case "jpeg":
                            encoder = new JpegBitmapEncoder();
                            break;

                        case "bmp":
                            encoder = new BmpBitmapEncoder();
                            break;

                        case "tif":
                        case "tiff":
                            encoder = new TiffBitmapEncoder();
                            break;

                        case "png":
                        default:
                            encoder = new PngBitmapEncoder();
                            break;
                    }

                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(fs);
                }
            }
            catch (Exception ex)
            {
                MBox.Error(string.Format(Messages.ErrorWhileExportingImage, ex.Message));
            }
        }

        /// <summary>
        /// Saves the given list of <see cref="BitmapSource"/> to a gif using each image as an individual frame.
        /// </summary>
        /// <param name="filePath">Savepath.</param>
        /// <param name="frames">List of frames for the gif animation.</param>
        public void SaveBitmapSourceToGif(string filePath, IEnumerable<BitmapSource> frames)
        {
            try
            {
                if (filePath != string.Empty)
                {
                    using FileStream fs = new FileStream(filePath, FileMode.Create);
                    BitmapEncoder encoder = new GifBitmapEncoder();

                    foreach (var frame in frames)
                    {
                        encoder.Frames.Add(BitmapFrame.Create(frame));
                    }

                    encoder.Save(fs);
                }
            }
            catch (Exception ex)
            {
                MBox.Error(string.Format(Messages.ErrorWhileExportingImage, ex.Message));
            }
        }
    }
}