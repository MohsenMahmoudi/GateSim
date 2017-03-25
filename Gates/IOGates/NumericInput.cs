using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    /// A numeric gate that takes the given integer values
    /// and converts it into a bit array.
    /// </summary>
    public class NumericInput : AbstractNumeric
    {
        
        protected override bool[] Compute(bool[] inp)
        {
            bool[] cbv = new bool[Bits];
            int v = IntValue;
            // we'll use the shift and test technique
            for (int i = 0; i < Bits; i++)
            {
                if ( (v & 1) != 0) cbv[i] = true;
                v >>= 1;
            }
            return cbv;
        }

        public NumericInput(int bits) : base(0, bits) { }

        public override AbstractGate Clone()
        {
            NumericInput nab = new NumericInput(Bits);
            nab.SelectedRepresentation = SelectedRepresentation;
            nab.Value = Value;
            return nab;
        }
    }
}
