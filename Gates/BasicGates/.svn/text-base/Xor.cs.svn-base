using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 *  Copyright (C) 2010 Steve Kollmansberger
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

namespace Gates.BasicGates
{
    /// <summary>
    /// Represents the boolean "xor" (exclusive-or), true iff either but not both of the inputs are true.
    /// </summary>
    public class Xor : AbstractGate
    {
        public override string Name
        {
            get { return "Xor"; }
        }

        protected override bool[] Compute(bool[] inp)
        {
            return new bool[] { inp[0] ^ inp[1] };
        }

        public Xor() : base(2, 1) { }
    }
}
