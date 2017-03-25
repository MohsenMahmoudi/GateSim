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
    /// A numeric gate that takes an array of bits and converts it
    /// into a value.
    /// </summary>
    public class NumericOutput : AbstractNumeric
    {
        

        
        public override string Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                throw new Exception("Numeric output is read-only");
            }
        }

        protected override bool[] Compute(bool[] inp)
        {
            int val = 0;
            for (int i = 0; i < inp.Length; i++)
                if (inp[i])
                    val += (int)Math.Pow(2, i);
            _intval = val;
            
            NotifyPropertyChanged("Value");
            NotifyPropertyChanged("IntValue");
            return new bool[0];
        }

        public NumericOutput(int bits) : base(bits, 0) { }

        public override AbstractGate Clone()
        {
            NumericOutput nab = new NumericOutput(Bits);
            nab.SelectedRepresentation = SelectedRepresentation;

            return nab;
        }
    }
}
