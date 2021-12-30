
namespace Sepidar.CodeGenerator
{
    public class Column
    {
        public string Name { get; set; }

        public string SqlDataType { get; set; }

        public string DotNetDataType { get; set; }

        public bool IsNullable { get; set; }

        public bool IsComputed { get; set; }

        public bool IsIdentity { get; set; }

        public int MaxLength { get; set; }
    }
}
