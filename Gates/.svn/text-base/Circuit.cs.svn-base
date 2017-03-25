using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;


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
    /// A circuit is a collection of gates with wires between them.  The circuit can
    /// be started or stopped, which indicates if a thread runs propagation over the wires.
    /// The circuit start/stop status recursively controls all ICs automatically.
    /// </summary>
    public class Circuit : BindingList<AbstractGate>
    {
        
        private Dictionary<Terminal, Terminal> wires;
        

        private bool runThread = false;

        private List<IC> ics = new List<IC>();

        /// <summary>
        /// Construct an empty circuit.  The circuit will not 
        /// propagate values until Start()'d
        /// </summary>
        public Circuit()
        {
            wires = new Dictionary<Terminal, Terminal>();
            this.ListChanged += new ListChangedEventHandler(Circuit_ListChanged);

            
            
        }

        /// <summary>
        /// Start the circuit thread.  If the thread is already running,
        /// an exception is thrown.  This command recursively starts
        /// all ICs.
        /// </summary>
        public void Start()
        {
            
            runThread = true;
            
            foreach (AbstractGate ag in this)
            {
                Gate_PropertyChanged(ag);
                if (ag is IC)
                    ((IC)ag).Circuit.Start();
            }
           
        }

        /// <summary>
        /// Request the circuit thread to stop.  No action if the thread is
        /// not running.  Recursively stops all ICs.
        /// </summary>
        public void Stop()
        {
            runThread = false;
            

            foreach (AbstractGate ag in this)
            {
                

                if (ag is IC)
                    ((IC)ag).Circuit.Stop();
            }
        }

        /// <summary>
        /// Indicates if this circuit is in a running state.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return runThread;
            }
        }

        /// <summary>
        /// Create a duplicate of this circuit by cloning all
        /// contained gates and wires.
        /// </summary>
        /// <returns></returns>
        public Circuit Clone()
        {
            Dictionary<int, int> x; // don't care, all indices will be same
            return Clone(this, out x);
        }


        /// <summary>
        /// Create a duplicate of part of this circuit by cloning those gates
        /// specified in the include list which are part of this circuit,
        /// and any wires that connect gates on the include list.
        /// The out parameter gives a mapping of original circuit gate index
        /// to new circuit gate indices.
        /// </summary>
        /// <returns></returns>
        public Circuit Clone(IEnumerable<AbstractGate> include, out Dictionary<int, int> idxmap)
        {
            Circuit nc = new Circuit();

            idxmap = new Dictionary<int, int>();
            int ncidx = 0;

            for (int i = 0; i < this.Count; i++)
            {
                AbstractGate ag = this[i];
                if (include.Contains(ag))
                {
                    nc.Add(ag.Clone());
                    idxmap[i] = ncidx++; // needed because we may skip some from source circuit, so indices don't line up
                }
            }

            
            for (int i = 0; i < this.Count; i++)
            {
                if (!include.Contains(this[i]))
                    continue;

                

                for (int it = 0; it < this[i].NumberOfInputs; it++)
                {
                    Terminal source = this.GetSource(new Terminal(it, this[i]));
                    if (source != null && include.Contains(source.gate))
                    {
                        int sidx = this.IndexOf(source.gate);
                        nc[new Terminal(it, nc[idxmap[i]])] = new Terminal(source.portNumber, nc[idxmap[sidx]]);
                    }
                }

                
            }

            return nc;
        }



       

      

        
       

        private void Circuit_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    //this[e.NewIndex].PropertyChanged += new PropertyChangedEventHandler(Gate_PropertyChanged);
                    if (this[e.NewIndex] is IC)
                    {
                        IC ic = this[e.NewIndex] as IC;
                        ics.Add(ic);
                        if (IsRunning && !ic.Circuit.IsRunning)
                            ic.Circuit.Start();
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    PruneOrphans();

                    
                        for (int i = 0; i < ics.Count; i++)
                        {
                            if (!this.Contains(ics[i]))
                            {
                                ics[i].Circuit.Stop();
                                ics.RemoveAt(i);
                                break;
                            }
                        }
                        
                    

                    break;
                /* case ListChangedType.Reset:
                    break; */
                case ListChangedType.ItemChanged:
                    Gate_PropertyChanged(this[e.NewIndex]);
                    break;

            }
        }

        /// <summary>
        /// When a circuit is removed, remove all wires which connected
        /// to or from that circuit.
        /// </summary>
        private void PruneOrphans() 
        {
            List<KeyValuePair<Terminal, Terminal>> kvToRemove = new List<KeyValuePair<Terminal, Terminal>>();
            foreach (KeyValuePair<Terminal, Terminal> wire in wires)
            {
                if (!this.Contains(wire.Key.gate) || !this.Contains(wire.Value.gate))
                    kvToRemove.Add(wire);

            }

            foreach (KeyValuePair<Terminal, Terminal> kv in kvToRemove)
                Disconnect(kv.Key);

        }

        

        protected void Gate_PropertyChanged(object sender)
        {
            if (!runThread)
                return;

            try
            {
                // propagate along wires, if any
                foreach (KeyValuePair<Terminal, Terminal> wire in wires)
                {
                    // if the output is from our gate
                    if (wire.Value.gate == sender)
                    {
                        // update the corresponding input connection
                        
                        PropagationThread.Instance.Enqueue(wire);
                    }
                }
            }
            catch (InvalidOperationException) 
            { 
                /* wires modified enroute */
                // try again
                Gate_PropertyChanged(sender);
            }
        }

        /// <summary>
        /// Attach a wire.  Use the syntax circuit[target] = source.
        /// A single source may provide values for multiple different targets.
        /// However, each target (input) may only receive values from one source.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Terminal this[Terminal input]
        {
            set
            {
                if (wires.ContainsKey(input)) 
                    throw new ArgumentException("input already in use");
                if (!this.Contains(input.gate))
                    throw new ArgumentException("input not a member of this circuit");
                if (!this.Contains(value.gate))
                    throw new ArgumentException("output not a member of this circuit");

                wires[input] = value;
                if (CircuitConnection != null)
                    CircuitConnection(this, input, value);

                // RECOMPUTE
                input.gate[input.portNumber] = value.gate.Output[value.portNumber];
            }
        }

        /// <summary>
        /// Find the source terminal that provides data to this input, if any.
        /// Returns null if this input is not connected.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Terminal GetSource(Terminal input)
        {
            if (!wires.ContainsKey(input)) 
                return null;

            return wires[input];
        }

        /// <summary>
        /// Gets a list of all connections from the given output.
        /// A given output may connect to zero or more inputs.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public List<Terminal> GetTargets(Terminal output)
        {
            List<Terminal> ts = new List<Terminal>();
            foreach (KeyValuePair<Terminal, Terminal> cnx in wires)
            {
                if (cnx.Value == output)
                    ts.Add(cnx.Key);
            }
            return ts;
        }

        /// <summary>
        /// Disconnect a given input terminal.
        /// The value of the terminal returns to "false".
        /// </summary>
        /// <param name="input"></param>
        public void Disconnect(Terminal input)
        {
            wires.Remove(input);
            input.gate[input.portNumber] = false;
            if (CircuitConnection != null)
                CircuitConnection(this, input, null);
        }

        private void RewireReplacement(AbstractGate original, AbstractGate newGate)
        {

            // all INPUTS from old gate wire to new gate
            for (int i = 0; i < Math.Min(original.NumberOfInputs, newGate.NumberOfInputs); i++)
            {
                Terminal t = GetSource(new Terminal(i, original));
                if (t != null)
                    this[new Terminal(i, newGate)] = t;
            }

            // all OUTPUTs from old gate wire from new gate
            List<KeyValuePair<Terminal, Terminal>> toChange = new List<KeyValuePair<Terminal, Terminal>>();
            for (int i = 0; i < Math.Min(original.Output.Length, newGate.Output.Length); i++)
            {
                foreach (KeyValuePair<Terminal, Terminal> wire in wires)
                {
                    if (wire.Value.gate == original)
                    {
                        toChange.Add(wire);
                    }
                }
            }
            foreach (KeyValuePair<Terminal, Terminal> change in toChange)
            {
                Disconnect(change.Key);
                if (change.Value.portNumber < newGate.Output.Length)
                    this[change.Key] = new Terminal(change.Value.portNumber, newGate);

            }
        }

        /// <summary>
        /// Replace the occurance (if any) of the given original gate
        /// with a newgate.  The new gate is used as-is (not cloned)
        /// as it is assumed original refers to one specific
        /// member of the circuit, and only at most one replacement will be made.
        /// This method is not recursive (e.g. does not check inside ICs within this circuit).
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newgate"></param>
        /// <returns>true if a replacement was made, false otherwise</returns>
        public bool ReplaceGate(AbstractGate original, AbstractGate newgate)
        {
            // this dict for event purposes only
            Dictionary<AbstractGate, AbstractGate> gatexch = new Dictionary<AbstractGate, AbstractGate>();

            int origcnt = this.Count;
            for (int idx = 0; idx < origcnt; idx++)
            {
                // not using foreach b/c we modify the collection
                // by adding the new replacement gate!
                AbstractGate ab = this[idx];
                if (ab == original)
                {
                    Insert(idx, newgate);
                    RewireReplacement(original, newgate);
                    Remove(original);

                    gatexch.Add(ab, newgate);
                }
            }

            if (ReplaceGates != null)
                ReplaceGates(this, gatexch);

            return (gatexch.Count > 0);

            
        }

        /// <summary>
        /// Replace all occurances of a given gate with an new gate
        /// This searches by name and performs its replacement
        /// recursively (throughout all ICs within this circuit, and their circuits, etc.)
        /// A clone of the new gate is created for each replacement made.
        /// Note the new gate may be null.  In this case, the function deletes all matching
        /// original gates but does not replace them with any new gate.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newIC">A template of the new IC to use.  May be null to simply delete all original matching gates.</param>
        /// <returns>A dictionary showing the new instance of each gate (value) for each old instance (key)</returns>
        public Dictionary<AbstractGate, AbstractGate> ReplaceICs(string name, IC newICTemplate)
        {
            Dictionary<AbstractGate, AbstractGate> gatexch = new Dictionary<AbstractGate, AbstractGate>();
            int origcnt = this.Count;
            for (int idx = 0; idx < origcnt; idx++)
            {
                // not using foreach b/c we modify the collection
                // by adding the new replacement gate!
                AbstractGate ab = this[idx];
                if (ab.Name == name)
                {
                    AbstractGate newIC;
                    if (newICTemplate != null)
                    {
                        newIC = newICTemplate.Clone();
                        Insert(idx, newIC);
                        RewireReplacement(ab, newIC);
                    }
                    else
                    {
                        newIC = null;
                        idx--; // so we will repeat the same index
                        origcnt--; // one less now
                    }
                    gatexch.Add(ab, newIC);
                    Remove(ab);
                }
                else if (ab is IC)
                {
                    
                    Dictionary<AbstractGate, AbstractGate> resexch = ((IC)ab).Circuit.ReplaceICs(name, newICTemplate);
                    foreach (KeyValuePair<AbstractGate, AbstractGate> rinst in resexch) 
                    {
                        gatexch.Add(rinst.Key, rinst.Value);
                    }
                }
                
            }
            if (ReplaceGates != null)
                ReplaceGates(this, gatexch);

            return gatexch;
        }

        

        /// <summary>
        /// Handles a circuit connection or disconnection event
        /// </summary>
        /// <param name="sender">The circuit on which the connection occured</param>
        /// <param name="input">The input terminal</param>
        /// <param name="output">The terminal which supplies the input terminal, or null if this input was disconnected</param>
        public delegate void CircuitConnectionEventHandler(Circuit sender, Terminal input, Terminal output);

        /// <summary>
        /// Occurs whenever a circuit connection or disconnect occurs
        /// </summary>
        public event CircuitConnectionEventHandler CircuitConnection;

        /// <summary>
        /// Provides a list of changes made to this circuit by the ReplaceICs or ReplaceGate method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="replacements">A dictionary showing the new instance of each gate (value) for each old instance (key).  Value may be null if gates were deleted.</param>
        public delegate void ReplaceGatesEventHandler(Circuit sender, Dictionary<AbstractGate, AbstractGate> replacements);

        /// <summary>
        /// Occurs after ReplaceGate is completed
        /// </summary>
        public event ReplaceGatesEventHandler ReplaceGates;
    }
}
