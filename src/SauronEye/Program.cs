﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SauronEye {
    public class Program {

        private static RegexSearch regexSearcher;

        public static void Main(string[] args) {
            Console.WriteLine("\n\t === SauronEye === \n");

            var ArgumentParser = new ArgumentParser();

            ArgumentParser.ParseArgumentsOptions(args);

            Console.WriteLine("Directories to search: " + string.Join(", ", ArgumentParser.Directories));
            Console.WriteLine("For file types: " + string.Join(", ", ArgumentParser.FileTypes));
            Console.WriteLine("Containing: " + string.Join(", ", ArgumentParser.Keywords));
            Console.WriteLine("Search contents: " + ArgumentParser.SearchContents.ToString());
            Console.WriteLine("Max file size: " + ArgumentParser.MaxFileSizeInKB.ToString() + " KB");
            Console.WriteLine("Search Program Files directories: " + ArgumentParser.SystemDirs.ToString());
            if (ArgumentParser.BeforeDate != DateTime.MinValue) {
                Console.WriteLine("Only files before: " + ArgumentParser.BeforeDate.ToString("yyyy-MM-dd") + "\n");
            }
            if (ArgumentParser.AfterDate != DateTime.MinValue) {
                Console.WriteLine("Only files after: " + ArgumentParser.AfterDate.ToString("yyyy-MM-dd") + "\n");
            }
            Stopwatch sw = new Stopwatch();

            sw.Start();

            var options = new ParallelOptions { MaxDegreeOfParallelism = ArgumentParser.Directories.Count };
            Parallel.ForEach(ArgumentParser.Directories, options, (dir) => {
                Console.WriteLine("Searching in parallel: " + dir);
                var fileSystemSearcher = new FSSearcher(dir, ArgumentParser.FileTypes, ArgumentParser.Keywords, ArgumentParser.SearchContents, ArgumentParser.MaxFileSizeInKB, ArgumentParser.SystemDirs, ArgumentParser.regexSearcher, ArgumentParser.BeforeDate, ArgumentParser.AfterDate);
                fileSystemSearcher.Search();

            });
            sw.Stop();

            Console.WriteLine("\n Done. Time elapsed = {0}", sw.Elapsed);

            if (Debugger.IsAttached)
                Console.ReadKey();
        }
    }


}
