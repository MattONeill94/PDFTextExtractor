using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFExtractor.Tests
{
    [TestClass()]
    public class MatrixTests
    {
        [TestMethod()]
        public void GetIdenityTest()
        {
            var item = Matrix.GetIdenity();

            Assert.AreEqual(item.A, 1);
            Assert.AreEqual(item.B, 0);
            Assert.AreEqual(item.C, 0);
            Assert.AreEqual(item.D, 0);
            Assert.AreEqual(item.E, 1);
            Assert.AreEqual(item.F, 0);
            Assert.AreEqual(item.G, 0);
            Assert.AreEqual(item.H, 0);
            Assert.AreEqual(item.I, 1);
        }

        [TestMethod()]
        public void ArrayLinesUpTest()
        {
            var item = new Matrix(1,2,3,4,5,6,7,8,9);

            Assert.AreEqual(item.A, 1);
            Assert.AreEqual(item.B, 2);
            Assert.AreEqual(item.C, 3);
            Assert.AreEqual(item.D, 4);
            Assert.AreEqual(item.E, 5);
            Assert.AreEqual(item.F, 6);
            Assert.AreEqual(item.G, 7);
            Assert.AreEqual(item.H, 8);
            Assert.AreEqual(item.I, 9);
        }

        [TestMethod]
        public void IdenityMultiplyTest()
        {
            var item1 = Matrix.GetIdenity();
            var item2 = Matrix.GetIdenity();

            var item = item1 * item2;

            Assert.AreEqual(item.A, 1);
            Assert.AreEqual(item.B, 0);
            Assert.AreEqual(item.C, 0);
            Assert.AreEqual(item.D, 0);
            Assert.AreEqual(item.E, 1);
            Assert.AreEqual(item.F, 0);
            Assert.AreEqual(item.G, 0);
            Assert.AreEqual(item.H, 0);
            Assert.AreEqual(item.I, 1);
        }

        [TestMethod]
        public void BaseMultiplyTest()
        {
            var item1 = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9);
            var item2 = new Matrix(10, 11, 12, 13, 14, 15, 16, 17, 18);

            var item = item1 * item2;

            Assert.AreEqual(item.A, 84);
            Assert.AreEqual(item.B, 90);
            Assert.AreEqual(item.C, 96);
            Assert.AreEqual(item.D, 201);
            Assert.AreEqual(item.E, 216);
            Assert.AreEqual(item.F, 231);
            Assert.AreEqual(item.G, 318);
            Assert.AreEqual(item.H, 342);
            Assert.AreEqual(item.I, 366);
        }

        [TestMethod]
        public void BaseTest()
        {
            //var test = PDFTextExtractor.GetPdfBreakdown("TestPDF.PDF").ToList();
            var test = PDFTextExtractor.GetPdfBreakdown("TestTable.PDF").ToList();
        }

    }
}