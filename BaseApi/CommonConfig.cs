namespace Sepidar.BaseApi
{
    public class CommonConfig : Config
    {
        public static string VasUsername
        {
            get
            {
                return GetSetting("Vas:Username");
            }
        }
    }
}
