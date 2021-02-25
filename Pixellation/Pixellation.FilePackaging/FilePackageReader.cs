using Pixellation.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pixellation.FilePackaging
{
    public class FilePackageReader
    {
        private readonly string _filepath;

        public FilePackageReader(string filepath)
        {
            _filepath = filepath;
        }

        public async Task<ProjectModel> LoadProjectModel(string metadataFileName, string projectDataFileName)
        {
            try
            {
                // Projectmodel
                ProjectModel project = null;

                // Open the package file
                using (var fs = new FileStream(_filepath, FileMode.Open))
                {
                    // Open the package file as a ZIP
                    using (var archive = new ZipArchive(fs))
                    {
                        // Metadata
                        var metadataFile = archive.Entries.Where(x => x.Name == metadataFileName).FirstOrDefault();
                        if (metadataFile != null)
                        {
                            var str = metadataFile.Open();
                            var metadata = await JsonSerializer.DeserializeAsync<FilePackageMetadata>(str);
                        }

                        // Projectdata
                        var projectDataFile = archive.Entries.Where(x => x.Name == projectDataFileName).FirstOrDefault();
                        if (projectDataFile != null)
                        {
                            var str = projectDataFile.Open();
                            project = await JsonSerializer.DeserializeAsync<ProjectModel>(str);
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