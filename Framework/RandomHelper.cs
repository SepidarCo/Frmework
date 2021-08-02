using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepidar.Framework
{
    public class RandomHelper
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);
        public const string NumericCharacters = "0123456789";
        public const string LowercaseAlphabeticCharacters = "abcdefghijklmnopqrstuvwxyz";
        public const string UppercaseAlphabeticCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GenerateAlphanumericToken(int length, bool includeUppercase = false, string specialCharacters = "")
        {
            var token = "";
            if (includeUppercase)
            {
                token = GenerateTokenFromCharacters(length, NumericCharacters + LowercaseAlphabeticCharacters + UppercaseAlphabeticCharacters + specialCharacters);
            }
            else
            {
                token = GenerateTokenFromCharacters(length, NumericCharacters + LowercaseAlphabeticCharacters + specialCharacters);
            }
            token = EnsureInclusionOfAllCharacterClasses(length, includeUppercase, specialCharacters, token);
            return token;
        }

        private static string EnsureInclusionOfAllCharacterClasses(int length, bool includeUppercase, string specialCharacters, string token)
        {
            bool hasAllCharacterClasses = false;
            while (!hasAllCharacterClasses)
            {
                if (!token.Any(i => NumericCharacters.Contains(i)))
                {
                    token = NumericCharacters[random.Next(0, NumericCharacters.Length)].ToString() + token;
                }
                if (!token.Any(i => LowercaseAlphabeticCharacters.Contains(i)))
                {
                    token = LowercaseAlphabeticCharacters[random.Next(0, LowercaseAlphabeticCharacters.Length)].ToString() + token;
                }
                if (!token.Any(i => UppercaseAlphabeticCharacters.Contains(i)) && includeUppercase)
                {
                    token = UppercaseAlphabeticCharacters[random.Next(0, UppercaseAlphabeticCharacters.Length)].ToString() + token;
                }
                if (!token.Any(i => specialCharacters.Contains(i)) && specialCharacters.IsSomething())
                {
                    token = specialCharacters[random.Next(0, specialCharacters.Length)].ToString() + token;
                }
                token = token.Substring(0, length);
                hasAllCharacterClasses = token.Any(x => RandomHelper.NumericCharacters.Contains(x));
                hasAllCharacterClasses &= token.Any(x => RandomHelper.LowercaseAlphabeticCharacters.Contains(x));
                hasAllCharacterClasses &= includeUppercase ? token.Any(x => RandomHelper.UppercaseAlphabeticCharacters.Contains(x)) : true;
                hasAllCharacterClasses &= specialCharacters.IsSomething() ? token.Any(x => specialCharacters.Contains(x)) : true;
            }
            return token;
        }

        private static string GenerateTokenFromCharacters(int length, string input)
        {
            StringBuilder builder = new StringBuilder();
            char character;
            for (int i = 0; i < length; i++)
            {
                character = input[random.Next(0, input.Length)];
                builder.Append(character);
            }
            return builder.ToString();
        }

        public static long GenerateNumericToken(int length)
        {
            var number = "";
            for (int i = 0; i < length; i++)
            {
                number += random.Next(1, 10).ToString();
            }
            return Convert.ToInt64(number);
        }

        public static TEnum RandomEnum<TEnum>()
        {
            Array values = Enum.GetValues(typeof(TEnum));
            TEnum value = (TEnum)values.GetValue(random.Next(values.Length));
            return value;
        }

        public static object Random(object[] objects)
        {
            if (objects.Count() == 0)
            {
                throw new FrameworkException("{0}[] has no element".Fill(objects.GetType().GetElementType().FullName));
            }
            var index = random.Next(objects.Length);
            var value = objects[index];
            return value;
        }
    }
}
