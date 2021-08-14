using Newtonsoft.Json.Linq;

namespace Sepidar.Validation
{
    public static class Extensions
    {
        public static Ensure Ensure(this object @object)
        {
            return new Ensure(@object);
        }

        public static bool IsJson(this string text)
        {
            text = text.Trim();
            try
            {
                if (text.StartsWith("["))
                {
                    var json = JArray.Parse(text);
                    return true;
                }
                if (text.StartsWith("{"))
                {
                    var json = JObject.Parse(text);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
