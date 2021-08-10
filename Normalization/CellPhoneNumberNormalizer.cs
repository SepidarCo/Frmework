using Sepidar.Validation;


namespace Sepidar.Normalization
{
    public class MobileNumberNormalizer
    {
        public static string NormalizeMobileNumber(string MobileNumber, bool validateMobileNumber = true)
        {
            MobileNumber = MobileNumber.Trim();
            MobileNumber = NumericNormalizer.ExtractNumbers(MobileNumber);

            if (validateMobileNumber)
            {
                MobileNumber.Ensure().AsString().IsMobileNumber();
            }
            else
            {
                MobileNumber.Ensure().AsString();
            }

            if (MobileNumber.StartsWith("+98"))
            {
                MobileNumber = "0" + MobileNumber.Substring(3);
            }
            if (MobileNumber.StartsWith("98"))
            {
                MobileNumber = "0" + MobileNumber.Substring(2);
            }
            if (MobileNumber.StartsWith("0098"))
            {
                MobileNumber = "0" + MobileNumber.Substring(4);
            }
            else if (MobileNumber.StartsWith("9"))
            {
                MobileNumber = "0" + MobileNumber;
            }
            return MobileNumber;
        }

        public static string NormalizeCountryCode(string MobileNumber, string prefix)
        {
            MobileNumber = NormalizeMobileNumber(MobileNumber);
            MobileNumber = MobileNumber.Remove(0, 1);
            MobileNumber = prefix + MobileNumber;
            return MobileNumber;
        }
    }
}
