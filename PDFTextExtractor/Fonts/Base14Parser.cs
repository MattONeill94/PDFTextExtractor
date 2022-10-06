using System;
using System.Linq;
using System.Data;
using System.IO;
using System.IO.Compression;
using PDFExtractor.Primitives;

namespace PDFExtractor.Fonts
{
    internal class Base14Parser
    {

        public static void FillFont(string File_Source, BaseFont font)
        {
            var AllLines = DecompressData(File_Source).Replace("\r", "").Split('\n');
            var CharacterLines = AllLines.Where(i => i.StartsWith("C ")).Select(i => new CharacterMetrics(i)).ToArray();
            var Lookup = AllLines
                            .Where(i => !i.StartsWith("C "))
                            .ToLookup(i => i.IndexOf(' ') > -1 ? i.Substring(0, i.IndexOf(' ')) : i
                                        , i => i.IndexOf(' ') > -1 ? i.Substring(i.IndexOf(' ') + 1).Trim() : string.Empty);

            var fontDesciptor = new FontDescriptor();
            fontDesciptor.FontName = Lookup["FontName"].First();
            fontDesciptor.FontFamily = Lookup["FamilyName"].First();
            fontDesciptor.FontWeight = GetWeight(Lookup["Weight"].First());
            fontDesciptor.ItalicAngle = double.Parse(Lookup["ItalicAngle"].First());
            fontDesciptor.CharSet = Lookup["CharacterSet"].First();
            fontDesciptor.CapHeight = double.Parse(Lookup["CapHeight"].First());
            fontDesciptor.XHeight = double.Parse(Lookup["XHeight"].First());
            fontDesciptor.Ascent = double.Parse(Lookup["Ascender"].First());
            fontDesciptor.Descent = double.Parse(Lookup["Descender"].First());
            fontDesciptor.StemH = double.Parse(Lookup["StdHW"].First());
            fontDesciptor.StemV = double.Parse(Lookup["StdVW"].First());
            fontDesciptor.FontFamily = Lookup["FamilyName"].First();

            fontDesciptor.AvgWidth = CharacterLines.Sum(i => i.WX) / CharacterLines.Length;
            fontDesciptor.MaxWidth = CharacterLines.Max(i => i.WX);


            var BBValues = Lookup["FontBBox"].First().Trim().Split(' ').Select(i => double.Parse(i)).ToArray();
            fontDesciptor.FontBBox = new PDFRectangle(new PointD(BBValues[0], BBValues[1]), new PointD(BBValues[2], BBValues[3]));

            font.FirstChar = CharacterLines.First().C;
            font.LastChar = CharacterLines.Max(i => i.C);
            foreach (var line in CharacterLines)
            {
                font.Widths.Add(line.WX);
            }
            font.FontDescriptor = fontDesciptor;
        }
        struct CharacterMetrics
        {
            public int C { get; set; }
            public int WX { get; set; }
            public string N { get; set; }
            public double[] B { get; set; }
            public CharacterMetrics(string line)
            {
                //C 32 ; WX 600 ; N space ; B
                var items = line.Split(';');
                C = int.Parse(items[0].Trim().Substring(1));
                WX = int.Parse(items[1].Trim().Substring(2));
                N = items[2].Trim().Substring(1).Trim();
                B = items[3].Trim().Substring(2).Split(' ').Select(i => double.Parse(i)).ToArray();
            }
        }
        private static int GetWeight(string weight)
        {
            switch (weight.ToUpperInvariant())
            {

                //case "NORMAL":
                //    return 100;
                //case "NORMAL":
                //    return 200;
                //case "NORMAL":
                //    return 300;
                case "NORMAL":
                case "MEDIUM":
                    return 400;
                //case "NORMAL":
                //    return 500;
                //case "NORMAL":
                //    return 600;
                case "BOLD":
                    return 700;
                //case "NORMAL":
                //    return 800;
                //case "NORMAL":
                //    return 900;
                default:
                    return 400;
            }
        }

        public static string DecompressData(string File_Source)
        {
            using (var memStream = new MemoryStream(Convert.FromBase64String(File_Source)))
            using (var compressionStream = new BrotliStream(memStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(compressionStream))
            {
                return reader.ReadToEnd();
            }
        }
        /*
         * Code I used to Imbend the AFM Files Source.
         * Found the files in this Repo:
         *  https://github.com/chbrown/afm
         */
        /*
        public static void Run()
        {
            var folder = @"";


            var builder = new StringBuilder();
            builder.AppendLine(@"var temp = new Dictionary<string, string>() {");

            foreach (var file in Directory.GetFiles(folder, "*.afm"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                builder.AppendLine($"public string {name}_Source => \"{ToCompressedString(file)}\";");
            }
            builder.AppendLine("}");
            var str = builder.ToString();
        }
        public static string ToCompressedString(string file)
        {
            using (var memStream = new MemoryStream())
            using (var compressionStream = new BrotliStream(memStream, CompressionLevel.SmallestSize))
            using (var writer = new StreamWriter(compressionStream))
            using (var reader = new StreamReader(file))
            {
                writer.Write(reader.ReadToEnd());
                writer.Flush();
                return Convert.ToBase64String(memStream.ToArray());
            }
        }
        */
    }
}
