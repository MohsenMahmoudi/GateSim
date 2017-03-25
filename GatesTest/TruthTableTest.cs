using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace GatesTest
{
    /// <summary>
    /// Summary description for TruthTableTest
    /// </summary>
    [TestClass]
    public class TruthTableTest
    {
        public TruthTableTest()
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

        private Gates.TruthTable.TruthTable CreateBasic()
        {
            Gates.TruthTable.TruthTable tt = new Gates.TruthTable.TruthTable(2, 2);
            Assert.AreEqual(4, tt.Table.GetLength(0));
            Assert.AreEqual(2, tt.Table[0].GetLength(0));

            // output 0 is or
            // output 1 is and
            tt[0, 0] = false;
            tt[0, 1] = false;

            tt[1, 0] = true;
            tt[1, 1] = false;

            tt[2, 0] = true;
            tt[2, 1] = false;

            tt[3, 0] = true;
            tt[3, 1] = true;

            return tt;
        }

        private void TestBasic(Gates.TruthTable.TruthTable tt)
        {
            tt[0] = false;
            tt[1] = false;
            Assert.AreEqual(false, tt.Output[0], "00 case");
            Assert.AreEqual(false, tt.Output[1], "00 case");

            tt[0] = true;
            tt[1] = false;
            Assert.AreEqual(true, tt.Output[0], "10 case");
            Assert.AreEqual(false, tt.Output[1], "10 case");

            tt[0] = false;
            tt[1] = true;
            Assert.AreEqual(true, tt.Output[0], "01 case");
            Assert.AreEqual(false, tt.Output[1], "01 case");

            tt[0] = true;
            tt[1] = true;
            Assert.AreEqual(true, tt.Output[0], "11 case");
            Assert.AreEqual(true, tt.Output[1], "11 case");
        }

        [TestMethod]
        public void BasicTTTest()
        {
            Gates.TruthTable.TruthTable tt = CreateBasic();
            TestBasic(tt);
            

        }

        private Gates.TruthTable.TruthTable CreateLines()
        {
            Gates.TruthTable.TruthTable tt = new Gates.TruthTable.TruthTable(3, 2);
            Assert.AreEqual(8, tt.Table.GetLength(0));
            Assert.AreEqual(2, tt.Table[0].GetLength(0));

            // output 0 is xor
            // output 1 is not first bit
            tt[new bool[] { false, false, false }, 0] = false;
            tt[new bool[] { false, false, false }, 1] = true;

            tt[new bool[] { false, false, true }, 0] = true;
            tt[new bool[] { false, false, true }, 1] = true;

            tt[new bool[] { false, true, false }, 0] = true;
            tt[new bool[] { false, true, false }, 1] = true;

            tt[new bool[] { true, false, false }, 0] = true;
            tt[new bool[] { true, false, false }, 1] = false;

            tt[new bool[] { false, true, true }, 0] = false;
            tt[new bool[] { false, true, true }, 1] = true;

            tt[new bool[] { true, true, false }, 0] = false;
            tt[new bool[] { true, true, false }, 1] = false;

            tt[new bool[] { true, true, true }, 0] = true;
            tt[new bool[] { true, true, true }, 1] = false;


            return tt;
        }

        private void TestLines(Gates.TruthTable.TruthTable tt)
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++)
                    {

                        tt[0] = i == 1;
                        tt[1] = j == 1;
                        tt[2] = k == 1;

                        Assert.AreEqual((i + j + k) % 2 == 1, tt.Output[0], i.ToString() + j.ToString() + k.ToString());
                        Assert.AreEqual(i == 0, tt.Output[1], i.ToString() + j.ToString() + k.ToString());

                    }
            
        }

        [TestMethod]
        public void BoolLinesTTTest()
        {

            Gates.TruthTable.TruthTable tt = CreateLines();
            TestLines(tt);


            


        }

        private Gates.TruthTable.StatefulTruthTable CreateSimpleState()
        {
            Gates.TruthTable.StatefulTruthTable stt = new Gates.TruthTable.StatefulTruthTable(2, 1, 1);
            
            // inputs first, state at end

            // 0 0 is keep state
            stt[new bool[] { false, false, false }, 0] = false;
            stt[new bool[] { false, false, false }, 1] = false;
            stt[new bool[] { false, false, true }, 0] = true;
            stt[new bool[] { false, false, true }, 1] = true;

            // 0 1 is reset
            stt[new bool[] { false, true, false }, 0] = false;
            stt[new bool[] { false, true, false }, 1] = false;
            stt[new bool[] { false, true, true }, 0] = false;
            stt[new bool[] { false, true, true }, 1] = false;


            // 1 0 is set
            stt[new bool[] { true, false, false }, 0] = true;
            stt[new bool[] { true, false, false }, 1] = true;
            stt[new bool[] { true, false, true }, 0] = true;
            stt[new bool[] { true, false, true }, 1] = true;

            return stt;
        }

        private void TestSimpleState(Gates.TruthTable.StatefulTruthTable stt)
        {
            Assert.AreEqual(2, stt.NumberOfInputs);
            Assert.AreEqual(1, stt.Output.Length);
            Assert.AreEqual(8, stt.Table.GetLength(0)); // 3 bits total (2 inp, 1 state)
            Assert.AreEqual(2, stt.Table[0].GetLength(0)); // 2 bits total (1 out, 1 state)


            // try it out several sequences
            for (int i = 0; i < 2; i++)
            {
                // RESET
                stt[0] = false;
                stt[1] = true;

                Assert.AreEqual(stt.Output[0], false);
                // HOLD
                stt[0] = false;
                stt[1] = false;

                Assert.AreEqual(stt.Output[0], false);

                // SET
                stt[0] = true;
                stt[1] = false;
                Assert.AreEqual(stt.Output[0], true);

                // HOLD
                stt[0] = false;
                stt[1] = false;

                Assert.AreEqual(stt.Output[0], true);
            }
        }

        [TestMethod]
        public void SimpleStateTTTest()
        {
            Gates.TruthTable.StatefulTruthTable stt = CreateSimpleState();
            TestSimpleState(stt);

            
        }

        private Gates.TruthTable.StatefulTruthTable CreateToggleState()
        {
            Gates.TruthTable.StatefulTruthTable stt = new Gates.TruthTable.StatefulTruthTable(1, 1, 1);
            
            // inputs first, state at end


            // !!! FOR TEST PURPOSES INVERT OUTPUT STATE

            // 0 is keep state
            stt[new bool[] { false, false }, 0] = true;
            stt[new bool[] { false, false }, 1] = false;
            stt[new bool[] { false, true }, 0] = false;
            stt[new bool[] { false, true }, 1] = true;

            // 1 is toggle
            stt[new bool[] { true, false }, 0] = false;
            stt[new bool[] { true, false }, 1] = true;
            stt[new bool[] { true, true }, 0] = true;
            stt[new bool[] { true, true }, 1] = false;

            return stt;
        }

        private void TestToggleState(Gates.TruthTable.StatefulTruthTable stt)
        {

            Assert.AreEqual(1, stt.NumberOfInputs);
            Assert.AreEqual(1, stt.Output.Length);
            Assert.AreEqual(4, stt.Table.GetLength(0)); // 4 bits total (1 inp, 1 state)
            Assert.AreEqual(2, stt.Table[0].GetLength(0)); // 2 bits total (1 out, 1 state)


            bool st = true;
            for (int i = 0; i < 10; i++)
            {
                stt[0] = false;
                Assert.AreEqual(st, stt.Output[0]);

                stt[0] = true;
                st = !st;
                Assert.AreEqual(st, stt.Output[0]);
            }
        }

        [TestMethod]
        public void ToggleStateTTTest()
        {

            Gates.TruthTable.StatefulTruthTable stt = CreateToggleState();
            TestToggleState(stt);

        }

        [TestMethod]
        public void CloneTT()
        {
            Gates.TruthTable.TruthTable tt = CreateLines();
            Gates.TruthTable.TruthTable tt2 = (Gates.TruthTable.TruthTable)tt.Clone();
            TestLines(tt2);

            Gates.TruthTable.StatefulTruthTable stt = CreateSimpleState();
            Gates.TruthTable.StatefulTruthTable stt2 = (Gates.TruthTable.StatefulTruthTable)stt.Clone();
            TestSimpleState(stt2);

        }

        [TestMethod]
        public void DefaultTTGenerator()
        {
            Gates.TruthTable.TruthTable tt = CreateLines();
            Gates.TruthTable.TruthTable tt2 = Gates.TruthTable.TruthTable.DefaultTruthTable(tt);
            TestLines(tt2);

            Gates.BasicGates.Xor xor = new Gates.BasicGates.Xor();
            Gates.TruthTable.TruthTable txor = Gates.TruthTable.TruthTable.DefaultTruthTable(xor);
            Assert.AreEqual(2, txor.NumberOfInputs);
            Assert.AreEqual(1, txor.Output.Length);
            Assert.AreEqual(4, txor.Table.GetLength(0));
            Assert.AreEqual(1, txor.Table[0].GetLength(0));

            txor[0] = false;
            txor[1] = false;
            Assert.AreEqual(false, txor.Output[0]);
            
            txor[0] = true;
            txor[1] = false;
            Assert.AreEqual(true, txor.Output[0]);

            txor[0] = true;
            txor[1] = true;
            Assert.AreEqual(false, txor.Output[0]);

            txor[0] = false;
            txor[1] = true;
            Assert.AreEqual(true, txor.Output[0]);

        }
    }
}
