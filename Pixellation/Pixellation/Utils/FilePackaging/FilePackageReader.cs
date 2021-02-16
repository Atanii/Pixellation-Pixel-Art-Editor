using Pixellation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pixellation.Utils.FilePackaging
{
    internal class FilePackageReader
    {
        private readonly string _filepath;

        public FilePackageReader(string filepath)
        {
            _filepath = filepath;
        }

        public async Task<ProjectModel> LoadProjectModel()
        {
            try
            {
                // Projectmodel
                var project = new ProjectModel();

                // Open the package file
                using (var fs = new FileStream(_filepath, FileMode.Open))
                {
                    // Open the package file as a ZIP
                    using (var archive = new ZipArchive(fs))
                    {
                        var metadataFileName = Properties.Resources.PackageContentFileNameForMetaData + "." + Properties.Resources.ExtensionForDataFile;
                        var projectDataFileName = Properties.Resources.PackageContentFileNameForProjectData + "." + Properties.Resources.ExtensionForDataFile;
                        var layersFileName = Properties.Resources.PackageContentFileNameForLayers + "." + Properties.Resources.ExtensionForLayersFile;

                        // Metadata
                        var metadataFile = archive.Entries.Where(x => x.Name == metadataFileName).FirstOrDefault();
                        if (metadataFile != null)
                        {
                            var str = metadataFile.Open();
                            var metadata = await JsonSerializer.DeserializeAsync<FilePackageMetadata>(str);
                            project.ModelVersion = metadata.Version;
                        }

                        // Projectdata
                        var projectDataFile = archive.Entries.Where(x => x.Name == projectDataFileName).FirstOrDefault();
                        if (projectDataFile != null)
                        {
                            var str = projectDataFile.Open();
                            project.ProjectData = await JsonSerializer.DeserializeAsync<ProjectDataModel>(str);
                        }

                        // Layers
                        var layersFile = archive.Entries.Where(x => x.Name == layersFileName).FirstOrDefault();
                        if (layersFile != null)
                        {
                            var str = layersFile.Open();
                            var formatter = new BinaryFormatter();
                            project.Layers = (List<LayerModel>)formatter.Deserialize(str);
                        }
                    }
                }

                return project;
            }
            catch (Exception e)
            {
                var errorMessage = "Unable to open/read the package. " + e.Message;
                throw new Exception(errorMessage);
            }
        }
    }
}