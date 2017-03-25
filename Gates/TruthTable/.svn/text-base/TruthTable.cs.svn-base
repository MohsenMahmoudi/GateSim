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

namespace Gates.TruthTable
{
    /// <summary>
    /// A truth table which has some inputs and outputs.  A row
    /// for each input combination is created (2^n).
    /// </summary>
    public class TruthTable : AbstractGate
    {
        /// <summary>
        /// The values.  The first index is input, the second is output.
        /// This allows slicing.
        /// </summary>
        protected bool[][] values; // not using [,] notation because I want to slice it

        public override string Name
        {
            get { return "Truth Table"; }
        }

        protected override bool[] Compute(bool[] inp)
        {
            int idx = BoolArrayToInt(inp);
            return values[idx];
            
            
        }

        /// <summary>
        /// Convert an array of bools into an int.
        /// First element in the array is most significant.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        protected int BoolArrayToInt(bool[] arr)
        {
            // convert input into index
            var str = String.Concat(arr.Select(x => x ? "1" : "0").ToArray());
            int idx = Convert.ToInt32(str, 2);
            return idx;
        }

        protected void DefineValues(int inputs, int outputs)
        {
            values = new bool[(int)Math.Pow(2, inputs)][];
            for (int i = 0; i < (int)Math.Pow(2, inputs); i++)
                values[i] = new bool[outputs];
        }

        public TruthTable(int inputs, int outputs) : base(inputs, outputs)
        {
            DefineValues(inputs, outputs);

        }

        /// <summary>
        /// Get or set a value in the truth table.
        /// </summary>
        /// <param name="input">Input # (value of input as integer)</param>
        /// <param name="output">Output #</param>
        /// <returns></returns>
        public bool this[int input, int output]
        {
            get
            {
                return values[input][output];
            }
            set
            {
                values[input][output] = value;
            }
        }

        /// <summary>
        /// Get or set a value in the truth table.
        /// </summary>
        /// <param name="input">Input # (as a line)</param>
        /// <param name="output">Output #</param>
        /// <returns></returns>
        public bool this[bool[] input, int output]
        {
            get
            {
                return this[BoolArrayToInt(input), output];
            }
            set
            {
                this[BoolArrayToInt(input), output] = value;
            }
        }

        /// <summary>
        /// Get the entire truth table.
        /// </summary>
        public bool[][] Table
        {
            get
            {
                return values;
            }
        }

        public override AbstractGate Clone()
        {
            TruthTable tt = new TruthTable(NumberOfInputs, Output.Length);
            for (int x = 0; x < values.GetLength(0); x++)
                for (int y = 0; y < values[0].GetLength(0); y++)
                    tt.values[x][y] = values[x][y];

            return tt;
        }


        /// <summary>
        /// Interrogate this gate with all input combinations
        /// to determine outputs.  Suitable only for single gates;
        /// do not use for ICs or any compound gate.
        /// This method is to provide an "easy" implementation of
        /// ICreateTruthTable for simple/basic gates.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static TruthTable DefaultTruthTable(AbstractGate g)
        {
            TruthTable tt = new TruthTable(g.NumberOfInputs, g.Output.Length);

            for (int i = 0; i < Math.Pow(2, g.NumberOfInputs); i++)
            {
                String binary = Convert.ToString(i, 2);
                binary = binary.PadLeft(g.NumberOfInputs, '0');
                bool[] vals = binary.ToCharArray().Select(x => x == '1').ToArray();
                for (int ii = 0; ii < g.NumberOfInputs; ii++)
                {
                    g[ii] = vals[ii];
                }
                for (int io = 0; io < g.Output.Length; io++)
                {
                    tt[vals, io] = g.Output[io];
                }


            }

            return tt;

        }
    }
}
