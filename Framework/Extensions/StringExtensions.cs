using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Sepidar.Framework.Extensions
{
    public static class StringExtensions
    {
        static Regex persianPattern = new Regex(@"[\p{IsArabic}]+");

        public static string Fill(this string input, params object[] args)
        {
            return string.Format(input, args);
            // todo: if an exception of type "index should be greater than zero, stuff" happened, somehow tell developer which argument is not provided, or which aregument is extra
        }

        public static object JsonDeserialize(this string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public static T JsonDeserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToPascalCase(this string text)
        {
            text = text.ToProperCase();
            TextInfo info = Thread.CurrentThread.CurrentCulture.TextInfo;
            text = info.ToTitleCase(text);
            string[] parts = text.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
            string result = String.Join(String.Empty, parts);
            return result;
        }

        public static string ToCamelCase(this string text)
        {
            text = text.ToPascalCase();
            var result = text.Substring(0, 1).ToLower() + text.Substring(1);
            return result;
        }

        public static T ToEnum<T>(this string value)
        {
            if (typeof(T).BaseType.Name != typeof(Enum).Name)
            {
                throw new Exception("Input type of generic method ToEnum<T>() is not an Enum");
            }
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string ToProperCase(this string text)
        {
            const string pattern = @"(?<=\w)(?=[A-Z])";
            string result = Regex.Replace(text, pattern, " ", RegexOptions.None);
            result = result.Substring(0, 1).ToUpper() + result.Substring(1);
            return result;
        }

        public static int ToInt(this string text)
        {
            int.TryParse(text, out int result);
            return result;
        }
        
        public static long ToLong(this string text)
        {
            long.TryParse(text, out long result);
            return result;
        }

        public static string Md5Hash(this string input)
        {
            return input.Hash(HashType.MD5);
        }

        public static bool IsNothing(this string text)
        {
            var isNothing = string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
            return isNothing;
        }

        public static bool IsSomething(this string text)
        {
            var isSomething = !text.IsNothing();
            return isSomething;
        }

        public static List<T> SplitCsv<T>(this string text)
        {
            List<T> result = new List<T>();
            if (text.IsNothing())
            {
                return result;
            }
            List<string> tokens = text.Split(',').Select(i => i.Trim()).ToList();
            tokens.ForEach(t =>
            {
                try
                {
                    result.Add((T)Convert.ChangeType(t, typeof(T)));
                }
                catch
                {
                    Logger.LogError("Error in converting item {0} to type {1} in SplitCsv<T>() method".Fill(t, typeof(T)));
                }
            });
            return result;
        }

        //public static string HttpGetParameterValue(this string key)
        //{
        //    if (HttpContext.Current.IsNull())
        //    {
        //        throw new NullReferenceException("HttpContext.Current is null");
        //    }
        //    HttpRequest request = HttpContext.Current.Request;
        //    if (request[key] == null)
        //    {
        //        return string.Empty;
        //    }
        //    return request[key];
        //}

        //public static string ToAbsolute(this string relativePath)
        //{
        //    return HttpContext.Current.Server.MapPath(relativePath);
        //}

        public static string ToLdsw(this string text)
        {
            // Lowercasing second capital letter, in double capital letter pairs
            Regex regex = new Regex("[A-Z]([A-Z])");
            foreach (Match match in regex.Matches(text))
            {
                Group group = match.Groups[1];
                text = text.Remove(group.Index, 1).Insert(group.Index, group.Value.ToLower());
            }
            List<string> words = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.ToPersianAlphaNumeric().ToLower()).ToList();
            var result = words.Aggregate((a, b) => "{0}-{1}".Fill(a, b)).Trim('-').Singlify('-');
            return result;
        }

        public static string PascalCase(this string text)
        {
            var result = string.Join("", text.Split('-').Select(i => "{0}{1}".Fill(i.ToArray().First().ToString().ToUpper(), i.Substring(1, i.Length - 1))));
            return result;

            // Lowering second capital letter, in double capital letter pairs.

            //Regex regex = new Regex("[A-Z]([A-Z])");
            //foreach (Match match in regex.Matches(text))
            //{
            //    Group group = match.Groups[1];
            //    text = text.Remove(group.Index, 1).Insert(group.Index, group.Value.ToLower());
            //}
            //return text;
        }

        public static string SentenceCase(this string text)
        {
            var result = string.Join("", text.Split('-').Aggregate((a, b) => "{0} {1}".Fill(a, b)));
            return result;
        }

        public static string ToPersianAlphaNumeric(this string text)
        {
            var result = Regex.Replace(text, "[^A-Za-z0-9]", string.Empty);
            return result;
        }

        public static string Singlify(this string text, char character)
        {
            var result = Regex.Replace(text, character.ToString() + "{2,}", character.ToString());
            return result;
        }

        public static string Trim(this string text, char character)
        {
            while (text.StartsWith(character.ToString()))
            {
                text = text.Remove(0, 1);
            }
            while (text.EndsWith(character.ToString()))
            {
                text = text.Remove(text.Length - 1, 1);
            }
            return text;
        }

        public static string ToBase64(this string text)
        {
            return Convert.ToBase64String(text.ToBytes());
        }

        public static string FromBase64(this string text)
        {
            return Convert.FromBase64String(text).GetString();
        }

        public static byte[] ToBytes(this string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public static string Hash(this string text, HashType type = HashType.MD5)
        {
            if (text.IsNothing())
            {
                return "";
            };
            HashAlgorithm algorithm;
            switch (type)
            {
                case HashType.MD5:
                    algorithm = MD5.Create();
                    break;
                case HashType.SHA1:
                    algorithm = SHA1.Create();
                    break;
                case HashType.SHA256:
                    algorithm = SHA256.Create();
                    break;
                case HashType.SHA512:
                    algorithm = SHA512.Create();
                    break;
                default:
                    throw new FrameworkException("Invalid hash type {0}".Fill(type));
            }
            // Adding something (salt) to text to make it harder to guess
            text += Config.HashSalt;
            var hash = Regex.Replace(algorithm.ComputeHash(text.ToBytes()).GetString().ToBase64(), @"[^a-zA-Z1-9]", string.Empty);
            return hash;
        }

        public static string Cut(this string text, int length)
        {
            if (text.Length < length)
            {
                return text;
            }
            string result = text.Substring(0, length);
            if (result.EndsWith(" "))
            {
                return "{0}...".Fill(result);
            }
            return "{0} ...".Fill(result);
        }

        public static int HexToInt(this string hex)
        {
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        public static List<string> Add(this string text1, string text2)
        {
            var result = new List<string>() { text1, text2 };
            return result;
        }

        public static string ReplacePunctuations(this string text, char replacement = ' ')
        {
            var textWithoutPunctuations = new string(text.ToCharArray().Select(c => char.IsPunctuation(c) ? replacement : c).ToArray());
            return textWithoutPunctuations;
        }

        public static string FilterAlphaNumericOnly(this string text)
        {
            return Regex.Replace(text, "[^a-zA-Z0-9]", " ", RegexOptions.IgnoreCase);
        }

        public static string SplitPascalCase(this string text)
        {
            var splittedText = Regex.Replace(text, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1])).Trim();
            return splittedText;
        }

        public static string SplitCamelCase(this string text)
        {
            return Regex.Replace(text, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        //public static List<string> FindMisspellings(this string text, List<string> permittedWords = null)
        //{
        //    List<string> mispelledWords = new List<string>();
        //    using (Hunspell checker = new Hunspell("en_us.aff", "en_us.dic"))
        //    {
        //        IncludePermittedWords(permittedWords, checker);
        //        text
        //        .RemoveHtml()
        //        .ReplacePunctuations()
        //        .FilterAlphaNumericOnly()
        //        .SplitCamelCase()
        //        .SplitPascalCase()
        //        .Replace("\r\n", " ")
        //        .Split(' ')
        //        .ToList()
        //        .ForEach(word =>
        //        {
        //            word = CheckWordSpelling(mispelledWords, checker, word);
        //        });
        //    }
        //    return mispelledWords;
        //}

        //private static string CheckWordSpelling(List<string> mispelledWords, Hunspell checker, string word)
        //{
        //    word = word.ReplacePunctuations();
        //    var misspelled = !(checker.Spell(word) || checker.Spell(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word)) || checker.Spell(word.ToUpper()) || checker.Spell(word.ToLower()));
        //    if (!checker.Spell(word))
        //    {
        //        mispelledWords.Add(word);
        //    }
        //    return word;
        //}

        //private static void IncludePermittedWords(List<string> permittedWords, Hunspell checker)
        //{
        //    if (permittedWords.IsNotNull())
        //    {
        //        foreach (var permittedWord in permittedWords)
        //        {
        //            checker.Add(permittedWord);
        //        }
        //    }
        //}

        public static string RemoveHtml(this string text)
        {
            var result = Regex.Replace(text, "<.*?>", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return result;
        }

        public static string ReplaceAll(this string text, string regex, string replacement, RegexOptions regexOptions)
        {
            return Regex.Replace(text, regex, replacement, regexOptions);
        }

        // todo: Why do I get "s" being rendered in the heart of the next letter? For example when "s" is followed by "i", I see the dollar sign $ rather than "si".
        // Type-Driven Development (You can do many things with a string, or with a number, or with a User object, thus you develop based on the type)
        //public static Image ToImage(this string text, string fontName = "Tahoma", float fontSize = 13f)
        //{
        //    if (text.IsNothing())
        //    {
        //        throw new FrameworkException("Text should not be null or empty, so that it can be converted into an image");
        //    }
        //    Font font = new Font(fontName, fontSize);
        //    //first, create a dummy bitmap just to get a graphics object
        //    Image img = new Bitmap(1, 1);
        //    Graphics drawing = Graphics.FromImage(img);

        //    //measure the string to see how big the image needs to be
        //    SizeF textSize = drawing.MeasureString(text, font);

        //    //free up the dummy image and old graphics object
        //    img.Dispose();
        //    drawing.Dispose();

        //    //create a new image of the right size
        //    img = new Bitmap((int)textSize.Width, (int)textSize.Height);

        //    drawing = Graphics.FromImage(img);

        //    //paint the background
        //    drawing.Clear(Color.White);

        //    //create a brush for the text
        //    Brush textBrush = new SolidBrush(Color.Black);

        //    // todo: How to include letter-spacing here?
        //    drawing.DrawString(text, font, textBrush, 0, 0);

        //    drawing.Save();

        //    textBrush.Dispose();
        //    drawing.Dispose();

        //    return img;
        //}

        public static bool IsPersian(this string text)
        {
            var isPersian = text.ToCharArray().All(i => persianPattern.IsMatch(i.ToString()));
            return isPersian;
        }

        public static bool HasPersian(this string text)
        {
            return persianPattern.IsMatch(text);
        }

        public static string ToBinary(this string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            var result = string.Join("", bytes.Select(@byte => Convert.ToString(@byte, 2).PadLeft(8, '0')));
            return result;
        }

        public static bool CanBeCastTo<T>(this string value)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                return false;
            }
            var canBeCast = Enum.GetNames(type).Contains(value);
            if (!canBeCast)
            {
                return false;
            }
            return true;
        }

        public static string ToCommaSeparated(this string value)
        {
            var result = $"{value.ToLong():n0}";
            return result;
        }

        //public static dynamic JsonToDynamic(this string json)
        //{
        //    dynamic result = new ExpandoObject();
        //    if (json.IsNull())
        //    {
        //        return result;
        //    }
        //    var data = json.JsonDeserialize<JObject>();
        //    foreach (var item in data)
        //    {
        //        string key = item.Key;
        //        JToken value = item.Value.ToString();
        //        Core.Framework.Extensions.ExpandoObjectExtensions.AddProperty(result, key, value);
        //    }
        //    return result;
        //}
    }
}