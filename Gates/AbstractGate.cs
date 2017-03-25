using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


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
    /// Generalized framework for a logic gate, consisting of inputs and outputs.
    /// </summary>
    public abstract class AbstractGate : INotifyPropertyChanged
    {
        private bool[] inp;
        private bool[] outp;

        /// <summary>
        /// Inputs and outputs are indexed from 0.  The total number of
        /// each is specified in the constructor parameters.
        /// </summary>
        /// <param name="numInputs"></param>
        /// <param name="numOutputs"></param>
        public AbstractGate(int numInputs, int numOutputs) 
        {
            inp = new bool[numInputs];
            outp = new bool[numOutputs];
        }

        /// <summary>
        /// Number of inputs for this gate.  The number of
        /// outputs can be determined by checking the length
        /// from the public Output property.
        /// </summary>
        public int NumberOfInputs
        {
            get
            {
                return inp.Length;
            }
        }

        /// <summary>
        /// View or edit the state of an input value.
        /// </summary>
        /// <param name="index">The input index</param>
        /// <returns>The input value</returns>
        public bool this[int index]
        {
            get
            {
                return inp[index];
            }
            set
            {
                inp[index] = value;
                NotifyPropertyChanged("this");
                RunCompute();
            }
        }

        /// <summary>
        /// Retrieve an output value.
        /// </summary>
        public bool[] Output
        {
            get
            {
                return (bool[])outp.Clone();
            }
        }

        /// <summary>
        /// Request a new value from the Compute method,
        /// and check if the outputs should be updated.
        /// If so, update them and send notification.
        /// This method is called automatically when any
        /// input values are changed.
        /// </summary>
        protected void RunCompute()
        {
            bool[] newoutp = Compute(inp);
            for (int i = 0; i < newoutp.Length; i++)
                if (outp[i] != newoutp[i])
                {
                    // make this the new output
                    outp = newoutp;
                    NotifyPropertyChanged("Output");
                    // no need to continue searching for a change
                    return;
                }
        }



        /// <summary>
        /// The name of the gate, usually "and", "not", etc.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Called when any input value may have changed.
        /// Each specific gate should fill in logic to
        /// compute the outputs based on the given input array.
        /// </summary>
        /// <param name="inp">The input values to use.</param>
        /// <returns>The appropriate output values.</returns>
        protected abstract bool[] Compute(bool[] inp);


        /// <summary>
        /// Create a deep clone of this gate. Recursively clones
        /// all the way down, to create a completely seperate gate.
        /// </summary>
        /// <returns></returns>
        public virtual AbstractGate Clone()
        {
            return (AbstractGate)Activator.CreateInstance(GetType());
        }
        

        #region INotifyPropertyChanged Members

        protected void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
