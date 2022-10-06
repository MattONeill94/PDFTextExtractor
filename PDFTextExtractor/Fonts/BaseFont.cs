using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data;

namespace PDFExtractor.Fonts
{
    internal class BaseFont
    {
        public string Type { get; protected set; }
        public string Name { get; protected set; }
        public PDFSubType SubType { get; protected set; }
        public string EncodingName { get; protected set; } = null;
        public EncodingDictionary Encoding { get; }

        public int FirstChar { get; set; }
        public int LastChar { get; set; }
        public List<double> Widths { get; } = new List<double>();

        public FontDescriptor FontDescriptor { get; set; }

        public BaseFont(PdfDictionary dictionary)
        {
            if (dictionary.Elements.ContainsKey("/Name"))
            {
                PdfItem item = dictionary.Elements["/Name"];
                Name = item.GetString().TrimStart('/');
            }

            if (dictionary.Elements.ContainsKey("/Subtype"))
            {
                SubType = (PDFSubType)Enum.Parse(typeof(PDFSubType), dictionary.Elements["/Subtype"].GetString());
            }


            if (dictionary.Elements.ContainsKey("/FontDescriptor"))
            {
                FontDescriptor = new FontDescriptor(dictionary.Elements["/FontDescriptor"].RemovePDfReference() as PdfDictionary);
            }

            if (dictionary.Elements.ContainsKey("/FirstChar"))
            {
                PdfItem item = dictionary.Elements["/FirstChar"].RemovePDfReference();
                FirstChar = item.GetInt();
            }

            if (dictionary.Elements.ContainsKey("/LastChar"))
            {
                PdfItem item = dictionary.Elements["/LastChar"].RemovePDfReference();
                LastChar = item.GetInt();
            }

            if (dictionary.Elements.ContainsKey("/Widths"))
            {
                PdfItem arr = dictionary.Elements["/Widths"].RemovePDfReference();

                if (arr is PdfArray)
                {
                    foreach (var item in arr as PdfArray)
                    {
                        Widths.Add(item.GetNumber());
                    }
                }
                else
                    throw new Exception($"Unknown Type for {Name} Font Widths: {arr}");
            }

            if (dictionary.Elements.ContainsKey("/Encoding"))
            {
                PdfItem item = dictionary.Elements["/Encoding"];
                if (item is PdfReference)
                {
                    /*
                     * TODO
                     */
                    var dict = (item as PdfReference).Value as PdfDictionary;
                    Encoding = new EncodingDictionary(dict);
                    EncodingName = Encoding.BaseEncoding;
                }
                else EncodingName = item.GetString();
            }
            if (EncodingName != null && EncodingName.StartsWith("/Identity-")) IsTwoByte = true;

            UpdateUnicode(dictionary);
        }

        //Functioned this out So base 14 Fonts can call this code in Font1
        protected void UpdateUnicode(PdfDictionary dictionary)
        {
            if (dictionary.Elements.ContainsKey("/ToUnicode"))
            { // parse to unicode
                PdfItem item = dictionary.Elements["/ToUnicode"];
                if (item is PdfReference) item = (item as PdfReference).Value;
                if (item is PdfDictionary)
                {
                    string map = (item as PdfDictionary).Stream.ToString();
                    toUnicode = ParseCMap(map);
                }
            }
            else
            {

                if (EncodingName != null)
                {
                    switch (EncodingName)
                    {
                        case "MacRomanEncoding":
                            toUnicode = EncodingTables.MacRoman;
                            break;
                        case "WinAnsiEncoding":
                            toUnicode = EncodingTables.WinAnsi;
                            break;
                        case "MacExpertEncoding":
                            toUnicode = EncodingTables.MacExpert;
                            break;
                        case "Standard":
                            toUnicode = EncodingTables.Standard;
                            break;
                        case "Symbol":
                            toUnicode = EncodingTables.Symbol;
                            break;
                    }
                }
                else
                {
                    if ((FontDescriptor?.Flags & Flag_Symbolic) != 0)
                        toUnicode = EncodingTables.Symbol;
                    else
                        toUnicode = EncodingTables.Standard;
                }
            }
        }

        public static BaseFont BuildFont(PdfDictionary dictionary)
        {
            if (!dictionary.Elements.ContainsKey("/Subtype"))
                throw new Exception("No subtype Found");

            var SubType = (PDFSubType)Enum.Parse(typeof(PDFSubType), dictionary.Elements["/Subtype"].GetString());
            switch (SubType)
            {
                case PDFSubType.Type1:
                    return new FontType1(dictionary);
                case PDFSubType.TrueType:
                    return new TrueTypeFont(dictionary);
                case PDFSubType.MMType1:
                    return new MMFontType1(dictionary);
                case PDFSubType.Type3:
                    return new FontType3(dictionary);
                default:
                    throw new Exception();
            }
        }


        public double GetCharacterWidth(char c)
        {
            if (c > LastChar)
                return FontDescriptor.AvgWidth / 1000;

            var index = c - FirstChar;

            if (index < 0)
                return FontDescriptor.AvgWidth / 1000;

            return Widths[index] / 1000;//move to glyph space

        }

        #region Borrowed Code
        const int Flag_Symbolic = 4;
        public int Flags { get; set; }
        public bool IsTwoByte { get; }

        Dictionary<ushort, string> toUnicode = new Dictionary<ushort, string>();
        public char nodef_char { get; set; } = '\xE202';
        Dictionary<ushort, string> ParseCMap(string map)
        {
            Dictionary<ushort, string> cmap = new Dictionary<ushort, string>();
            try
            {
                map = map.ToLower();
                int bf = map.IndexOf("beginbfrange");
                while (bf >= 0)
                {
                    int ef = map.IndexOf("endbfrange", bf);
                    if (ef < 0) ef = map.Length;

                    // parsing ranges
                    string[] Ranges = map.Substring(bf + 13, ef - (bf + 13)).Split('\n', '\r');
                    foreach (string range in Ranges)
                    {
                        Match m = Regex.Match(range, "<([0-9abcdef]+)> <([0-9abcdef]+)> <([0-9abcdef]+)>");
                        if (m.Success)
                        {
                            int st = int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
                            int end = int.Parse(m.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
                            char uni = (char)int.Parse(m.Groups[3].Value, System.Globalization.NumberStyles.HexNumber);
                            end = Math.Min(ushort.MaxValue - 1, end);
                            st = Math.Min(st, end);
                            for (ushort q = (ushort)st; q <= end; q++)
                                cmap[q] = "" + uni++;
                            continue;
                        }
                        m = Regex.Match(range, @"<([0-9abcdef]+)> <([0-9abcdef]+)> \[(.+)\]");
                        if (m.Success)
                        {
                            int st = int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
                            int end = int.Parse(m.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
                            end = Math.Min(ushort.MaxValue - 1, end);
                            st = Math.Min(st, end);
                            foreach (Match mm in Regex.Matches(m.Groups[3].Value, "<([0-9abcdef]+)>"))
                            {
                                if (mm.Groups.Count > 1)
                                {
                                    cmap[(ushort)st++] = new string(mm.Groups[1].Value.Select((x, i) => new { x, i }).GroupBy(o => o.i / 4).Select(g => new string(g.Select(o => o.x).ToArray()))
                                        .Select(s => (char)int.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToArray());
                                    if (st >= end) break;
                                }

                            }
                        }
                    }
                    bf = map.IndexOf("beginbfrange", ef);

                }
                bf = map.IndexOf("beginbfchar");
                while (bf >= 0)
                {
                    int ef = map.IndexOf("endbfchar", bf);
                    if (ef < 0) ef = map.Length;

                    // parsing ranges
                    string[] Ranges = map.Substring(bf + 11, ef - (bf + 11)).Split('\n', '\r');
                    foreach (string range in Ranges)
                    {
                        Match m = Regex.Match(range, "<([0-9abcdef]+)> <([0-9abcdef]+)>");
                        if (m.Success)
                        {
                            int st = int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
                            st = Math.Min(st, ushort.MaxValue - 1);
                            cmap[(ushort)st] = new string(m.Groups[2].Value.Select((x, i) => new { x, i }).GroupBy(o => o.i / 4).Select(g => new string(g.Select(o => o.x).ToArray()))
                                .Select(s => (char)int.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToArray());
                            continue;
                        }
                    }
                    bf = map.IndexOf("beginbfchar", ef);

                }
            }
            catch //(Exception e)
            {
                Console.WriteLine("Error parsing cmap range");
            }

            return cmap;
        }

        public string ToUnicode(ushort val)
        {
            if (toUnicode != null)
            {
                if (toUnicode.TryGetValue(val, out string str)) return str;
                else
                {
                    Console.WriteLine($"Warning! No unicode symbol for {val}!");
                }
            }
            return "";
        }
        #endregion

    }
}
