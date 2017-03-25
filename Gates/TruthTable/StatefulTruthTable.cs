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

namespace Gates.TruthTable
{
    /// <summary>
    /// A truth table that records and acts on some state as well
    /// as input.  The state bits are append to the input and the output
    /// sizes so that all possibilities are considered.
    /// This can make tables very large.
    /// </summary>
    public class StatefulTruthTable : TruthTable
    {
        protected bool[] state;

        public override string Name
        {
            get { return "Stateful Truth Table"; }
        }

        public StatefulTruthTable(int inputs, int outputs, int state)
            : base(inputs, outputs)
        {
            // redefine values to include state
            DefineValues(inputs + state, outputs + state);

            this.state = new bool[state];
        }

        protected override bool[] Compute(bool[] inp)
        {
            // append state
            bool[] topass = new bool[NumberOfInputs + state.Length];
            Array.Copy(inp, topass, inp.Length);
            Array.Copy(state, 0, topass, inp.Length, state.Length);

            // perform standard lookup
            bool[] reslts = base.Compute(topass);

            // extract state and outputs
            bool[] outputs = new bool[Output.Length];
            Array.Copy(reslts, outputs, Output.Length);

            Array.Copy(reslts, Output.Length, state, 0, state.Length);

            return outputs;

            
        }

        public override AbstractGate Clone()
        {
            int numState = state.Length;
            StatefulTruthTable tt = new StatefulTruthTable(NumberOfInputs, Output.Length, numState);

            // values all look the same, just needed to diff. state to have inputs and outputs
            for (int x = 0; x < values.GetLength(0); x++)
                for (int y = 0; y < values[0].GetLength(0); y++)
                    tt.values[x][y] = values[x][y];
            
            return tt;
        }
    }
}
