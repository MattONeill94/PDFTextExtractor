using PdfSharp.Pdf;
using System.Linq;
using System.Data;
using PDFExtractor.Primitives;

namespace PDFExtractor.Fonts
{
    internal class FontDescriptor
    {
        public string Type { get; set; }
        public string FontName { get; set; }
        public string FontFamily { get; set; }
        public string FontStretch { get; set; }
        public double FontWeight { get; set; }
        public int Flags { get; set; }
        public PDFRectangle FontBBox { get; set; }
        public double ItalicAngle { get; set; }
        public double Ascent { get; set; }
        public double Descent { get; set; }
        public double Leading { get; set; }
        public double CapHeight { get; set; }
        public double XHeight { get; set; }
        public double StemV { get; set; }
        public double StemH { get; set; }
        public double AvgWidth { get; set; }
        public double MaxWidth { get; set; }
        public double MissingWidth { get; set; }
        public string FontFile { get; set; }
        public string FontFile2 { get; set; }
        public string FontFile3 { get; set; }
        public string CharSet { get; set; }

        public FontDescriptor()
        {

        }
        public FontDescriptor(PdfDictionary dictionary)
        {
            foreach (var prop in GetType().GetProperties())
            {
                var key = $"/{prop.Name}";
                if (prop.PropertyType == typeof(double))
                {
                    if (dictionary.Elements.ContainsKey(key))
                        prop.SetValue(this, dictionary.Elements[key].GetNumber());
                }
                else if (prop.PropertyType == typeof(string))
                {
                    if (dictionary.Elements.ContainsKey(key))
                        prop.SetValue(this, dictionary.Elements[key].GetString());
                }
                else if (prop.PropertyType == typeof(int))
                {
                    if (dictionary.Elements.ContainsKey(key))
                        prop.SetValue(this, dictionary.Elements[key].GetInt());
                }
            }
            //FontBBox
            if (dictionary.Elements.ContainsKey("/FontBBox"))
            {
                var dimentions = (dictionary.Elements["/FontBBox"].RemovePDfReference() as PdfArray).Select(i => double.Parse(i.GetString())).ToArray();
                FontBBox = new PDFRectangle(new PointD(dimentions[0], dimentions[1]), new PointD(dimentions[2], dimentions[3]));
            }
        }

    }
}
