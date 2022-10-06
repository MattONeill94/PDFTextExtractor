using PDFExtractor.Primitives;
using PdfSharp.Pdf;
using System.Collections.Generic;

namespace PDFExtractor.Fonts
{
    internal class FontType3 : BaseFont
    {
        public FontType3(PdfDictionary dictionary) : base(dictionary)
        {
        }

        /// <summary>
        /// If all 0's, No Assumptions can be made abotu Glyph Sizes
        /// </summary>
        public PDFRectangle FontBBox { get; }

        public Matrix FontMatrix { get; }

        public Dictionary<string, object> CharProcs { get; } = new Dictionary<string, object>();

        public Dictionary<string, object> Resources { get; } = new Dictionary<string, object>();


    }
}
