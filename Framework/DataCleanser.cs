//using Quartz;
//using Core.Framework.Extensions;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//
//namespace Core.Framework
//{
//    public class DataCleanser : IFrameworkJob
//    {
//        public const string ConnectionStringKey = "DataCleansingConnectionString";
//        string connectionString;
//
//        public static void Clean()
//        {
//            var queryPaths = LoadQueries();
//            foreach (var queryPath in queryPaths)
//            {
//                RunQuery(queryPath);
//            }
//        }
//
//        private static void RunQuery(string queryPath)
//        {
//            try
//            {
//                var lines = File.ReadAllLines(queryPath).ToList();
//                var cronExpression = ExtractAndRemoveLine(lines, "cron expression");
//                //var message = ExtractAndRemoveLine(lines, "message");
//                var query = string.Join("\r\n", lines);
//                // based on crontabschedule, see if it should be run now, and if yes, run it. That's all.
//            }
//            catch (Exception ex)
//            {
//                Logger.Log(ex);
//                Logger.LogError("Error reading cleansing query {0}".Fill(queryPath));
//            }
//        }
//
//        private static List<string> LoadQueries()
//        {
//            var queriesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataCleansingQueries");
//            var queryPaths = new List<string>();
//            if (Directory.Exists(queriesFolder))
//            {
//                queryPaths = Directory.GetFiles(queriesFolder).ToList();
//            }
//            return queryPaths;
//        }
//
//        public override void Execute(IJobExecutionContext context)
//        {
//            if (context.Get(ConnectionStringKey).IsNull())
//            {
//                throw new FrameworkException("Connection string is not determined for data cleansing job. Define it using '{0}' key.".Fill(ConnectionStringKey));
//            }
//            connectionString = context.Get(ConnectionStringKey).ToString();
//            Clean();
//        }
//
//        private static string ExtractAndRemoveLine(List<string> lines, string key)
//        {
//            var line = lines.SingleOrDefault(i => i.ToLower().StartsWith(key));
//            if (line.IsNull())
//            {
//                throw new FrameworkException("Query file does not have {0}".Fill(key));
//            }
//            lines.Remove(line);
//            return Regex.Replace(line, key + @"\s*=>\s*", "");
//        }
//    }
//}
