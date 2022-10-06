using System.Collections.Generic;
using PDFExtractor.Primitives;

namespace PDFExtractor
{
    public class TextPage : List<ExtractedText>
    {
        public SizeD Size { get; set; }
    }
}
