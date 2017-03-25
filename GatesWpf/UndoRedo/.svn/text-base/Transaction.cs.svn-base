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
    /// A transaction is an ordered list of undoables, performed together in sequence.
    /// Undo is performed most recently added to first added; redo performed in opposite order.
    /// </summary>
    public class Transaction : List<IUndoable>, IUndoable
    {

        private bool undone = false;
        private string _name;

        public Transaction(string name)
        {
            _name = name;
        }
        #region IUndoable Members

        public void Undo()
        {
            if (!CanUndo())
                throw new InvalidOperationException();
            for (int i = this.Count - 1; i >= 0; i--)
                this[i].Undo();
            undone = true;

        }

        public void Redo()
        {
            if (!CanRedo())
                throw new InvalidOperationException();
            for (int i = 0; i < this.Count; i++)
                this[i].Redo();

            undone = false;
        }

        public bool CanUndo()
        {
            
            if (undone) return false;
            if (this.Count == 0) return true;
            return this[this.Count - 1].CanUndo();
            
        }

        public bool CanRedo()
        {
            if (!undone) return false;
            if (this.Count == 0) return true;
            return this[0].CanRedo();
            
        }

        public string Name
        {
            get { return _name; }
        }

        #endregion
    }
}
