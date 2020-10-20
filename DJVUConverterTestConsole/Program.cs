using Logger;
using PDFToDJVU;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DJVUConverterTestConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true, Filter = "PDF|*.pdf" };
            List<List<string>> batch = new List<List<string>>();
            LogWriter.AddLogger(new FileLogger("D:/"), LogDepth.Debug);
            DialogResult result;
            while ((result = openFileDialog.ShowDialog()) == DialogResult.OK)
                batch.Add(new List<string>() { "123123123.djvu" });
            Parallel.ForEach(batch, new ParallelOptions() { MaxDegreeOfParallelism = 3 }, djvuBundle =>
           {
               Convert(djvuBundle.ToArray(), Path.Combine(Path.GetDirectoryName(djvuBundle.First()), "1.djvu"), 92);
           });

        }
        public static void Convert(string[] inputBundle, string output, int dpi = 250)
        {
            string args = $"-o \"{output}\" -d{dpi} --page-id-template=nb" + "{dpage:04*}.djvu";
            foreach (var page in inputBundle)
                args += $" \"{page}\"";
            string exe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), @"Binaries\pdf2djvu.exe").Replace("file:\\", "");
            Process process = new Process() { StartInfo = { FileName = exe, Arguments = args, UseShellExecute = false, RedirectStandardOutput = true} };
            process.OutputDataReceived += Process_OutputDataReceived;
            process.Start();           
            process.WaitForExit();
            Console.ReadKey();
        }
        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
