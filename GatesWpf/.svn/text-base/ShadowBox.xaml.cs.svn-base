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
    /// The shadow box is a visual wrapper that provides a rounded yellow border
    /// and supports mouse-over fading.
    /// </summary>
    public partial class ShadowBox : UserControl
    {
        public ShadowBox()
        {
            InitializeComponent();
            //Grid1.Children.Remove(mask);
        }

        public Orientation Orientation
        {
            get
            {
                return spContent.Orientation;
            }
            set
            {
                spContent.Orientation = value;
            }
        }

        public UIElementCollection Children
        {
            get
            {
                return spContent.Children;
            }
        }
        

    }
}
