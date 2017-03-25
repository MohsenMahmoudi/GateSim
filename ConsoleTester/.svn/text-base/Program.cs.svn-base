using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gates;
using System.Threading;

namespace ConsoleTester
{
    class Program
    {
        static void TestAndNotLoop(bool oscillation)
        {
            Circuit c = new Circuit();
            Gates.BasicGates.And mand = new Gates.BasicGates.And();
            Gates.BasicGates.And mand2 = new Gates.BasicGates.And();
            Gates.BasicGates.Not mnot = new Gates.BasicGates.Not();
            mand2.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(mand2_PropertyChanged);
            mnot.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(mnot3_PropertyChanged);
            c.Add(mand);
            c.Add(mand2);
            c.Add(mnot);
            mand2[0] = true;

            
            c[new Terminal(1, mand2)] = new Terminal(0, mand);
            c[new Terminal(0, mnot)] = new Terminal(0, mand2);
            if (oscillation)
                c[new Terminal(0, mand)] = new Terminal(0, mnot);

            Console.Out.WriteLine(mand2.Output[0].ToString());

            mand[1] = true;
            Console.Out.WriteLine(mand2.Output[0].ToString());

            mand[0] = true;
            Console.Out.WriteLine(mand2.Output[0].ToString());

            Console.Out.Write(mnot.Output[0].ToString());
            Gates.PropagationThread.Instance.WaitOnPropagation();
            Console.Out.Write(mnot.Output[0].ToString());

            Console.Out.Write(mnot[0].ToString());

            Console.Out.Write("X");
        }

        static void TestRing(bool oscillation)
        {
            Circuit c = new Circuit();
            Gates.BasicGates.Not mnot1 = new Gates.BasicGates.Not();
            Gates.BasicGates.Not mnot2 = new Gates.BasicGates.Not();
            Gates.BasicGates.Not mnot3 = new Gates.BasicGates.Not();

            c.Add(mnot1);
            c.Add(mnot2);
            c.Add(mnot3);

            mnot3.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(mnot3_PropertyChanged);

            mnot1[0] = false;

            c[new Terminal(0, mnot2)] = new Terminal(0, mnot1);
            c[new Terminal(0, mnot3)] = new Terminal(0, mnot2);
            if (oscillation)
                c[new Terminal(0, mnot1)] = new Terminal(0, mnot3);


            Thread.Sleep(1000);
            c.Disconnect(new Terminal(0, mnot1));
        
        }

        static void mnot3_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Gates.BasicGates.Not mnot = (Gates.BasicGates.Not)sender;
            Console.Out.WriteLine("MNOT3 NOTIFY: " + mnot.Output[0].ToString());
        }

        static void TestUserIO()
        {
            Circuit c = new Circuit();
            Gates.BasicGates.Not mnot1 = new Gates.BasicGates.Not();
            Gates.BasicGates.And mand = new Gates.BasicGates.And();
            Gates.BasicGates.Not mnot3 = new Gates.BasicGates.Not();
            Gates.BasicGates.Not mnot2 = new Gates.BasicGates.Not();

            Gates.IOGates.UserInput ui = new Gates.IOGates.UserInput();
            Gates.IOGates.UserOutput uo = new Gates.IOGates.UserOutput();

            c.Add(mnot1);
            c.Add(mnot2);
            c.Add(mand);
            c.Add(mnot3);

            c.Add(ui);
            c.Add(uo);


            uo.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(uo_PropertyChanged);

            c[new Terminal(0, mand)] = new Terminal(0, mnot2);
            c[new Terminal(0, mnot3)] = new Terminal(0, mand);
            c[new Terminal(0, mnot1)] = new Terminal(0, mnot3);
            c[new Terminal(0, mnot2)] = new Terminal(0, mnot1);

            c[new Terminal(1, mand)] = new Terminal(0, ui);
            c[new Terminal(0, uo)] = new Terminal(0, mnot3);

            

            do
            {
                Console.Out.WriteLine("Ready for X");
                Console.In.ReadLine();
                ui.Value = !ui.Value;
            } while (true);



        }

        static void uo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Gates.IOGates.UserOutput uo = (Gates.IOGates.UserOutput)sender;
            if (e.PropertyName == "Value")
                Console.Out.WriteLine("UO NOTIFY: " + uo.Value.ToString());
        }


        static void TestClock()
        {
            Circuit c = new Circuit();
            Gates.IOGates.Clock clk1 = new Gates.IOGates.Clock(1000);
            Gates.IOGates.Clock clk2 = new Gates.IOGates.Clock(250);
            Gates.BasicGates.And mand = new Gates.BasicGates.And();

            c.Add(clk1);
            c.Add(clk2);
            c.Add(mand);
            
            mand.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(mand2_PropertyChanged);

            //mand[1] = true;

            c[new Terminal(0, mand)] = new Terminal(0, clk1);
            c[new Terminal(1, mand)] = new Terminal(0, clk2);



        }

        static void mand2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Gates.BasicGates.And mand2 = (Gates.BasicGates.And)sender;
            Console.Out.WriteLine("NOTIFY: " + mand2.Output[0].ToString());
        }

        static IC Nor()
        {
            Circuit nor = new Circuit();
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

            nor[new Terminal(0, mnot1)] = new Terminal(0, in1);
            nor[new Terminal(0, mnot2)] = new Terminal(0, in2);
            nor[new Terminal(0, mand)] = new Terminal(0, mnot1);
            nor[new Terminal(1, mand)] = new Terminal(0, mnot2);
            nor[new Terminal(0, out1)] = new Terminal(0, mand);

            return new IC(nor, new Gates.IOGates.UserInput[] { in1, in2 },
                new Gates.IOGates.UserOutput[] { out1 }, "Nor");
            

        }

        static void TestIC1()
        {
            Circuit c = new Circuit();
            IC nor = Nor();

            c.Add(nor);

            nor.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(nor_PropertyChanged);


            nor[0] = false;
            nor[1] = false;
            Thread.Sleep(100);
            Console.Out.WriteLine("--");
            nor[0] = true;
            Thread.Sleep(100);
            Console.Out.WriteLine("--");
            nor[1] = true;
            Thread.Sleep(100);
            Console.Out.WriteLine("--");
            nor[0] = false;
            Thread.Sleep(100);
            Console.Out.WriteLine("--");
            nor[1] = false;
        }

        static void nor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IC ic = (Gates.IC)sender;
            Console.Out.WriteLine("NOTIFY: " + ic.Output[0].ToString());
        }

        static IC SRLatch()
        {
            Circuit sr = new Circuit();
            IC nor1 = Nor();
            IC nor2 = Nor();

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

            nor1.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(nor_PropertyChanged);

            sr[new Terminal(0, nor1)] = new Terminal(0, in1);
            sr[new Terminal(1, nor2)] = new Terminal(0, in2);

            sr[new Terminal(1, nor1)] = new Terminal(0, nor2);
            sr[new Terminal(0, nor2)] = new Terminal(0, nor1);

            sr[new Terminal(0, out1)] = new Terminal(0, nor1);
            sr[new Terminal(0, out2)] = new Terminal(0, nor2);

            return new IC(sr, new Gates.IOGates.UserInput[] { in1, in2 },
                new Gates.IOGates.UserOutput[] { out1, out2 }, "SRLatch");
        }

        static void TestIC2()
        {
            Circuit c = new Circuit();

            Gates.IOGates.UserInput ui_r = new Gates.IOGates.UserInput();
            Gates.IOGates.UserInput ui_s = new Gates.IOGates.UserInput();
            Gates.IOGates.UserOutput uo = new Gates.IOGates.UserOutput();
            IC latch = SRLatch();

            c.Add(ui_r);
            c.Add(ui_s);
            c.Add(uo);
            c.Add(latch);

            c[new Terminal(0, latch)] = new Terminal(0, ui_r);
            c[new Terminal(1, latch)] = new Terminal(0, ui_s);
            c[new Terminal(0, uo)] = new Terminal(0, latch);

            uo.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(uo_PropertyChanged);

            while (true)
            {
                Console.WriteLine("Ready for input (S = set, R = reset, X = hold, I = illegal)");
                string val = Console.In.ReadLine();
                if (val == "S")
                {
                    ui_r.Value = false;
                    ui_s.Value = true;
                }
                if (val == "R")
                {
                    ui_s.Value = false;
                    ui_r.Value = true;
                }
                if (val == "X")
                {
                    ui_s.Value = false;
                    ui_r.Value = false;
                }
                if (val == "I")
                {
                    ui_s.Value = true;
                    ui_r.Value = true;
                }
            }
        }

        static void Main(string[] args)
        {
            //TestIC2();
            TestAndNotLoop(false);
            
        }

    }
}
