using PdfSharp.Pdf;
using System.Linq;

namespace PDFExtractor.Fonts
{
    internal class MMFontType1 : FontType1
    {
        public string[] DesignCoordinates { get; }
        public MMFontType1(PdfDictionary dictionary) : base(dictionary)
        {
            var split = BaseFont.Split('_');
            BaseFont = split.FirstOrDefault();
            DesignCoordinates = split.Skip(1).ToArray();
        }


    }
}
