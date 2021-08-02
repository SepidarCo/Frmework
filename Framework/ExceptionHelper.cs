using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.Framework
{
    public class ExceptionHelper
    {
        public static string GetSpecificInfo(Exception ex)
        {
            if (ex is ReflectionTypeLoadException)
            {
                var loaderExceptions = ((ReflectionTypeLoadException)ex).LoaderExceptions;
                var result = "";
                foreach (var loaderException in loaderExceptions)
                {
                    result += "\r\n{0}".Fill(loaderException.Message);
                }
                return result;
            }
            //else if (ex is HttpResponseException)
            //{
            //    var exception = ((HttpResponseException)ex);
            //    var result = "{0} - {1}".Fill(exception.Response.StatusCode, exception.Response.ReasonPhrase);
            //}
            else if (ex is FileNotFoundException)
            {
                return ((FileNotFoundException)ex).FileName;
            }
            return "";
        }

        public static string TranslateToFriendlyMessage(Exception ex)
        {
            string result = "";
            while (ex.IsNotNull())
            {
                if (ex is FrameworkException || ex is BusinessException)
                {
                    return ex.Message;
                }
                else if (ex is FileNotFoundException)
                {
                    return "فایل {0} یافت نشد".Fill(Path.GetFileNameWithoutExtension(((FileNotFoundException)ex).FileName));
                }
                else if (!string.IsNullOrEmpty(ExceptionTranslator.Translate(ex.Message)))
                {
                    return ExceptionTranslator.Translate(ex.Message.ToLower());
                }
                if (result.IsSomething())
                {
                    break;
                }
                ex = ex.InnerException;
            }
            return "خطایی رخ داده است. لطفا مجددا تلاش کنید. در صورت تکرار خطا، لطفا مشکل را اطلاع رسانی نمایید.";
        }

        private static void BuildExceptionString(Exception ex, ref string message)
        {
            message += "\r\n***********\r\n";
            message += ex.Message;
            message += "\r\n***********\r\n";
            message += ExceptionHelper.GetSpecificInfo(ex);
            message += "\r\n***********\r\n";
            message += FilterStackTrace(ex.StackTrace);
            if (ex.InnerException != null)
            {
                BuildExceptionString(ex.InnerException, ref message);
            }
            if (ex is AggregateException)
            {
                var innerExceptions = ((AggregateException)ex).InnerExceptions;
                foreach (var innerException in innerExceptions)
                {
                    message += "\r\n***********\r\n";
                    BuildExceptionString(innerException, ref message);
                }
            }
        }

        public static string BuildExceptionString(Exception ex)
        {
            string error = "";
            BuildExceptionString(ex, ref error);
            return error;
        }

        private static string FilterStackTrace(string stackTrace)
        {
            if (stackTrace.IsNothing())
            {
                return "";
            }
            var newStackTrace = stackTrace.ReplaceAll(@"^(\s*at\s*(System)+|-*\s*End|\s*at\s*lambda).*$", "", RegexOptions.Multiline).ReplaceAll(@"\n\n", "", RegexOptions.Multiline);
            return newStackTrace;
        }
    }
}