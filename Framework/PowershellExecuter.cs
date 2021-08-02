using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Text;


namespace Sepidar.Framework
{
    public class PowershellExecuter
    {
        //public static string Execute(string command)
        //{
        //    command = command + " | Out-String";
        //    var text = new StringBuilder();
        //    foreach (var item in Get(command))
        //    {
        //        text.AppendLine(item.ToString());
        //    }
        //    return text.ToString();

        //}

        //public static bool IsTrue(string command)
        //{
        //    var text = new StringBuilder();
        //    foreach (var item in Get(command))
        //    {
        //        text.AppendLine(item.ToString());
        //    }
        //    //Logger.LogInfo("Powershell execution\r\nChecking true result\r\nCommand is\r\n{0}\r\nResult is\r\n{0}".Fill(command, text.ToString()));
        //    var result = false;
        //    bool.TryParse(text.ToString(), out result);
        //    return result;
        //}

        //public static bool HasResult(string command)
        //{
        //    var result = Get(command);
        //    return result.IsNotNull() && result.Count > 0;
        //}

        //public static List<PSObject> Get(string command)
        //{
        //    using (PowerShell powershell = PowerShell.Create())
        //    {
        //        powershell.AddScript(command);
        //        var result = powershell.Invoke();
        //        return result.ToList();
        //    }
        //}
    }
}
