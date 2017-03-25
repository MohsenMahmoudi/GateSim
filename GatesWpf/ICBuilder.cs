using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Builds an IC based on a collection of gates.  The primary purpose
    /// of this class is to locate user input and user output controls and transform
    /// them into endpoints for the IC, and to position them as top/left/right/bottom
    /// for the terminals.
    /// </summary>
    public class ICBuilder
    {

        public delegate GateLocation GatePosition(Gates.AbstractGate gate);

        private List<GTerm> posGates;
        private double avgX = 0, avgY = 0;
        private Gates.Circuit c;
        private GatePosition pos;

        private struct GTerm
        {
            public Gate.Position pos;
            public double offset;
            public Gates.AbstractGate gate;
            public GTerm(Gates.AbstractGate gate, double offset, Gate.Position pos)
            {
                this.pos = pos;
                this.offset = offset;
                this.gate = gate;
            }
        }


        private void PositionAndHold(Gates.AbstractGate g)
        {
            // determine quadrant
            Rect gr = pos(g).GetRect();
            Point gp = new Point((gr.Left + gr.Right) / 2, (gr.Top + gr.Bottom) / 2);
            // the goal here is to make the bracket square
            // and not rectangular if one side is longer


            double dx = gp.X - avgX;
            double dy = gp.Y - avgY;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                // left or right
                // note that we invert Y for left so it goes from bottom to top
                if (dx < 0)
                    posGates.Add(new GTerm(g, -gp.Y, Gate.Position.LEFT));
                else
                    posGates.Add(new GTerm(g, gp.Y, Gate.Position.RIGHT));
            }
            else
            {
                // top or bottom
                if (dy < 0)
                    posGates.Add(new GTerm(g, gp.X, Gate.Position.TOP));
                else
                    posGates.Add(new GTerm(g, gp.X, Gate.Position.BOTTOM));
            }

        }

        public Rect createCenter()
        {
            posGates = new List<GTerm>();

            List<Gates.IOGates.UserInput> uis = new List<Gates.IOGates.UserInput>();
            List<Gates.IOGates.UserOutput> uos = new List<Gates.IOGates.UserOutput>();

            // Determine the center-point of the X
            bool fst = true;
            double maxX = 0, maxY = 0, minX = 0, minY = 0;
            foreach (Gates.AbstractGate g in c)
            {
                Rect p = pos(g).GetRect();
                if (fst)
                {
                    maxX = p.Right;
                    minX = p.X;
                    maxY = p.Bottom;
                    minY = p.Y;
                    fst = false;
                }
                maxX = Math.Max(maxX, p.Right);
                maxY = Math.Max(maxY, p.Bottom);
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                //avgX += p.X;
                //avgY += p.Y;
            }

            //avgX /= c.Count;
            //avgY /= c.Count;
            return new Rect(minX, minY, maxX - minX, maxY - minY);

        }



        /// <summary>
        /// Create an IC based on the circuit.  All user input and outputs
        /// will be turned into terminals on this IC.  The technique is to establish
        /// a center-point of the gates involved, and establish a "X" shape originating
        /// from this center.  The position of the user i/o gates within this X shape
        /// determine how they appear on the IC.  The goal is to have the layout of
        /// terminals match the layout and ordering of user inputs and outputs.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIGates.IC CreateIC(string name)
        {

            posGates = new List<GTerm>();

            List<Gates.IOGates.UserInput> uis = new List<Gates.IOGates.UserInput>();
            List<Gates.IOGates.UserOutput> uos = new List<Gates.IOGates.UserOutput>();

            // Determine the center-point of the X
            
            double maxX = 0, maxY = 0, minX = 0, minY = 0;
            

            var pts = createCenter();
            maxX = pts.Right;
            minX = pts.Left;
            maxY = pts.Bottom;
            minY = pts.Top;

            avgX = (maxX + minX) / 2.0;
            avgY = (maxY + minY) / 2.0;

            // For all user i/o gates, determine where they fall in the X
            foreach (Gates.AbstractGate g in c)
            {

                if (g is Gates.IOGates.UserInput)
                {
                    uis.Add((Gates.IOGates.UserInput)g);
                    PositionAndHold(g);
                }
                if (g is Gates.IOGates.UserOutput)
                {
                    uos.Add((Gates.IOGates.UserOutput)g);
                    PositionAndHold(g);
                }
            }

            // Apply a sort to order user i/o gates with respect
            // to the standard ordering that the IC uses to display its terminals
            // see Gate class
            posGates.Sort((firstVal, nextVal) =>
            {
                if (firstVal.pos != nextVal.pos)
                    return firstVal.pos.CompareTo(nextVal.pos);
                else
                    return firstVal.offset.CompareTo(nextVal.offset);
            });

            Gates.IC nic = new Gates.IC(c, uis.ToArray(), uos.ToArray(), name);
            List<Gate.TerminalID> tids = new List<Gate.TerminalID>();

            // constuct the terminal id list based on the sorted sequence
            foreach (GTerm gt in posGates)
            {
                tids.Add(new Gate.TerminalID(gt.gate is Gates.IOGates.UserInput,
                    gt.gate is Gates.IOGates.UserInput ?
                    uis.IndexOf((Gates.IOGates.UserInput)gt.gate) : uos.IndexOf((Gates.IOGates.UserOutput)gt.gate),
                    gt.pos));
            }




            UIGates.IC nuic = new UIGates.IC(nic, tids.ToArray());

            // finally, hint the IC so that we can remember
            // where things are placed visually in the future
            foreach (Gates.AbstractGate g in c)
            {
                nuic.locationHints.Add(g, pos(g));
            }


            // in some cases, the caller may be using these gates
            // so we create a clone so there is no interference
            return (UIGates.IC)nuic.CreateUserInstance();
        }

        /// <summary>
        /// Create an IC, but ignore user input and output.  This is useful if
        /// the IC is just a package for some other operation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIGates.IC CreateNonTerminaledIC(string name)
        {
            Gates.IC nic = new Gates.IC(c, new Gates.IOGates.UserInput[0],
                new Gates.IOGates.UserOutput[0], name);
            UIGates.IC nuic = new UIGates.IC(nic, new Gate.TerminalID[0]);

            // finally, hint the IC so that we can remember
            // where things are placed visually in the future
            foreach (Gates.AbstractGate g in c)
            {
                nuic.locationHints.Add(g, pos(g));
            }

            return (UIGates.IC)nuic.CreateUserInstance();
        }



        public ICBuilder(Gates.Circuit c, GatePosition pos)
        {
            this.c = c;
            this.pos = pos;
        }

        //public UIGates.IC CreateICQuad(string name)
        //{

        //    posGates = new List<GTerm>();

        //    List<Gates.IOGates.UserInput> uis = new List<Gates.IOGates.UserInput>();
        //    List<Gates.IOGates.UserOutput> uos = new List<Gates.IOGates.UserOutput>();

        //    // Determine the center-point of the X
        //    bool fst = true;
        //    double maxX = 0, maxY = 0, minX = 0, minY = 0;
        //    foreach (Gates.AbstractGate g in c)
        //    {
        //        Point p = pos(g).GetPoint();
        //        if (fst)
        //        {
        //            maxX = p.X;
        //            minX = p.X;
        //            maxY = p.Y;
        //            minY = p.Y;
        //            fst = false;
        //        }
        //        maxX = Math.Max(maxX, p.X);
        //        maxY = Math.Max(maxY, p.Y);
        //        minX = Math.Min(minX, p.X);
        //        minY = Math.Min(minY, p.Y);
        //        //avgX += p.X;
        //        //avgY += p.Y;
        //    }

        //    //avgX /= c.Count;
        //    //avgY /= c.Count;
        //    avgX = (maxX + minX) / 2.0;
        //    avgY = (maxY + minY) / 2.0;
            
        //}
    }
}
