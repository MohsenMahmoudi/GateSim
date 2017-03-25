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
using System.Windows.Media.Animation;


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
    /// An animated wire which can show a flow of current from an origin to destination.
    /// </summary>
    public partial class Wire : UserControl
    {
        private TranslateTransform flowOffset;
        private LinearGradientBrush flow;
        private BezierSegment bz;
        private PathFigure pf;
        private DoubleAnimation da;
        private Point _origin, _dest;
        private delegate void SetFillDelegate(bool value);


        public Wire()
        {
            InitializeComponent();

            flow = new LinearGradientBrush(Colors.White, Colors.Red, 0);
            flow.MappingMode = BrushMappingMode.Absolute;
            flow.SpreadMethod = GradientSpreadMethod.Repeat;

            pf = new PathFigure();
            bz = new BezierSegment();
            pf.Segments.Add(bz);
            PathGeometry pg = new PathGeometry(new PathFigure[] { pf });
            Inner.Data = pg;
            Outer.Data = pg;

            Inner.Stroke = Brushes.White;
        }

        
        private void Recompute()
        {
            
            double x1,y1,x2,y2;
            x1 = _origin.X;
            y1 = _origin.Y;
            x2 = _dest.X;
            y2 = _dest.Y;
            pf.StartPoint = new Point(x1, y1);
            bz.Point1 = new Point(x1 * 0.6 + x2 * 0.4, y1 );
            bz.Point2 = new Point(x1 * 0.4 + x2 * 0.6,  y2);
            bz.Point3 = new Point(x2, y2);

            
            // if x2 == x1 or y2 == y1 then the flow will not happen
            // so we can "override" those cases with slightly fake values
            if (x2 == x1)
                x2 += 0.00001;
            if (y2 == y1)
                y2 += 0.00001;
            
            flow.StartPoint = pf.StartPoint;
            flow.EndPoint = bz.Point3;

            TransformGroup rt = new TransformGroup();
            rt.Children.Add(new ScaleTransform(10.0 / Math.Abs(x2 - x1), 10.0 / Math.Abs(y2 - y1)));
            
            flowOffset = new TranslateTransform(0, 0);

            if (x1 < x2)
                da =
                    new DoubleAnimation(0.0, 1.0, new Duration(new TimeSpan(0, 0, (int)Math.Abs(x2 - x1) / 10)));
            else
                da =
                    new DoubleAnimation(1.0, 0.0, new Duration(new TimeSpan(0, 0, (int)Math.Abs(x2 - x1) / 10)));

            da.RepeatBehavior = RepeatBehavior.Forever;
            da.IsCumulative = true;
            flowOffset.BeginAnimation(TranslateTransform.XProperty, da);

            if (y1 < y2)
                da =
                    new DoubleAnimation(0.0, 1.0, new Duration(new TimeSpan(0, 0, (int)Math.Abs(y2 - y1) / 10)));
            else
                da =
                    new DoubleAnimation(1.0, 0.0, new Duration(new TimeSpan(0, 0, (int)Math.Abs(y2 - y1) / 10)));


            da.RepeatBehavior = RepeatBehavior.Forever;
            da.IsCumulative = true;
            flowOffset.BeginAnimation(TranslateTransform.YProperty, da);

            rt.Children.Add(flowOffset);
            flow.RelativeTransform = rt;
        }

        

        private void setFill(bool value)
        {
            if (value)
            {
                Inner.Stroke = flow;
            }
            else
            {
                Inner.Stroke = Brushes.White;
            }
        }

       

        /// <summary>
        /// Set the flow (on or off) for this wire.  
        /// </summary>
        public bool Value
        {
            set
            {
                setFill(value);

            }
        }

        /// <summary>
        /// Gets or sets the origin point of this wire.
        /// </summary>
        public Point Origin
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
                Recompute();
            }
        }

        /// <summary>
        /// Gets or sets the destination point of this wire.
        /// </summary>
        public Point Destination
        {
            get
            {
                return _dest;
            }
            set
            {
                _dest = value;
                Recompute();
            }
        }
        
    }
}
