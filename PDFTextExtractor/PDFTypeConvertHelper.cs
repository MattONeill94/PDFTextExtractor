using PdfSharp.Pdf.Content.Objects;
using System.Linq;
using System.Text;
using System.Data;
using PDFExtractor.Fonts;
using System;

namespace PDFExtractor
{
    internal static class PDFTypeConvertHelper
    {
        public static string GetText(this CObject COperator, BaseFont font)
        {
            if (COperator is not CString)
                throw new Exception("Operator isn't a string:" + COperator.ToString());
            var stringOperator = COperator as CString;
            StringBuilder target = new StringBuilder();
            if (font.IsTwoByte)
            {
                foreach (var s in stringOperator.Value.Select((c, i) => new { c = c << (1 - i % 2) * 8, i }).GroupBy(o => o.i / 2).Select(g => (char)g.Sum(o => o.c))
                    .Select(c => font.ToUnicode(c)))
                    target.Append(s);
            }
            else
                foreach (var s in stringOperator.Value.Select(c => font.ToUnicode(c))) target.Append(s);
            return target.ToString();
        }
        public static double GetNumber(this CObject numb)
        {
            if (numb is CReal)
                return ((CReal)numb).Value;
            if (numb is CInteger)
                return ((CInteger)numb).Value;
            else
                return 0;
        }
    }


}
