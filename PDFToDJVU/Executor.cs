using Logger;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Security;
using System.ComponentModel;
using System.Collections.Generic;

namespace PDFToDJVU
{
    public class Executor
    {

        public static void NewPrepareDJVU(string inputDocument, string output, CancellationTokenSource source, int dpi = 250)
        {
            string args = $"-o \"{output}\" -d{dpi} --page-id-template=nb" + "{dpage:04*}.djvu" + $"\"{inputDocument}\"";
            string exe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), @"Binaries\pdf2djvu.exe").Replace("file:\\", "");
            Process process = new Process { StartInfo = { FileName = exe, Arguments = args, UseShellExecute = false, RedirectStandardOutput = true } };
            process.OutputDataReceived += Process_OutputDataReceived;
            var processStartRegistration = source.Token.Register(() => process.Kill());
            Task task = new Task(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    LogWriter.LogMessage($"\r\nwith arument:\r\n{args}", LogDepth.Debug);
                    process.Start();
                    LogWriter.LogMessage($"Process with ID {process.Id} started.", LogDepth.Debug);
                    while (!process.WaitForExit(20000))
                        LogWriter.LogMessage($"Awaiting for the process to create file {output}", LogDepth.UserLevel);
                    stopwatch.Stop();
                    LogWriter.LogMessage($"Process with ID {process.Id} ended. It was woring for {stopwatch.ElapsedMilliseconds}ms", LogDepth.Debug);
                    processStartRegistration.Dispose();
                }
                catch (Exception ex)
                {
                    LogWriter.LogMessage(ex.Message, LogDepth.Debug);
                    processStartRegistration.Dispose();
                    throw;
                }
            });
            task.Start();
            task.Wait();
            if (task.IsCanceled)
                throw new Exception($"File {output} creation process was aborted.");
            if (task.IsFaulted)
                throw new Exception($"The process was terminated unexpectedly. File {output} was not created.");
            if (!File.Exists(output) || new FileInfo(output).Length == 0)
                throw new Exception($"File {output} was not created");
            LogWriter.LogMessage($"File {output} is created", LogDepth.UserLevel);
        }

        public static void PrepareDJVU(string[] inputBundle, string output, CancellationTokenSource source, string rootFolder, int dpi = 250)
        {
            string args = $"-o \"{output}\" -d{dpi} --page-id-template=nb" + "{dpage:04*}.djvu";
            int i = 0;
            
            string batchFolder = "";
            while (Directory.Exists(batchFolder = Path.Combine(rootFolder, i.ToString())))
                i++;
            foreach (var page in inputBundle)
            {
                Directory.CreateDirectory(batchFolder);
                int j = 0;
                string outputPage = string.Empty;
                while (File.Exists(outputPage = Path.Combine(batchFolder, $"{j}.pdf")))
                    j++;
                LogWriter.LogMessage($"Creating temp file for {page}");
                File.Copy(page, outputPage, true);
                args += $" \"{outputPage}\"";
            }
            string exe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), @"Binaries\pdf2djvu.exe").Replace("file:\\", "");
            Process process = new Process { StartInfo = { FileName = exe, Arguments = args, UseShellExecute = false, RedirectStandardOutput = true } };
            process.OutputDataReceived += Process_OutputDataReceived;
            var processStartRegistration = source.Token.Register(() => process.Kill());
            Task task = new Task(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    LogWriter.LogMessage($"\r\nwith arument:\r\n{args}", LogDepth.Debug);
                    process.Start();
                    LogWriter.LogMessage($"Process with ID {process.Id} started.", LogDepth.Debug);
                    while (!process.WaitForExit(20000))
                        LogWriter.LogMessage($"Awaiting for the process to create file {output}", LogDepth.UserLevel);
                    stopwatch.Stop();
                    LogWriter.LogMessage($"Process with ID {process.Id} ended. It was woring for {stopwatch.ElapsedMilliseconds}ms", LogDepth.Debug);
                    processStartRegistration.Dispose();
                }
                catch (Exception ex)
                {
                    LogWriter.LogMessage(ex.Message, LogDepth.Debug);
                    processStartRegistration.Dispose();
                    throw;
                }
            });
            task.Start();
            task.Wait();
            if (task.IsCanceled)
                throw new Exception($"File {output} creation process was aborted.");
            if (task.IsFaulted)
                throw new Exception($"The process was terminated unexpectedly. File {output} was not created.");
            if (!File.Exists(output) || new FileInfo(output).Length == 0)
                throw new Exception($"File {output} was not created");
            LogWriter.LogMessage($"File {output} is created", LogDepth.UserLevel);
            Directory.Delete(batchFolder, true);
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            LogWriter.LogMessage(e.Data, LogDepth.Default);
        }
    }
}