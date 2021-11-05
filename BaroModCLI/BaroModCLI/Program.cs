using BaroModCLI.Util;
using System;
using System.IO;

namespace BaroModCLI
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {

            if (args.Length != 3)
            {
                Console.WriteLine("baracli requires 3 arguments:");
                Console.WriteLine("baracli <bararoot> <inputmodpath> <outputpath>");
                return;
            }
            processFiles(args[0], args[1], args[2]);

            

        }

        private static void processFiles(string baroRootDirectory, string inputDirectory, string outputDirectory)
        {
            if (!Directory.Exists(baroRootDirectory))
                throw new ArgumentException("The Barotrauma Root Directory provided does not exist.");
            if (!Directory.Exists(outputDirectory))
                throw new ArgumentException("The output directory provided does not exist.");


            // multi

            Console.WriteLine($"Creating files from diffs found in {inputDirectory}");
            foreach (string inputDiff in Directory.EnumerateFiles(inputDirectory, "*.*",
                                                                    SearchOption.AllDirectories))
            {
                string relativeOutputDirectory =
                    Path.GetRelativePath(inputDirectory, Path.GetDirectoryName(inputDiff)!);
                string outputFileDirectory = Path.Combine(outputDirectory, relativeOutputDirectory);
                Directory.CreateDirectory(outputFileDirectory);
                if (Path.GetExtension(inputDiff) == ".xml")
                    FormUtils.CreatePatchedFile(inputDiff, outputFileDirectory, baroRootDirectory);
                else
                    File.Copy(inputDiff, Path.Combine(outputFileDirectory, Path.GetFileName(inputDiff)), true);
            }

            Console.WriteLine("Finished!");
            
        }
    }
}
