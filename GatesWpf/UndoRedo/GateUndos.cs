using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;


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
    /// An undo event for handling renames or other text changes.
    /// A method to change the text must be provided.
    /// </summary>
    public class ChangeUserText : IUndoable
    {
        private Gate g;
        private string oldtext, newtext;
        public delegate void SetName(string n);
        private SetName snf;

        public ChangeUserText(Gate g, string oldtext, string newtext, SetName snf)
        {
            this.g = g;
            this.oldtext = oldtext;
            this.newtext = newtext;
            this.snf = snf;
        }
        #region IUndoable Members

        public void Undo()
        {
            snf(oldtext);
        }

        public void Redo()
        {
            snf(newtext);
        }

        public bool CanUndo()
        {
            return true;
        }

        public bool CanRedo()
        {
            return true;
        }

        public string Name
        {
            get { return "Change User Text"; }
        }

        #endregion
    }

    /// <summary>
    /// An undo event handling the replacement of a gate
    /// in an ICList with a different gate.
    /// </summary>
    public class ReplaceIC : IUndoable
    {
        private ICList icl;
        private UIGates.IC oic;
        private UIGates.IC nic;
        public ReplaceIC(ICList icl, UIGates.IC oic, UIGates.IC nic)
        {
            this.icl = icl;
            this.oic = oic;
            this.nic = nic;
        }

        #region IUndoable Members

        public void Undo()
        {
            icl[icl.IndexOf(nic)] = oic;
        }

        public void Redo()
        {
            icl[icl.IndexOf(oic)] = nic;
        }

        public bool CanUndo()
        {
            return icl.Contains(nic);
        }

        public bool CanRedo()
        {
            return !icl.Contains(nic);
        }

        public string Name
        {
            get
            {
                if (nic.AbGate.Name == oic.AbGate.Name)
                {
                    return "Edit IC " + nic.AbGate.Name;
                }
                else
                {
                    return "Rename " + oic.AbGate.Name + " to " + nic.AbGate.Name;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// An undo event handling this creation of an ic and its addition to an ICList
    /// </summary>
    public class CreateIC : IUndoable
    {
        private ICList icl;
        private UIGates.IC ic;
        public CreateIC(ICList icl, UIGates.IC ic)
        {
            this.icl = icl;
            this.ic = ic;

        }
        #region IUndoable Members

        public void Undo()
        {
            icl.Remove(ic);
        }

        public void Redo()
        {
            icl.Add(ic);
        }

        public bool CanUndo()
        {
            return icl.Contains(ic);
        }

        public bool CanRedo()
        {
            return !icl.Contains(ic);
        }

        public string Name
        {
            get { return "Create IC"; }
        }

        #endregion
    }

    /// <summary>
    /// A "tracking gate" is an undo which deals with a gate that may be replaced.
    /// If the gate is replaced, the undo automatically adjusts to apply to the new gate.
    /// This is primarily used for ICs which may be replaced with essentially identical
    /// instances from time to time.
    /// </summary>
    public abstract class TrackingGate : IUndoable
    {
        protected Gate g;
        protected GateCanvas gc;

        public TrackingGate(Gate g, GateCanvas gc)
        {
            this.g = g;
            this.gc = gc;
            gc.Circuit.ReplaceGates += (sender2, e2) =>
            {
                if (e2.ContainsKey(this.g.AbGate) && e2[this.g.AbGate] != null)
                {
                    Gate og = this.g;
                    Gate ng = gc.FindGate(e2[this.g.AbGate]);
                    this.g = ng;

                }
            };
        }

        #region IUndoable Members

        public abstract void Undo();
        public abstract void Redo();
        public abstract bool CanUndo();
        public abstract bool CanRedo();
        public abstract string Name
        {
            get;
        }

        #endregion
    }

    /// <summary>
    /// An undo event dealing with a gate being moved.
    /// </summary>
    public class MoveGate : TrackingGate
    {
        
        private Point origin;
        private Point dest;
        

        public MoveGate(Gate g, GateCanvas gc, Point origin, Point dest) : base(g, gc)
        {
            
            this.origin = origin;
            this.dest = dest;
            
        }

        #region IUndoable Members

        public override void Undo()
        {
            g.Margin = new Thickness(origin.X, origin.Y, 0, 0);
            ((GateLocation)g.Tag).X = origin.X;
            ((GateLocation)g.Tag).Y = origin.Y;

            gc.UpdateLayout();
            gc.UpdateWireConnections();
        }

        public override void Redo()
        {
            g.Margin = new Thickness(dest.X, dest.Y, 0, 0);
            ((GateLocation)g.Tag).X = dest.X;
            ((GateLocation)g.Tag).Y = dest.Y;

            gc.UpdateLayout();
            gc.UpdateWireConnections();
        }

        public override bool CanUndo()
        {
            return true;
        }

        public override bool CanRedo()
        {
            return true;
        }

        public override string Name
        {
            get { return "Move Gate"; }
        }

        #endregion
    }

    /// <summary>
    /// An undo event dealing with a gate being rotated.
    /// </summary>
    public class RotateGate : TrackingGate
    {
        
        private double origin, dest;
        

        public RotateGate(Gate g, GateCanvas gc, double origin, double dest) : base(g, gc)
        {
            
            this.origin = origin;
            this.dest = dest;
            
        }

        #region IUndoable Members

        public override void Undo()
        {
            ((RotateTransform)g.RenderTransform).Angle = origin;
            ((GateLocation)g.Tag).Angle = origin;

            gc.UpdateWireConnections();
            

        }

        public override void Redo()
        {
            ((RotateTransform)g.RenderTransform).Angle = dest;
            ((GateLocation)g.Tag).Angle = dest;

            gc.UpdateWireConnections();
        }

        public override bool CanUndo()
        {
            return true;
        }

        public override bool CanRedo()
        {
            return true;
        }

        public override string Name
        {
            get { return "Rotate Gate"; }
        }

        #endregion
    }

    /// <summary>
    /// An undo event that handles changing the number of inputs in a gate.
    /// This actually assumes you are replacing the old gate with a new gate
    /// because we can't just change the number of inputs after the fact.
    /// </summary>
    public class ChangeNumInputs : IUndoable
    {
        private Gates.AbstractGate orig_gate;
        private Gates.AbstractGate new_gate;
        private Gates.Circuit c;

        public ChangeNumInputs(Gates.Circuit c, Gates.AbstractGate original, Gates.AbstractGate replacement)
        {
            this.c = c;
            this.orig_gate = original;
            this.new_gate = replacement;
        }



        #region IUndoable Members

        public void Undo()
        {
            c.ReplaceGate(new_gate, orig_gate);
        }

        public void Redo()
        {
            c.ReplaceGate(orig_gate, new_gate);
        }

        public bool CanUndo()
        {
            return (c.Contains(new_gate) && !c.Contains(orig_gate));
        }

        public bool CanRedo()
        {
            return (c.Contains(orig_gate) && !c.Contains(new_gate));
        }

        public string Name
        {
            get 
            { 
                // set name based on how the inputs change
                if (new_gate.NumberOfInputs > orig_gate.NumberOfInputs)
                    return "Add Input";
                else
                    return "Remove Input";
            }
        }

        #endregion
    }

    /// <summary>
    /// An undo event dealing with connecting a wire between two terminals.
    /// Note that this event monitors the circuit for gate replacement
    /// in a similar way to the TrackingGate series.
    /// </summary>
    public class ConnectWire : IUndoable
    {

        private Gates.Circuit c;
        private Gates.Terminal origin;
        private Gates.Terminal dest;

        public ConnectWire(Gates.Circuit c, Gates.Terminal origin, Gates.Terminal dest)
        {
            this.c = c;
            this.origin = origin;
            this.dest = dest;

            c.ReplaceGates += (sender2, e2) =>
            {
                if (e2.ContainsKey(origin.gate) && e2[origin.gate] != null)
                {
                    origin.gate = e2[origin.gate];
                }

                if (e2.ContainsKey(dest.gate) && e2[dest.gate] != null)
                {
                    dest.gate = e2[dest.gate];
                }
            };
        }

        #region IUndoable Members

        public void Undo()
        {
            c.Disconnect(dest);
        }

        public void Redo()
        {
            c[dest] = origin;
        }

        public bool CanUndo()
        {
            return (c.GetSource(dest) == origin);
        }

        public bool CanRedo()
        {
            return (c.GetSource(dest) == null);
        }

        public string Name
        {
            get { return "Connect Wire";  }
        }

        #endregion
    }

    /// <summary>
    /// An undo event for adding a new gate to the canvas.
    /// </summary>
    public class AddGate : TrackingGate
    {


        
        public AddGate(GateCanvas gc, Gate gate) : base(gate, gc)
        {
         
        }

        #region IUndoable Members

        public override void Undo()
        {
            gc.RemoveGate(g);
        }

        public override void Redo()
        {
            gc.AddGate(g, new GateLocation() {
                X = g.Margin.Left, 
                Y = g.Margin.Top, 
                Angle = ((RotateTransform)g.RenderTransform).Angle
            });
        }

        public override bool CanUndo()
        {
            return gc.Circuit.Contains(g.AbGate);
        }

        public override bool CanRedo()
        {
            return !gc.Circuit.Contains(g.AbGate);
        }

        public override string Name
        {
            get { return "Add " + g.AbGate.Name + " Gate"; }
        }

        #endregion
    }

    /// <summary>
    /// "Reverses" an undo event.  For example, disconnecting a wire may be see as
    /// the reverse of connecting a wire; or perhaps deleting a gate is the reverse
    /// of adding a gate.
    /// </summary>
    public class Reverse : IUndoable
    {
        private string name;
        private IUndoable iud;
        public Reverse(IUndoable iud, string name)
        {
            this.iud = iud;
            this.name = name;
        }

        public Reverse(IUndoable iud)
        {
            this.iud = iud;
            this.name = iud.Name;
        }
        #region IUndoable Members

        public void Undo()
        {
            iud.Redo();
        }

        public void Redo()
        {
            iud.Undo();
        }

        public bool CanUndo()
        {
            return iud.CanRedo();
        }

        public bool CanRedo()
        {
            return iud.CanUndo();
        }

        public string Name
        {
            get { return name; }
        }

        #endregion
    }
}
