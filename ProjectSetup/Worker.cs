using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Sepidar.ProjectSetup
{
    public class Worker
    {
        private static string hostsPath = Config.ExpandEnvironmentVariables(@"%windir%\system32\drivers\etc\hosts");

        public static void Setup()
        {
            var appsData = Worker.GetAppsData();
            foreach (var appData in appsData)
            {
                Worker.SetupApp(appData);
                Logger.LogSuccess(appData.IisSiteName);
               
            }
            Worker.ExecuteCustomScript();
            Logger.LogSuccess("Done. Press any key...");
        }

        public static List<AppData> GetAppsData()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setup.json");
            var text = File.ReadAllText(path);
            var appsData = text.JsonDeserialize<List<AppData>>();
            

            foreach (var appData in appsData)
            {
                appData.Path = Config.ExpandEnvironmentVariables(appData.Path);
                appData.Bindings = new List<string> { Reverse(appData.IisSiteName) };
            }

            Logger.LogSuccess("Loaded setup profile");
          
            return appsData;
        }

        private static string Reverse(string siteName)
        {
            var items = siteName.Split('.').Reverse();
            return string.Join(".", items);
        }

        public static void SetupApp(AppData appData)
        {
            //CreateWindowsUser(appData.IisSiteName);
            CreateIisWebsite(appData);
            AddHostHeadersToLocalDns(appData.Bindings);
        }

        private static void AddHostHeadersToLocalDns(List<string> hostHeaders)
        {
            var currentLines = File.ReadAllLines(hostsPath).ToList();
            currentLines = currentLines.Where(i => !i.Trim().StartsWith("#")).ToList();
            foreach (var hostHeader in hostHeaders)
            {
                currentLines.RemoveAll(i => i.Split().Any(x => x == hostHeader));
                currentLines.Add("127.0.0.1     {0}".Fill(hostHeader));
            }
            File.WriteAllLines(hostsPath, currentLines.ToArray());
        }

        private static void CreateIisWebsite(AppData appData)
        {
            DeleteSite(appData.IisSiteName);
            DeleteAppPool(appData.IisSiteName);
            CreateAppPool(appData.IisSiteName);
            //SetAppPoolUser(appData.IisSiteName);
            CreateSite(appData);
            AssignSiteToAppPool(appData.IisSiteName);
        }

        private static void AssignSiteToAppPool(string siteName)
        {
            ExecuteAppCmdCommand("set app \"{0}/\" /applicationPool:{0}".Fill(siteName));
        }

        private static void CreateSite(AppData appData)
        {
            if (appData.ServesStatics)
            {
                appData.Bindings.Add("statics.{0}".Fill(appData.Bindings[0]));
                for (int i = 1; i <= 8; i++)
                {
                    appData.Bindings.Add("statics{0}.{1}".Fill(i, appData.Bindings[0]));
                }
            }
            string bindings = appData.Bindings.Select(i => "http://{0}:80".Fill(i)).Aggregate((a, b) => "{0},{1}".Fill(a, b));
            ExecuteAppCmdCommand(@"add site /name:{0} /bindings:{1} /physicalPath:{2}".Fill(appData.IisSiteName, bindings, appData.Path));
            if (appData.ServesBlob)
            {
                ExecuteAppCmdCommand(@"set site {0} -limits.maxUrlSegments:60".Fill(appData.IisSiteName));
            }
            if (appData.ServesStatics)
            {
                ExecuteAppCmdCommand(@"set config {0} -section:system.webServer/httpProtocol /+customHeaders.[name='Access-Control-Allow-Origin',value='*'] /commit:apphost".Fill(appData.IisSiteName));
            }
        }

        private static void SetAppPoolUser(string appPoolName)
        {
            ExecuteAppCmdCommand("set config /section:applicationPools /[name='{0}'].processModel.identityType:SpecificUser /[name='{0}'].processModel.userName:{0} /[name='{0}'].processModel.password:{0}".Fill(appPoolName));
        }

        private static void CreateAppPool(string appPoolName)
        {
            ExecuteAppCmdCommand("add apppool /name:{0} /managedRuntimeVersion:v4.0".Fill(appPoolName));
        }

        private static void DeleteAppPool(string appPoolName)
        {
            ExecuteAppCmdCommand("delete apppool /apppool.name:{0}".Fill(appPoolName));
        }

        private static void DeleteSite(string siteName)
        {
            ExecuteAppCmdCommand("delete site /site.name:{0}".Fill(siteName));
        }

        private static string ExecuteAppCmdCommand(string command)
        {
            var appCmdPath = Config.ExpandEnvironmentVariables(@"%windir%\system32\inetsrv\appcmd.exe");
            if (!File.Exists(appCmdPath))
            {
                //throw new FrameworkException("IIS is not installed");
                Console.WriteLine("IIS is not installed");
            }
            return CommandLine.App(appCmdPath).Execute(command);
        }

        private static void CreateWindowsUser(string username)
        {
            ExecuteNetUserCommand("user {0} /delete".Fill(username));
            ExecuteNetUserCommand("user /add {0} {0} /passwordchg:no /active:yes /passwordreq:yes /expires:never".Fill(username));
            ExecuteNetUserCommand("localgroup administrators {0} /add".Fill(username));
            //ExecuteWmicCommand("wmic useraccount where \"name='{0}'\" set passwordexpires=false".Fill(username));
        }

        private static void ExecuteWmicCommand(string command)
        {
            CommandLine.App(Config.ExpandEnvironmentVariables(@"%windir%\system32\cmd")).Execute(command);
        }

        private static void ExecuteNetUserCommand(string command)
        {
            var path = Config.ExpandEnvironmentVariables(@"%windir%\system32\net.exe");
            CommandLine.App(path).Execute(command);
        }

        public static void ExecuteCustomScript()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "CustomScript.bat");
            if (File.Exists(path))
            {
                Process.Start(path);
            }
        }

        private static void AddStatics(string projectName)
        {
            var statics = GetStaticsList();
            UpdateProjectFile(projectName, statics);
            CopyToTarget(projectName, statics);
            Logger.LogSuccess("Added statics");
        }

        private static List<string> GetStaticsList()
        {
            var statics = new List<string>
            {
                @"Fonts\SegoeUILight.woff",
                @"Fonts\SegoeUI.woff",
                @"Fonts\Tunisia.woff",
                @"Fonts\IconFont.woff",
                @"Fonts\Typography.css",

                @"Scripts\AngularJs.js",
                @"Scripts\Utilities.js",

                @"Styles\Reset.css",
                @"Styles\FluidGrid.css",
                @"Styles\Common.css",
                @"Styles\Login.css"
            };
            var staticsListPath = Path.Combine(Environment.CurrentDirectory, "StaticsList.txt");
            if (File.Exists(staticsListPath))
            {
                statics = File.ReadLines(staticsListPath).Where(i => i.IsSomething()).ToList();
            }
            return statics;
        }

        private static void UpdateProjectFile(string projectName, List<string> statics)
        {
            var projectPath = @"%SunProjectsRoot%\{0}\Site\Site.csproj".Fill(projectName);
            projectPath = Config.ExpandEnvironmentVariables(projectPath);
            var projectDom = XDocument.Parse(File.ReadAllText(projectPath));
            var itemGroup = new XElement(projectDom.Root.GetDefaultNamespace() + "ItemGroup");
            foreach (var @static in statics)
            {
                var hasItem = projectDom.Descendants(projectDom.Root.GetDefaultNamespace() + "Content").Any(i => i.Attribute("Include").IsNotNull() && i.Attribute("Include").Value == @static);
                if (!hasItem)
                {
                    var item = new XElement(projectDom.Root.GetDefaultNamespace() + "Content");
                    item.SetAttributeValue("Include", @static);
                    itemGroup.Add(item);
                }
            }
            if (itemGroup.HasElements)
            {
                projectDom.Root.Add(itemGroup);
                File.WriteAllText(projectPath, projectDom.ToString());
            }
        }

        private static void CopyToTarget(string projectName, List<string> statics)
        {
            foreach (var @static in statics)
            {
                var sourcePath = @"%SunProjectsRoot%\Framework\Web\{0}".Fill(@static);
                sourcePath = Config.ExpandEnvironmentVariables(sourcePath);
                var targetPath = @"%SunProjectsRoot%\{0}\Site\{1}".Fill(projectName, @static);
                targetPath = Config.ExpandEnvironmentVariables(targetPath);
                var targetDirectory = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                CommandLine.App(Config.ExpandEnvironmentVariables(@"%windir%\System32\cmd.exe")).Execute("/c mklink /H {0} {1}".Fill(targetPath, sourcePath));
            }
        }

        public static void Empty()
        {
            DeleteAllSites();
            DeleteAllAppPools();
            DeleteAllDnsEntries();
        }

        private static void DeleteAllDnsEntries()
        {
            var currentLines = File.ReadAllLines(hostsPath).ToList();
            currentLines = currentLines.Where(i => !i.Trim().StartsWith("#")).ToList();
            File.WriteAllLines(hostsPath, new string[] { });
            Logger.LogSuccess("Deleted dns entries");
        }

        private static void DeleteAllAppPools()
        {
            Logger.LogInfo("Deleting all apppools...");
            var appPoolPattern = new Regex(@"APPPOOL ""([^""]*)""");
            var output = ExecuteAppCmdCommand("list apppool");
            var appPools = appPoolPattern.Matches(output).OfType<Match>().Select(i => i.Groups[1].Value).ToList();
            foreach (var appPool in appPools)
            {
                DeleteAppPool(appPool);
                Logger.LogSuccess("Deleted {0}".Fill(appPool));
            }
        }

        private static void DeleteAllSites()
        {
            Logger.LogInfo("Deleting all sites...");
            var siteNamePattern = new Regex(@"SITE ""([^""]*)""");
            var output = ExecuteAppCmdCommand("list site");
            var sites = siteNamePattern.Matches(output).OfType<Match>().Select(i => i.Groups[1].Value).ToList();
            foreach (var site in sites)
            {
                DeleteSite(site);
                Logger.LogSuccess("Deleted {0}".Fill(site));
            }
        }
    }
}