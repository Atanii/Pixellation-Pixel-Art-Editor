using System.Collections.Generic;

namespace Pixellation.FilePackaging
{
    /// <summary>
    /// Represents a file package containing the files to be saved.
    /// </summary>
    public class FilePackage
    {
        /// <summary>
        /// Savepath of the file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// List for files to be stored in the final filepackage.
        /// </summary>
        public IEnumerable<string> ContentFilePathList { get; set; }
    }
}