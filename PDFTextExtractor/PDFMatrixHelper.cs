using PdfSharp.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDFExtractor
{
    /// <summary>
    /// So, this is where the magic of calculating most of the text location is.
    /// Futher comments will be add, 
    /// Link to the PDF Refence used:https://www.adobe.com/content/dam/acom/en/devnet/pdf/pdfs/PDF32000_2008.pdf
    /// </summary>
    internal class PDFMatrixHelper
    {
        public static void CalculateStringLengthOffset(string text, PDFPageState pdfState)
        {
            foreach (var c in text)
            {
                var OffsetMat = CalculateTJOffSetWidth(pdfState.CurrentFont.GetCharacterWidth(c), 0, pdfState.FontSize, pdfState.CharacterSpacing, pdfState.WordSpace);
                pdfState.Location = OffsetMat * pdfState.Location;
            }
        }

        public static Matrix CalculateTJOffSetWidth(double w0, double Tj, double Tfs, double Tc, double Tw, double Th = 100)
        {
            /*
             * Equation found on Page 252 of PDF 1.7 Specification PDF 32000-1:2008
             * w0 - Character width
             * Tj - Space offset
             * Tfs - font size
             * Tc - Character Space
             * Tw - Word Space
             * Th - horizontal Scaling(currently unused)
             */
            var tx = (((w0 - (Tj / 1000)) * Tfs) + Tc + Tw) * (Th / 100);
            return Matrix.GetPointMatrix(tx, 0);
        }

        public static double CalculateStringMaxHeight(string text, PDFPageState pdfState) => text.Max(c => CalculateTextHeight(pdfState.CurrentFont.GetCharacterWidth(c), 0, pdfState.FontSize, pdfState.CharacterSpacing, pdfState.WordSpace));

        public static double CalculateTextHeight(double w1, double Tj, double Tfs, double Tc, double Tw)
        {
            /*
            * Equation found on Page 252
            * w1 - Character Height
            * Tj - Space offset
            * Tfs - font size
            * Tc - Character Space
            * Tw - Word Space
            */
            return ((w1 - (Tj / 1000)) * Tfs) + Tc + Tw;
        }

        public static Matrix CalculateNewLine(COperator NewLine)
        {
            if (NewLine.Operands.Count != 2)
                throw new Exception($"New Line Matrix is overSized.(Actual:{NewLine.Operands.Count}VS.Expected:2)");
            var values = new List<double>();
            foreach (var cOperand in NewLine.Operands)
                if (cOperand is CInteger)
                    values.Add(Convert.ToDouble((cOperand as CInteger).Value));
                else if (cOperand is CReal)
                    values.Add((cOperand as CReal).Value);

            return Matrix.GetPointMatrix(values[0], values[1]);//MakeCordOffsetMat(values[0], values[1]);
        }

        public static Matrix GetLineHeight(double LineHeight) => Matrix.GetPointMatrix(0, -LineHeight); //MakeCordOffsetMat(0, -LineHeight);

        public static Matrix GetLineHeight(COperator LineHeight)
        {
            if (LineHeight.Operands.Count != 1)
                throw new Exception($"Line Height Matrix is overSized.(Actual:{LineHeight.Operands.Count}VS.Expected:1)");
            var values = new List<double>();
            foreach (var cOperand in LineHeight.Operands)
                if (cOperand is CInteger)
                    values.Add(Convert.ToDouble((cOperand as CInteger).Value));
                else if (cOperand is CReal)
                    values.Add((cOperand as CReal).Value);

            return GetLineHeight(values.First());
        }

        public static Matrix GetLocationMatrix(COperator LocationOperator)
        {
            if (LocationOperator.Operands.Count != 6)
                throw new Exception($"Start Location Matrix is overSized.(Actual:{LocationOperator.Operands.Count}VS.Expected:6)");
            var values = new List<double>();
            foreach (var cOperand in LocationOperator.Operands)
                if (cOperand is CInteger)
                    values.Add(Convert.ToDouble((cOperand as CInteger).Value));
                else if (cOperand is CReal)
                    values.Add((cOperand as CReal).Value);

            var rawLocationMatrix = new double[,] {
                {values[0],values[1],0},
                {values[2],values[3],0},
                {values[4],values[5],1}
            };
            return new Matrix(rawLocationMatrix);
        }

    }


}
