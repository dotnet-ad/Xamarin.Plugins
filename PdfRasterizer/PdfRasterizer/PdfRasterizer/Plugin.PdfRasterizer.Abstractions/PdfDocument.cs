using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.PdfRasterizer.Abstractions
{
    public class PdfDocument
    {
        public string Name { get; set; }

        public DateTime RenderingDate { get; set; }

        public IEnumerable<PdfPage> Pages { get; set; }
    }
}
