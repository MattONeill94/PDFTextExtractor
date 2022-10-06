namespace PDFExtractor.Primitives
{
    public struct SizeD
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public SizeD(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString() => $"Width:{Width},Height:{Height}";

    }
}
