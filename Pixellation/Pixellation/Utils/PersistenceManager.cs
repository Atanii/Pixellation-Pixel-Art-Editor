using Pixellation.Components.Editor;
using Pixellation.Models;
using Pixellation.Properties;
using Pixellation.Utils.FilePackaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static Pixellation.Components.Editor.PixelEditor;

namespace Pixellation.Utils
{
    public class PersistenceManager
    {
        private static PersistenceManager instance;

        public static PersistenceManager GetInstance()
        {
            if (instance == null)
            {
                instance = new PersistenceManager();
            }
            return instance;
        }

        public async Task<ProjectModel> LoadProject(string filePath)
        {
            var fpr = new FilePackageReader(filePath);
            var data = await fpr.LoadProjectModel();
            return data;
        }

        public WriteableBitmap LoadImage(string filePath)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);
            return writeableBitmap;
        }

        public void SaveProject(string filePath, PixelEditor pe)
        {
            var filePaths = new List<string>();

            // Saving LayerModels
            var layersPath = Resources.PackageContentFileNameForLayers + "." + Resources.ExtensionForLayersFile;
            var formatter = new BinaryFormatter();
            var stream = new FileStream(layersPath, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, pe.GetLayerModels());
            stream.Close();
            filePaths.Add(layersPath);

            // Saving Metadata
            var fpmd = new FilePackageMetadata
            {
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                SaveDate = DateTime.Now.ToString()
            };

            var metaDataPath = Resources.PackageContentFileNameForMetaData + "." + Resources.ExtensionForDataFile;
            string jsonString = JsonSerializer.Serialize(fpmd);
            File.WriteAllText(metaDataPath, jsonString);
            filePaths.Add(metaDataPath);

            // Saving Project Data
            var fppd = new ProjectDataModel
            {
                ProjectName = filePath.Split('.')[0].Split('\\')[^1]
            };

            var projectInfoPath = Resources.PackageContentFileNameForProjectData + "." + Resources.ExtensionForDataFile;
            jsonString = JsonSerializer.Serialize(fppd);
            File.WriteAllText(projectInfoPath, jsonString);
            filePaths.Add(projectInfoPath);

            // Packaging
            var fp = new FilePackage
            {
                FilePath = filePath,
                ContentFilePathList = filePaths
            };

            var fpwr = new FilePackageWriter(fp);
            fpwr.SaveProjectModel();
        }

        public void ExportAsImage(string filePath, VisualManager vm)
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