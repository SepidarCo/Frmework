using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public static class RegularExpressions
    {
        public const string Email = @"^(('[\w-\s]+')|([\w-]+(?:\.[\w-]+)*)|('[\w-\s]+')([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)i";

        public const string Url = @"(?i)\b((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))";

        public const string MciMobile = @"^((00|\+)?98|0)?9(10|11|12|13|14|15|16|17|18|19|01|02|90|21)\d{7}$";

        public const string Mobile = @"^((00|\+)?98|0)?9\d{9}$";

        public const string PersianDateTime = @"(13\d{2})[-/](\d{1,2})[-/](\d{1,2})([ -]*(\d{1,2}:?){2,3})?";

        public const string LatinDateTime = @"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})";

        public const string PhoneNumber = @"^0(21|26|25|86|24|23|81|28|31|44|11|74|83|51|45|17|41|54|87|71|66|34|56|13|77|76|61|38|58|84|35)\d{8}$";

        public const string UserName = @"^(?=.{5,20}$)(?![_.])[a-zA-Z0-9._]+(?<![_.])$";
    }
}
