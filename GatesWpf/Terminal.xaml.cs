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
    /// A visual representation of a connection terminal.  The terminal has an arrow
    /// which indicates whether it is an input or output terminal.  It supports a true/false
    /// value fill, as well as a "light up" indication designed to support hinting to users
    /// that it may be connected.
    /// 
    /// Note that the terminal is an endpoint -- it doesn't do anything or propagate anything,
    /// it only displays what it is told to display.
    /// </summary>
    public partial class Terminal : UserControl
    {
        private bool _high = false;

        public Terminal(bool isInput)
        {
            InitializeComponent();
            polyInput.Visibility = isInput ? Visibility.Visible : Visibility.Hidden;
            polyOutput.Visibility = (!isInput) ? Visibility.Visible : Visibility.Hidden;

        }

        
        /// <summary>
        /// Gets or sets the highlight state, usually used to indicate if this terminal is connectable.
        /// </summary>
        public bool Highlight
        {
            get
            {
                return _high;
            }
            set
            {
                _high = value;
                if (value)
                    elSelector.Fill = Brushes.LightGreen;
                else
                    elSelector.Fill = Brushes.White;
            }
        }

        private void setFill(bool value)
        {
            if (value)
            {
                polyInput.Fill = Brushes.Red;
                polyOutput.Fill = Brushes.Red;
            }
            else
            {
                polyInput.Fill = Brushes.DarkGray;
                polyOutput.Fill = Brushes.DarkGray;
            }
        }

        /// <summary>
        /// Sets the true/false value of this terminal.
        /// </summary>
        public bool Value
        {
            set
            {
                setFill(value);
                
            }
        }
    }
}
