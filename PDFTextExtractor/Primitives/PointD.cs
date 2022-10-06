namespace PDFExtractor.Primitives
{
    public struct PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override string ToString() => $"X:{X},Y:{Y}";

        public bool InArea(PDFRectangle area)
        {
            if (X >= area.Left && X <= area.Right)
                if (Y >= area.Top && Y <= area.Bottom)
                    return true;
            return false;
        }
    }
}
