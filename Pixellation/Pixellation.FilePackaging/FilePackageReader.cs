using Pixellation.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pixellation.FilePackaging
{
    /// <summary>
    /// Class reading the saving the project and for creating a <see cref="ProjectModel"/> from it.
    /// </summary>
    public class FilePackageReader
    {
        private readonly string _filepath;

        /// <summary>
        /// Inits filepath.
        /// </summary>
        /// <param name="filepath"></param>
        public FilePackageReader(string filepath)
        {
            _filepath = filepath;
        }

        /// <summary>
        /// Loads the Pixellation project and generates a <see cref="ProjectModel"/> from the content in it.
        /// </summary>
        /// <param name="metadataFileName">Path for the metadata file in the package.</param>
        /// <param name="projectDataFileName">Path for the projectdatafile in the package.</param>
        /// <returns></returns>
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