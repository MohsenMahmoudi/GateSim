using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
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

namespace GatesWpf
{
    /// <summary>
    /// A graphical wrapper around a single gate.  Allows for the gates inputs and outputs
    /// (terminals) to be represented graphically on any of the four sides.
    /// Provides resizing and selection capabilities.
    /// </summary>
    public partial class Gate : UserControl, IEnumerable<Gate.TerminalID>
    {

        private DropShadowEffect glow;
        protected Gates.AbstractGate _gate;
        protected TerminalID[] _termsid;
        private bool _sel;
        

        /// <summary>
        /// Where a given terminal should be located.
        /// </summary>
        public enum Position
        {
            TOP, LEFT, RIGHT, BOTTOM
        }

        /// <summary>
        /// A terminal ID is a "catch-all" class which combines information
        /// about the actual gate, which input or output is involved, where it is
        /// on the visual gate, and the actual instance of the visual terminal class.
        /// </summary>
        public class TerminalID {

            /// <summary>
            /// Is this an input or output?
            /// </summary>
            public bool isInput;

            /// <summary>
            /// What is the input or output number?  This connects to the gate's
            /// input or output array.
            /// </summary>
            public int ID;

            /// <summary>
            /// Where is the terminal displayed?
            /// </summary>
            public Position pos;

            /// <summary>
            /// Actual terminal being displayed
            /// </summary>
            public Terminal t;

            /// <summary>
            /// Gate being referenced
            /// </summary>
            public Gates.AbstractGate abgate;
            public TerminalID(bool isInput, int ID, Position pos) {
                this.isInput = isInput;
                this.ID = ID;
                this.pos = pos;
                t = null;
                
                abgate = null;
            }
        }


        private Terminal AddTerminal(Grid grid, int pos, bool isInput)
        {
            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(cd);
            Terminal myt = new Terminal(isInput);
            grid.Children.Add(myt);
            Grid.SetColumn(myt, pos);
            
            return myt;
        }

        
        /// <summary>
        /// The gate "behind" this visual gate
        /// </summary>
        public Gates.AbstractGate AbGate
        {
            get
            {
                return _gate;
            }
        }

        
        /// <summary>
        /// Retrieve a terminal by index.  The total number of terminals
        /// should be the number of inputs + the number of outputs for the gate.
        /// </summary>
        /// <param name="termidx"></param>
        /// <returns></returns>
        public TerminalID this[int termidx]
        {
            get
            {
                return _termsid[termidx];
            }
        }

        /// <summary>
        /// Number of terminals.  Should be # of inputs + # of outputs for the gate.
        /// </summary>
        public int Count
        {
            get
            {
                return _termsid.Count();
            }
        }

       

        private bool _showTF = true;
        public virtual bool ShowTrueFalse
        {
            get
            {
                return _showTF;

            }
            set
            {
                _showTF = value;
                _gate_PropertyChanged(null, null);
            }
        }

        /// <summary>
        /// Find a specific terminal that represents the given input or output
        /// on the gate.  ID refers to the 0-base array index for either
        /// the input or output array.
        /// </summary>
        /// <param name="isInput"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public TerminalID FindTerminal(bool isInput, int ID)
        {
            foreach (TerminalID ti in _termsid)
            {
                if (ti.isInput == isInput && ti.ID == ID)
                    return ti;
            }
            return null;
        }


        private void _gate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            foreach (TerminalID tid in _termsid)
            {
                if (ShowTrueFalse)
                    tid.t.Value = tid.isInput ? _gate[tid.ID] : _gate.Output[tid.ID];
                else
                    tid.t.Value = false;
            }

            ToolTip = _gate.Name;   
        }

        public Gate(Gates.AbstractGate gate, TerminalID[] termsid)
        {
            InitializeComponent();
            _gate = gate;
            _termsid = termsid;

            ToolTip = _gate.Name;

            int ntop = 0, nleft = 0, nright = 0, nbottom = 0;
            for (int i = 0; i < termsid.Length; i++)
            {
                int posid;
                Grid target;
                switch (termsid[i].pos)
                {
                    case Position.TOP:
                        posid = ++ntop;
                        target = topGrid;
                        break;
                    case Position.LEFT:
                        posid = ++nleft;
                        target = leftGrid;
                        break;
                    case Position.RIGHT:
                        posid = ++nright;
                        target = rightGrid;
                        break;
                    case Position.BOTTOM:
                        posid = ++nbottom;
                        target = bottomGrid;
                        break;
                    default:
                        throw new ArgumentException("Position unspecified");
                }
                termsid[i].t = AddTerminal(target, posid, termsid[i].isInput);
                termsid[i].abgate = _gate;
            }



            //  width and height depend on # of terminals
            this.SizeChanged += new SizeChangedEventHandler(Gate_SizeChanged);
            int horz = Math.Max(ntop, nbottom);
            int vert = Math.Max(nleft, nright);
            Width = Math.Max(horz * 20, Width);
            Height = Math.Max(vert * 20, Height);
           

            glow = new DropShadowEffect();
            glow.ShadowDepth = 0;
            glow.Color = Colors.Blue;
            glow.BlurRadius = 5;

            

            //_gate.PropertyChanged += EventDispatcher.CreateDispatchedHandler(Dispatcher, _gate_PropertyChanged);
            _gate.PropertyChanged += EventDispatcher.CreateBatchDispatchedHandler(_gate, _gate_PropertyChanged);

            _gate_PropertyChanged(null, null); // manually force load of initial values

            IsReadOnly = false;
        }

        private void Gate_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            topGrid.Width = Width;
            bottomGrid.Width = Width;
            // these last two are still width because they are just rotated
            leftGrid.Width = Height;
            rightGrid.Width = Height;
            
            SetValue(ExtraWidthProperty, Width - 64.0);
            SetValue(ExtraHeightProperty, Height - 64.0);
            
        }

        /// <summary>
        /// Gets or sets if this gate is selected. Note that in this application
        /// the gate canvas also retains a selected list which is used for most
        /// selected decisions.
        /// </summary>
        public bool Selected
        {
            get
            {
                return _sel;
            }
            set
            {
                _sel = value;
                if (value)
                    Effect = glow;
                else
                    Effect = null;
            }
        }

        public virtual Gate CreateUserInstance()
        {
            return (Gate)Activator.CreateInstance(GetType());
        }

        public virtual bool IsReadOnly { get; set; }

        protected static readonly DependencyProperty ExtraWidthProperty =
            DependencyProperty.Register("ExtraWidth", typeof(double), typeof(Gate));

        protected static readonly DependencyProperty ExtraHeightProperty =
            DependencyProperty.Register("ExtraHeight", typeof(double), typeof(Gate));

       

        #region IEnumerable<TerminalID> Members

        public IEnumerator<Gate.TerminalID> GetEnumerator()
        {
            return new List<Gate.TerminalID>(_termsid).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new List<Gate.TerminalID>(_termsid).GetEnumerator();
        }

        #endregion

        
    }
}
