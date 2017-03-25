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
    /// Presents a dialog which prompts the user to select a number of bits,
    /// from 2 to 16, for a numeric i/o control.
    /// </summary>
    public partial class BitDialog : Window
    {
        public BitDialog()
        {
            InitializeComponent();
            _bits = -1;
        }

        protected int _bits;

        /// <summary>
        /// Number of bits selected by the user.  This is set when the user
        /// selects OK.  If the user cancels the dialog, this value is -1.
        /// </summary>
        public int Bits
        {
            get
            {
                return _bits;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _bits = (int)bitSlider.Value;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _bits = -1;
            Close();
        }
    }
}
