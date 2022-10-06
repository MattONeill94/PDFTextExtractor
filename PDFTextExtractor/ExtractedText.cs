using PDFExtractor.Primitives;
using PdfSharp.Pdf;

namespace PDFExtractor
{
    public class ExtractedText
    {
        public PDFRectangle Bounds { get; }
        public string Text { get; }

        public ExtractedText(PDFRectangle bounds, string text)
        {
            Bounds = bounds;
            Text = text;
        }
        public override string ToString() => $"Bounds:{Bounds};Text:{Text}";
    }
}
