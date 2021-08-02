using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Sepidar.Framework
{
    public static class ExtensionMethods
    {
        //public static string Handle(this Exception ex, string message = "")
        //{
        //    Exception originalException = ex;
        //    string result = ex.Message;
        //    ex.Message.LogError();
        //    while (ex.InnerException != null)
        //    {
        //        ex = ex.InnerException;
        //        ex.Message.LogError();
        //    }
        //    ex.StackTrace.LogVerbosely();
        //    // todo: Translate exception message before returning it. If it's a FrameworkException, then return the message directly, otherwise, search a ExceptionTranslations.xml file on the root of the website (or inside a directory with a web.config file to prevent direct access using NotFoundHandler), and if you found an entry in that file, return it, otherwise see if there is a fallback message or not and return it on its presence, and at last return the original message.
        //    if (ex is FrameworkException)
        //    {
        //        return ex.Message;
        //    }
        //    result = TranslateExceptionMessage(ex.Message);
        //    if (result == ex.Message)
        //    {
        //        // Exception message can not be translated. Return the fallback message.
        //        result.Log();
        //        if (message.IsNothing())
        //        {
        //            message = Config.GeneralErrorMessage;
        //        }
        //        return Config.IsDeveloping ? result : message;
        //    }
        //    else
        //    {
        //        // Exception is translated. 
        //        return result;
        //    }
        //}

        //private static string TranslateExceptionMessage(string message)
        //{
        //    try
        //    {
        //        XDocument doc = XDocument.Load(HttpContext.Current.Server.MapPath("/ExceptionTranslation.xml"));
        //        XElement element = doc.Root.Elements().Where(e => e.Attribute("message").Value == message || message.Contains(e.Attribute("message").Value)).SingleOrDefault();
        //        if (element.IsNull())
        //        {
        //            return message;
        //        }
        //        return element.Attribute("translation").Value;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.Log();
        //        return message;
        //    }
        //}

        public static string ToJavaScriptArray(this Type type)
        {
            string[] names;
            if (type.BaseType.Name == "Enum")
            {
                names = type.GetEnumNames().Select(n => n.ToLdsw()).ToArray();
            }
            else
            {
                names = type.GetProperties().Select(pi => pi.Name.ToLdsw()).ToArray();
            }
            return "['{0}']".Fill(string.Join("', '", names));
        }

        //public static string ToBase64(this Image image)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        image.Save(ms, ImageFormat.Png);
        //        return Convert.ToBase64String(ms.ToArray());
        //    }
        //}

        public static string SchemeHostPort(this Uri uri)
        {
            return "{0}://{1}{2}".Fill(uri.Scheme, uri.Host, uri.Port == 80 ? "" : ":{1}".Fill(uri.Port));
        }
    }
}