using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using PdfSharp.Pdf.IO;
using System.IO;
using PDFExtractor.Fonts;
using PDFExtractor.Primitives;

namespace PDFExtractor
{
    /// <summary>
    /// Big big thank you to Alexey Zakharchenko, and his repositry:
    /// https://github.com/alexarchen/PdfSharpTextExtractor
    /// this was used for pulling the character that are Unicode or other type.
    /// </summary>
    /// 
    public class PDFTextExtractor
    {
        /// <summary>
        /// Get a ParsePage for all pages in PDF
        /// </summary>
        /// <param name="file">a PDF file</param>
        /// <returns></returns>
        public static IEnumerable<ParsePage> GetParsePages(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("PDF file doen't exist", file);

            using var doc = PdfReader.Open(file);
            foreach (var page in doc.Pages)
                yield return GetText(page);
        }

        /// <summary>
        /// Get a ParsePage for a single pdfPage
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        internal static ParsePage GetText(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            //Setup save variable
            StringBuilder PDFText = new StringBuilder();
            TextPage AllLocationText = new TextPage();
            var pdfState = new PDFPageState();
            SizeD size = page.GetSize();

            var fonts = GetFontSetup(page);
            var UsableOperators = GetTextOperators(page);

            void LogString(string text)
            {
                //add string to Text Output
                PDFText.Append(text);

                //get current location
                var Loc = (pdfState.Location * pdfState.RenderMatrim).ToPointD();

                //move location by length of string
                PDFMatrixHelper.CalculateStringLengthOffset(text, pdfState);

                //get location after shift to find length of string
                var afterLoc = (pdfState.Location * pdfState.RenderMatrim).ToPointD();

                //get the height of the string
                var height = PDFMatrixHelper.CalculateStringMaxHeight(text, pdfState);

                //calc Height with Render Matrix Accounted for
                height = height * Math.Abs(pdfState.RenderMatrim.E);

                //Log the Location and the string
                if (afterLoc.X - Loc.X < 0.0009)
                {//vertical Text
                    AllLocationText.Add(new ExtractedText(new PDFRectangle(Loc, height, afterLoc.Y - Loc.Y), text));
                }
                else
                    AllLocationText.Add(new ExtractedText(new PDFRectangle(Loc, afterLoc.X - Loc.X, height), text));
            }

            foreach (var Operator in UsableOperators)
            {
                //Update States
                switch (Operator.OpCode.OpCodeName)
                {
                    //Render Text Matrix
                    case OpCodeName.cm:
                        pdfState.RenderMatrim = pdfState.RenderMatrim * PDFMatrixHelper.GetLocationMatrix(Operator);
                    break;

                    //Ending and start tag
                    case OpCodeName.BT:
                    case OpCodeName.ET:
                        pdfState.Location = Matrix.GetIdenity();//Tm
                        pdfState.LineLocation = Matrix.GetIdenity();// Tlm
                        break;

                    //setting the location
                    case OpCodeName.Tm:
                        pdfState.Location = PDFMatrixHelper.GetLocationMatrix(Operator);
                        pdfState.LineLocation = PDFMatrixHelper.GetLocationMatrix(Operator);
                        break;
                    //specify line height
                    case OpCodeName.TL:
                        pdfState.LineHeight = PDFMatrixHelper.GetLineHeight(Operator);
                        break;

                    //set CharSpace Variable
                    case OpCodeName.Tc:
                        if (Operator.Operands.Count < 1)
                            throw new Exception("Unable to Parse CharSpace");
                        pdfState.CharacterSpacing = double.Parse(Operator.Operands[0].ToString());
                        break;

                    //set wordSpace Variable
                    case OpCodeName.Tw:
                        if (Operator.Operands.Count < 1)
                            throw new Exception("Unable to Parse WordSpace");
                        pdfState.WordSpace = double.Parse(Operator.Operands[0].ToString());
                        break;

                    //Set which font to use
                    case OpCodeName.Tf:
                        if (Operator.Operands.Count != 2)
                            throw new Exception("Unable To Read Font");

                        //retrive and get current font
                        string nF = Operator.Operands[0].ToString();
                        pdfState.CurrentFont = fonts[nF];

                        //get Font size
                        pdfState.FontSize = Operator.Operands[1].GetNumber();

                        //throw error if no font found
                        if (pdfState.CurrentFont == null)
                            throw new Exception("Unable To Load Font");

                        //keep on hand the size of a space
                        pdfState.SpaceSize = pdfState.CurrentFont.GetCharacterWidth(' ');
                        break;

                    //Set the location matrix and set the Line height
                    case OpCodeName.TD:
                    {
                        //add New line text output
                        PDFText.Append("\n");

                        //Set line height and translate location down 1 line
                        var NewLineOffsetMat = PDFMatrixHelper.CalculateNewLine(Operator);
                        pdfState.LineHeight = PDFMatrixHelper.GetLineHeight(NewLineOffsetMat.ToPointD().Y);
                        pdfState.TranslateLocation(NewLineOffsetMat);
                        break;
                    }

                    //Set the location matrix
                    case OpCodeName.Td:
                    {
                        //add New line text output
                        PDFText.Append("\n");

                        //translate location by the amount specified in the Td tag
                        var NewLineOffsetMat = PDFMatrixHelper.CalculateNewLine(Operator);
                        pdfState.TranslateLocation(NewLineOffsetMat);
                        break;
                    }

                    //Go to the next line in the PDF
                    case OpCodeName.Tx:
                    case OpCodeName.QuoteSingle:
                    case OpCodeName.QuoteDbl:
                        //add New line text output
                        PDFText.Append("\n");

                        //translate location down 1 line
                        pdfState.TranslateNewLine();
                        break;

                }

                //Log Text and location
                switch (Operator.OpCode.OpCodeName)
                {
                    //Save the text and location
                    case OpCodeName.Tj:
                    case OpCodeName.QuoteSingle:
                        if (Operator.Operands.Count != 1)
                            throw new Exception("Unable to Parse String Operator in PDF");

                        if (Operator.Operands[0] is not CString)
                            throw new Exception("Found not a String in a String Section");

                        LogString(Operator.Operands[0].GetText(pdfState.CurrentFont));
                        break;

                    //Save and arrays with of strings(with offsets between each string
                    case OpCodeName.TJ:
                        if (Operator.Operands.Count != 1)
                            throw new Exception("Unable to Parse List of Strings in PDF");
                        if (!(Operator.Operands[0] is CSequence))
                            throw new Exception("Unable to Parse List of Strings Operator in PDF");

                        foreach (var Oper in Operator.Operands[0] as CSequence)
                        {
                            if (Oper is CString)
                            {//Save text and location
                                LogString(Oper.GetText(pdfState.CurrentFont));
                            }
                            else if (Oper is CNumber)
                            {//move location matrix by a fixed amount & add spaces to the text output
                                var offsetAmount = Oper.GetNumber();
                                if (Math.Abs(offsetAmount) / 1000 > pdfState.SpaceSize)
                                {
                                    PDFText.Append(new string(' ', (int)(Math.Abs(offsetAmount) / 1000 / pdfState.SpaceSize)));
                                }

                                //calculate the offset size and translate location by it
                                var offsetMat = PDFMatrixHelper.CalculateTJOffSetWidth(0, offsetAmount, pdfState.FontSize, pdfState.CharacterSpacing, pdfState.WordSpace);
                                pdfState.Location = offsetMat * pdfState.Location;
                            }
                        }
                        break;

                    //set the character spacing and wordspace, the save string
                    case OpCodeName.QuoteDbl:
                        //NOTE: aleady Added new line
                        if (Operator.Operands.Count < 2)
                            throw new Exception("Unable to Parse Double Quote");
                        pdfState.WordSpace = double.Parse(Operator.Operands[0].ToString());
                        pdfState.CharacterSpacing = double.Parse(Operator.Operands[1].ToString());
                        LogString(Operator.Operands[2].GetText(pdfState.CurrentFont));
                        break;

                }
            }

            return new ParsePage(PDFText.ToString(), AllLocationText, size);
        }

        /// <summary>
        /// find all the fonts used in the PDF 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        static Dictionary<string, BaseFont> GetFontSetup(PdfPage page)
        {
            Dictionary<string, BaseFont> fonts = new Dictionary<string, BaseFont>();

            if ((page.Resources != null) && (page.Resources.Elements["/Font"] != null))
            {
                var obj = page.Resources.Elements["/Font"].RemovePDfReference();
                if (obj is PdfDictionary)
                {
                    foreach (var kp in (obj as PdfDictionary).Elements)
                    {
                        PdfItem fobj = kp.Value.RemovePDfReference();
                        if (fobj is PdfDictionary)
                        {
                            var dic = (PdfDictionary)fobj;

                            fonts.Add(kp.Key, BaseFont.BuildFont(dic));
                        }
                        else
                            throw new Exception("Unable to Handle Font");
                    }
                }
            }
            return fonts;
        }

        /// <summary>
        /// Gets all tags between BT and ET tags
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        static IEnumerable<COperator> GetTextOperators(PdfPage page)
        {
            var content = ContentReader.ReadContent(page);
            bool SaveText = false;
            bool QGraphicSaveState = false;
            foreach (var cObject in content)
            {
                var test = cObject.ToString();
                if (cObject is CSequence)
                    throw new Exception("File Sequance Error");
                if (cObject is COperator)
                {
                    var Op = cObject as COperator;

                    if (Op.OpCode.OpCodeName == OpCodeName.q)
                        QGraphicSaveState = true;
                    else if(Op.OpCode.OpCodeName == OpCodeName.Q)
                        QGraphicSaveState = false;

                    if (Op.OpCode.OpCodeName == OpCodeName.BT)
                        SaveText = true;
                    else if (Op.OpCode.OpCodeName == OpCodeName.ET)
                        SaveText = false;
                    else if (!QGraphicSaveState && Op.OpCode.OpCodeName == OpCodeName.cm)
                        yield return cObject as COperator;
                }
                

                if (SaveText)
                    yield return cObject as COperator;
            }
        }
    }


}
