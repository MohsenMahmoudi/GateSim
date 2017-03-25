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

namespace Gates.BasicGates
{
    /// <summary>
    /// Represents the boolean "nand", an and followed by a not.
    /// </summary>
    public class Nand : AbstractGate, IVariableInputs
    {
        public override string Name
        {
            get { return "Nand"; }
        }

        protected override bool[] Compute(bool[] inp)
        {
            return new bool[] { !(inp.All(x => { return x; })) };
        }

        /// <summary>
        /// Construct a 2 input NAnd gate
        /// </summary>
        public Nand() : this(2) { }

        /// <summary>
        /// Construct a 2 or more input NAnd gate
        /// </summary>
        /// <param name="numInputs"></param>
        public Nand(int numInputs)
            : base(numInputs, 1)
        {
            if (numInputs < 2)
                throw new ArgumentOutOfRangeException("numInputs must be at least 2");

            // because the initial output is true we must
            // run the compute to update this as the default
            // is false
            RunCompute();
        }

        public override AbstractGate Clone()
        {
            return Clone(NumberOfInputs);
        }

        #region IVariableInputs Members

        public AbstractGate Clone(int numInputs)
        {
            return new Nand(numInputs);
        }

        #endregion
    }
}
