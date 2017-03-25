using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;


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
    /// A user configurable clock.  The user can type in the period.
    /// </summary>
    public class Clock : Gate
    {
        private Gates.IOGates.Clock _clock;
        private TextBox nval;

        public Clock() : this(new Gates.IOGates.Clock(0)) { }

        public Clock(Gates.IOGates.Clock gate)
             : base(gate, new TerminalID[] { new TerminalID(false, 0, Position.TOP) } ) {

                    _clock = gate;

                    

                    Rectangle r = new Rectangle();
                    r.Margin = new System.Windows.Thickness(5,17,5,17);
                    r.Width = this.Width - 10;
                    r.Height = this.Height - 34;
                    r.Stroke = Brushes.Black;
                    r.StrokeThickness = 2;
                    r.Fill = Brushes.White;
                    myCanvas.Children.Add(r);

                    Path ph = new Path();
                    ph.Data = StreamGeometry.Parse("M 10,22 h 5 v 5 h -5 v 5 h 5 v 5 h -5 v 5 h 5");
                    ph.Stroke = Brushes.Black;
                    ph.StrokeThickness = 2;
                    ph.Fill = Brushes.White;
                    myCanvas.Children.Add(ph);

                    nval = new TextBox();
                    // green team :
                    // set event handler to call GotFocus()
                    nval.GotFocus += new RoutedEventHandler(nval_GotFocus);
                    nval.Margin = new System.Windows.Thickness(20,23,10,23);
                    nval.FontFamily = new FontFamily("Courier New");
                    nval.FontSize = 12;
                    nval.TextAlignment = TextAlignment.Center;
                    nval.Width = 34;
                    nval.Height = 18;
                    nval.Background = Brushes.AntiqueWhite;
                    

                    Binding bind = new Binding("Milliseconds");
                    bind.Source = _clock;
                    bind.FallbackValue = "0";
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bind.ValidatesOnExceptions = true;
                    nval.SetBinding(TextBox.TextProperty, bind);

            
                    Binding bindve = new Binding("(Validation.Errors)[0].Exception.InnerException.Message");
                    bindve.Source = nval;
                    bindve.Mode = BindingMode.OneWay;
                    bindve.FallbackValue = "Clock period in milliseconds";
                    nval.SetBinding(TextBox.ToolTipProperty, bindve);
            

                    myCanvas.Children.Add(nval);


        }


        //Green team:
        // wrote property to hold current canvas

        public GateCanvas MyGateCanvas { get; set; }

        // method for clearing all selections and giving focus to text box on bin input gate
        void nval_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MyGateCanvas != null) MyGateCanvas.ClearSelection();
            nval.Focus();
        }

        public override bool IsReadOnly
        {
            get
            {
                return base.IsReadOnly;
            }
            set
            {
                base.IsReadOnly = value;
                if (nval != null)
                    nval.IsReadOnly = value;
            }
        }

        public override Gate CreateUserInstance()
        {
            return new Clock(new Gates.IOGates.Clock(_clock.Milliseconds));
        }

    }
}
