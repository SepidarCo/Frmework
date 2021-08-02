//using Newtonsoft.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sepidar.Framework
{
    public static class ExceptionTranslator
    {
        private static string frameworkTranslationFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "FrameworkExceptionTranslations.json");
        private static string overridingTranslationsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OverrideExceptionTranslations.json");
        private static List<ExceptionTranslation> translations;

        static ExceptionTranslator()
        {
            LoadExceptionTranslations();
        }

        private static void LoadExceptionTranslations()
        {
            if (!File.Exists(frameworkTranslationFilePath))
            {
                Logger.LogWarning("There is no FrameworkExceptionTransaltions.json file in the working directory.");
                translations = new List<ExceptionTranslation>();
                return;
            }
            var content = File.ReadAllText(frameworkTranslationFilePath);
            translations = JsonConvert.DeserializeObject<List<ExceptionTranslation>>(content).ToList();
            OverrideTranslations();
        }

        private static void OverrideTranslations()
        {
            if (!File.Exists(overridingTranslationsFilePath))
            {
                CreateSampleFile();
                return;
            }
            var content = File.ReadAllText(overridingTranslationsFilePath);
            var localTranslations = JsonConvert.DeserializeObject<List<ExceptionTranslation>>(content).ToList();
            var intersects = translations.Intersect(localTranslations);
            translations = translations.Except(intersects).ToList();
            translations.AddRange(localTranslations);
        }

        private static void CreateSampleFile()
        {
            var translationList = new List<ExceptionTranslation>();
            var SampleExceptionTranslation = new ExceptionTranslation() { Type = MessageType.Error, ServerMessage = "Sample Error Message", ClientMessage = "این یک پیغام نمونه است" };
            translationList.Add(SampleExceptionTranslation);
            var SampleJson = JsonConvert.SerializeObject(translationList);
            File.WriteAllText(overridingTranslationsFilePath, SampleJson);
        }

        public static string GetJsonMsesage(string serverMessage)
        {
            //var msgContents = File.ReadAllText(overridingTranslationsFilePath);
            //List<ExceptionTranslation> messages = JsonConvert.DeserializeObject<List<ExceptionTranslation>>(msgContents).ToList();
            var message = translations.FirstOrDefault(m => m.ServerMessage.ToLower().Contains(serverMessage.ToLower()) || serverMessage.ToLower().Contains(m.ServerMessage.ToLower()));
            if (message == null)
                return null;
            var jsonMsg = JsonConvert.SerializeObject(message);
            return jsonMsg;
        }

        public static string Translate(string serverMessage)
        {
            //var msgContents = File.ReadAllText(overridingTranslationsFilePath);
            //List<ExceptionTranslation> messages = JsonConvert.DeserializeObject<List<ExceptionTranslation>>(msgContents).ToList();
            var message = translations.FirstOrDefault(m => m.ServerMessage.ToLower().Contains(serverMessage.ToLower()) || serverMessage.ToLower().Contains(m.ServerMessage.ToLower()));
            if (message == null)
                return null;
            if (string.IsNullOrEmpty(message.ClientMessage))
                return "برای پیغام یک معادل خوب در فایل ترجمه تعریف کنید " + message.ServerMessage;
            return message.ClientMessage;
        }
    }
}