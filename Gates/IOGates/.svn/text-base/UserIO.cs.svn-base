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

namespace Gates.IOGates
{
    /// <summary>
    /// A gate that represents a single boolean value which may be received
    /// or provided to the user.
    /// </summary>
    public abstract class UserIO : AbstractGate
    {
        protected bool _val;

        protected string _name;

        public void SetName(string name)
        {
            _name = name;
            NotifyPropertyChanged("Name");
        }

        public override string Name
        {
            get { return _name; }
        }

        public bool Value
        {
            get
            {
                return _val;
            }
            set
            {
                _val = value;
                RunCompute();
                NotifyPropertyChanged("Value");
            }
        }

        public UserIO(int numInputs, int numOutputs) : base(numInputs, numOutputs) { }
    }

    /// <summary>
    /// A User I/O gate that accepts a value from the user.
    /// </summary>
    public class UserInput : UserIO
    {


        protected override bool[] Compute(bool[] inp)
        {
            return new bool[] { _val };
        }

        public UserInput() : base(0, 1) { _name = "UserInput";  }

        public override AbstractGate Clone()
        {
            UserInput ui = new UserInput();
            ui.SetName(Name);
            return ui;
        }
    }

    /// <summary>
    /// A User I/O gate that provides a value to the user.
    /// </summary>
    public class UserOutput : UserIO
    {

        protected override bool[] Compute(bool[] inp)
        {
            _val = inp[0];
            NotifyPropertyChanged("Value");
            return new bool[0];
        }

        public UserOutput() : base(1, 0) { _name = "UserOutput"; }

        public override AbstractGate Clone()
        {
            UserOutput uo = new UserOutput();
            uo.SetName(Name);
            return uo;
        }
    }
}
