using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GatesWpf.UIGates
{
    class And : Gate
    {
        public And() : this(new Gates.BasicGates.And()) { }

        public And(Gates.BasicGates.And mand) : base(mand,
            new TerminalID[] { new TerminalID(true, 0, Position.LEFT),
                new TerminalID(true, 1, Position.LEFT),
                new TerminalID(false, 0 , Position.RIGHT)}) {


            Path ph = new Path();
            ph.Data = StreamGeometry.Parse("M 15,12 v 40 h 15 a 2,2 1 0 0 0,-40 h -15");
            ph.Stroke = Brushes.Black;
            ph.StrokeThickness = 2;
            ph.Fill = Brushes.White;
            myCanvas.Children.Add(ph);
                        

        }

        


    }
}
