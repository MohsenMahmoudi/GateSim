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

namespace Gates
{
    /// <summary>
    /// Represents a single input or output on a gate.
    /// Terminal consists of a gate and port number.  Whether this is an
    /// input or output port is not specified -- it depends on the context
    /// in which the terminal type is used.  
    /// </summary>
    public class Terminal
    {
        public int portNumber;
        public AbstractGate gate;
        public Terminal(int portNumber, AbstractGate gate)
        {
            this.portNumber = portNumber;
            this.gate = gate;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            try
            {
                return (portNumber == ((Terminal)obj).portNumber && gate == ((Terminal)obj).gate);
            }
            catch (InvalidCastException) { return false; }
        }

        public override int GetHashCode()
        {
            return portNumber + gate.GetHashCode();
        }

        public static bool operator==(Terminal a, Terminal b)
        {
            if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b, null)) return true;
            
            // if one or the other is null, but not both
            if ((Object.ReferenceEquals(a, null) ^ Object.ReferenceEquals(b, null))) return false;

            return a.Equals(b);
        }

        public static bool operator !=(Terminal a, Terminal b)
        {
            return !(a == b);
        }
    }
}
