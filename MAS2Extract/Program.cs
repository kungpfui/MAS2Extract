using System;
using System.IO;
using System.Collections.Generic;

namespace MAS2Extract
{
    class Program
    {
        static string usageText =
@"mas2extract.exe [<options>] <filename.MAS> [<target-directory>]
MAS2 extractor tool for rFactor 2 MAS game files.

options:                
   -s: will only show all files inside the archive, but not extract them
   -c: do not create target-directory

If no extract directory is entered, the location of mas2extract.exe will be used.

Example show content only:
  mas2extract.exe -s CorvettePC_CAR.mas
Example rextract content into new created CorvettePC_CAR.masdir directory:
  mas2extract.exe CorvettePC_CAR.mas
";        
        
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine(usageText);
                Environment.ExitCode = 1; // Parameter Error
                return;
            }

            // parse the command line for options and other arguments
            var options = new List<string>();
            var arguments = new List<string>();

            bool endOfOptions = false;
            foreach (string arg in args)
            {
                if (endOfOptions) arguments.Add(arg);
                else if (arg[0] == '-') options.Add(arg.TrimStart(new char[] { '-' }));
                else 
                {
                    arguments.Add(arg);
                    endOfOptions = true;
                }
            }
            
            bool optionShowOnly = options.IndexOf("s") >= 0;
            bool optionNotCreate = options.IndexOf("c") >= 0;
            string sourceFile   = arguments.Count > 0 ? arguments[0] : null;
            string targetDir    = arguments.Count > 1 ? arguments[1] : null;

            if(sourceFile == null || !File.Exists(sourceFile))
            {
                Console.Error.WriteLine("Cannot find target MAS file!");
                Environment.ExitCode = 2; // Runtime Error
                return;
            }
            
            if (!optionShowOnly)
            {                                    
                if(targetDir == null)
                {
                    // build targetDir from filename and create if not already exists
                    targetDir = "./" + Path.GetFileNameWithoutExtension(sourceFile) + ".masdir";
                    optionNotCreate = false; // true makes no sense and will be ignored
                }

                if (!Directory.Exists(targetDir))
                {                    
                    if (optionNotCreate)
                    {
                        Console.Error.WriteLine("Target extract directory doesn't exist");
                        Environment.ExitCode = 2; // Runtime Error
                        return;
                    }
                    Directory.CreateDirectory(targetDir);                    
                }
            }

            MAS2Reader reader;
            try
            {
                reader = new MAS2Reader(sourceFile);
                Console.WriteLine(reader.Count + " files in archive");

                Console.WriteLine("+{0}+", new string('-', 77));
                Console.WriteLine("| {0,-33} | {1,-10} | {2,-9} | {3,-6} | {4,-5} |",
                    "Name", "Compressed", "Raw Size", "Ratio", "Type");
                Console.WriteLine("+{0}+", new string('-', 77));

                foreach(MAS2File file in reader.Files)
                {
                    double ratio = (file.UncompressedSize == 0) ? 0.0 : 1.0 - (double)file.CompressedSize / (double)file.UncompressedSize;
                    Console.Write("| {0,-33} | {1,10} | {2,9} | {3,6:P1} | {5,1}{4,4} |",
                        LimitString(file.Filename, 33), file.CompressedSize, file.UncompressedSize, ratio, file.FileType & ~0x10010,
                        // "Z" = Zlib deflate, "M" = MAS2 encoded, " " = plain
                        (file.FileType & 0x10) != 0 ? "Z" : ((file.FileType & 0x10000)!=0 ? "M" : " ")
                    );

                    if (!optionShowOnly)
                    {
                        try
                        {
                            reader.ExtractFile(file, Path.Combine(targetDir, file.Filename));
                        }
                        catch (Exception ex)
                        {
                            Console.Write("FAIL");
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("+{0}+", new string('-', 77));

                Console.WriteLine("Press ENTER to terminate");
                Console.ReadLine();
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to open MAS archive. Is it a rFactor 2 archive?");
                Environment.ExitCode = 2;
            }
        }


        /// <summary>
        /// Limit string length
        /// </summary>
        /// <param name="s">string to strip</param>
        /// <param name="max_width">maximum number of characters</param>
        /// <returns>stripped string</returns>
        private static string LimitString(string s, int maxWidth)
        {
            if (s.Length <= maxWidth) return s;
            return s.Substring(0, maxWidth - 3) + "...";            
        }

    }
}
