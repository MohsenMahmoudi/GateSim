using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gates.IOGates;

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

namespace Gates
{
    /// <summary>
    /// An "integrated circuit" is represented as encapsulating
    /// a circuit with inputs and outputs as a single gate.
    /// All inputs/outputs must be specified at construction.
    /// No changes to the encapsulated circuit are allowed;
    /// such changes are undefined.
    /// A new instance of the circuit and the IC are required
    /// for each use; multiple ICs wrapping the same circuit
    /// instance will lead to incorrect behavior.
    /// </summary>
    public class IC : AbstractGate
    {
       

        private Circuit _circuit;
        private UserInput[] _inputs;
        private UserOutput[] _outputs;
        private string _name;

        /// <summary>
        /// Define an IC based on a circuit with identified input and output ports.
        /// Ports must be a member of the circuit.
        /// </summary>
        /// <param name="circuit"></param>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <param name="name"></param>
        public IC(Circuit circuit, UserInput[] inputs, UserOutput[] outputs, string name)
            : base(inputs.Length, outputs.Length)
        {
            foreach (UserInput p in inputs)
                if (!circuit.Contains(p))
                    throw new ArgumentException("not all inputs part of circuit");

            // we wire up the output ports so that any internal
            // change in the circuit results in
            // passing notification up the chain
            // this is not needed for input because
            // any input change will cause compute to be called
            // automatically
            foreach (UserOutput p in outputs)
            {
                p.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(p_PropertyChanged);
                if (!circuit.Contains(p))
                    throw new ArgumentException("not all outputs part of circuit");
            }
            _inputs = inputs;
            _outputs = outputs;
            _circuit = circuit;
            _name = name;

            RunCompute();
        }

        

        private void p_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
                RunCompute();
        }

        public override string Name
        {
            get { return _name; }
        }

        protected override bool[] Compute(bool[] inp)
        {
            for (int i = 0; i < inp.Length; i++)
                _inputs[i].Value = inp[i];
            
            bool[] outp = new bool[_outputs.Length];
            for (int i = 0; i < outp.Length; i++)
                outp[i] = _outputs[i].Value;

            return outp;

        }

        /// <summary>
        /// Access the circuit inside this IC
        /// </summary>
        public Circuit Circuit
        {
            get
            {
                return _circuit;
            }
        }

        /// <summary>
        /// Access the user inputs that receive values.  Do NOT set input values
        /// this way!
        /// </summary>
        public UserInput[] Inputs
        {
            get
            {
                return _inputs;
            }
        }

        /// <summary>
        /// Access the user outputs that produce values.  Do NOT read output values
        /// this way!
        /// </summary>
        public UserOutput[] Outputs
        {
            get
            {
                return _outputs;
            }
        }
       

        public IC Clone(string newName)
        {
            Circuit nc = _circuit.Clone();

            UserInput[] nui = new UserInput[_inputs.Length];
            for (int i = 0; i < _inputs.Length; i++)
                nui[i] = (UserInput)nc[_circuit.IndexOf(_inputs[i])];

            UserOutput[] nuo = new UserOutput[_outputs.Length];
            for (int i = 0; i < _outputs.Length; i++)
                nuo[i] = (UserOutput)nc[_circuit.IndexOf(_outputs[i])];

            IC nic = new IC(nc, nui, nuo, newName);

            return nic;
        }

        public override AbstractGate Clone()
        {
            return Clone(_name);

        }

        /// <summary>
        /// Perform a recursive sweep looking for an IC within this
        /// IC that matches the given name.  Will also return true
        /// if this IC has the given name.
        /// </summary>
        /// <param name="icname"></param>
        /// <returns></returns>
        public bool DeepIncludes(string icname)
        {
            if (Name == icname)
                return true;

            foreach (AbstractGate ag in Circuit)
            {
                if (ag is Gates.IC)
                {
                    if (((Gates.IC)ag).DeepIncludes(icname))
                        return true;
                }
            }

            return false;
        }
    }
}
