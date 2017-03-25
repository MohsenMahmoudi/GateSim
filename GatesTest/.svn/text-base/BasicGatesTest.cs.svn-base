using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GatesTest
{
    /// <summary>
    /// Test the basic gates individually
    /// </summary>
    [TestClass]
    public class BasicGatesTest
    {
        public BasicGatesTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void And()
        {
            Gates.BasicGates.And and = new Gates.BasicGates.And();
            and[0] = false;
            and[1] = false;

            Assert.AreEqual(and.Output[0], false);

            and[0] = true;
            and[1] = false;

            Assert.AreEqual(and.Output[0], false);

            and[0] = false;
            and[1] = true;

            Assert.AreEqual(and.Output[0], false);

            and[0] = true;
            and[1] = true;

            Assert.AreEqual(and.Output[0], true);

        }

        [TestMethod]
        public void Not()
        {
            Gates.BasicGates.Not not = new Gates.BasicGates.Not();
            not[0] = false;

            Assert.AreEqual(not.Output[0], true);

            not[0] = true;
            Assert.AreEqual(not.Output[0], false);

        }


        [TestMethod]
        public void Or()
        {
            Gates.BasicGates.Or or = new Gates.BasicGates.Or();
            or[0] = false;
            or[1] = false;

            Assert.AreEqual(or.Output[0], false);

            or[0] = true;
            or[1] = false;

            Assert.AreEqual(or.Output[0], true);

            or[0] = false;
            or[1] = true;

            Assert.AreEqual(or.Output[0], true);

            or[0] = true;
            or[1] = true;

            Assert.AreEqual(or.Output[0], true);

        }

        [TestMethod]
        public void Nand()
        {
            Gates.BasicGates.Nand nand = new Gates.BasicGates.Nand();
            nand[0] = false;
            nand[1] = false;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = true;
            nand[1] = false;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = false;
            nand[1] = true;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = true;
            nand[1] = true;

            Assert.AreEqual(nand.Output[0], false);

        }

        [TestMethod]
        public void Nor()
        {
            Gates.BasicGates.Nor nor = new Gates.BasicGates.Nor();
            nor[0] = false;
            nor[1] = false;

            Assert.AreEqual(nor.Output[0], true);

            nor[0] = true;
            nor[1] = false;

            Assert.AreEqual(nor.Output[0], false);

            nor[0] = false;
            nor[1] = true;

            Assert.AreEqual(nor.Output[0], false);

            nor[0] = true;
            nor[1] = true;

            Assert.AreEqual(nor.Output[0], false);

        }

        [TestMethod]
        public void Xor()
        {
            Gates.BasicGates.Xor xor = new Gates.BasicGates.Xor();
            xor[0] = false;
            xor[1] = false;

            Assert.AreEqual(xor.Output[0], false);

            xor[0] = true;
            xor[1] = false;

            Assert.AreEqual(xor.Output[0], true);

            xor[0] = false;
            xor[1] = true;

            Assert.AreEqual(xor.Output[0], true);

            xor[0] = true;
            xor[1] = true;

            Assert.AreEqual(xor.Output[0], false);

        }


        [TestMethod]
        public void Xnor()
        {
            Gates.BasicGates.Xnor xnor = new Gates.BasicGates.Xnor();
            xnor[0] = false;
            xnor[1] = false;

            Assert.AreEqual(xnor.Output[0], true);

            xnor[0] = true;
            xnor[1] = false;

            Assert.AreEqual(xnor.Output[0], false);

            xnor[0] = false;
            xnor[1] = true;

            Assert.AreEqual(xnor.Output[0], false);

            xnor[0] = true;
            xnor[1] = true;

            Assert.AreEqual(xnor.Output[0], true);

        }

        [TestMethod]
        public void Buffer()
        {
            Gates.BasicGates.Buffer bf = new Gates.BasicGates.Buffer();

            bf[0] = false;

            Assert.AreEqual(bf.Output[0], false);

            bf[0] = true;

            Assert.AreEqual(bf.Output[0], true);

        }

        [TestMethod]
        public void ThreeInputAnd()
        {
            Gates.BasicGates.And and = new Gates.BasicGates.And(3);
            and[0] = false;
            and[1] = false;
            and[2] = false;

            Assert.AreEqual(and.Output[0], false);

            and[0] = true;
            and[1] = false;
            and[2] = false;

            Assert.AreEqual(and.Output[0], false);

            and[0] = false;
            and[1] = true;
            and[2] = false;

            Assert.AreEqual(and.Output[0], false);

            and[0] = true;
            and[1] = true;
            and[2] = false;

            Assert.AreEqual(and.Output[0], false);

            and[0] = false;
            and[1] = false;
            and[2] = true;

            Assert.AreEqual(and.Output[0], false);

            and[0] = true;
            and[1] = false;
            and[2] = true;

            Assert.AreEqual(and.Output[0], false);

            and[0] = false;
            and[1] = true;
            and[2] = true;

            Assert.AreEqual(and.Output[0], false);

            and[0] = true;
            and[1] = true;
            and[2] = true;

            Assert.AreEqual(and.Output[0], true);
        }


        [TestMethod]
        public void ThreeInputOr()
        {
            Gates.BasicGates.Or or = new Gates.BasicGates.Or(3);
            or[0] = false;
            or[1] = false;
            or[2] = false;

            Assert.AreEqual(or.Output[0], false);

            or[0] = true;
            or[1] = false;
            or[2] = false;

            Assert.AreEqual(or.Output[0], true);

            or[0] = false;
            or[1] = true;
            or[2] = false;

            Assert.AreEqual(or.Output[0], true);

            or[0] = true;
            or[1] = true;
            or[2] = false;

            Assert.AreEqual(or.Output[0], true);

            or[0] = false;
            or[1] = false;
            or[2] = true;

            Assert.AreEqual(or.Output[0], true);

            or[0] = true;
            or[1] = false;
            or[2] = true;

            Assert.AreEqual(or.Output[0], true);

            or[0] = false;
            or[1] = true;
            or[2] = true;

            Assert.AreEqual(or.Output[0], true);

            or[0] = true;
            or[1] = true;
            or[2] = true;

            Assert.AreEqual(or.Output[0], true);
        }


        [TestMethod]
        public void ThreeInputNOr()
        {
            Gates.BasicGates.Nor nor = new Gates.BasicGates.Nor(3);
            nor[0] = false;
            nor[1] = false;
            nor[2] = false;

            Assert.AreEqual(nor.Output[0], true);

            nor[0] = true;
            nor[1] = false;
            nor[2] = false;

            Assert.AreEqual(nor.Output[0], false);

            nor[0] = false;
            nor[1] = true;
            nor[2] = false;

            Assert.AreEqual(nor.Output[0], false);

            nor[0] = true;
            nor[1] = true;
            nor[2] = false;

            Assert.AreEqual(nor.Output[0], false);

            nor[0] = false;
            nor[1] = false;
            nor[2] = true;

            Assert.AreEqual(nor.Output[0], false);

            nor[0] = true;
            nor[1] = false;
            nor[2] = true;

            Assert.AreEqual(nor.Output[0], false);

            nor[0] = false;
            nor[1] = true;
            nor[2] = true;

            Assert.AreEqual(nor.Output[0], false);

            nor[0] = true;
            nor[1] = true;
            nor[2] = true;

            Assert.AreEqual(nor.Output[0], false);
        }

        [TestMethod]
        public void ThreeInputNAnd()
        {
            Gates.BasicGates.Nand nand = new Gates.BasicGates.Nand(3);
            nand[0] = false;
            nand[1] = false;
            nand[2] = false;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = true;
            nand[1] = false;
            nand[2] = false;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = false;
            nand[1] = true;
            nand[2] = false;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = true;
            nand[1] = true;
            nand[2] = false;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = false;
            nand[1] = false;
            nand[2] = true;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = true;
            nand[1] = false;
            nand[2] = true;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = false;
            nand[1] = true;
            nand[2] = true;

            Assert.AreEqual(nand.Output[0], true);

            nand[0] = true;
            nand[1] = true;
            nand[2] = true;

            Assert.AreEqual(nand.Output[0], false);
        }
    }
}
