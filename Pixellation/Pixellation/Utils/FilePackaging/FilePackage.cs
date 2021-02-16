using System.Collections.Generic;

namespace Pixellation.Utils.FilePackaging
{
    internal class FilePackage
    {
        public string FilePath { get; set; }
        public IEnumerable<string> ContentFilePathList { get; set; }
    }
}