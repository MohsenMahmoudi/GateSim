using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GatesTest
{
    /// <summary>
    /// Summary description for IOGatesTest
    /// </summary>
    [TestClass]
    public class IOGatesTest
    {
        public IOGatesTest()
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
        public void UserInput()
        {
            Gates.IOGates.UserInput ui = new Gates.IOGates.UserInput();

            ui.Value = false;
            Assert.AreEqual(ui.Output[0], false);

            ui.Value = true;
            Assert.AreEqual(ui.Output[0], true);
        }

        [TestMethod]
        public void UserOutput()
        {
            Gates.IOGates.UserOutput uo = new Gates.IOGates.UserOutput();


            uo[0] = false;
            Assert.AreEqual(uo.Value, false);

            uo[0] = true;
            Assert.AreEqual(uo.Value, true);
        }

        [TestMethod]
        public void Clock()
        {
            Gates.IOGates.Clock clk = new Gates.IOGates.Clock(500);

            Gates.IOGates.Clock.CalculatePrecession();

            int cnt = 0;
            bool oldval = false;
            DateTime start = DateTime.Now;
            while (DateTime.Now - start < new TimeSpan(0, 0, 10))
            {
                if (clk.Output[0] != oldval)
                {
                    oldval = clk.Output[0];
                    cnt++;
                }
            }

            // 10 seconds at half a second period, two clicks per period ~ 40 clicks
            if (cnt < 38 || cnt > 42)
                Assert.Fail("Unacceptable count = " + cnt.ToString());

        }
        
        [TestMethod()]
        public void NumericOutput1()
        {
            Gates.IOGates.NumericOutput no = new Gates.IOGates.NumericOutput(4);

            no[0] = false;
            no[1] = false;
            no[2] = false;
            no[3] = false;

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
            Assert.AreEqual("0", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
            Assert.AreEqual("0000", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
            Assert.AreEqual("0", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
            Assert.AreEqual("0", no.Value);

            Assert.AreEqual(4, no.Bits);
            Assert.AreEqual(0, no.IntValue);
            Assert.AreEqual(15, no.MaxValue);


            no[0] = true;

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
            Assert.AreEqual("1", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
            Assert.AreEqual("0001", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
            Assert.AreEqual("1", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
            Assert.AreEqual("1", no.Value);
            Assert.AreEqual(1, no.IntValue);

            no[2] = true;

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
            Assert.AreEqual("5", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
            Assert.AreEqual("0101", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
            Assert.AreEqual("5", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
            Assert.AreEqual("5", no.Value);
            Assert.AreEqual(5, no.IntValue);

            no[3] = true;

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
            Assert.AreEqual("13", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
            Assert.AreEqual("1101", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
            Assert.AreEqual("15", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
            Assert.AreEqual("D", no.Value);
            Assert.AreEqual(13, no.IntValue);


            no[1] = true;

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
            Assert.AreEqual("15", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
            Assert.AreEqual("1111", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
            Assert.AreEqual("17", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
            Assert.AreEqual("F", no.Value);
            Assert.AreEqual(15, no.IntValue);




        }




        [TestMethod()]
        public void NumericOutput2()
        {
            Gates.IOGates.NumericOutput no = new Gates.IOGates.NumericOutput(7);

            no[0] = false;
            no[1] = false;
            no[2] = false;
            no[3] = false;
            no[4] = false;
            no[5] = false;
            no[6] = false;

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
            Assert.AreEqual("0", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
            Assert.AreEqual("0000000", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
            Assert.AreEqual("0", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
            Assert.AreEqual("0", no.Value);

            Assert.AreEqual(7, no.Bits);
            Assert.AreEqual(0, no.IntValue);
            Assert.AreEqual(127, no.MaxValue);

            no[1] = true;
            no[3] = true;
            no[5] = true;

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
            Assert.AreEqual("42", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
            Assert.AreEqual("0101010", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
            Assert.AreEqual("52", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
            Assert.AreEqual("2A", no.Value);

            Assert.AreEqual(42, no.IntValue);


            no[1] = false;
            no[2] = true;
            no[3] = false;
            no[4] = true;
            no[6] = true;
            no[5] = false;

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
            Assert.AreEqual("84", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
            Assert.AreEqual("1010100", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
            Assert.AreEqual("124", no.Value);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
            Assert.AreEqual("54", no.Value);

            Assert.AreEqual(84, no.IntValue);

        }

        [TestMethod()]
        public void NumericInput()
        {
            Gates.IOGates.NumericInput ni = new Gates.IOGates.NumericInput(5);

            ni.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
            ni.Value = "01010";
            Assert.AreEqual(ni.IntValue, 10);
            Assert.AreEqual(ni.Output[0], false);
            Assert.AreEqual(ni.Output[1], true);
            Assert.AreEqual(ni.Output[2], false);
            Assert.AreEqual(ni.Output[3], true);
            Assert.AreEqual(ni.Output[4], false);

            ni.Value = "11001";
            Assert.AreEqual(ni.IntValue, 25);
            Assert.AreEqual(ni.Output[0], true);
            Assert.AreEqual(ni.Output[1], false);
            Assert.AreEqual(ni.Output[2], false);
            Assert.AreEqual(ni.Output[3], true);
            Assert.AreEqual(ni.Output[4], true);

            ni.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
            Assert.AreEqual(ni.IntValue, 25);
            Assert.AreEqual(ni.Output[0], true);
            Assert.AreEqual(ni.Output[1], false);
            Assert.AreEqual(ni.Output[2], false);
            Assert.AreEqual(ni.Output[3], true);
            Assert.AreEqual(ni.Output[4], true);

            ni.Value = "20";
            Assert.AreEqual(ni.IntValue, 20);
            Assert.AreEqual(ni.Output[0], false);
            Assert.AreEqual(ni.Output[1], false);
            Assert.AreEqual(ni.Output[2], true);
            Assert.AreEqual(ni.Output[3], false);
            Assert.AreEqual(ni.Output[4], true);

            ni.Value = "31";
            Assert.AreEqual(ni.IntValue, 31);
            Assert.AreEqual(ni.Output[0], true);
            Assert.AreEqual(ni.Output[1], true);
            Assert.AreEqual(ni.Output[2], true);
            Assert.AreEqual(ni.Output[3], true);
            Assert.AreEqual(ni.Output[4], true);

            try
            {
                ni.Value = "32";
                Assert.Fail("Allowed 32");
            }
            catch
            {
                // good
            }

            try
            {
                ni.Value = "-1";
                Assert.Fail("Allowed neg");
            }
            catch
            {
                // good
            }

            ni.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
            ni.Value = "12";
            Assert.AreEqual(ni.IntValue, 18);

            Assert.AreEqual(ni.Output[0], false);
            Assert.AreEqual(ni.Output[1], true);
            Assert.AreEqual(ni.Output[2], false);
            Assert.AreEqual(ni.Output[3], false);
            Assert.AreEqual(ni.Output[4], true);



            ni.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
            ni.Value = "12";
            Assert.AreEqual(ni.IntValue, 10);

            Assert.AreEqual(ni.Output[0], false);
            Assert.AreEqual(ni.Output[1], true);
            Assert.AreEqual(ni.Output[2], false);
            Assert.AreEqual(ni.Output[3], true);
            Assert.AreEqual(ni.Output[4], false);



        }

        [TestMethod()]
        public void BCD()
        {
            Gates.IOGates.NumericInput ni = new Gates.IOGates.NumericInput(8);

            ni.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BCD;
            ni.Value = "00";
            Assert.AreEqual(ni.IntValue, 0);

            ni.Value = "59";
            Assert.AreEqual(ni.IntValue, 89); // 59 = 01011001 = 89
            
            ni.Value = "90"; // 90 = 10010000 = 144
            Assert.AreEqual(ni.IntValue, 144);

            try
            {
                ni.Value = "111"; // too long
                Assert.Fail("Allowed 1111");
            }
            catch (Exception)
            {
                // good
            }

            Gates.IOGates.NumericOutput no = new Gates.IOGates.NumericOutput(8);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BCD;
            no[0] = true;
            no[1] = false;
            no[2] = false;
            no[3] = true;

            no[4] = false;
            no[5] = true;
            no[6] = true;
            no[7] = false;
            
            Assert.AreEqual(no.Value, "69");

            no[0] = true;
            no[1] = true;
            no[2] = false;
            no[3] = false;

            no[4] = true;
            no[5] = false;
            no[6] = true;
            no[7] = false;

            Assert.AreEqual(no.Value, "53");

            try
            {
                no[0] = true;
                no[1] = false;
                no[2] = true;
                no[3] = true;

                no[4] = true;
                no[5] = true;
                no[6] = true;
                no[7] = false;

                Assert.Fail("Allowed too much digit");
            }
            catch (Exception)
            {
                // good
            }
        }

        [TestMethod()]
        public void TwosComplement()
        {
            Gates.IOGates.NumericInput ni = new Gates.IOGates.NumericInput(4);

            ni.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.D2C;
            ni.Value = "0";
            Assert.AreEqual(ni.IntValue, 0);

            ni.Value = "7";
            Assert.AreEqual(ni.IntValue, 7); 

            ni.Value = "-8"; // -8 = 1000 = 8
            Assert.AreEqual(ni.IntValue, 8);

            ni.Value = "-1"; // -1 = 1111 = 15
            Assert.AreEqual(ni.IntValue, 15);

            try
            {
                ni.Value = "-9";
                Assert.Fail("Allows -9");
            }
            catch (Exception)
            {
                // good
            }

            try
            {
                ni.Value = "8";
                Assert.Fail("Allows 8");
            }
            catch (Exception)
            {
                // good
            }

            Gates.IOGates.NumericOutput no = new Gates.IOGates.NumericOutput(4);

            no.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.D2C;

            no[0] = false;
            no[1] = false;
            no[2] = false;
            no[3] = false;

            Assert.AreEqual("0", no.Value);
            Assert.AreEqual(0, no.IntValue);

            no[0] = true;
            no[1] = false;
            no[2] = true;
            no[3] = false;

            Assert.AreEqual("5", no.Value);
            Assert.AreEqual(5, no.IntValue);

            no[0] = false;
            no[1] = true;
            no[2] = false;
            no[3] = true;

            Assert.AreEqual("-6", no.Value);
            Assert.AreEqual(10, no.IntValue);

            no[0] = true;
            no[1] = true;
            no[2] = true;
            no[3] = true;

            Assert.AreEqual("-1", no.Value);
            Assert.AreEqual(15, no.IntValue);

        }
        
    }
}
