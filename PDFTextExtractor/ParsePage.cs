using PDFExtractor.Primitives;

namespace PDFExtractor
{
    /// <summary>
    /// Wraps a TextPage(List of string and Location) and a semi-accurate text output
    /// </summary>
    public class ParsePage
    {
        public string PageText { get; }
        public TextPage TextBlocks { get; }
        public bool IsEmpty => TextBlocks.Count == 0;
        public SizeD Size => TextBlocks.Size;

        public ParsePage(string pageText, TextPage textBlocks, SizeD size)
        {
            PageText = pageText;
            TextBlocks = textBlocks;
            TextBlocks.Size = size;
        }
    }
}
