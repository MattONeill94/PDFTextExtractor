using PdfSharp.Pdf;
using System.Collections.Generic;

namespace PDFExtractor.Fonts
{
    internal class EncodingDictionary
    {
        public EncodingDictionary(PdfDictionary dictionary)
        {
            if (dictionary.Elements.ContainsKey("/Type"))
            {
                Type = dictionary.Elements["/Type"].GetString();
            }

            if (dictionary.Elements.ContainsKey("/BaseEncoding"))
            {
                BaseEncoding = dictionary.Elements["/BaseEncoding"].GetString();
            }

            if (dictionary.Elements.ContainsKey("/Differences"))
            {
                Differences.AddRange(ParseDifferences(dictionary.Elements["/Differences"] as PdfArray));
            }
        }

        public string Type { get; set; }
        public string BaseEncoding { get; set; }

        public List<EncodingDifference> Differences { get; } = new List<EncodingDifference>();

        IEnumerable<EncodingDifference> ParseDifferences(PdfArray arr)
        {
            int id = -1;
            List<string> descriptions = new List<string>();
            foreach (var item in arr)
            {
                var itm = item.RemovePDfReference();
                if (itm is PdfInteger)
                {
                    if (id != -1)
                    {
                        yield return new EncodingDifference() { Code = id, Names = descriptions.ToArray() };
                        descriptions.Clear();
                    }
                    id = itm.GetInt();
                }
                else
                    descriptions.Add(itm.GetString());
            }
            if (id != -1)
                yield return new EncodingDifference() { Code = id, Names = descriptions.ToArray() };
        }

    }
}
