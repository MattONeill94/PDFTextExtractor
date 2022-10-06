using PdfSharp.Pdf;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using PdfSharp.Drawing;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text;

namespace PDFExtractor.Fonts
{
    internal class FontType1 : BaseFont
    {
        public string BaseFont { get; protected set; }
        public string SubsetFontName { get; }


        public FontType1(PdfDictionary dictionary) : base(dictionary)
        {
            if (dictionary.Elements.ContainsKey("/BaseFont"))
            {
                PdfItem item = dictionary.Elements["/BaseFont"];
                BaseFont = item.GetString().TrimStart('/');

                //remove Font Subset Name
                if (BaseFont.Contains('+'))
                {
                    var split = BaseFont.Split('+');
                    SubsetFontName = split.FirstOrDefault();
                    BaseFont = string.Join(string.Empty, split.Skip(1));
                }
                //for MM may want to be about to extract Spaced Values 
            }

            FillStandard14();
            UpdateUnicode(dictionary);
        }

        private void FillStandard14()
        {
            switch (BaseFont)
            {
                case "Times-Roman":
                    Base14Parser.FillFont(Standard14FontsSource.Times_Roman_Source, this);
                    break;
                case "Helvetica":
                    Base14Parser.FillFont(Standard14FontsSource.Helvetica_Source, this);
                    break;
                case "Courier":
                    Base14Parser.FillFont(Standard14FontsSource.Courier_Source, this);
                    break;
                case "Symbol":
                    Base14Parser.FillFont(Standard14FontsSource.Symbol_Source, this);
                    break;
                case "Times-Bold":
                    Base14Parser.FillFont(Standard14FontsSource.Times_Bold_Source, this);
                    break;
                case "Helvetica-Bold":
                    Base14Parser.FillFont(Standard14FontsSource.Helvetica_Bold_Source, this);
                    break;
                case "Courier-Bold":
                    Base14Parser.FillFont(Standard14FontsSource.Courier_Bold_Source, this);
                    break;
                case "ZapfDingbats":
                    Base14Parser.FillFont(Standard14FontsSource.ZapfDingbats_Source, this);
                    break;
                case "Times-Italic":
                    Base14Parser.FillFont(Standard14FontsSource.Times_Italic_Source, this);
                    break;
                case "Helvetica-Oblique":
                    Base14Parser.FillFont(Standard14FontsSource.Helvetica_Oblique_Source, this);
                    break;
                case "Courier-Oblique":
                    Base14Parser.FillFont(Standard14FontsSource.Courier_Oblique_Source, this);
                    break;
                case "Times-BoldItalic":
                    Base14Parser.FillFont(Standard14FontsSource.Times_BoldItalic_Source, this);
                    break;
                case "Helvetica-BoldOblique":
                    Base14Parser.FillFont(Standard14FontsSource.Helvetica_BoldOblique_Source, this);
                    break;
                case "Courier-BoldOblique":
                    Base14Parser.FillFont(Standard14FontsSource.Courier_BoldOblique_Source, this);
                    break;

            }


        }

    }
}
