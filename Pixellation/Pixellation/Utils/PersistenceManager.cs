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
using System.Windows;
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
            PreviousFullPath = filePath;
            var fpr = new FilePackageReader(filePath);
            var data = await fpr.LoadProjectModel(MetadataFileName, ProjectDataFileName);
            var res = ModelConverterExtensions.GetProjectData(data, helper);
            return res;
        }

        public WriteableBitmap LoadImage(string filePath)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);
            return writeableBitmap;
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
            fpwr.SaveProjectModel();

            AlreadySaved = true;
            PreviousFullPath = filePath;

            return true;
        }

        public void Reset()
        {
            AlreadySaved = false;
            PreviousFullPath = "";
        }

        public void ExportAsImage(string filePath, ILayerManager vm)
        {
            var wrBitmap = vm.GetAllMergedWriteableBitmap();
            SaveBitmapSourceToFile(filePath, wrBitmap);
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
                    case "png":
                        encoder = new PngBitmapEncoder();
                        break;

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

                    default:
                        MessageBox.Show($"Saving into (.{extension}) image format is not supported!", "Error");
                        return;
                }
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fs);
            }
        }
    }
}