using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace Sepidar.Framework
{
    public class Config
    {
        private static readonly byte[] salt = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

        public static string HashSalt
        {
            get
            {
                return GetSetting("Security:HashSalt");
            }
        }

        public static string ExpandEnvironmentVariables(string path)
        {
            if (!path.Contains("%"))
            {
                return path;
            }
            var newPath = Environment.ExpandEnvironmentVariables(path);
            if (path == newPath)
            {
                throw new FrameworkException("{0} uses environment varaibles, but they are not defined. Please define them first".Fill(path));
            }
            return newPath;
        }

        public static string GetEnvironmentVariable(string key)
        {
            return GetEnvironmentVariable(key, null);
        }

        public static string GetEnvironmentVariable(string key, string alternative)
        {
            var result = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User);
            if (result.IsNothing())
            {
                result = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
            }
            if (result.IsNothing())
            {
                result = alternative;
            }
            if (result.IsNothing())
            {
                throw new FrameworkException("{0} should be defined in Environment Variables, or a fallback should be provided.".Fill(key));
            }
            return result;
        }

        public static ParallelOptions ParallelOptions
        {
            get
            {
                var options = new ParallelOptions();
                options.MaxDegreeOfParallelism = Environment.ProcessorCount;
                return options;
            }
        }

        private static string logFolderPath;

        public static string LogFolderPath
        {
            get
            {
                if (logFolderPath.IsSomething())
                {
                    return logFolderPath;
                }
                var keys = new string[] { "LogFolderPath", "LogsFolderPath", "LogsFolder", "LogFolder", "Logs", "Log", "LogPath", "LogsPath" };
                foreach (string key in keys)
                {
                    if (HasSetting(key))
                    {
                        logFolderPath = GetSetting(key);
                        return logFolderPath;
                    }
                }
                logFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                return logFolderPath;
            }
        }

        public static MessageType LogVerbosity
        {
            get
            {
                string key = "Log:VerbosityLevel";
                if (HasSetting(key))
                {
                    return GetSetting(key).ToEnum<MessageType>();
                }
                return MessageType.Error;
            }
        }

        public static bool HasKey(string key)
        {
            string result = GetSetting(key);
            return result.IsNotNull();
        }

        public static string GetSetting(string key)
        {
            string setting;
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            string value;
            if (key.Split(":").Length == 1)
            {
                value = configuration["Settings:" + key];
            }
            else
            {
                value = configuration[key];
            }
            
            if (!string.IsNullOrEmpty(value))
            {
                setting = value;
            }
            else
            {
                throw new FrameworkException("There is no app setting for {0} in Web.config file.".Fill(key));
            }
            return setting;
        }

        public static bool HasSetting(string key)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            string value;
            if (key.Split(":").Length == 1)
            {
                value = configuration["Settings:" + key];
            }
            else
            {
                value = configuration[key];
            }
            return value != null;
        }

        public static int SmallPageItemsCount
        {
            get
            {
                return GetSetting("SmallPageItemsCount").ToInt();
            }
        }

        public static int MediumPageItemsCount
        {
            get
            {
                return GetSetting("MediumPageItemsCount").ToInt();
            }
        }

        public static int LargePageItemsCount
        {
            get
            {
                return GetSetting("LargePageItemsCount").ToInt();
            }
        }

        public static int ThumbnailMaxWidth
        {
            get
            {
                return GetSetting("ThumbnailMaxWidth").ToInt();
            }
        }

        public static int ThumbnailMaxHeight
        {
            get
            {
                return GetSetting("ThumbnailMaxHeight").ToInt();
            }
        }

        public static int PagingLinksCount
        {
            get
            {
                return GetSetting("PagingLinksCount").ToInt();
            }
        }

        public static List<string> AdminEmails
        {
            get
            {
                return GetSetting("AdminEmails").SplitCsv<string>();
            }
        }

        public static string ResourcesHost
        {
            get
            {
                return GetSetting("ResourcesHost");
            }
        }

        public static string GeneralSuccessMessage
        {
            get { return GetSetting("GeneralSuccessMessage"); }
        }

        public static string GeneralErrorMessage
        {
            get { return GetSetting("GeneralErrorMessage"); }
        }

        public static bool IsDeveloping
        {
            get
            {
                string value = GetSetting("IsDeveloping");
                if (value == null)
                {
                    return false;
                }
                bool result = false;
                Boolean.TryParse(value, out result);
                return result;
            }
        }

        public static byte[] AesKey
        {
            get
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(GetSetting("Security:AesKey"), salt);
                return pdb.GetBytes(32);
            }
        }

        public static byte[] AesVector
        {
            get
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(GetSetting("Security:AesVector"), salt);
                return pdb.GetBytes(16);
            }
        }

        public static int DefaultPageSize
        {
            get
            {
                if (HasSetting("DefaultPageSize"))
                {
                    string pageSize = GetSetting("DefaultPageSize");
                    if (pageSize.IsNumeric())
                    {
                        return Convert.ToInt32(pageSize);
                    }
                }
                return 10;
            }
        }

        public static string AbsoluteTempFolder
        {
            get
            {
                if (HasSetting("TempFolder"))
                {
                    return GetSetting("TempFolder");
                }
                return Path.GetTempPath();
            }
        }

        public static string RelativeTempFolder
        {
            get
            {
                return "/Temp";
            }
        }

        public static string GetConnectionString(string name)
        {
            string connectionString;
            var builder = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            var value = configuration["ConnectionStrings:" + name];
            if (!string.IsNullOrEmpty(value))
            {
                connectionString = value;
            }
            else
            {
                throw new FrameworkException("No connection string with name {0} can be found. Check your configuration files.".Fill(name));
            }
            return connectionString;
        }

        public static string ProjectsRootKey
        {
            get
            {
                return GetSetting("ProjectsRootKey");
            }
        }

        public static string ProjectsRoot
        {
            get
            {
                return GetEnvironmentVariable(ProjectsRootKey);
            }
        }

        public static string DiskBlobsRoot
        {
            get
            {
                return GetSetting("DiskBlobsRoot");
            }
        }

        public static int LogBufferSize
        {
            get
            {
                return BufferConfiguration("LogBufferSize", 1000);
            }
        }

        public static int LogBufferFlushIntervalInMilliseconds
        {
            get
            {
                return BufferConfiguration("LogBufferFlushIntervalInMilliseconds", 1000);
            }
        }

        public static int DatabaseBufferSize
        {
            get
            {
                return BufferConfiguration("DatabaseBufferSize", 1000);
            }

        }

        public static int DatabaseBufferFlushIntervalInMilliseconds
        {
            get
            {
                return BufferConfiguration("DatabaseBufferFlushIntervalInMilliseconds", 1000);
            }
        }

        public static int CacheExpirationTimeInSeconds
        {
            get
            {
                var key = "CacheExpirationTimeInSeconds";
                if (HasSetting(key) && GetSetting(key).IsNumeric())
                {
                    return GetSetting(key).ToInt();
                }
                return 10 * 60;
            }
        }

        private static int BufferConfiguration(string key, int defaultValue)
        {
            if (HasSetting(key) && GetSetting(key).IsNumeric())
            {
                return GetSetting(key).ToInt();
            }
            Logger.LogWarning("No value is set for {0} in configuration. Default value of {1} would be used.".Fill(key, defaultValue));
            return defaultValue;
        }

        public static int CacheUpdateTimeInMinutes
        {
            get
            {
                var key = "CacheUpdateTimeInMinutes";
                if (HasSetting(key) && GetSetting(key).IsNumeric())
                {
                    return GetSetting(key).ToInt();
                }
                return 60;
            }
        }
    }
}
