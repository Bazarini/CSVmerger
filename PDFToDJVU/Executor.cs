﻿using Logger;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace PDFToDJVU
{
    public class Executor
    {

        public static void Convert(string[] inputBundle, string output, CancellationToken token, int dpi = 250)
        {
            string args = $"-o \"{output}\" -d{dpi} --page-id-template=nb" + "{dpage:04*}.djvu";
            foreach (var page in inputBundle)
                args += $" \"{page}\"";
            string exe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), @"Binaries\pdf2djvu.exe").Replace("file:\\", "");
            Process process = new Process() { StartInfo = { FileName = exe, Arguments = args } };
            token.Register(() => process.Kill());
            Task task = new Task(() => 
            {                
                process.Start();
                process.WaitForExit();
            }, token);            
            LogWriter.LogMessage($"Attempting to start: {exe}\r\nwith arument:\r\n{args}");
            task.Start();
            while (!task.IsCompleted)
            {
                task.Wait(5000);
                LogWriter.LogMessage($"Awaiting for process to create file {output}");
            }
            if (task.IsCanceled)
                throw new Exception($"File {output} creation process was aborted by the user");
            if (task.IsFaulted)
                throw new Exception($"The process was terminated unexpectedly. File {output} was not created");
            if (new FileInfo(output).Length == 0)
                throw new Exception("The file was not created");
            LogWriter.LogMessage($"File {output} is created");
        }

    }
}