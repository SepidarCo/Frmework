using System.Globalization;

namespace Sepidar.Framework
{
    public class EnglishPluralizationService
    {
        Inflector.Inflector inflector = new Inflector.Inflector(new CultureInfo("en"));

        public string Pluralize(string name)
        {
            return inflector.Pluralize(name);
        }

        public string Singularize(string name)
        {
            return inflector.Singularize(name);
        }
    }
}
