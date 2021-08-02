using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public class MenuItem
    {
        public string Text { get; set; }

        public bool IsLink { get; set; }

        public string Href { get; set; }

        public List<MenuItem> SubmenuItems { get; set; }
    }
}
