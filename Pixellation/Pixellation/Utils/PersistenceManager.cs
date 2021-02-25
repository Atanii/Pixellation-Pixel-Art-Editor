﻿using Pixellation.Components.Editor;
using Pixellation.FilePackaging;
using Pixellation.Interfaces;
using Pixellation.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Utils
{
    internal class PersistenceManager
    {
        public readonly string MetadataFileName = Resources.PackageContentFileNameForMetaData + "." + Resources.ExtensionForDataFile;
        public readonly string ProjectDataFileName = Resources.PackageContentFileNameForProjectData + "." + Resources.ExtensionForDataFile;

        private string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private string CurrentSaveDate => DateTime.Now.ToString();

        public bool AlreadySaved { get; private set; }
        public string PreviousFullPath { get; private set; }

        private static PersistenceManager _instance;

        public static PersistenceManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PersistenceManager();
            }
            return _instance;
        }

        private PersistenceManager() { }

        private FilePackageMetadata CurrentMetadata => new FilePackageMetadata
        {
            Version = CurrentVersion,
            SaveDate = CurrentSaveDate
        };

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
            catch(Exception ex)
            {
                MessageBox.Show(string.Format(Messages.ErrorWhileLoadingProject, ex.Message, "Error"));
            }

            return res;
        }

        public WriteableBitmap LoadImage(string filePath)
        {
            WriteableBitmap res = null;

            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                res = new WriteableBitmap(bitmap);
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format(Messages.ErrorWhileLoadingImage, ex.Message, "Error"));
            }

            return res;
        }

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
            var pm = ModelConverterExtensions.MakeProjectModel(filePath.Split('.')[0].Split('\\')[^1], frames);
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
            catch(Exception ex)
            {
                MessageBox.Show(string.Format(Messages.ErrorWhileSavingProject, ex.Message, "Error"));
                return false;
            }
        }

        public void Reset()
        {
            AlreadySaved = false;
            PreviousFullPath = "";
        }

        public void ExportAsImage(string filePath, IFrameProvider vm, ExportModes exportMode = ExportModes.FRAME, int rows = 0, int cols = 0)
        {
            try
            {
                WriteableBitmap bmp = null;

                switch (exportMode)
                {
                    case ExportModes.LAYER:
                        var index = vm.GetActiveLayerIndex();
                        if (index != -1)
                        {
                            bmp = vm.Layers[index].GetWriteableBitmapWithAppliedOpacity();
                        }
                        break;

                    case ExportModes.FRAME:
                        bmp = MergeAndExportUtils.MergeAll(vm.Frames[vm.ActiveFrameIndex].Layers);
                        break;

                    case ExportModes.FRAME_ALL:
                        bmp = MergeAndExportUtils.MergeAll(vm.Frames);
                        break;

                    case ExportModes.SPRITESHEET_FRAME:
                        bmp = MergeAndExportUtils.GenerateSpriteSheetFromLayers(
                            vm.Frames[vm.ActiveFrameIndex].Layers, WriteableBitmapExtensions.BlendMode.Alpha,
                            rows, cols,
                            Colors.Transparent);
                        break;

                    case ExportModes.SPRITESHEET_ALL_FRAME:
                        bmp = MergeAndExportUtils.GenerateSpriteSheetFromFrames(
                            vm.Frames, WriteableBitmapExtensions.BlendMode.Alpha,
                            rows, cols,
                            Colors.Transparent);
                        break;

                    default:
                        break;
                }

                SaveBitmapSourceToFile(filePath, bmp);
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format(Messages.ErrorWhileExportingImage, ex.Message, "Error"));
            }
        }

        public void SaveBitmapSourceToFile(string filePath, BitmapSource image)
        {
            if (filePath != string.Empty)
            {
                string extension = filePath.Split('.')[^1];
                // Saving
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

                    case "tiff":
                        encoder = new TiffBitmapEncoder();
                        break;

                    case "gif":
                        encoder = new GifBitmapEncoder();
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
    }
}