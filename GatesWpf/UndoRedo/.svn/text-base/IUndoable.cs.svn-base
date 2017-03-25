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

namespace GatesWpf.UndoRedo
{
    /// <summary>
    /// Describes an action which may be undone and/or redone
    /// </summary>
    public interface IUndoable
    {
        /// <summary>
        /// Undo this operation
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo this operation
        /// </summary>
        void Redo();

        /// <summary>
        /// Indicates if an operation may be undone
        /// </summary>
        /// <returns></returns>
        bool CanUndo();

        /// <summary>
        /// Indicates if an operation may be redone
        /// </summary>
        /// <returns></returns>
        bool CanRedo();

        /// <summary>
        /// The user-readable name of this operation
        /// </summary>
        string Name { get; }
    }
}
