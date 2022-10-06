namespace PDFExtractor.Primitives
{

    public struct PDFRectangle
    {
        /// <summary>
        /// X Coord in PDF Page Space
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y Coord in PDF Page Space. (0,0) is Bottom left of the Page
        /// </summary>
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double Right => X + Width;
        public double Left => X;
        public double Bottom => Y;
        public double Top => Y + Height;

        /// <summary>
        /// X Coord in Screen Space
        /// </summary>
        public double ScreenSpaceX => Left;

        /// <summary>
        /// Y Coord in Screen Space. (0,0) is Top Left of the Screen
        /// </summary>
        public double ScreenSpaceY => Top;

        public PDFRectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public PDFRectangle(PointD loc, double width, double height) : this(loc.X, loc.Y, width, height) { }
        public PDFRectangle(PointD loc, SizeD size) : this(loc.X, loc.Y, size.Width, size.Height) { }
        public PDFRectangle(SizeD size) : this(0, 0, size.Width, size.Height) { }

        public PDFRectangle(PDFRectangle area) : this(area.X, area.Y, area.Width, area.Height) { }

        public PDFRectangle(PointD LowerLeft, PointD UpperRight) : this(LowerLeft.X, LowerLeft.Y, UpperRight.X - LowerLeft.X, UpperRight.Y - LowerLeft.Y) { }

        public bool InArea(PDFRectangle CheckArea)
        {
            //return (this.Left < CheckArea.Right &&
            //   this.Right > CheckArea.Left &&
            //   this.Top < CheckArea.Bottom &&
            //   this.Bottom > CheckArea.Top);

            if (Left >= CheckArea.Right ||
               Right <= CheckArea.Left)
                return false;

            if (Top <= CheckArea.Bottom ||
                Bottom >= CheckArea.Top)
                return false;

            return true;

        }

        public override string ToString() => $"X:{X:0.00},Y:{Y:0.00},Width:{Width:0.00},Height:{Height:0.00}";


    }


}
