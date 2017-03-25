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
using System.Globalization;

/*
 *  Copyright (C) 2010 Steve Kollmansberger
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

namespace GatesWpf.Charting
{
    /// <summary>
    /// A ruler which marks off a line based on time elapsed.  Supports seconds and milliseconds resolutions.
    /// </summary>
    public partial class TickRuler : UserControl
    {
        private double _pixelsPerSecond = 1;
        private double _offset = 0;
        private double _majorLine = 1;

        public TickRuler()
        {
            InitializeComponent();
            SizeChanged += (s2, e2) => { GenLine(); };
        }

        /// <summary>
        /// What value to start the line at.
        /// </summary>
        public double Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                GenLine();
            }
        }

        /// <summary>
        /// Line resolution.
        /// </summary>
        public double PixelsPerSecond
        {
            get
            {
                return _pixelsPerSecond;
            }
            set
            {
                _pixelsPerSecond = value;
                GenLine();
            }
        }

        /// <summary>
        /// How many pixels apart the major lines are appearing.
        /// </summary>
        public double MajorLine
        {
            get
            {
                return _majorLine;
            }
        }
        

        private void GenLine()
        {
            // calculate div points
            StringBuilder dta = new StringBuilder();
            dta.Append("M "+(-Offset).ToString()+",0 v 10 v -10");
            double width = 0;
            int count = 0;
            int cnt_scale = 100;
            int skip = 1;
            labels.Children.Clear();

            double sixtyPxGivesMe = 100.0 / _pixelsPerSecond;

            labelType.Text = "Seconds";
            // First look for zoom-in millisecond scale
            if (sixtyPxGivesMe > 100)
            {
                while (sixtyPxGivesMe > 100)
                {
                    if (cnt_scale == 1)
                    {
                        cnt_scale *= 1000;
                        labelType.Text = "Milliseconds";
                    }
                    sixtyPxGivesMe /= 10.0;
                    cnt_scale /= 10;
                    
                }
            }
            else if (sixtyPxGivesMe < 10)
            {
                // zoom out multi-step seconds
                while (sixtyPxGivesMe < 10)
                {
                    sixtyPxGivesMe *= 10.0;
                    cnt_scale *= 10;
                    
                }
            }

            while (skip * sixtyPxGivesMe < 30)
                if (skip == 1)
                    skip = 5;
                else
                    skip += 5;

            _majorLine = sixtyPxGivesMe;

            while (width < ActualWidth + Offset)
            {
                if (sixtyPxGivesMe > 30)
                {
                    for (double x = 0; x < sixtyPxGivesMe / 2; x += sixtyPxGivesMe / 10)
                        dta.Append(" h " + (sixtyPxGivesMe / 10).ToString() + " v 5 v -5");
                    dta.Append(" v 8 v -8");
                    for (double x = 0; x < sixtyPxGivesMe / 2; x += sixtyPxGivesMe / 10)
                        dta.Append(" h " + (sixtyPxGivesMe / 10).ToString() + " v 5 v -5");
                }
                else
                {
                    dta.Append(" h " + sixtyPxGivesMe.ToString());
                    dta.Append(" v 5 v -5");

                    
                    
                }

                
                count++;
                if (count % skip == 0 && -Offset + count * sixtyPxGivesMe > 0)
                {
                    if (count / skip % 2 == 0 || sixtyPxGivesMe > 30)
                        dta.Append(" v 12 v -12");
                    else
                        dta.Append(" v 8 v -8");

                    TextBlock tb = new TextBlock() { Text = (count * cnt_scale).ToString() };
                    FormattedText ft = new FormattedText(tb.Text, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
                        new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                        tb.FontSize, tb.Foreground);
                    tb.Margin = new Thickness(-Offset + count * sixtyPxGivesMe - ft.Width / 2.0, 0, 0, 0);
                    labels.Children.Add(tb);
                }

                width += sixtyPxGivesMe;

            }
            
            pLine.Data = StreamGeometry.Parse(dta.ToString());
            

        }
    }
}
