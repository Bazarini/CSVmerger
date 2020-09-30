using Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;

namespace PDFToDJVU
{
    public class Executor
    {
        public static void Convert(string[] inputBundle, string output, int dpi = 250)
        {
            string args = $"-o \"{output}\" -d{dpi} --page-id-template=nb" + "{dpage:04*}.djvu";
            foreach (var page in inputBundle)
                args += $" \"{page}\"";
            string exe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), @"Binaries\pdf2djvu.exe").Replace("file:\\","");
            Process process = new Process() { StartInfo = { FileName = exe, Arguments = args } };
            Task task = new Task(() => { process.Start(); process.WaitForExit(); });
            LogWriter.LogMessage($"Trying to start: {exe}\r\nwith arument:\r\n{args}");
            task.Start();
            while (!task.IsCompleted)
            {
                task.Wait(5000);
                LogWriter.LogMessage($"Awaiting for process to create file {output}");
            }
            LogWriter.LogMessage($"File {output} is created");
        }

    }
}