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
    /// An input or output gate that represents a value numerically.
    /// The value may be represented in any of the four supported bases.
    /// The value is represented as an unsigned integer.
    /// 
    /// </summary>
    public abstract class AbstractNumeric : AbstractGate
    {

        protected string _name = "Numeric";

        /// <summary>
        /// The current unsigned integer value
        /// </summary>
        protected int _intval;
        private int _bits;

        /// <summary>
        /// Supported display representations for the value.
        /// </summary>
        public enum Representation
        {
            BINARY = 2,
            OCTAL = 8,
            DECIMAL = 10,

            /// <summary>
            /// Two's Complement Decimal
            /// </summary>
            D2C = 11, 

            /// <summary>
            /// Binary Coded Decimal (one digit per 4 bits)
            /// </summary>
            BCD = 12,
            HEXADECIMAL = 16,


        }

        protected Representation _rep = Representation.BINARY;

        /// <summary>
        /// Gets or sets the selected representation of the value.
        /// This effects the Value property, but not the integer
        /// value.
        /// </summary>
        public Representation SelectedRepresentation
        {
            get
            {
                return _rep;
            }
            set
            {
                if (value != Representation.BINARY &&
                    value != Representation.DECIMAL &&
                    value != Representation.HEXADECIMAL &&
                    value != Representation.OCTAL &&
                    value != Representation.BCD &&
                    value != Representation.D2C)
                    throw new ArgumentException("Invalid representation");
                
                if (value == Representation.BCD &&
                    Bits % 4 != 0)
                    throw new ArgumentException("Can't use BCD without bits multiply of 4");

                _rep = value;
                NotifyPropertyChanged("SelectedRepresentation");
                NotifyPropertyChanged("Value");
                
            }
        }

        private string ToBCD()
        {
            string result = "";

            // convert intval to bit string 
            string bits = Convert.ToString(IntValue, 2);
            bits = bits.PadLeft(Bits, '0');

            // consider each 4-bit block
            while (bits.Length > 0) 
            {
                
                // convert each 4 bit block into a digit
                int dval = Convert.ToInt32(bits.Substring(0, 4), 2);
                if (dval > 9)
                    throw new InvalidOperationException("Not valid BCD");

                result += dval.ToString();
                bits = bits.Substring(4);
            }

            return result;

            

        }


        private int FromBCD(string value)
        {
            
            // create 4 bit entry for each digit
            string bits = "";
            foreach (char x in value)
                bits += Convert.ToString(Convert.ToInt32(x.ToString()), 2).PadLeft(4, '0');

            return Convert.ToInt32(bits, 2);



        }

        /// <summary>
        /// Gets or sets the current value, given in the representation selected.
        /// Values range from 0 to MaxValue, inclusively.
        /// </summary>
        public virtual string Value
        {
            get
            {
                int bbase = (int)_rep;
                int val = _intval;
                
                if (_rep == Representation.BCD || _rep == Representation.D2C)
                    bbase = 10;
                
                if (_rep == Representation.D2C)
                {
                    if (val >= Math.Pow(2, Bits - 1))
                    {
                        val -= (int)Math.Pow(2, Bits);
                    }
                }

                if (_rep == Representation.BCD)
                {
                    return ToBCD();
                }

                // to upper because by default hex letters come out lowercase
                string res = Convert.ToString(val, bbase).ToUpper();
                if (_rep == Representation.BINARY)
                    res = res.PadLeft(Bits, '0');
                return res;
            }
            set
            {

                int bbase = (int)_rep;
                
                if (_rep == Representation.BCD || _rep == Representation.D2C)
                    bbase = 10;

                
                int nv = Convert.ToInt32(value, bbase);

                if (_rep == Representation.BCD)
                {
                    if (value.Length > Bits / 4)
                        throw new ArgumentOutOfRangeException("Value outside range of given bits");
                    nv = FromBCD(value);
                }


                if (_rep == Representation.D2C)
                {

                    // must also check for input too large or too small
                    // really too small would get caught below, but not all
                    if (nv < -Math.Pow(2,Bits - 1) || nv >= Math.Pow(2, Bits - 1))
                        throw new ArgumentOutOfRangeException("Value outside range of given bits");

                    if (nv < 0)
                    {
                        nv += (int)Math.Pow(2, Bits);
                    }
                    
                }

                if (nv < 0 || nv > MaxValue)
                    throw new ArgumentOutOfRangeException("Value outside range of given bits");
                _intval = nv;

                RunCompute();
                NotifyPropertyChanged("Value");
                NotifyPropertyChanged("IntValue");
                
            }
        }

        public void SetName(string name)
        {
            _name = name;
            NotifyPropertyChanged("Name");
        }

        public override string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The number of bits managed.
        /// </summary>
        public int Bits
        {
            get
            {
                return _bits;
            }
        }

        /// <summary>
        /// The current integer value.  This value does not change
        /// when selected representation is changed.  This value
        /// is based on unsigned bit representation and cannot
        /// be used for BCD and D2C representations.
        /// </summary>
        public int IntValue
        {
            get
            {
                return _intval;
            }
        }

        /// <summary>
        /// The maximum inclusive integer value. Same as 2^Bits - 1
        /// </summary>
        public int MaxValue
        {
            get
            {
                return (int)Math.Pow(2, Bits) - 1;
            }
        }

        

        /// <summary>
        /// The number of inputs/outputs decides the number of bits.  One of these
        /// values must be zero; an input AND output gate is not permitted.
        /// </summary>
        /// <param name="numInputs"></param>
        /// <param name="numOutputs"></param>
        public AbstractNumeric(int numInputs, int numOutputs) : base(numInputs, numOutputs) 
        {
            _bits = Math.Max(numInputs, numOutputs);
            if (!(numOutputs == 0 || numInputs == 0))
                throw new ArgumentException("Either numInputs or numOutputs must be 0");

            if (numOutputs != 0)
                SetName("Numeric Input");
            else
                SetName("Numeric Output");
        }

        
    }
}
