﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PCG.Library.Models.Validation
{
    [XmlRoot("WorkInfo")]
    public class BasicWorkInformation
    {
        [XmlAttribute("UID")]
        public Guid ReportId { get; set; }

        [XmlAttribute("participantFullName")]
        public string StudentFullName { get; set; }

        [XmlIgnore]
        public string ValidFullName
        {
            get
            {
                var original = StudentFullName;

                foreach (var ch in Path.GetInvalidPathChars())
                {
                    original = original.Replace(ch.ToString(), "");
                }

                return original;
            }
        }

        [XmlAttribute("totalTime")]
        public decimal TotalTime { get; set; }

        [XmlElement("extended")]
        public ExtendedInformation ExtendedInfo { get; set; }

        [XmlAttribute("activeThreadsCount")]
        public int ThreadCount { get; set; }

        [XmlAttribute("computerInfo")]
        public string ComputerInfo { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }

        public const string HeadersString = "\"ID отчета\";\"ФИО студента\";\"Наименование файла\";\"Кол-во потоков\";\"Затраченное время на все топологии по n-узлам\";\"В днях:часах:минутах\"" +
                                            "\"Состояние завершенности\";\"Кол-во узлов\";\"Диаметр\";\"Средняя длина\";\"Оптимальные описания\";\"Полное время, затрачиваемое на весь запуск (по отчету)\";\"Инфо о компьютере\"";

        public string ToCsvString(string fileName)
        {
            var sb = new StringBuilder();

            foreach (var ext in ExtendedInfo.Descriptions)
            {
                if (ext.IsFinished)
                {
                    var allOptimalInNodes = ext.OptimalCirculants != null ? string.Join(", ", ext.OptimalCirculants.Select(x => $"C({x.NodesCount}; {string.Join(", ", x.Generatrixes.Select(y => y.ToString()))})")) : string.Empty;

                    var str = $"\"{ReportId}\";\"{StudentFullName}\";\"{fileName}\";\"{ThreadCount}\";\"{ext.OptimalCirculants?.FirstOrDefault()?.TotalMilliseconds}\";\"{new TimeSpan(0, 0, 0, 0, (int)(ext.OptimalCirculants?.FirstOrDefault()?.TotalMilliseconds ?? 0)):g}\"" +
                              $";\"{(ext.IsFinished ? "1" : "0")}\";" +
                              $"\"{ext.OptimalCirculants?.FirstOrDefault()?.NodesCount}\";\"{ext.OptimalCirculants?.FirstOrDefault()?.Diameter}\";" +
                              $"\"{ext.OptimalCirculants?.FirstOrDefault()?.AverageLength}\";\"{allOptimalInNodes}\";\"{TotalTime}\"" +
                              $";\"{ComputerInfo}\"";
                    sb.AppendLine(str);
                }
                else if (ext.LastState.NotCheckedConfig?.Any() ?? false)
                {
                    var str = $"\"{ReportId}\";\"{StudentFullName}\";\"{fileName}\";\"0\";\"{ext.LastState.CurrentNodesCount}\";\"{ext.LastState?.Diameter}\";\"{ext.LastState?.AverageDiameter}\";\"{TotalTime}\"";
                    sb.AppendLine(str);
                }
            }

            return sb.ToString();
        }
    }
}