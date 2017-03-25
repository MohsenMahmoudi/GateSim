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
using System.ComponentModel;
using Gates;


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
    /// The gate canvas contains a group of visual gates and wires which represent a circuit.
    /// Changes may be made at various levels; to the circuit directly (which then manifest visually),
    /// or to visual representations which in some cases can be transmitted to the circuit.
    /// The gate canvas is one of the center-points of this application, coordinating activity
    /// between many different objects.
    /// </summary>
    public partial class GateCanvas : UserControl
    {
        /// <summary>
        /// The name of IC being represented, if any
        /// </summary>
        public string ICName;

        /// <summary>
        /// A list of selected gates.  Must be coordinated with the Selected
        /// property on each gate.
        /// </summary>
        public BindingList<Gate> selected = new BindingList<Gate>();

        
        private ICList icl;
        private bool _ro = false;
        private bool _iso = false;
        private Gates.Circuit c;
        private Dictionary<Gates.AbstractGate, Gate> gates = new Dictionary<Gates.AbstractGate, Gate>();
        private Dictionary<Gates.Terminal, Wire> wires = new Dictionary<Gates.Terminal, Wire>();
        private Dictionary<Gates.AbstractGate, GateLocation> oldgatepositions = new Dictionary<Gates.AbstractGate, GateLocation>();
        private DragState dragging = DragState.NONE;
        private bool ReadyToSelect = false;
        private Point mp;
        private Point sp;
        private Gate.TerminalID beginTID;
        private double _zoom = 1.0;
        private UndoRedo.Transaction moves;
        private Brush oldBackground;
        private bool _mute;
        private UndoRedo.UndoManager _undman;


        private const double ANGLE_SNAP_DEG = 10;
        private const double DELTA_SNAP = 5;
        private const double GRID_SIZE = 32;

        /// <summary>
        /// All UI Gates on this canvas.
        /// </summary>
        public IEnumerable<Gate> Gates
        {
            get
            {
                return gates.Values;
            }
        }
        
        /// <summary>
        /// Provision of an undo manager allows many actions to be undone
        /// </summary>
        public UndoRedo.UndoManager UndoProvider
        {
            get
            {
                return _undman;
            }
            set
            {
                _undman = value;
                foreach (Gate uigate in gates.Values)
                {
                    if (uigate is UIGates.UserIO)
                        ((UIGates.UserIO)uigate).UndoProvider = UndoProvider;

                    if (uigate is UIGates.Comment)
                        ((UIGates.Comment)uigate).UndoProvider = UndoProvider;
                }
            }
        }

        
        /// <summary>
        /// Gets or sets the read only state of this canvas.  This is propagated
        /// to all gates within the canvas, and also affects the context menu of ICs.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _ro;
            }
            set
            {
                _ro = value;
                foreach (Gate g in gates.Values)
                {
                    g.IsReadOnly = value;
                    if (g is UIGates.IC || g.AbGate is Gates.IVariableInputs)
                        g.ContextMenu.IsEnabled = !value;

                    SetInfoLine(g);
                }

                foreach (Wire w in wires.Values)
                {
                    if (!IsReadOnly)
                        InfoLine.SetInfo(w, "Click to disconnect");
                    else
                        InfoLine.SetInfo(w, "");
                }
            }
        }

        private bool _showTF = true;
        /// <summary>
        /// Indicates if gates and wires in this circuit should display red/white based on their value,
        /// or if they should all show white regardless of value.
        /// </summary>
        public bool ShowTrueFalse
        {
            get
            {
                return _showTF;
            }
            set
            {
                _showTF = value;
                foreach (Gate g in gates.Values)
                    g.ShowTrueFalse = value;
                foreach (ConnectedWire cw in wires.Values)
                    cw.ShowTrueFalse = value;

            }
        }

        private bool _endUser = false;
        public bool EndUserMode
        {
            get
            {
                return _endUser;
            }
            set
            {
                _endUser = value;
                ClearSelection();

                List<UIElement> toFade = new List<UIElement>();

                var fadeGates = gates.Values.GroupBy(g => (g is UIGates.UserIO || g is UIGates.Numeric || g is UIGates.Clock || g is UIGates.Comment));
                
                // all gates that are not i/o
                try
                {
                    toFade.AddRange(fadeGates.Where(fg => !fg.Key).Single().ToList());
                }
                catch (Exception) { } // exception occurs if all gates are i/o (that is, there is no false group).  this is weird, but not bad

                // all wires
                toFade.AddRange(wires.Values);

                // all terminals on gates that ARE i/o
                try
                {
                    var uigates = fadeGates.Where(fg => fg.Key).Single();
                    var uig_terms = uigates.SelectMany(g => g.Select(tid => tid.t));
                    toFade.AddRange(uig_terms);
                }
                catch (Exception) { } // exception occurs if no i/o gates exist.  this  is VERY weird (for this case), but again, hey, whatever you want bud

                Fader.AnimateOpacity(value ? 0 : 1, toFade.ToArray());


            }
        }

        #region ICL
        /// <summary>
        /// Provide an IC List for this canvas to work with.
        /// After this value is set, consider called SetCaptureICLChanges
        /// </summary>
        public ICList ICL
        {
            set
            {
                icl.ChangeIC -= new ICList.ChangeICEventHandler(icl_ChangeIC);
                icl = value;
                //icl.ChangeIC += new ICList.ChangeICEventHandler(icl_ChangeIC);
            }
        }

        /// <summary>
        /// If any IC is replaced or changed in the ICL, the gate canvas should
        /// replace its visual representations with the new representation.
        /// Call this method to enable catching the ChangeIC event from the given icl.
        /// </summary>
        public void SetCaptureICLChanges()
        {
            icl.ChangeIC += new ICList.ChangeICEventHandler(icl_ChangeIC);
            
        }

        private void icl_ChangeIC(object sender, ICList.ChangeICEventArgs e)
        {
            
            if (e.newic == null)
                c.ReplaceICs(e.original.AbGate.Name, null);
            else
                c.ReplaceICs(e.original.AbGate.Name, (Gates.IC)e.newic.AbGate);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// An empty canvas based on a new circuit and iclist.  Not too useful.
        /// </summary>
        public GateCanvas() : this(new Gates.Circuit(), new ICList()) { }

        /// <summary>
        /// Construct a gate canvas to represent a given IC.  The location hints
        /// within the IC are used to create and position visual gates equivalent to the
        /// circuit in the IC.
        /// </summary>
        /// <param name="ic"></param>
        /// <param name="icl"></param>
        public GateCanvas(UIGates.IC ic, ICList icl)
            : this(((Gates.IC)ic.AbGate).Circuit, icl) 
        {
            ICName = ic.AbGate.Name;

            foreach (KeyValuePair<Gates.AbstractGate, GateLocation> gp in ic.locationHints)
            {
                if (gp.Key is Gates.IC)
                {
                    // must get terminal id template
                    UIGates.IC templateic = icl.GetIC(gp.Key.Name);
                    
                    AddGate(UIGates.IC.CreateFromTemplate( (Gates.IC)gp.Key, templateic), gp.Value);
                } else
                AddGate(gp.Key, gp.Value);
            }
           // this.Loaded += ((sender, e) =>
            {
                foreach (KeyValuePair<Gates.AbstractGate, GateLocation> gp in ic.locationHints)
                {
                    for (int i = 0; i < gp.Key.NumberOfInputs; i++)
                    {
                        Gates.Terminal inp = new Gates.Terminal(i, gp.Key);
                        if (c.GetSource(inp) != null)
                        {
                            c_CircuitConnection(c, inp,
                                c.GetSource(inp));
                        }
                    }
                }
            }//);

            this.Loaded += (sender, e) => { UpdateWireConnections(); };
           
           
        }

        /// <summary>
        /// Wire up all event notifications for circuit changes.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="icl"></param>
        protected GateCanvas(Gates.Circuit c, ICList icl)
        {

            InitializeComponent();
            this.icl = icl;
            this.c = c;
            c.CircuitConnection += new Gates.Circuit.CircuitConnectionEventHandler(c_CircuitConnection);
            c.ListChanged += new System.ComponentModel.ListChangedEventHandler(c_ListChanged);
            c.ReplaceGates += new Gates.Circuit.ReplaceGatesEventHandler(c_ReplaceGates);
        }

        #endregion

        /// <summary>
        /// Given a circuit gate, find the corresponding visual gate in this canvas
        /// </summary>
        /// <param name="abGate"></param>
        /// <returns></returns>
        public Gate FindGate(Gates.AbstractGate abGate)
        {
            return gates[abGate];
        }

        /// <summary>
        /// When dragging the mouse, we could be working a selection, or connecting
        /// a wire in either direction.  None is useful for when not dragging.
        /// </summary>
        protected enum DragState
        {
            NONE, MOVE, CONNECT_TO, CONNECT_FROM
        }
        

        


        /// <summary>
        /// Update all wires to ensure they are properly laid out and visually
        /// connect to their appropriate terminals
        /// </summary>
        public void UpdateWireConnections()
        {
            // how do we figure out which wires are connected to this one?
            // input is easy but output is hard
            // maybe just brute force update
            // and if perf. becomes an issue we'll keep a list
            foreach (KeyValuePair<Gates.Terminal, Wire> wire in wires)
                ((ConnectedWire)(wire.Value)).Connect();
        }


        #region Circuit Events


        private void c_ReplaceGates(Gates.Circuit sender, Dictionary<Gates.AbstractGate, Gates.AbstractGate> replacements)
        {
            foreach (KeyValuePair<Gates.AbstractGate, Gates.AbstractGate> replacement in replacements)
            {
                if (oldgatepositions.ContainsKey(replacement.Key) && replacement.Value != null)
                {
                    GateLocation np = oldgatepositions[replacement.Key];
                    gates[replacement.Value].Margin = new Thickness(np.X, np.Y, 0, 0);
                    ((RotateTransform)gates[replacement.Value].RenderTransform).Angle = np.Angle;
                    ((GateLocation)gates[replacement.Value].Tag).X = np.X;
                    ((GateLocation)gates[replacement.Value].Tag).Y = np.Y;
                    ((GateLocation)gates[replacement.Value].Tag).Angle = np.Angle;
                }
            }
            foreach (Gate tic in gates.Values)
            {
                if (tic is UIGates.IC)
                    ((UIGates.IC)tic).UpdateLocationHints(replacements);
            }

            UpdateLayout();
            // otherwise the wires don't know where to go
            UpdateWireConnections();

        }

        private void c_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemAdded)
            {
                if (!gates.ContainsKey(c[e.NewIndex]))
                    AddGate(c[e.NewIndex], new GateLocation());
            }
            if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted)
            {
                List<Gates.AbstractGate> toRemove = new List<Gates.AbstractGate>();
                // find and remove from canvas any removed gates
                foreach (KeyValuePair<Gates.AbstractGate, Gate> abg in gates)
                {
                    if (!c.Contains(abg.Key))
                    {
                        GC.Children.Remove(abg.Value);
                        toRemove.Add(abg.Key);
                        oldgatepositions[abg.Key] = new GateLocation()
                        {
                            X = abg.Value.Margin.Left,
                            Y = abg.Value.Margin.Top,
                            Angle = ((RotateTransform)abg.Value.RenderTransform).Angle
                        };
                    }
                }

                foreach (Gates.AbstractGate ab in toRemove)
                    gates.Remove(ab);

                // find and remove any stray wires (although should be none)
                List<Gates.Terminal> toRemoveW = new List<Gates.Terminal>();

                foreach (KeyValuePair<Gates.Terminal, Wire> wire in wires)
                {
                    if (!c.Contains(wire.Key.gate))
                    {
                        toRemoveW.Add(wire.Key);
                    }
                }

                foreach (Gates.Terminal t in toRemoveW)
                    wires.Remove(t);

            }
        }


        private void c_CircuitConnection(Gates.Circuit sender, Gates.Terminal input, Gates.Terminal output)
        {
            Wire cw = wires.ContainsKey(input) ? wires[input] : null;
            if (cw != null)
            {
                GC.Children.Remove(cw);
                wires.Remove(input);
            }
            if (output != null)
            {
                Gate inp = gates[input.gate];
                Gate outp = gates[output.gate];
                ConnectedWire ncw = new ConnectedWire(output.gate, outp.FindTerminal(false, output.portNumber),
                    input.gate, inp.FindTerminal(true, input.portNumber));
                ncw.Value = output.gate.Output[output.portNumber];
                ncw.ShowTrueFalse = ShowTrueFalse;
                wires[input] = ncw;
                GC.Children.Insert(0, ncw);
                ncw.Connect();

                ncw.MouseDown += (snd, e) =>
                    {
                        if (!IsReadOnly)
                        {
                            
                            Gates.Terminal t = c.GetSource(input); 
                            c.Disconnect(input);

                            if (UndoProvider != null)
                                UndoProvider.Add(new UndoRedo.Reverse(new UndoRedo.ConnectWire(c, t, input), "Disconnect Wire"));
                            
                        }
                    };

                if (!IsReadOnly)
                    InfoLine.SetInfo(ncw, "Click to disconnect");
            }


        }
        #endregion

        /// <summary>
        /// Retrieve the circuit which backs this canvas
        /// </summary>
        public Gates.Circuit Circuit
        {
            get
            {
                return c;
            }
        }

        private void SetInfoLine(Gate g)
        {
            if (_ro)
            {
                InfoLine.SetInfo(g, "");

                

            } 
            else
            {
                string infoline = "Left-drag to move, right-drag to rotate, left-drag terminals to connect";

                if (g is UIGates.IC)
                    infoline += ", double-click to view, right-click for options";

                if (g is UIGates.UserIO)
                    infoline += ", right-click to rename";

                if (g is UIGates.Numeric)
                    infoline += ", click on the representation to change";

                if (g is UIGates.Numeric || g is UIGates.Clock || g is UIGates.Comment)
                    infoline += ", click and type to enter a value";

                if (g.AbGate is Gates.IVariableInputs)
                {
                    // can only remove if more than 2
                    infoline += ", right-click to add" + (g.AbGate.NumberOfInputs > 2 ? "/remove" : "") + " inputs";
                }

                InfoLine.SetInfo(g, infoline);

                
            }
        }

        #region Add and Remove Gates

        /// <summary>
        /// Delete all those gates in the selected list.  This returns an
        /// undo transaction which can be added to an undo manager,
        /// or folded into a larger transaction.
        /// </summary>
        /// <returns></returns>
        public UndoRedo.IUndoable DeleteSelectedGates()
        {
            if (selected.Count == 0)
                return new UndoRedo.Transaction("Delete 0 gates");

            UndoRedo.Transaction del = new UndoRedo.Transaction("Delete " +
                    (selected.Count == 1 ? "gate" : (selected.Count.ToString() + " gates")));
            
                foreach (Gate g in selected)
                {
                    // have to remember all its connections
                    
                    // check all this gate's inputs
                    for (int i = 0; i < g.AbGate.NumberOfInputs; i++)
                    {
                        Gates.Terminal t = c.GetSource(new Gates.Terminal(i, g.AbGate));
                        if (t != null)
                        {
                            del.Add(new UndoRedo.Reverse(new UndoRedo.ConnectWire(c, t, new Gates.Terminal(i, g.AbGate))));
                        }
                    }

                    // and all outputs, BUT
                    // avoid getting in trouble with recursive connections
                    for (int i = 0; i < g.AbGate.Output.Length; i++) 
                    {
                        List<Gates.Terminal> outs = c.GetTargets(new Gates.Terminal(i, g.AbGate));
                        foreach (Gates.Terminal t in outs)
                        {
                            if (g.AbGate != t.gate)
                                del.Add(new UndoRedo.Reverse(new UndoRedo.ConnectWire(c, new Gates.Terminal(i, g.AbGate), t)));
                        }
                    }

                    del.Add(new UndoRedo.Reverse(new UndoRedo.AddGate(this, g)));
                    RemoveGate(g);
                    
                }

                oldgatepositions.Clear(); // just used for gate replacement
                ClearSelection();
                return del;
        }

        /// <summary>
        /// Removes a given gate from the canvas and the circuit.
        /// No undo event created.
        /// </summary>
        /// <param name="uigate"></param>
        public void RemoveGate(Gate uigate)
        {
            c.Remove(uigate.AbGate);
            uigate.MouseDown -= new MouseButtonEventHandler(uigate_MouseDown);
            uigate.MouseUp -= new MouseButtonEventHandler(uigate_MouseUp);

            if (uigate is UIGates.IC)
                uigate.MouseDoubleClick -= new MouseButtonEventHandler(uigate_MouseDoubleClick);


        }

        
        /// <summary>
        /// Add a pre-existing visual gate at a particular location.  The gate will also
        /// be added to the circuit if it is not already a member.
        /// </summary>
        /// <param name="uigate"></param>
        /// <param name="pos"></param>
        public void AddGate(Gate uigate, GateLocation pos)
        {

            ClearSelection();
            Gates.AbstractGate gate = uigate.AbGate;

            gates[gate] = uigate;
            uigate.Margin = new Thickness(pos.X, pos.Y, 0, 0);
            GC.Children.Add(uigate);
            if (!c.Contains(gate))
                c.Add(gate);

            uigate.RenderTransform = new RotateTransform(pos.Angle, uigate.Width / 2.0, uigate.Height / 2.0);
            uigate.Tag = new GateLocation() { X = pos.X, Y = pos.Y, Angle = pos.Angle };
            uigate.ShowTrueFalse = ShowTrueFalse;

            // NOTE that we need a separate angle and transform
            // for the snap-to to work properly
            // so I am using the tag to store the angle
            
            uigate.MouseDown += new MouseButtonEventHandler(uigate_MouseDown);
            uigate.MouseUp += new MouseButtonEventHandler(uigate_MouseUp);

            if (uigate.ContextMenu == null)
                uigate.ContextMenu = new ContextMenu();

            if (uigate is UIGates.IC)
            {
                uigate.MouseDoubleClick += new MouseButtonEventHandler(uigate_MouseDoubleClick);
                MenuItem inline = new MenuItem();
                inline.Header = "Inline Circuit";
                inline.Tag = uigate;
                uigate.ContextMenu.Items.Add(inline);
                inline.Click += new RoutedEventHandler(inline_Click);
                uigate.ContextMenu.IsEnabled = !IsReadOnly;
            }

            // can add inputs
            if (uigate.AbGate is Gates.IVariableInputs)
            {
                MenuItem addInput = new MenuItem();
                addInput.Header = "Add Input";
                addInput.Tag = uigate;
                uigate.ContextMenu.Items.Add(addInput);
                addInput.Click += (sender2, e2) =>
                {
                    Gates.AbstractGate newgate = ((Gates.IVariableInputs)uigate.AbGate).Clone(uigate.AbGate.NumberOfInputs + 1);
                    c.ReplaceGate(uigate.AbGate, newgate);
                    if (UndoProvider != null)
                        UndoProvider.Add(new UndoRedo.ChangeNumInputs(c, uigate.AbGate, newgate));

                };
                if (uigate.AbGate.NumberOfInputs > 2)
                {
                    MenuItem removeInput = new MenuItem();
                    removeInput.Header = "Remove Input";
                    removeInput.Tag = uigate;
                    uigate.ContextMenu.Items.Add(removeInput);
                    removeInput.Click += (sender2, e2) =>
                    {
                        UndoRedo.Transaction removeInp = new GatesWpf.UndoRedo.Transaction("Remove Input");

                        // remember wires connected to removed input
                        Gates.Terminal dest = new Gates.Terminal(uigate.AbGate.NumberOfInputs - 1, uigate.AbGate);
                        Gates.Terminal origin = c.GetSource(dest);
                        if (origin != null)
                            removeInp.Add(new UndoRedo.Reverse(new UndoRedo.ConnectWire(c, origin, dest)));

                        Gates.AbstractGate newgate = ((Gates.IVariableInputs)uigate.AbGate).Clone(uigate.AbGate.NumberOfInputs - 1);
                        c.ReplaceGate(uigate.AbGate, newgate);
                        
                        removeInp.Add(new UndoRedo.ChangeNumInputs(c, uigate.AbGate, newgate));
                        if (UndoProvider != null)
                            UndoProvider.Add(removeInp);


                    };
                }

                uigate.ContextMenu.IsEnabled = !IsReadOnly;
            }

            if (uigate is UIGates.UserIO)
                ((UIGates.UserIO)uigate).UndoProvider = UndoProvider;

            if (uigate is UIGates.Comment)
            {
                ((UIGates.Comment)uigate).UndoProvider = UndoProvider;
                ((UIGates.Comment)uigate).MyGateCanvas = this;
            }


            //Green team:
            // added if statement to set property of MyGateCanvas
            if (uigate is UIGates.Numeric)
                ((UIGates.Numeric)uigate).MyGateCanvas = this;

            if (uigate is UIGates.Clock)
                ((UIGates.Clock)uigate).MyGateCanvas = this;


            MenuItem isolate = new MenuItem();
            isolate.Header = "Isolate";
            
            uigate.ContextMenu.Items.Add(isolate);
            isolate.Click += (sender2, e2) =>
            {
                List<AbstractGate> backGateList = new List<AbstractGate>();

                foreach (KeyValuePair<AbstractGate, Gate> pair in gates)  
                {  
                     Gate value = pair.Value;
                     Fader.AnimateOpacity(.2, value);
                }
                foreach (ConnectedWire cw in wires.Values)
                {
                    Fader.AnimateOpacity(.2, cw);
                }
                IsolateLoop(uigate.AbGate, ref backGateList);
                _iso = true;
            };
            SetInfoLine(uigate);  
        }

        public void IsolateLoop(AbstractGate gate, ref List<AbstractGate> list)
        {
            if (list.Contains(gate))
            {
                return;
            }
            list.Add(gate);

            Fader.AnimateOpacity(1, gates[gate]);
            foreach (ConnectedWire cw in wires.Values)
            {
                if (cw.DestinationGate.Equals(gate))
                {
                    Fader.AnimateOpacity(1, cw);
                    IsolateLoop(cw.OriginGate, ref list);
                }
            }
        }

        /// <summary>
        /// Add all gates from a given canvas into our canvas at the selected offset.
        /// A clone of each gate will be made, both at the visual and internal level.
        /// A map will be produced indicated the relationship between the gates in the original
        /// canvas and the gates added to this canvas.
        /// </summary>
        /// <param name="icgc"></param>
        /// <param name="offset"></param>
        /// <param name="gatemap"></param>
        /// <returns></returns>
        protected UndoRedo.Transaction AddGates(GateCanvas icgc, Point offset, out Dictionary<Gate, Gate> gatemap)
        {
            UndoRedo.Transaction addgates = new UndoRedo.Transaction("Add Gates");
            gatemap = new Dictionary<Gate, Gate>();

            // step 1. bring the circuit in
            foreach (Gate g in icgc.gates.Values)
            {
                gatemap[g] = g.CreateUserInstance();
                AddGate(gatemap[g], new GateLocation() {
                    X = offset.X + g.Margin.Left, 
                    Y = g.Margin.Top + offset.Y, 
                    Angle = ((RotateTransform)g.RenderTransform).Angle
                });
                addgates.Add(new UndoRedo.AddGate(this, gatemap[g]));
            }

            // step 2. connect internal wiring
            foreach (ConnectedWire cw in icgc.wires.Values)
            {
                Gates.Terminal target = new Gates.Terminal(cw.DestTerminalID.ID, gatemap[icgc.FindGate(cw.DestinationGate)].AbGate);
                Gates.Terminal source = new Gates.Terminal(cw.OriginTerminalID.ID, gatemap[icgc.FindGate(cw.OriginGate)].AbGate);
                c[target] = source;
                addgates.Add(new UndoRedo.ConnectWire(c, source, target));
            }

            return addgates;
        }

        /// <summary>
        /// Add a gate to this canvas.  An appropriate visual representation will be created.
        /// </summary>
        /// <param name="gate"></param>
        /// <param name="pos"></param>
        public void AddGate(Gates.AbstractGate gate, GateLocation pos)
        {
            // maybe we could use extension methods
            // to add a method to create a UIGate for each AbstractGate?

            Gate uigate;
            if (gate is Gates.BasicGates.And)
            {
                uigate = new UIGates.And((Gates.BasicGates.And)gate);
            }
            else if (gate is Gates.BasicGates.Not)
            {
                uigate = new UIGates.Not((Gates.BasicGates.Not)gate);
            }
            else if (gate is Gates.BasicGates.Or)
            {
                uigate = new UIGates.Or((Gates.BasicGates.Or)gate);
            }
            else if (gate is Gates.BasicGates.Nand)
            {
                uigate = new UIGates.Nand((Gates.BasicGates.Nand)gate);
            }
            else if (gate is Gates.BasicGates.Nor)
            {
                uigate = new UIGates.Nor((Gates.BasicGates.Nor)gate);
            }
            else if (gate is Gates.BasicGates.Xor)
            {
                uigate = new UIGates.Xor((Gates.BasicGates.Xor)gate);
            }
            else if (gate is Gates.BasicGates.Xnor)
            {
                uigate = new UIGates.Xnor((Gates.BasicGates.Xnor)gate);
            }
            else if (gate is Gates.BasicGates.Buffer)
            {
                uigate = new UIGates.Buffer((Gates.BasicGates.Buffer)gate);
            }
            else if (gate is Gates.IOGates.UserInput)
            {
                uigate = new UIGates.UserInput((Gates.IOGates.UserInput)gate);
            }
            else if (gate is Gates.IOGates.UserOutput)
            {
                uigate = new UIGates.UserOutput((Gates.IOGates.UserOutput)gate);
            }
            else if (gate is Gates.IOGates.AbstractNumeric)
            {
                uigate = new UIGates.Numeric((Gates.IOGates.AbstractNumeric)gate);
            }
            else if (gate is Gates.IOGates.Clock)
            {
                uigate = new UIGates.Clock((Gates.IOGates.Clock)gate);
            }
            else if (gate is Gates.IOGates.Comment)
            {
                uigate = new UIGates.Comment((Gates.IOGates.Comment)gate);
            }
            else if (gate is Gates.IC)
            {
                uigate = UIGates.IC.CreateFromTemplate((Gates.IC)gate, icl.GetIC(gate.Name));
            }
            else throw new ArgumentException("gate not of known subclass");

            AddGate(uigate, pos);
        }
        #endregion

        /// <summary>
        /// Recursively flatten this circuit by replacing all ICs within it
        /// with their contents.  Repeat until no ICs remain.
        /// </summary>
        public void Flatten()
        {
            Queue<UIGates.IC> toFlatten = new Queue<GatesWpf.UIGates.IC>();
            UndoRedo.Transaction flatten = new UndoRedo.Transaction("Flatten All");
            do
            {
                while (toFlatten.Count > 0)
                    flatten.Add(Flatten(toFlatten.Dequeue()));
                
                foreach (Gate g in gates.Values)
                {
                    if (g is UIGates.IC)
                    {
                        toFlatten.Enqueue(g as UIGates.IC);
                    }
                }

            } while (toFlatten.Count > 0);


            ClearSelection();
            UpdateLayout();
            UpdateWireConnections();

            if (UndoProvider != null)
                UndoProvider.Add(flatten);

        }

        private UndoRedo.IUndoable Flatten(UIGates.IC ic)
        {
            GateCanvas icgc = new GateCanvas((UIGates.IC)ic, icl);


            UndoRedo.Transaction undo_inline = new UndoRedo.Transaction("Inline Circuit");
            // step 1. make room for the circuit
            // NOTE: exclude user i/o gates
            // because these will be removed anyways!
            Rect bounds = GetBounds(icgc.gates.Values.Where(g => !(g is UIGates.UserIO)), 0);
            
           
            foreach (Gate g in gates.Values)
            {
                if (g.Margin.Left > ic.Margin.Left || g.Margin.Top > ic.Margin.Top)
                {
                    Point origin = new Point(g.Margin.Left, g.Margin.Top);
                    double left = g.Margin.Left;
                    double top = g.Margin.Top;
                    if (g.Margin.Left > ic.Margin.Left && bounds.Width - ic.Width > 0)
                        left += bounds.Width - ic.Width;

                    if (g.Margin.Top > ic.Margin.Top && bounds.Height - ic.Height > 0)
                        top += bounds.Height - ic.Height;

                    g.Margin = new Thickness(left, top, 0, 0);
                    ((GateLocation)g.Tag).X = left;
                    ((GateLocation)g.Tag).Y = top;
                    undo_inline.Add(new UndoRedo.MoveGate(g, this, origin, new Point(g.Margin.Left, g.Margin.Top)));
                }

            }

            // steps 2 and 3.
            // bring in circuit and connect internal wiring
            Dictionary<Gate, Gate> newgates; // = new Dictionary<Gate, Gate>();
            undo_inline.Add(AddGates(icgc, new Point(-bounds.Left + ic.Margin.Left, -bounds.Top + ic.Margin.Top), out newgates));


            // step 4. connect external wiring 
            Gates.IC gic = ic.AbGate as Gates.IC;
            // step 4a. connect inputs
            for (int i = 0; i < gic.NumberOfInputs; i++)
            {
                // for each input, find out which circuits within the ic
                // it is connected to
                List<Gates.Terminal> targets = gic.Circuit.GetTargets(new Gates.Terminal(0, gic.Inputs[i]));

                // and this particular ic input, what supplies it from outside the ic?
                Gates.Terminal source = c.GetSource(new Gates.Terminal(i, gic));

                // then disconnect those inputs
                // and connect it them from the outer
                foreach (Gates.Terminal t in targets)
                {
                    Gates.Terminal tt = new Gates.Terminal(t.portNumber, newgates[icgc.FindGate(t.gate)].AbGate);
                    undo_inline.Add(new UndoRedo.Reverse(new UndoRedo.ConnectWire(c, c.GetSource(tt), tt)));
                    c.Disconnect(tt);

                    if (source != null)
                    {
                        c[tt] = source;
                        undo_inline.Add(new UndoRedo.ConnectWire(c, source, tt));
                    }
                }

            }
            // step 4b. connect outputs
            for (int i = 0; i < gic.Output.Length; i++)
            {
                // for each output, find out which circuit within the ic
                // it comes from
                Gates.Terminal source = gic.Circuit.GetSource(new Gates.Terminal(0, gic.Outputs[i]));

                // translate into our circuit
                if (source != null)
                {
                    source = new Gates.Terminal(source.portNumber, newgates[icgc.FindGate(source.gate)].AbGate);
                }

                // and this particular output supplies which sources in our circuit?
                List<Gates.Terminal> targets = c.GetTargets(new Gates.Terminal(i, gic));

                // then disconnect those inputs
                // and connect it them from the outer
                foreach (Gates.Terminal t in targets)
                {
                    undo_inline.Add(new UndoRedo.Reverse(new UndoRedo.ConnectWire(c, c.GetSource(t), t)));
                    c.Disconnect(t);

                    if (source != null)
                    {
                        c[t] = source;
                        undo_inline.Add(new UndoRedo.ConnectWire(c, source, t));
                    }
                }

            }


            // step 5. delete user i/o and ic
            ClearSelection();
            for (int i = 0; i < gic.NumberOfInputs; i++)
            {
                selected.Add(newgates[icgc.FindGate(gic.Inputs[i])]);
            }
            for (int i = 0; i < gic.Outputs.Length; i++)
            {
                selected.Add(newgates[icgc.FindGate(gic.Outputs[i])]);
            }
            selected.Add(ic);
            undo_inline.Add(DeleteSelectedGates());

            // step 6. set selection to newly inlined gates
            ClearSelection();
            foreach (Gate g in newgates.Values)
            {
                if (!(g is UIGates.UserIO))
                {
                    selected.Add(g);
                    g.Selected = true;
                }
            }

            return undo_inline;
        }

        private void inline_Click(object sender, RoutedEventArgs e)
        {
            MenuItem inline = (MenuItem)sender;
            UIGates.IC ic = (UIGates.IC)inline.Tag;

            UndoRedo.IUndoable undo_inline = Flatten(ic);

            UpdateLayout();
            UpdateWireConnections();

            if (UndoProvider != null)
                UndoProvider.Add(undo_inline);
            e.Handled = true;
        }


        private static Rect GetBounds(IEnumerable<Gate> gts, double padding)
        {
            double minx = 0, maxx = 0, miny = 0, maxy = 0;
            bool fst = true;
            foreach (Gate g in gts)
            {
                if (fst)
                {
                    minx = g.Margin.Left;
                    maxx = g.Margin.Left + g.Width;
                    miny = g.Margin.Top;
                    maxy = g.Margin.Top + g.Height;
                    fst = false;
                }

                minx = Math.Min(minx, g.Margin.Left - padding);
                maxx = Math.Max(maxx, g.Margin.Left + g.Width + padding);
                miny = Math.Min(miny, g.Margin.Top - padding);
                maxy = Math.Max(maxy, g.Margin.Top + g.Height + padding);
            }

            return new Rect(minx, miny, maxx - minx, maxy - miny);
        }

        /// <summary>
        /// Determine the extent of visual gates on this canvas, taking into account
        /// any requested padding, and whether all gates should be considered or
        /// only selected gates.
        /// </summary>
        /// <param name="padding"></param>
        /// <param name="selectedOnly"></param>
        /// <returns></returns>
        public Rect GetBounds(double padding, bool selectedOnly)
        {
            if (selectedOnly)
                return GetBounds(selected, padding);
            else
                return GetBounds(gates.Values, padding);
        }

        /// <summary>
        /// Copies the contents of the given IC into this canvas at a slight offset
        /// from its original location hinted positions.  This creates an undo action.
        /// </summary>
        /// <param name="ic"></param>
        public void PasteIC(UIGates.IC ic)
        {
            // +32 so they don't appear right on top of the originals
            Point offset = new Point(32, 32);
            Dictionary<Gate, Gate> blah;
            
            ClearSelection();
            UndoRedo.IUndoable paste = AddGates(new GateCanvas(ic, icl), offset, out blah);
            foreach (Gate g in blah.Values)
            {
                g.Selected = true;
                selected.Add(g);
            }

            if (UndoProvider != null)
            {
                // use a transaction as a wrapper to change the name
                // to paste instead of add
                UndoRedo.Transaction pt = new UndoRedo.Transaction("Paste " + paste.Name.Substring(4)); // 4 is "Add "
                pt.Add(paste);
                UndoProvider.Add(pt);
            }

            UpdateLayout();
            UpdateWireConnections();

        }

        
        /// <summary>
        /// Moves all gates so that the upper edge of the bounds is along the top,
        /// and the left edge of the bounds is along the left.
        /// </summary>
        public void AlignUpperLeft()
        {
            Rect r = GetBounds(0, false);
            UndoRedo.Transaction align = new UndoRedo.Transaction("Align Gates to Upper-Left"); ;

            foreach (Gate g in gates.Values)
            {
                Point origin = new Point(g.Margin.Left, g.Margin.Top);
                Point dest = new Point(g.Margin.Left - r.Left, g.Margin.Top - r.Top);
                g.Margin = new Thickness(dest.X, dest.Y, 0, 0);
                ((GateLocation)g.Tag).X = dest.X;
                ((GateLocation)g.Tag).Y = dest.Y;
                UndoRedo.MoveGate mg = new UndoRedo.MoveGate(g, this, origin, dest);
                align.Add(mg);
            }
            if (UndoProvider != null)
                UndoProvider.Add(align);

            UpdateLayout();
            UpdateWireConnections();
        }

        #region Gate and Canvas Mouse Events
        private void uigate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Window1 icw = new Window1((UIGates.IC)sender, icl, Window1.EditLevel.VIEW);
            
            icw.Show();
            Gates.AbstractGate org = ((UIGates.IC)sender).AbGate;
            c.ReplaceGates += (s2, e2) =>
            {
                if (e2.ContainsKey(org))
                {
                    if (e2[org] == null)
                    {
                        icw.Close();
                    }
                    else
                    {
                        icw.RefreshGateCanvas((UIGates.IC)gates[e2[org]]);
                        org = e2[org];
                    }
                }
            };
            
            
            
        }

        //mouse up event for gate clicks
        private void uigate_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (dragging == DragState.CONNECT_FROM ||
                dragging == DragState.CONNECT_TO)
            {
                foreach (Gate.TerminalID tid in (Gate)sender)
                {
                    if (tid.t.IsMouseOver)
                    {
                        Gates.Terminal origin = null, dest = null;

                        if (tid.isInput && dragging == DragState.CONNECT_FROM &&
                            !wires.ContainsKey(new Gates.Terminal(tid.ID, tid.abgate)))
                        {
                            origin = new Gates.Terminal(beginTID.ID, beginTID.abgate);
                            dest = new Gates.Terminal(tid.ID, tid.abgate);
                        }


                        if (!tid.isInput && dragging == DragState.CONNECT_TO)
                        {
                            origin = new Gates.Terminal(tid.ID, tid.abgate);
                            dest = new Gates.Terminal(beginTID.ID, beginTID.abgate);
                                
                        }

                        if (origin != null)
                        {
                            c[dest] = origin;
                            UndoRedo.ConnectWire cw = new UndoRedo.ConnectWire(c, origin, dest);
                            if (UndoProvider != null)
                                UndoProvider.Add(cw);
                        }

                    }
                }
            }

        }

        //mouse up event for canvas clicks
        private void GateCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragging = DragState.NONE;
            dragSelect.Width = 0;
            dragSelect.Height = 0;
            dragSelect.Margin = new Thickness(0, 0, 0, 0);
            dragSelect.Visibility = Visibility.Hidden;

            dragWire.Destination = new Point(0, 0);
            dragWire.Origin = new Point(0, 0);

            // unhightlight all
            foreach (Gates.AbstractGate ag in gates.Keys)
            {
                
                for (int i = 0; i < ag.Output.Length; i++)
                {
                    gates[ag].FindTerminal(false, i).t.Highlight = false;
                }

                for (int i = 0; i < ag.NumberOfInputs; i++)
                {
                    gates[ag].FindTerminal(true, i).t.Highlight = false;
                }
            }

            if (UndoProvider != null && moves != null && moves.Count > 0)
                UndoProvider.Add(moves);
            moves = null;

            ReadyToSelect = false;
        }

        //mouse down event for canvas clicks
        private void GateCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // only come here if the uigate doesn't handle it
            ClearSelection();

            // prepare for a dragging selection
            Point mp2 = e.GetPosition(GC);
            sp = new Point(mp2.X, mp2.Y);

            ReadyToSelect = true;
            if (_iso)
            {
                _iso = false;
                foreach (KeyValuePair<AbstractGate, Gate> pair in gates)
                {
                    Gate value = pair.Value;
                    Fader.AnimateOpacity(1, value);
                }
                foreach (ConnectedWire cw in wires.Values)
                {
                    Fader.AnimateOpacity(1, cw);
                }
            }
        }

        //mouse move event for canvas clicks
        private void GateCanvas_MouseMove(object sender, MouseEventArgs e)
        {

            if (IsReadOnly || EndUserMode)
                return;

            Point mp2 = e.GetPosition(GC);

            if (e.LeftButton == MouseButtonState.Pressed ||
                e.RightButton == MouseButtonState.Pressed)
            {
                GC.BringIntoView(new Rect(new Point(mp2.X - 10, mp2.Y - 10), 
                    new Point(mp2.X + 10, mp2.Y + 10)));

                switch (dragging)
                {
                    case DragState.CONNECT_FROM:
                        dragWire.Destination = mp2;
                        break;
                    case DragState.CONNECT_TO:
                        dragWire.Origin = mp2;
                        break;


                    case DragState.MOVE:

                        foreach (Gate g in selected)
                        {

                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                                double dx = mp2.X - mp.X;
                                double dy = mp2.Y - mp.Y;
                                ((GateLocation)g.Tag).X = ((GateLocation)g.Tag).X + dx;
                                ((GateLocation)g.Tag).Y = ((GateLocation)g.Tag).Y + dy;
                                double cx = ((GateLocation)g.Tag).X % GRID_SIZE;
                                double cy = ((GateLocation)g.Tag).Y % GRID_SIZE;

                                Point op = new Point(g.Margin.Left, g.Margin.Top);

                                if ((Math.Abs(cx) < DELTA_SNAP || Math.Abs(GRID_SIZE - cx) < DELTA_SNAP) &&
                                    (Math.Abs(cy) < DELTA_SNAP || Math.Abs(GRID_SIZE - cy) < DELTA_SNAP))
                                {
                                    g.Margin = new Thickness(Math.Round(g.Margin.Left / GRID_SIZE) * GRID_SIZE,
                                        Math.Round(g.Margin.Top / GRID_SIZE) * GRID_SIZE, 0, 0);

                                }
                                else
                                {
                                    g.Margin = new Thickness(((GateLocation)g.Tag).X, ((GateLocation)g.Tag).Y, 0, 0);
                                }

                                Point np = new Point(g.Margin.Left, g.Margin.Top);
                                if (op != np)
                                    moves.Add(new UndoRedo.MoveGate(g, this, op, np));

                                SizeCanvas();
                                g.BringIntoView(); // still needed because gate larger than 20px block

                            }
                            if (e.RightButton == MouseButtonState.Pressed)
                            {
                                // if we use g here we rotate relative to each
                                // gate
                                // instead, ensure a consistent rotation
                                // by rotating relative to the last
                                // selected gate
                                Gate rotateSrc = selected[selected.Count - 1];
                                Point mpp = rotateSrc.TranslatePoint(new Point(32, 32), GC);
                                double dx = mp2.X - mpp.X;
                                double dy = mp2.Y - mpp.Y;


                                double newtheta = Math.Atan2(dy, dx) * (180.0 / Math.PI);

                                dx = mp.X - mpp.X;
                                dy = mp.Y - mpp.Y;
                                double theta = Math.Atan2(dy, dx) * (180.0 / Math.PI);


                                // the snap-to messes up the rotation
                                // so we store smooth angle in the tag
                                // and actual snapped angle in the transform
                                double na = ((GateLocation)g.Tag).Angle + newtheta - theta;
                                if (na < 0) na += 360;
                                if (na >= 360) na -= 360;
                                ((GateLocation)g.Tag).Angle = na;

                                double or = ((RotateTransform)g.RenderTransform).Angle;

                                // snap-to corners
                                if (na >= 360 - ANGLE_SNAP_DEG || na <= ANGLE_SNAP_DEG) na = 0;
                                if (na >= 270 - ANGLE_SNAP_DEG && na <= 270 + ANGLE_SNAP_DEG) na = 270;
                                if (na >= 180 - ANGLE_SNAP_DEG && na <= 180 + ANGLE_SNAP_DEG) na = 180;
                                if (na >= 90 - ANGLE_SNAP_DEG && na <= 90 + ANGLE_SNAP_DEG) na = 90;


                                ((RotateTransform)g.RenderTransform).Angle = na;


                                moves.Add(new UndoRedo.RotateGate(g, this, or, na));
                            }

                        }

                        UpdateWireConnections();

                        break;

                    case DragState.NONE:
                        // not dragging
                        // creating a selection rectangle
                        if (ReadyToSelect)
                        {
                            double x1 = Math.Min(mp2.X, sp.X);
                            double width = Math.Abs(mp2.X - sp.X);

                            double y1 = Math.Min(mp2.Y, sp.Y);
                            double height = Math.Abs(mp2.Y - sp.Y);

                            dragSelect.Margin = new Thickness(x1, y1, 0, 0);
                            dragSelect.Width = width;
                            dragSelect.Height = height;
                            dragSelect.Visibility = Visibility.Visible;

                            // select any gates inside the rectangle
                            Rect select = new Rect(x1, y1, width, height);
                            foreach (Gate g in gates.Values)
                            {
                                Rect grect = new Rect(g.Margin.Left, g.Margin.Top, g.Width, g.Height);
                                if (select.IntersectsWith(grect) && !g.Selected)
                                {
                                    g.Selected = true;
                                    selected.Add(g);
                                }

                                // this is not the same as just "not" or else the above
                                if (!select.IntersectsWith(grect) && g.Selected)
                                {
                                    g.Selected = false;
                                    selected.Remove(g);
                                }


                            }
                        }
                        break;

                }
            }
            mp = mp2;
        }

        //mouse up event for gate clicks
        private void uigate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly || EndUserMode)
                return;

            Gate tg = (Gate)sender;

            // green team
            // sets focus on mouse click
            tg.Focus();

            // to avoid sticking on other gates, move this one to the top
            GC.Children.Remove(tg);
            GC.Children.Add(tg);

            if (!tg.Selected)
            {
                // can use ctrl or alt to multi-select
                if (Keyboard.Modifiers == ModifierKeys.None)
                    ClearSelection();

                selected.Add(tg);
                ((GateLocation)tg.Tag).X = tg.Margin.Left;
                ((GateLocation)tg.Tag).Y = tg.Margin.Top;
                tg.Selected = true;
            }

            foreach (Gate.TerminalID tid in tg)
            {
                if (tid.t.IsMouseOver)
                {
                    // ok, so are we connecting to or from
                    // is this an input or output?
                    if (tid.isInput)
                    {
                        // can only connect from an input
                        // if there is no other connection here
                        if (wires.ContainsKey(new Gates.Terminal(tid.ID, tid.abgate)))
                            continue;

                        dragging = DragState.CONNECT_TO;
                        dragWire.Value = false;

                        // highlight all terminals which provide output
                        foreach (Gates.AbstractGate ag in gates.Keys)
                        {
                            for (int i = 0; i < ag.Output.Length; i++)
                            {
                                gates[ag].FindTerminal(false, i).t.Highlight = true;
                            }
                        }
                    }
                    else
                    {
                        dragging = DragState.CONNECT_FROM;
                        // TODO: if the value of the output changes
                        // while being dragged, this won't update
                        dragWire.Value = tid.abgate.Output[tid.ID];


                        // highlight all terminals which accept input
                        // note this is all inputs NOT already connected
                        foreach (Gates.AbstractGate ag in gates.Keys)
                        {
                            for (int i = 0; i < ag.NumberOfInputs; i++)
                            {
                                if (c.GetSource(new Gates.Terminal(i, ag)) == null)
                                    gates[ag].FindTerminal(true, i).t.Highlight = true;
                            }
                        }
                    }
                    beginTID = tid;

                    dragWire.Destination = tid.t.TranslatePoint(new Point(5, 5), GC);
                    dragWire.Origin = tid.t.TranslatePoint(new Point(5, 5), GC);
                    e.Handled = true;
                    return;

                }
            }

            dragging = DragState.MOVE;

            moves = new UndoRedo.Transaction(
                            (e.LeftButton == MouseButtonState.Pressed ?
                            "Move" : "Rotate") + " " +
                            (selected.Count == 1 ?
                            "Gate" : selected.Count.ToString() + " Gates"));

            e.Handled = true;

        }
        #endregion

        /// <summary>
        /// Clear the selection.  This empties the selected list and sets selected to false
        /// on all gates which were in it.
        /// </summary>
        /// 
        // Green Team Notes:  This clears selection
        public void ClearSelection()
        {
            foreach (Gate g in selected)
                g.Selected = false;

            selected.Clear();
        }
       
        
        /// <summary>
        /// Select all gates.
        /// </summary>
        public void SelectAll()
        {
            ClearSelection();

            foreach (Gate g in gates.Values)
            {
                g.Selected = true;
                selected.Add(g);
            }
        }
        

        private void SizeCanvas()
        {
            // green team notes:  increased size to make scroll bars visible on start
            // this will ensure the mouse center zoom method works on start up
            double maxx = (GCScroller.ViewportWidth / _zoom) ;
            double maxy = (GCScroller.ViewportHeight / _zoom) ;

            foreach (Gate g in gates.Values)
            {
                maxx = Math.Max(maxx, g.Margin.Left + g.Width + 64);
                maxy = Math.Max(maxy, g.Margin.Top + g.Height + 64);

            }

            GC.Width = maxx;
            GC.Height = maxy;

        }

        public static readonly DependencyProperty ZoomCenterProperty =
            DependencyProperty.Register(
            "ZoomCenter", typeof(Point), typeof(GateCanvas));

        /// <summary>
        /// When zooming in or out, do so with this point as the center, iff
        /// UseZoomCenter is set to true.  Otherwise, the center of the displayed area
        /// is calculated and used as the zoom center.
        /// </summary>
        public Point ZoomCenter { 
            get 
            {
                return (Point)GetValue(ZoomCenterProperty);
            }
            set
            {
                SetValue(ZoomCenterProperty, value);
            }
        }
        
        /// <summary>  
         /// Calculate the center of the displayed area which would make a zoom center  
         ///  to zoom "straight" in or out.  
         /// </summary>  
         public void SetZoomCenter()  
         {  
             double centerX = (GCScroller.HorizontalOffset + GCScroller.ViewportWidth / 2.0) / _zoom;  
             double centerY = (GCScroller.VerticalOffset + GCScroller.ViewportHeight / 2.0) / _zoom;  
             ZoomCenter = new Point(centerX, centerY);  
  
         }  
  
         /// <summary>  
         /// Indicate if the user provided value in ZoomCenter should be used  
         /// </summary>  
         public bool UseZoomCenter { get; set; }  
  
         /// <summary>  
         /// Sets the zoom factor.  Changes to zoom occur based on the center of the displayed area,  
         /// or, if UseZoomCenter is set to true, around the user defined zoom center.  
         /// </summary>  
         public double Zoom  
         {
             get
             {
                 return _zoom;
             }
             set  
             {  
                
  
                 double centerX = (GCScroller.HorizontalOffset + GCScroller.ViewportWidth / 2.0) / _zoom;  
                 double centerY = (GCScroller.VerticalOffset + GCScroller.ViewportHeight / 2.0) / _zoom;  
                 _zoom = value;  
                 GC.LayoutTransform = new ScaleTransform(value, value);  
                 if (UseZoomCenter)  
                 {  
                     centerX = ZoomCenter.X;  
                     centerY = ZoomCenter.Y;  
                 }  
  
                 if (!double.IsNaN(centerX))  
                 {  
                     GCScroller.ScrollToHorizontalOffset((centerX * _zoom) - GCScroller.ViewportWidth / 2.0);  
                     GCScroller.ScrollToVerticalOffset((centerY * _zoom) - GCScroller.ViewportHeight / 2.0);  
                 }  
  
             }  
         }  
  
 



        /// <summary>
        /// Given a point on the canvas, find the nearest "snap" point.  Useful
        /// for placing new gates.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point GetNearestSnapTo(Point p)
        {
            return new Point(Math.Round(p.X / GRID_SIZE) * GRID_SIZE,
                                        Math.Round(p.Y / GRID_SIZE) * GRID_SIZE);
        }

        /// <summary>
        /// Given a relative point on the GateCanvas control,
        /// adjust it using the current zoom and scroll settings
        /// to reflect an actual point on the unscrolled, unzoomed canvas.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point TranslateScrolledPoint(Point p)
        {
            Point np = new Point();
            np.X = GCScroller.HorizontalOffset + p.X;
            np.Y = GCScroller.VerticalOffset + p.Y;
            np.X /= _zoom;
            np.Y /= _zoom;
            return np;
        }

        

        
        /// <summary>
        /// Some operations may work on subsets of the canvas gates.
        /// </summary>
        public enum SELECTED_GATES
        {
            /// <summary>
            /// All gates in the canvas
            /// </summary>
            ALL, 

            /// <summary>
            /// Only those selected gates in the canvas, if any
            /// </summary>
            SELECTED,

            /// <summary>
            /// If 2 or more gates are selected, use selected gates.
            /// Otherwise, use all gates.
            /// </summary>
            SELECTED_IF_TWO_OR_MORE
        };

        /// <summary>
        /// Create an IC based on the circuit in this gate.  The position of the gates
        /// will be used to setup the location hints.  If desired, a subset of the gates
        /// may be used to create the IC.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectedOnly"></param>
        /// <returns></returns>
        public UIGates.IC CreateIC(string name, SELECTED_GATES selectedOnly)
        {
            Gates.Circuit nc;
            Dictionary<int, int> idxmap = null;

            if ( (selected.Count > 1 && selectedOnly == SELECTED_GATES.SELECTED_IF_TWO_OR_MORE) || selectedOnly == SELECTED_GATES.SELECTED)
                nc = c.Clone(selected.ToList().ConvertAll(g => g.AbGate), out idxmap);
            else
                nc = c.Clone();

            ICBuilder icb = new ICBuilder(nc, new ICBuilder.GatePosition( gate =>
            {
                int idx = nc.IndexOf(gate);
                if (idxmap != null) {
                    foreach (KeyValuePair<int, int> kv in idxmap)
                        if (kv.Value == idx)
                        {
                            idx = kv.Key;
                            break;
                        }// this makes me want to cry
                    // there must be a better way to look up by value
                    // because the value is the new circuit idx
                    // and the key is the old circuit idx
                    // don't just reverse it on create
                    // we use it the other way too

                }

                Gates.AbstractGate oldgate = c[idx];
                return new GateLocation() {
                    X = gates[oldgate].Margin.Left, 
                    Y = gates[oldgate].Margin.Top, 
                    Width = gates[oldgate].Width,
                    Height = gates[oldgate].Height,
                    Angle = ((RotateTransform)gates[oldgate].RenderTransform).Angle
                };
            }));
            return icb.CreateIC(name);
        }

        /// <summary>
        /// Create an IC based on the circuit in this gate.  The position of the gates
        /// will be used to setup the location hints.
        /// </summary>
        /// <returns></returns>
        public UIGates.IC CreateIC()
        {
            return CreateIC(ICName, SELECTED_GATES.ALL);
        }

        /// <summary>
        /// Sets the background to white temporarily for image capture or printing
        /// </summary>
        public bool Mute
        {
            set
            {
                if (value)
                {
                    oldBackground = GC.Background;
                    GC.Background = Brushes.White;

                    
                }
                else
                {
                    GC.Background = oldBackground;

                }
                _mute = value;
            }
        }

        public bool DisableSizeCanvas { get; set; }

        /// <summary>
        /// Create an image out of this canvas
        /// </summary>
        /// <returns></returns>
        public BitmapSource CreateImage()
        {
            double oldZoom = _zoom;

            //Zoom = 1.0;
            //UpdateLayout();

            GCScroller.ScrollToHome();
            UpdateLayout();
            
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(GC.ActualWidth * _zoom), (int)(GC.ActualHeight * _zoom), 96, 96, PixelFormats.Pbgra32);
            rtb.Render(GC);
           
            //Zoom = oldZoom;

            return rtb;
        }
       
        /// <summary>
        /// Print this canvas.  The dialog should have already been displayed and confirmed.
        /// </summary>
        /// <param name="printDlg"></param>
        public void Print(PrintDialog printDlg)
        {
            

            System.Printing.PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

            Mute = true;

            BitmapSource bs = CreateImage();

            double wid = capabilities.PageImageableArea.ExtentWidth / bs.Width;
            double hei = capabilities.PageImageableArea.ExtentHeight / bs.Height;

            Image i = new Image();
            i.Source = bs;
            i.Stretch = Stretch.Uniform;
            i.Width = capabilities.PageImageableArea.ExtentWidth;
            i.Height = capabilities.PageImageableArea.ExtentHeight;

            Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

            //update the layout of the visual to the printer page size.

            i.Measure(sz);

            i.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

            
            //now print the visual to printer to fit on the one page.

            printDlg.PrintVisual(i, "Circuit");

            Mute = false;
            UseZoomCenter = false;
            
            SizeCanvas();
        }

        private void GCScroller_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_mute && !DisableSizeCanvas)
                SizeCanvas();
        }

        private void GCScroller_LayoutUpdated(object sender, EventArgs e)
        {
            if (!_mute && !DisableSizeCanvas)
                SizeCanvas();
        }

        private void GCScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            if (dragging == DragState.MOVE)
            {
                System.Threading.Thread.Sleep(100); // don't let them scroll themselves into oblivion
            }

        }
    }
}
