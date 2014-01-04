using System;
using System.IO;

namespace MAS2Extract
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("mas2extract.exe filename.MAS [-s] [target_directory]");
                Console.WriteLine("MAS2 extractor tool for rFactor 2 MAS game files.");
                Console.WriteLine("");
                Console.WriteLine("Parameter -s will only show all files inside the archive, but not extract them");
                Console.WriteLine("If no extract directory is entered, the location of mas2extract.exe will be used.");
                return;
            }
            var f= args[0];
            if(File.Exists(f) == false)
            {
                Console.WriteLine("Cannot find target MAS file!");
                return;
            }

            string target = (args.Length == 2) ? args[1] : "./";                
            bool unpack = true;
            if (target == "-s")
            {
                unpack = false;
            }else
            {
                if (!Directory.Exists(target))
                {
                    Console.WriteLine("Target extract directory doesn't exist");
                    return;
                }
            }

            MAS2Reader reader;
            try
            {
                reader = new MAS2Reader(f);
                Console.WriteLine(reader.Count + " files in archive");

                Console.WriteLine("+{0}+", new string('-', 77));
                Console.WriteLine("| {0,-34} | {1,-10} | {2,-9} | {3,-6} | {4,-4} |",
                    "Name", "Compressed", "Raw Size", "Ratio", "Type");
                Console.WriteLine("+{0}+", new string('-', 77));

                foreach(MAS2File file in reader.Files)
                {
                    double ratio = (file.UncompressedSize == 0) ? 0.0 : 1.0 - (double)file.CompressedSize / (double)file.UncompressedSize;
                    Console.Write("| {0,-34} | {1,10} | {2,9} | {3,6:P1} | {4,4} |",
                        LimitString(file.Filename, 34), file.CompressedSize, file.UncompressedSize, ratio, file.FileType);
                    try
                    {
                        if (unpack)
                            reader.ExtractFile(file, Path.Combine(target, file.Filename));
                    }catch(Exception ex)
                    {
                        Console.Write("FAIL");
                    }
                    Console.WriteLine();
                }
                
                Console.WriteLine(
                    "+-----------------------------------------------------------------------------+");
                Console.WriteLine("Press ENTER to terminate");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to open MAS archive. Is it a rFactor 2 archive?");
            }
        }


        private static string LimitString(string name, int max_width)
        {
            if (name.Length <= max_width) return name;            
            return name.Substring(0, max_width - 3) + "...";            
        }

    }
}
