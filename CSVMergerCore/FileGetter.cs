using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVMergerCore
{
    public class FileGetter
    {
        public static string[] GetFiles(string rootFolder, string fileFormat)
        {
            List<string> files = new List<string>();
            var currentDirectoryFiles = Directory.GetFiles(rootFolder).Where(w => w.Contains(fileFormat)).ToArray();
            if (currentDirectoryFiles.Length > 0)
                foreach (string file in currentDirectoryFiles)
                {
                    files.Add(file);                    
                }
            foreach (var directory in Directory.GetDirectories(rootFolder))
                files.AddRange(GetFiles(directory, fileFormat));
            return files.ToArray();
        }
    }
}
