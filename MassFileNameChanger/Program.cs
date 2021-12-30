using Sepidar.Framework;
using Sepidar.Console;
using Sepidar.Framework.Extensions;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sepidar.MassFileNameChanger
{
    [FrameworkConsole.PreMain]
    class Program
    {
        static Parameters parameters;

        static void Main(string[] args)
        {
            //FrameworkConsole.SetSize();
            args = new string[] { @"Path=D:\SepidarCo\Kido", "OldWord=Kidon", "NewWord=Kido" };

            ParseArgsIntoParameters(args);
            var files = Directory.EnumerateFiles(parameters.Path, "*.*", SearchOption.AllDirectories);
            var pattern = new Regex(@".*{0}.*".Fill(parameters.OldWord));
            files = files.Where(i => pattern.Match(i).Success).ToList();
            foreach (var file in files)
            {
                ChangeToNewWord(file);
            }
        }
        private static void ChangeToNewWord(string file)
        {
            var newName = file.Replace(parameters.OldWord, parameters.NewWord);
            var newPath = Path.GetDirectoryName(newName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            File.Move(file, newName);
        }

        private static void ParseArgsIntoParameters(string[] args)
        {
            parameters = new Parameters();
            var pathPart = args.SingleOrDefault(i => i.Contains("Path"));
            var oldWordPart = args.SingleOrDefault(i => i.Contains("OldWord"));
            var newWordPart = args.SingleOrDefault(i => i.Contains("NewWord"));
            if (pathPart.IsNull() || oldWordPart.IsNull() || newWordPart.IsNull())
            {
                throw new FrameworkException("Missing Path, OldWord, or NewWord argument(s).");
            }
            parameters.Path = pathPart.Split('=')[1];
            parameters.OldWord = oldWordPart.Split('=')[1];
            parameters.NewWord = newWordPart.Split('=')[1];
        }
    }
}
