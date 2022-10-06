using PDFExtractor.Fonts;

namespace PDFExtractor
{
    internal class PDFPageState
    {
        public Matrix RenderMatrim { get; set; }//CTM
        public Matrix Location { get; set; }//Tm
        public Matrix LineLocation { get; set; }//Tlm
        public Matrix LineHeight { get; set; }
        public BaseFont CurrentFont { get; set; }
        public double FontSize { get; set; }
        public double CharacterSpacing { get; set; }
        public double WordSpace { get; set; }
        public double SpaceSize { get; set; }

        public PDFPageState()
        {
            RenderMatrim = Matrix.GetIdenity();//Tm
            Location = Matrix.GetIdenity();//Tm
            LineLocation = Matrix.GetIdenity();// Tlm
            LineHeight = null;
            CurrentFont = null;
            FontSize = 12;
            CharacterSpacing = 0;
            WordSpace = 0;
            SpaceSize = 750;
        }

        public void TranslateLocation(Matrix translateBy)
        {
            LineLocation = translateBy * LineLocation;
            LineLocation.CopyTo(Location);
        }

        public void TranslateNewLine() => TranslateLocation(LineHeight);
    }


}
