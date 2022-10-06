
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTextExtractorTests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    class PDFTextExtractorTests
    {
        [TestMethod]
        public void BaseTest()
        {
            var test = PDFTextExtractor.GetPdfBreakdown("TestPDF.PDF").ToList();
        }

    }
}
