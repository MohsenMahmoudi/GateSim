using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows;


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
    /// Provides an XML serialization/deserialization for circuits,
    /// including location of gates and ICs.
    /// </summary>
    public class CircuitXML
    {
        /// <summary>
        /// Create an XML representation of a given IC.  Nested ICs will be referenced,
        /// but not created by this method.
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public XElement CreateCircuitXML(UIGates.IC cc)
        {
            XElement circuit = new XElement("Circuit");
            circuit.SetAttributeValue("Name", cc.AbGate.Name);

            

            XElement gates = new XElement("Gates");
            Dictionary<Gates.AbstractGate, int> gid = new Dictionary<Gates.AbstractGate, int>();
            int cid = 1;
            Gates.Circuit circ = ((Gates.IC)cc.AbGate).Circuit;
            foreach (Gates.AbstractGate g in circ)
            {
                XElement gt = new XElement("Gate");
                gt.SetAttributeValue("Type", g.GetType().Name);
                gt.SetAttributeValue("Name", g.Name);
                gt.SetAttributeValue("ID", cid);
                gt.Add(new XElement("Point"));
                gt.Element("Point").SetAttributeValue("X", cc.locationHints[g].X);
                gt.Element("Point").SetAttributeValue("Y", cc.locationHints[g].Y);
                gt.Element("Point").SetAttributeValue("Angle", cc.locationHints[g].Angle);

                if (g is Gates.IVariableInputs)
                {
                    gt.SetAttributeValue("NumInputs", g.NumberOfInputs);
                }

                if (g is Gates.IOGates.AbstractNumeric)
                {
                    gt.SetAttributeValue("Bits", ((Gates.IOGates.AbstractNumeric)g).Bits);
                    gt.SetAttributeValue("SelRep", (int) (((Gates.IOGates.AbstractNumeric)g).SelectedRepresentation));
                    gt.SetAttributeValue("Value", ((Gates.IOGates.AbstractNumeric)g).Value);
                }
                if (g is Gates.IOGates.Clock)
                {
                    gt.SetAttributeValue("Milliseconds", ((Gates.IOGates.Clock)g).Milliseconds);
                }
                if (g is Gates.IOGates.Comment)
                {
                    gt.Add(new XElement("Comment", ((Gates.IOGates.Comment)g).Value));
                }
                gates.Add(gt);
                gid.Add(g, cid);
                cid++;
            }

            XElement wires = new XElement("Wires");
            foreach (Gates.AbstractGate g in circ)
            {
                
                for (int i = 0; i < g.NumberOfInputs; i++)
                {
                    Gates.Terminal t = circ.GetSource(new Gates.Terminal(i, g));
                    if (t != null)
                    {
                        XElement wire = new XElement("Wire",
                            new XElement("From"), new XElement("To"));
                        wire.Element("From").SetAttributeValue("ID", gid[t.gate]);
                        wire.Element("From").SetAttributeValue("Port", t.portNumber);
                        wire.Element("To").SetAttributeValue("ID", gid[g]);
                        wire.Element("To").SetAttributeValue("Port", i);
                        wires.Add(wire);    
                    }
                    
                }

            }


            circuit.Add(gates);
            circuit.Add(wires);
            return circuit;

        }

        /// <summary>
        /// Create an XML representation of this IC and all nested ICs, recursively.
        /// </summary>
        /// <param name="ic"></param>
        /// <returns></returns>
        public XElement CreateXML(UIGates.IC ic)
        {
            XElement root = new XElement("CircuitGroup");
            root.SetAttributeValue("Version", "1.2");

            TopologicalSort ts = new TopologicalSort();
            List<UIGates.IC> ics = ts.Sort(ic, icl);

            foreach (UIGates.IC theic in ics)
            {
                root.Add(CreateCircuitXML(theic));
            }
            return root;
        }

        /// <summary>
        /// Create an XML representation of the given IC, and all nested ICs recursively,
        /// and save it to the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ic"></param>
        public void Save(string path, UIGates.IC ic)
        {

            XElement root = CreateXML(ic);

            root.Save(path);
        }

        /// <summary>
        /// Create an XML representation of the circuit in the given gate canvas,
        /// together with ALL ICs in the gate canvas' icl (whether they are referenced or not)
        /// and save the whole thing to a file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="gc"></param>
        public void Save(string path, GateCanvas gc)
        {
            XElement root = new XElement("CircuitGroup");
            root.SetAttributeValue("Version", "1.2");

            XElement maincircuit = CreateCircuitXML(gc.CreateIC("", GateCanvas.SELECTED_GATES.ALL));
            maincircuit.SetAttributeValue("Name", null);


            TopologicalSort ts = new TopologicalSort();
            List<UIGates.IC> ics = ts.Sort(icl, icl);

            foreach (UIGates.IC theic in ics)
            {
                root.Add(CreateCircuitXML(theic));
            }
            root.Add(maincircuit);

            root.Save(path);
        }

        

        private Gates.AbstractGate CreateGate(XElement gate)
        {
            int numInputs = 2; // default for variable input gates
            if (gate.Attribute("NumInputs") != null)
                numInputs = int.Parse(gate.Attribute("NumInputs").Value);

            switch (gate.Attribute("Type").Value)
            {
                case "And":
                    return new Gates.BasicGates.And(numInputs);
                case "Not":
                    return new Gates.BasicGates.Not();
                case "Or":
                    return new Gates.BasicGates.Or(numInputs);
                case "Nand":
                    return new Gates.BasicGates.Nand(numInputs);
                case "Nor":
                    return new Gates.BasicGates.Nor(numInputs);
                case "Xor":
                    return new Gates.BasicGates.Xor();
                case "Xnor":
                    return new Gates.BasicGates.Xnor();
                case "Buffer":
                    return new Gates.BasicGates.Buffer();
                case "UserInput":
                    Gates.IOGates.UserInput ui = new Gates.IOGates.UserInput();
                    ui.SetName(gate.Attribute("Name").Value);
                    return ui;
                case "UserOutput":
                    Gates.IOGates.UserOutput uo = new Gates.IOGates.UserOutput();
                    uo.SetName(gate.Attribute("Name").Value);
                    return uo;
                case "NumericInput":
                    Gates.IOGates.NumericInput ni = new Gates.IOGates.NumericInput(int.Parse(gate.Attribute("Bits").Value));
                    ni.SelectedRepresentation = (Gates.IOGates.AbstractNumeric.Representation)int.Parse(gate.Attribute("SelRep").Value);
                    ni.Value = gate.Attribute("Value").Value;
                    return ni;
                case "NumericOutput":
                    Gates.IOGates.NumericOutput no = new Gates.IOGates.NumericOutput(int.Parse(gate.Attribute("Bits").Value));
                    no.SelectedRepresentation = (Gates.IOGates.AbstractNumeric.Representation)int.Parse(gate.Attribute("SelRep").Value);
                    return no;
                case "Clock":
                    Gates.IOGates.Clock clk = new Gates.IOGates.Clock(int.Parse(gate.Attribute("Milliseconds").Value));
                    return clk;
                case "IC":
                    // check if this ic has been renamed
                    string cname = gate.Attribute("Name").Value;
                    // check first if we need to rename
                    if (UpdateICNames.ContainsKey(cname))
                        cname = UpdateICNames[cname];
                
                    return icl.GetIC(cname).AbGate.Clone();
                case "Comment":
                    Gates.IOGates.Comment cmt = new Gates.IOGates.Comment();
                    cmt.Value = gate.Element("Comment").Value;
                    return cmt;
                
            }
            throw new ArgumentException("unknown gate");
        }

        private Gates.Terminal CreateTerminal(Dictionary<int, Gates.AbstractGate> gid, XElement terminal)
        {
            Gates.Terminal t = new Gates.Terminal(
                int.Parse(terminal.Attribute("Port").Value),
                gid[int.Parse(terminal.Attribute("ID").Value)]);
            return t;

        }

        
        /// <summary>
        /// Given an XML circuit representation, create an IC.
        /// The local IC List will be used to deference any nested ICs.
        /// </summary>
        /// <param name="circuit"></param>
        /// <returns></returns>
        public UIGates.IC LoadCircuit(XElement circuit)
        {
            Gates.Circuit c = new Gates.Circuit();
            
            
            Dictionary<int, Gates.AbstractGate> gid = new Dictionary<int, Gates.AbstractGate>();
            Dictionary<Gates.AbstractGate, GateLocation> gpt = new Dictionary<Gates.AbstractGate, GateLocation>();
            foreach (XElement gate in circuit.Element("Gates").Elements())
            {
                Gates.AbstractGate abgate = CreateGate(gate);
                c.Add(abgate);
                gid.Add(int.Parse(gate.Attribute("ID").Value), abgate);

                double x = double.Parse(gate.Element("Point").Attribute("X").Value);
                double y = double.Parse(gate.Element("Point").Attribute("Y").Value);
                double angle = double.Parse(gate.Element("Point").Attribute("Angle").Value);

                gpt.Add(abgate, new GateLocation() { X = x, Y = y, Angle = angle });

            }

            foreach (XElement wire in circuit.Element("Wires").Elements())
            {
                c[CreateTerminal(gid, wire.Element("To"))] = CreateTerminal(gid, wire.Element("From"));
            }

            ICBuilder icb = new ICBuilder(c, (Gates.AbstractGate abgate) =>
            {
                return gpt[abgate];
            });

            // for top level circuit, must not create terminals
            // otherwise input/output overridden
            if (circuit.Attribute("Name") != null)
            {
                string cname = circuit.Attribute("Name").Value;
                // check first if we need to rename
                if (UpdateICNames.ContainsKey(cname))
                    cname = UpdateICNames[cname];
                
                return icb.CreateIC(cname);
                
            }
            else
                return icb.CreateNonTerminaledIC("");
            

        }

        private Dictionary<string, string> UpdateICNames;

        public delegate void StoreIC(UIGates.IC ic);

        /// <summary>
        /// Load a circuit group from a file.  All ICs in the file will be stored
        /// using the provided StoreIC method. The IC with no name will be returned
        /// as the primary circuit.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sic"></param>
        /// <returns></returns>
        public UIGates.IC Load(string path, StoreIC sic)
        {
            XElement root = XElement.Load(path);
            // v1.0 and v1.1 did not mark version attribute, so check for null
            if (root.Attribute("Version") != null && root.Attribute("Version").Value != "1.2")
                throw new Exception("Unsupport version " + root.Attribute("Version").Value);

            UIGates.IC retic = null;
            foreach (XElement circuit in root.Elements()) 
            {
                if (circuit.Attribute("Name") != null)
                {
                    // check if we need to rename this circuit
                    try
                    {
                        icl.GetIC(circuit.Attribute("Name").Value);
                        // already has an IC by this name
                        UpdateICNames[circuit.Attribute("Name").Value] =
                            icl.GenerateAvailableName(circuit.Attribute("Name").Value);
                    }
                    catch (ArgumentException)
                    {
                        // not found, no problem
                    }
                    
                }
                UIGates.IC ic = LoadCircuit(circuit);
                if (circuit.Attribute("Name") != null)
                    sic(ic);
                else
                    retic = ic;
            }

            return retic;
            
        }

        private ICList icl;
        public CircuitXML(ICList icl)
        {
            this.icl = icl;
            UpdateICNames = new Dictionary<string, string>();
        }
    }
}
