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
    /// Indicates a gate or circuit can create a truth table version of itself
    /// with all the same functionality.
    /// </summary>
    public interface ICreateTruthTable
    {
        /// <summary>
        /// Creates a truth table equivalent to this item.
        /// </summary>
        /// <returns></returns>
        TruthTable CreateTruthTable();

        

        
    }
}
