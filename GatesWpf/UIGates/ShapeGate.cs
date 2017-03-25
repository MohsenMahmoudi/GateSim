using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;


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

namespace GatesWpf.UIGates
{
    /// <summary>
    /// Designed to represent a standard boolean logic gate.  The gate
    /// may have any number of inputs, but only one output.  Inputs
    /// are displayed from the left and output to the right. A visual
    /// path is provided which describes how the gate appears.
    /// </summary>
    public class ShapeGate : Gate
    {
        private Path ph;
        private static TerminalID[] CreateTerminals(int inputs)
        {
            TerminalID[] tids = new TerminalID[inputs + 1];
            for (int i = 0; i < inputs; i++)
                tids[i] = new TerminalID(true, i, Position.LEFT);
            tids[inputs] = new TerminalID(false, 0, Position.RIGHT);

            return tids;
        }

        /// <summary>
        /// Create a shape gate from a given gate and visual path.
        /// The visual path is based on the StreamGeometry.
        /// See http://msdn.microsoft.com/en-us/library/ms752293.aspx for details
        /// of the path syntax.
        /// </summary>
        /// <param name="abgate"></param>
        /// <param name="path"></param>
        public ShapeGate(Gates.AbstractGate abgate, string path)
            : base(abgate,
                 CreateTerminals(abgate.NumberOfInputs))
        {
            

            ph = new Path();
            ph.StrokeEndLineCap = PenLineCap.Square;
            ph.StrokeStartLineCap = PenLineCap.Triangle;
            ph.Data = StreamGeometry.Parse(path);
            ph.Stroke = Brushes.Black;
            ph.StrokeThickness = 2;
            ph.Fill = Brushes.White;
            myCanvas.Children.Add(ph);

        }

        // NOTE: THIS IS *NOT* WIRED BY DEFAULT!
        protected void ShapeGate_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            // note that this based on 64x64 gates only with with centered 30 tall shapes
            // not suitable for other sizes
            ph.RenderTransform = new ScaleTransform(1, (Height - 32.0) / 32.0, 0, 15.0);   
        }
    }

    public class And : ShapeGate
    {
        public And() : this(new Gates.BasicGates.And()) { }

        public And(Gates.AbstractGate abgate) : base(abgate, "M 17,17 v 30 h 15 a 2,2 1 0 0 0,-30 h -15") 
        { 
            this.SizeChanged += ShapeGate_SizeChanged; 
        }

    }

    public class Not : ShapeGate
    {
        public Not() : this(new Gates.BasicGates.Not()) { }

        public Not(Gates.AbstractGate abgate) : base(abgate, "M 15,17 v 30 l 30,-15 l -30,-15 M 46,33.5 a 3,3 1 1 1 0.1,0.1") { }

    }

    public class Or : ShapeGate
    {
        public Or() : this(new Gates.BasicGates.Or()) { }

        public Or(Gates.AbstractGate abgate) : base(abgate, "M 15,17 h 10 c 10,0 20,5 25,15 c -5,10 -15,15 -25,15 h -10 c 5,-10 5,-20 0,-30") 
        {
            this.SizeChanged += ShapeGate_SizeChanged; 
        }
    }

    public class Nor : ShapeGate
    {
        public Nor() : this(new Gates.BasicGates.Nor()) { }

        public Nor(Gates.AbstractGate abgate) : base(abgate, "M 15,17 h 5 c 10,0 20,5 25,15 c -5,10 -15,15 -25,15 h -5 c 5,-10 5,-20 0,-30 M 46,33.5 a 3,3 1 1 1 0.1,0.1") 
        {
            this.SizeChanged += ShapeGate_SizeChanged; 
        }

    }

    public class Nand : ShapeGate
    {
        public Nand() : this(new Gates.BasicGates.Nand()) { }

        public Nand(Gates.AbstractGate abgate) : base(abgate, "M 15,17 v 30 h 15 a 2,2 1 0 0 0,-30 h -15 M 46,33.5 a 3,3 1 1 1 0.1,0.1") 
        {
            this.SizeChanged += ShapeGate_SizeChanged; 
        }

    }

    public class Xor : ShapeGate
    {
        public Xor() : this(new Gates.BasicGates.Xor()) { }

        // we draw the outer curve twice, because otherwise it creates a block
        // and breaks the lines
        public Xor(Gates.AbstractGate abgate) : base(abgate, "M 13,47 c 5,-10 5,-20 0,-30 M 13,17 c 5,10 5,20 0,30 M 18,17 h 7 c 10,0 20,5 25,15 c -5,10 -15,15 -25,15 h -7 c 5,-10 5,-20 0,-30") { }
    }

    public class Xnor : ShapeGate
    {
        public Xnor() : this(new Gates.BasicGates.Xnor()) { }

        public Xnor(Gates.AbstractGate abgate) : base(abgate, "M 13,47 c 5,-10 5,-20 0,-30 M 13,17 c 5,10 5,20 0,30 M 18,17 h 2 c 10,0 20,5 25,15 c -5,10 -15,15 -25,15 h -2 c 5,-10 5,-20 0,-30 M 46,33.5 a 3,3 1 1 1 0.1,0.1") { }

    }

    public class Buffer : ShapeGate
    {
        public Buffer() : this(new Gates.BasicGates.Buffer()) { }

        public Buffer(Gates.AbstractGate abgate) : base(abgate, "M 12,12 v 8 l 8,-4 l -8,-4") 
        {
            this.Width = 32;
            this.Height = 32;
        }
    }
}
