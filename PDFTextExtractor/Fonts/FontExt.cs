using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;

namespace PDFExtractor.Fonts
{
    internal static class FontExt
    {

        public static PdfItem RemovePDfReference(this PdfItem item) => item is PdfReference ? (item as PdfReference).Value : item;

        public static string GetString(this PdfItem item, bool TrimSlash = true)
        {
            item = item.RemovePDfReference();
            var val = item is PdfString ? (item as PdfString).Value : item.ToString();
            return TrimSlash ? val.TrimStart('/') : val;
        }

        public static int GetInt(this PdfItem item)
        {
            item = item.RemovePDfReference();
            return item is PdfInteger ? (item as PdfInteger).Value : int.Parse(item.GetString());
        }

        public static double GetNumber(this PdfItem item)
        {
            item = item.RemovePDfReference();
            if (item is PdfInteger)
                return (item as PdfInteger).Value;
            else if (item is PdfReal)
                return (item as PdfReal).Value;//Change if PDFNumber gets a value
            else
                return double.Parse(item.GetString());
        }


    }
}
