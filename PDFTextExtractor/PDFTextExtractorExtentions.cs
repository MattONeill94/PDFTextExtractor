using PDFExtractor.Primitives;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using System;
using System.Collections.Generic;

namespace PDFExtractor
{
    internal static class PDFTextExtractorExt
    {
        public static SizeD GetSize(this PdfPage page) => page.MediaBox != null ? new SizeD(page.MediaBox.Width, page.MediaBox.Height) : new SizeD(0, 0);

        public static ParsePage GetParsePage(this PdfPage page) => PDFTextExtractor.GetText(page);
    }
}
