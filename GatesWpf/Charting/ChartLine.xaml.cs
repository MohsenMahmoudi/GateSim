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

namespace GatesWpf.Charting
{
    /// <summary>
    /// A single line which shows a value over time.  Supports pausing, offseting,
    /// and various (positive) ranges.
    /// </summary>
    public partial class ChartLine : UserControl
    {
        private DateTime lastDataArrived;
        private double lastData;
        private bool _paused;
        private double minValue, maxValue;
        
        private List<Point> data;
        private double horzSoFar;
        private double _scale;
        private double _majorLine;
        private double _offset;

        /// <summary>
        /// Sets the distance between each major line marker, in pixels.
        /// </summary>
        public double MajorLine
        {
            set
            {
                _majorLine = Math.Max(1, value);
            }
        }

        /// <summary>
        /// Sets the scale (in 1 / pixels per second).
        /// </summary>
        public double Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                GenLine();
            }
        }

        /// <summary>
        /// Pauses or unpauses the line.  Paused lines ignore data and do not add length.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return _paused;
            }
            set
            {
                if (!_paused && data.Count > 0)
                {
                    DataArrives(lastData);
                    data.Add(new Point(horzSoFar, lastData));

                    horzSoFar = 0;
                    lastDataArrived = DateTime.Now;
                }
                _paused = value;
                if (!_paused)
                    lastDataArrived = DateTime.Now;
            }
        }

        /// <summary>
        /// Clears all data and restarts from the beginning.
        /// </summary>
        public void Reset()
        {
            data.Clear();
            _offset = 0;
            lastDataArrived = DateTime.Now;
        }

        /// <summary>
        /// Creates a chart line based on a given range and offset from the left.
        /// </summary>
        /// <param name="Minimum"></param>
        /// <param name="Maximum"></param>
        /// <param name="Offset"></param>
        public ChartLine(double Minimum, double Maximum, double Offset)
        {
            InitializeComponent();

            if (Minimum < 0)
                throw new ArgumentOutOfRangeException("Minimum must be zero or more");
            if (Minimum >= Maximum)
                throw new ArgumentOutOfRangeException("Maximum must be greater than minimum");

            _paused = true;
            minValue = Minimum;
            maxValue = Maximum;
            _offset = Offset;
            data = new List<Point>();
            _scale = 1;
        }
        public ChartLine() : this(0, 1, 0)
        {
            
        }

        /// <summary>
        /// Adds blank space and moves to the given offset from left.  offset is absolute.
        /// </summary>
        /// <param name="offset"></param>
        public void JumpToOffset(double offset)
        {
            data.Add(new Point(offset, -1));
        }

        /// <summary>
        /// Gets or sets the color used to paint the line.
        /// </summary>
        public Brush Stroke
        {
            get
            {
                return pLine.Stroke;
            }
            set
            {
                pLine.Stroke = value;
            }
        }

        private double ScaleValue(double value)
        {
            double vp = 1.0 - (value - minValue) / (maxValue - minValue);
            vp *= (Height - 10);
            return vp;
        }

        public void GenLine()
        {
            StringBuilder path;
            path = new StringBuilder();

            double initial_val = 0;
            
            if (data.Count != 0) 
                initial_val = data[0].Y;
            
            path.Append("M " + (_offset * _scale).ToString() + "," + ScaleValue(initial_val).ToString());
            foreach (Point p in data)
            {
                if (p.Y >= 0)
                    path.Append(" h " + (p.X * _scale).ToString() + " V " + ScaleValue(p.Y).ToString());
                else
                    path.Append(" M " + (p.X * _scale).ToString() + ",0");
            }

            pLine.Data = StreamGeometry.Parse(path.ToString() + " h " + (horzSoFar * _scale).ToString());

            StringBuilder dpath = new StringBuilder();
            dpath.Append("M 0,0");
            for (double x = _majorLine; x < pLine.ActualWidth; x += _majorLine)
            {
                dpath.Append(" M " + x.ToString() + ",-5 v " + Height.ToString());
            }
            dashedLine.Data = StreamGeometry.Parse(dpath.ToString());

            Width = pLine.ActualWidth;
        }

        /// <summary>
        /// Provides data to the line.  The when indicates when the line should act as if
        ///  the data had arrived.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="when"></param>
        public void DataArrives(double value, DateTime when)
        {
            if (value < minValue || value > maxValue)
                throw new ArgumentOutOfRangeException();

            if (IsPaused)
                return; // ignore arriving data when paused

            // length based on time since last data arrived,
            // 1 px = 1 sec

            double len = (when - lastDataArrived).TotalSeconds;



            if (value == lastData && data.Count > 0)
            {
                // just extend the line
                horzSoFar = len;
            }
            else
            {
                data.Add(new Point(len, value));

                horzSoFar = 0;
                lastData = value;
                lastDataArrived = when;
            }

            //GenLine();
        }

        /// <summary>
        /// Provides data to the line.  The current time is used as when the data arrived.
        /// </summary>
        /// <param name="value"></param>
        public void DataArrives(double value)
        {


            DataArrives(value, DateTime.Now);
           
        }
    }
}
