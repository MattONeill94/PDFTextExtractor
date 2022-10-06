using PdfSharp.Pdf;

namespace PDFExtractor.Fonts
{
    internal class TrueTypeFont : FontType1
    {
        public TrueTypeFont(PdfDictionary dictionary) : base(dictionary)
        {
            //Ignoring Name of Font for TureType for now.
        }
    }
}
