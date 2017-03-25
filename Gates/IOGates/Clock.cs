using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

/*
 *  Copyright (C) 2011 Steve Kollmansberger
 * 
 *  This file is part of Logic Gate Simulator.
 *
 *  Logic Gate Simulator is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Logic Gate Simulator is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Logic Gate Simulator.  If not, see <http://www.gnu.org/licenses/>.
 */

namespace Gates.IOGates
{
    /// <summary>
    /// A timer which alternates between on and off states
    /// at a regular, specified interval.
    /// </summary>
    public class Clock : AbstractGate
    {
        public override string Name
        {
            get { return "Clock"; }
        }

        protected override bool[] Compute(bool[] inp)
        {
            return new bool[1] { _val };
        }

        private Timer t;
        private bool _val;
        private int _ms;
        
        private static int? _precession = null;
        private static Timer _precTimer;

        /// <summary>
        /// Calculate the clock precession.  This is done by using a timer
        /// to count off a 500-millisecond period, and measuring how long
        /// the period actually took.  The calculated value is then
        /// used to adjust all timing requests made for clocks.
        /// </summary>
        public static void CalculatePrecession()
        {
            DateTime? begin = null;
            _precTimer = new Timer((obj) =>
            {
                if (begin.HasValue)
                {
                    int v = (int)(DateTime.Now - begin.Value).TotalMilliseconds - 500;
                    if (v >= 0)
                    {
                        _precession = v;
                        System.Diagnostics.Debug.Print("Clock precession found to be " + _precession.ToString() + "ms");
                        _precTimer.Dispose();
                    }
                    else begin = DateTime.Now;
                }
                else begin = DateTime.Now;
            }, null, 100, 500);
        }

        /// <summary>
        /// Construct a clock gate.  The clock starts immediately.
        /// </summary>
        /// <param name="milliseconds">The clock period, in milliseconds</param>
        public Clock(int milliseconds)
            : base(0, 1)
        {

            t = new Timer((obj) =>
            {
                _val = !_val;
                RunCompute();
            });

            Milliseconds = milliseconds;
            

        }

        /// <summary>
        /// Gets or sets the period, in milliseconds of this clock.  If the period
        /// is set to zero, the clock is stopped in the off position.  The period
        /// may not be negative.
        /// </summary>
        public int Milliseconds
        {
            get
            {
                return _ms;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Milliseconds must be at least 0");

                

                _ms = value;
                if (value > 0)
                {
                    int usePrec = _precession.HasValue ? _precession.Value : 0;

                    if (value / 2 <= usePrec)
                        value = usePrec * 2 + 2;

                    t.Change(0, value / 2 - usePrec); // value = period
                }
                else
                {
                    t.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    _val = false; // always disable the clock "down"
                    RunCompute();
                }
                NotifyPropertyChanged("Milliseconds");
            }
        }

        public override AbstractGate Clone()
        {
            return new Clock(_ms);
        }

    }
}
