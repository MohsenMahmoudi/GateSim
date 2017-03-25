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

namespace GatesWpf
{
    /// <summary>
    /// Provides a save, don't save, cancel dialog.  Could be done with
    /// MessageBox and yes, no, cancel; but that's not quite as easy
    /// to understand.
    /// </summary>
    public partial class SaveClose : Window
    {

        private Result _r = Result.CANCEL;


        public enum Result
        {
            SAVE, DONT_SAVE, CANCEL
        }

        
        /// <summary>
        /// Result selected by the user.  Defaults to CANCEL
        /// </summary>
        public Result Selected
        {
            get
            {
                return _r;
            }
        }

        public SaveClose(string circuitName)
        {
            InitializeComponent();

            lblCircuit.Text = String.Format(lblCircuit.Text, circuitName);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _r = Result.SAVE;
            Close();
        }

        private void btnDontSave_Click(object sender, RoutedEventArgs e)
        {
            _r = Result.DONT_SAVE;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _r = Result.CANCEL;
            Close();
        }


    }
}
