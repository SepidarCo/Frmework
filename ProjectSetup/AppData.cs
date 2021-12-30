using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.ProjectSetup
{
    public class AppData
    {
        public string IisSiteName { get; set; }

        public List<string> Bindings { get; set; }

        public string Path { get; set; }

        public bool ServesBlob { get; set; }

        public bool ServesStatics { get; set; }
    }
}
