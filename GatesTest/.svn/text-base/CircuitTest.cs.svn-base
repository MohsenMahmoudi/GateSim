using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GatesTest
{
    /// <summary>
    /// Summary description for CircuitTest
    /// </summary>
    [TestClass]
    public class CircuitTest
    {
        public CircuitTest()
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
        [TestCleanup()]
        public void MyTestCleanup() 
        {
            Gates.PropagationThread.Instance.Clear();
        }
        //
        #endregion

        [TestMethod]
        public void ThruCircuit1()
        {
            Gates.Circuit c = new Gates.Circuit();
            c.Start();

            Gates.BasicGates.And mand = new Gates.BasicGates.And();
            Gates.BasicGates.And mand2 = new Gates.BasicGates.And();
            Gates.BasicGates.Not mnot = new Gates.BasicGates.Not();

            c.Add(mand);
            c.Add(mand2);
            c.Add(mnot);
            mand2[0] = true;

            c[new Gates.Terminal(1, mand2)] = new Gates.Terminal(0, mand);
            c[new Gates.Terminal(0, mnot)] = new Gates.Terminal(0, mand2);
           
            
            Assert.AreEqual(true, mnot.Output[0]);

            mand[1] = true;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(true, mnot.Output[0]);

            mand[0] = true;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, mnot.Output[0]);
        }

        [TestMethod()]
        public void ThruCircuit2()
        {
            Gates.Circuit c = new Gates.Circuit();
            c.Start();

            Gates.BasicGates.Not mnot1 = new Gates.BasicGates.Not();
            Gates.BasicGates.Not mnot2 = new Gates.BasicGates.Not();
            Gates.BasicGates.Not mnot3 = new Gates.BasicGates.Not();

            c.Add(mnot1);
            c.Add(mnot2);
            c.Add(mnot3);

            mnot1[0] = true;

            c[new Gates.Terminal(0, mnot2)] = new Gates.Terminal(0, mnot1);
            c[new Gates.Terminal(0, mnot3)] = new Gates.Terminal(0, mnot2);



            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, mnot3.Output[0]);
            
            mnot1[0] = false;
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(true, mnot3.Output[0]);

            c.Disconnect(new Gates.Terminal(0, mnot2));
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Assert.AreEqual(false, mnot3.Output[0]);
        }


        [TestMethod]
        public void OscillationCircuit1()
        {
            Gates.Circuit c = new Gates.Circuit();
            c.Start();

            Gates.BasicGates.And mand = new Gates.BasicGates.And();
            Gates.BasicGates.And mand2 = new Gates.BasicGates.And();
            Gates.BasicGates.Not mnot = new Gates.BasicGates.Not();

            c.Add(mand);
            c.Add(mand2);
            c.Add(mnot);
            mand2[0] = true;
            mand[1] = false;

            c[new Gates.Terminal(1, mand2)] = new Gates.Terminal(0, mand);
            c[new Gates.Terminal(0, mnot)] = new Gates.Terminal(0, mand2);
            c[new Gates.Terminal(0, mand)] = new Gates.Terminal(0, mnot);


            int cnt = 0;
            bool oldval = true;
            DateTime start = DateTime.Now;
            while (DateTime.Now - start < new TimeSpan(0, 0, 5))
            {
                if (mnot.Output[0] != oldval)
                {
                    oldval = mnot.Output[0];
                    cnt++;
                }
            }
            // 5 seconds but circuit should be "off" due to first and
            Assert.AreEqual(0, cnt);

            // turn circuit on and try again
            mand[1] = true;
            cnt = 0;
            oldval = false;
            start = DateTime.Now;
            while (DateTime.Now - start < new TimeSpan(0, 0, 5))
            {
                if (mnot.Output[0] != oldval)
                {
                    oldval = mnot.Output[0];
                    cnt++;
                }
            }

            // 5 seconds at arbitrarily high propagation rate
            // we'll assume AT LEAST 1 prop a second
            if (cnt < 5)
                Assert.Fail("Unacceptable count = " + cnt.ToString());

            c.Stop();
        }


        [TestMethod()]
        public void OscillationCircuit2()
        {
            Gates.Circuit c = new Gates.Circuit();
            c.Start();

            Gates.BasicGates.Not mnot1 = new Gates.BasicGates.Not();
            Gates.BasicGates.Not mnot2 = new Gates.BasicGates.Not();
            Gates.BasicGates.Not mnot3 = new Gates.BasicGates.Not();

            c.Add(mnot1);
            c.Add(mnot2);
            c.Add(mnot3);

            mnot1[0] = true;

            c[new Gates.Terminal(0, mnot2)] = new Gates.Terminal(0, mnot1);
            c[new Gates.Terminal(0, mnot3)] = new Gates.Terminal(0, mnot2);
            c[new Gates.Terminal(0, mnot1)] = new Gates.Terminal(0, mnot3);


            int cnt = 0;
            bool oldval = true;
            DateTime start = DateTime.Now;
            while (DateTime.Now - start < new TimeSpan(0, 0, 5))
            {
                if (mnot1.Output[0] != oldval)
                {
                    oldval = mnot1.Output[0];
                    cnt++;
                }
            }

            // after 5 seconds expect at least 5 oscillations
            if (cnt < 5)
                Assert.Fail("Unacceptable count = " + cnt.ToString());
            
            
            c.Disconnect(new Gates.Terminal(0, mnot2));
            cnt = 0;
            oldval = false;
            start = DateTime.Now;
            while (DateTime.Now - start < new TimeSpan(0, 0, 5))
            {
                if (mnot1.Output[0] != oldval)
                {
                    oldval = mnot1.Output[0];
                    cnt++;
                }
            }
            // wire is disconnected; no changes should occur
            // 1 change allowable for final sync
            if (cnt > 1)
                Assert.Fail("Disconnect didn't: " + cnt.ToString());

            

        }
    }
}
