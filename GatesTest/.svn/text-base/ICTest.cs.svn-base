using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GatesTest
{
    /// <summary>
    /// Summary description for ICTest
    /// </summary>
    [TestClass]
    public class ICTest
    {
        public ICTest()
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

        Gates.IC CreateNor()
        {
            Gates.Circuit nor = new Gates.Circuit();
            nor.Start();

            Gates.BasicGates.And mand = new Gates.BasicGates.And();
            Gates.BasicGates.Not mnot1 = new Gates.BasicGates.Not();
            Gates.BasicGates.Not mnot2 = new Gates.BasicGates.Not();

            Gates.IOGates.UserInput in1 = new Gates.IOGates.UserInput();
            Gates.IOGates.UserInput in2 = new Gates.IOGates.UserInput();
            Gates.IOGates.UserOutput out1 = new Gates.IOGates.UserOutput();

            nor.Add(mand);
            nor.Add(mnot1);
            nor.Add(mnot2);
            nor.Add(in1);
            nor.Add(in2);
            nor.Add(out1);

            nor[new Gates.Terminal(0, mnot1)] = new Gates.Terminal(0, in1);
            nor[new Gates.Terminal(0, mnot2)] = new Gates.Terminal(0, in2);
            nor[new Gates.Terminal(0, mand)] = new Gates.Terminal(0, mnot1);
            nor[new Gates.Terminal(1, mand)] = new Gates.Terminal(0, mnot2);
            nor[new Gates.Terminal(0, out1)] = new Gates.Terminal(0, mand);

            return new Gates.IC(nor, new Gates.IOGates.UserInput[] { in1, in2 },
                new Gates.IOGates.UserOutput[] { out1 }, "Nor");


        }

        [TestMethod]
        public void Nor()
        {
            Gates.IC nor = CreateNor();
           

            nor[0] = false;
            nor[1] = true;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, nor.Output[0]);

            nor[0] = true;
            nor[1] = false;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, nor.Output[0]);

            nor[0] = false;
            nor[1] = false;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(true, nor.Output[0]);

            nor[0] = true;
            nor[1] = true;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, nor.Output[0]);
            
        }


        [TestMethod]
        public void NorInCiruit()
        {
            Gates.IC nor = CreateNor();
            Gates.Circuit c = new Gates.Circuit();
            c.Start();
            c.Add(nor);


            // primarily just tests that c's wait
            // also makes the nor wait

            nor[0] = false;
            nor[1] = true;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, nor.Output[0]);

            nor[0] = true;
            nor[1] = false;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, nor.Output[0]);

            nor[0] = false;
            nor[1] = false;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(true, nor.Output[0]);

            nor[0] = true;
            nor[1] = true;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, nor.Output[0]);

        }

        Gates.IC CreateSRLatch()
        {
            Gates.Circuit sr = new Gates.Circuit();
            sr.Start();
            Gates.IC nor1 = CreateNor();
            Gates.IC nor2 = CreateNor();

            Gates.IOGates.UserInput in1 = new Gates.IOGates.UserInput();
            Gates.IOGates.UserInput in2 = new Gates.IOGates.UserInput();
            Gates.IOGates.UserOutput out1 = new Gates.IOGates.UserOutput();
            Gates.IOGates.UserOutput out2 = new Gates.IOGates.UserOutput();

            sr.Add(nor1);
            sr.Add(nor2);

            sr.Add(in1);
            sr.Add(in2);
            sr.Add(out1);
            sr.Add(out2);


            sr[new Gates.Terminal(0, nor1)] = new Gates.Terminal(0, in1);
            sr[new Gates.Terminal(1, nor2)] = new Gates.Terminal(0, in2);

            sr[new Gates.Terminal(1, nor1)] = new Gates.Terminal(0, nor2);
            sr[new Gates.Terminal(0, nor2)] = new Gates.Terminal(0, nor1);

            sr[new Gates.Terminal(0, out1)] = new Gates.Terminal(0, nor1);
            sr[new Gates.Terminal(0, out2)] = new Gates.Terminal(0, nor2);

            return new Gates.IC(sr, new Gates.IOGates.UserInput[] { in1, in2 },
                new Gates.IOGates.UserOutput[] { out1, out2 }, "SRLatch");
        }

        [TestMethod()]
        public void SRLatch()
        {
            Gates.Circuit c = new Gates.Circuit();
            c.Start();

            Gates.IOGates.UserInput ui_r = new Gates.IOGates.UserInput();
            Gates.IOGates.UserInput ui_s = new Gates.IOGates.UserInput();
            Gates.IOGates.UserOutput uo = new Gates.IOGates.UserOutput();
            Gates.IC latch = CreateSRLatch();

            c.Add(ui_r);
            c.Add(ui_s);
            c.Add(uo);
            c.Add(latch);

            c[new Gates.Terminal(0, latch)] = new Gates.Terminal(0, ui_r);
            c[new Gates.Terminal(1, latch)] = new Gates.Terminal(0, ui_s);
            c[new Gates.Terminal(0, uo)] = new Gates.Terminal(0, latch);

            for (int i = 0; i < 4; i++)
            {
                // SET
                ui_r.Value = false;
                ui_s.Value = true;
                Gates.PropagationThread.Instance.WaitOnPropagation();
                Assert.AreEqual(true, uo.Value);

                // HOLD
                ui_r.Value = false;
                ui_s.Value = false;
                Gates.PropagationThread.Instance.WaitOnPropagation();
                Assert.AreEqual(true, uo.Value);

                // RESET
                ui_r.Value = true;
                ui_s.Value = false;
                Gates.PropagationThread.Instance.WaitOnPropagation();
                Assert.AreEqual(false, uo.Value);

                // HOLD
                ui_r.Value = false;
                ui_s.Value = false;
                Gates.PropagationThread.Instance.WaitOnPropagation();
                Assert.AreEqual(false, uo.Value);
            }
        }

        private Gates.IC CreateAndChain(int len)
        {
            Gates.Circuit andc = new Gates.Circuit();
            andc.Start();
            
            
            

            Gates.IOGates.UserInput in1 = new Gates.IOGates.UserInput();
            andc.Add(in1);
            Gates.IOGates.UserOutput out1 = new Gates.IOGates.UserOutput();
            andc.Add(out1);

            if (len > 1)
            {
                Gates.IC cand1 = CreateAndChain(len / 2);
                Gates.IC cand2 = CreateAndChain(len / 2);
                andc.Add(cand1);
                andc.Add(cand2);
                andc[new Gates.Terminal(0, cand1)] = new Gates.Terminal(0, in1);
                andc[new Gates.Terminal(0, cand2)] = new Gates.Terminal(0, cand1);
                andc[new Gates.Terminal(0, out1)] = new Gates.Terminal(0, cand2);
            }
            else
            {
                Gates.BasicGates.And mand = new Gates.BasicGates.And();
                andc.Add(mand);
                andc[new Gates.Terminal(0, mand)] = new Gates.Terminal(0, in1);
                andc[new Gates.Terminal(1, mand)] = new Gates.Terminal(0, in1);
                andc[new Gates.Terminal(0, out1)] = new Gates.Terminal(0, mand);
            }

            return new Gates.IC(andc, new Gates.IOGates.UserInput[] { in1 },
                new Gates.IOGates.UserOutput[] { out1 }, "AndChain");
        }

        [TestMethod()]
        public void PropagationTest()
        {
            // this test checks for the accuracy
            // of wait for propagation
            // by determining if it fully waits for all propagation
            // to occur through the contained sub-circuits

            // this came from a problem where all sub-circuits
            // were instructed to wait BUT
            // the loop didn't repeat this based on that
            // so one could pass off an incomplete result
            // to another and the waiting could end before
            // it was all really done
            Gates.Circuit c = new Gates.Circuit();
            c.Start();

            // EXTREMELY SPECIFIC
            // The long chain appears FIRST in the circuit add list
            // But the input connects to the short chain first
            // Then the wait waits on the long chain, which has nothing
            // then briefly on the short chain, which passes a change
            // to the long chain...
            // but it DOESN'T wait for the long chain to finish
            // (because it already waited on the long chain before it had any work!)
            Gates.IC ac1 = CreateAndChain(64);
            Gates.IC ac2 = CreateAndChain(10);

            Gates.IOGates.UserInput ui = new Gates.IOGates.UserInput();
            Gates.BasicGates.And mand = new Gates.BasicGates.And();

            c.Add(ac1);
            c.Add(ac2);
            c.Add(ui);
            c.Add(mand);

            c[new Gates.Terminal(0, ac2)] = new Gates.Terminal(0, ui);
            c[new Gates.Terminal(0, ac1)] = new Gates.Terminal(0, ac2);
            c[new Gates.Terminal(0, mand)] = new Gates.Terminal(0, ac1);
            c[new Gates.Terminal(1, mand)] = new Gates.Terminal(0, ac2);

            ui.Value = false;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, mand.Output[0]);

            ui.Value = true;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(true, mand.Output[0]);
        }


        [TestMethod()]
        public void CloneTest()
        {
            Gates.IC nor = CreateNor();
            Gates.IC nor2 = (Gates.IC)nor.Clone();
            nor2.Circuit.Start();

            nor[0] = false;
            nor2[0] = false;

            Gates.PropagationThread.Instance.WaitOnPropagation();
            //Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(true, nor.Output[0]);
            Assert.AreEqual(true, nor2.Output[0]);
            
            nor[0] = true;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            //nor2.WaitOnPropagation();
            Assert.AreEqual(false, nor.Output[0]);
            Assert.AreEqual(true, nor2.Output[0]);

            nor2[0] = true;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            //nor2.WaitOnPropagation();
            Assert.AreEqual(false, nor.Output[0]);
            Assert.AreEqual(false, nor2.Output[0]);

            nor[0] = false;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            //nor2.WaitOnPropagation();
            Assert.AreEqual(true, nor.Output[0]);
            Assert.AreEqual(false, nor2.Output[0]);
            
        }
    }
}
