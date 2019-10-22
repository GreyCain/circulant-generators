using Microsoft.Win32;
using PCG.Library.Models.GeneratorObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace PCG.Library.Utilities
{
    public static class SuperUtilities
    {
        private static readonly IFormatProvider NumberFormat = new NumberFormatInfo
        {
            NumberDecimalSeparator = "."
        };

        public static GeneratorTask LoadTask(string filePath)
        {
            var xmlSerialize = new XmlSerializer(typeof(GeneratorTask));

            using (var rs = new StreamReader(filePath))
            {
                return (GeneratorTask)xmlSerialize.Deserialize(rs);
            }
        }

        public static void SaveTask(GeneratorTask task, string filePath)
        {
            var xmlSerialize = new XmlSerializer(typeof(GeneratorTask));

            var fileInfo = new FileInfo(filePath);

            if (!string.IsNullOrEmpty(fileInfo.DirectoryName) && !Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }

            using (var rs = new StreamWriter(filePath))
            {
                xmlSerialize.Serialize(rs, task);
            }
        }

        public static LastState RestoreLastState(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new LastState();
            }

            var xmlSerialize = new XmlSerializer(typeof(LastState));

            using (var rs = new StreamReader(filePath))
            {
                return (LastState)xmlSerialize.Deserialize(rs);
            }
        }

        public static void SaveLastState(LastState state, string filePath)
        {
            var xmlSerialize = new XmlSerializer(typeof(LastState));
            var fileInfo = new FileInfo(filePath);

            if (!string.IsNullOrEmpty(fileInfo.DirectoryName) && !Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }

            using (var rs = new StreamWriter(filePath))
            {
                xmlSerialize.Serialize(rs, state);
            }
        }

        public static void SaveRecordInFolder(IEnumerable<CirculantParameters> pars, string outputFolder)
        {

        }

        public static void SaveResultsInFolder(IEnumerable<CirculantParameters> pars, string outputFolder, bool inMyFormat = true)
        {
            var groupsByGrade = pars.GroupBy(x => x.Generatrixes.Length);

            Directory.CreateDirectory(outputFolder);
            const string headerString = "Кол-во вершин;Конфигурация графа;Диаметр;Средний путь;Время (мс);Кол-во соединений";

            foreach (var g in groupsByGrade)
            {
                var groupsByNodes = g.GroupBy(y => y.NodesCount);

                foreach (var gn in groupsByNodes)
                {
                    var fileName = $"{gn.First().Generatrixes.Length}-{gn.First().NodesCount}.csv";

                    var l = pars.Select(x => $"{x.NodesCount};C({x.NodesCount}, {string.Join(", ", x.Generatrixes)});{x.Diameter.ToString(NumberFormat)};{x.AverageLength.ToString(NumberFormat)};{x.TotalMilliseconds};{x.NodesCount * g.Key}").ToList();
                    l.Insert(0, headerString);

                    File.WriteAllLines(Path.Combine(outputFolder, fileName), l);
                }
            }
        }


        public static string GetProcessorInfo()
        {
            //  Console.WriteLine("\n\nDisplaying Processor Name....");
            RegistryKey processorName = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree); //This registry entry contains entry for processor info.

            if (processorName?.GetValue("ProcessorNameString") != null)
            {
                return processorName.GetValue("ProcessorNameString").ToString(); //Display processor info.
            }

            return string.Empty;
        }
    }
}