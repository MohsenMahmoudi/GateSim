using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.ComponentModel;


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
    /// A general representation for a numeric gate.  A textbox, which may or may not
    /// allow user input, is provided.  The user may select a representation by clicking on
    /// the representation indicator at the bottom of the gate.
    /// </summary>
    public class Numeric : Gate
    {

        private Gates.IOGates.AbstractNumeric _nu;

        private static TerminalID[] CreateTIDs(Gates.IOGates.AbstractNumeric gate)
        {
            // note that only ONE of these can be true
            // but we do an add so that it works for whichever
            // one is selected
            TerminalID[] tids = new TerminalID[gate.NumberOfInputs + gate.Output.Length];
            int tidx = 0;
            // for the top line we invert the ordering
            // this matches standard IC numbering
            // AND provides for matching up of LSB
            for (int i = 0; i < gate.NumberOfInputs; i++)
                tids[tidx++] = new TerminalID(true, gate.NumberOfInputs - i - 1, Position.TOP);

            // for the bottom, we will purposefully
            // violate standard IC numbering
            // BECAUSE we want the visual appearance of
            // a binary # to line up with the illuminated ports
            for (int i = 0; i < gate.Output.Length; i++)
                tids[tidx++] = new TerminalID(false, gate.Output.Length - i - 1, Position.BOTTOM);
            
            return tids;
        }

        private TextBox nval;

        public Numeric(Gates.IOGates.AbstractNumeric gate)
             : base(gate, CreateTIDs(gate) ) {

                    _nu = gate;

                    

                    Rectangle r = new Rectangle();
                    r.Margin = new System.Windows.Thickness(15);
                    r.Width = this.Width - 30;
                    r.Height = this.Height - 30;
                    r.Stroke = Brushes.Black;
                    r.StrokeThickness = 2;
                    r.Fill = Brushes.White;
                    myCanvas.Children.Add(r);
            

                    nval = new TextBox();
                    // green team :
                    // set event handler to call GotFocus()
                    nval.GotFocus += new RoutedEventHandler(nval_GotFocus);
                    nval.Margin = new System.Windows.Thickness(20);
                    nval.FontFamily = new FontFamily("Courier New");
                    nval.FontSize = 12;
                    nval.TextAlignment = TextAlignment.Center;
                    nval.Width = this.Width - 40;
                    nval.Height = 18;
                    nval.Background = Brushes.AntiqueWhite;
                    

                    Binding bind = new Binding("Value");
                    bind.Source = _nu;
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bind.ValidatesOnExceptions = true;
                    nval.SetBinding(TextBox.TextProperty, bind);

            
                    Binding bindve = new Binding("(Validation.Errors)[0].Exception.InnerException.Message");
                    bindve.Source = nval;
                    bindve.Mode = BindingMode.OneWay;
                    nval.SetBinding(TextBox.ToolTipProperty, bindve);
            

                    myCanvas.Children.Add(nval);

                    TextBlock tb = new TextBlock();
                    tb.Margin = new Thickness(20, 36, 20, 20);
                    tb.FontSize = 8;
                    tb.Text = "!!!";
                    tb.Width = this.Width - 40;
                    tb.TextAlignment = TextAlignment.Center;
                    tb.ToolTip = "Click to Change Representation";
            
                    Binding bindrep = new Binding("SelectedRepresentation");
                    bindrep.Converter = new RepConverter();
                    bindrep.Source = _nu;
                    bindrep.Mode = BindingMode.OneWay;
                    tb.SetBinding(TextBlock.TextProperty, bindrep);

                    tb.MouseDown += new System.Windows.Input.MouseButtonEventHandler(tb_MouseDown);
                    tb.MouseEnter += new System.Windows.Input.MouseEventHandler(tb_MouseEnter);
                    tb.MouseLeave += new System.Windows.Input.MouseEventHandler(tb_MouseLeave);

            
                    myCanvas.Children.Add(tb);

        }
        //Green team:
        // wrote property to hold current canvas

        public GateCanvas MyGateCanvas { get; set; }

        // method for clearing all selections and giving focus to text box on bin input gate
        void nval_GotFocus(object sender, RoutedEventArgs e)
        {

            if(MyGateCanvas != null)   MyGateCanvas.ClearSelection();
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

        void tb_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsReadOnly)
            {
                ((TextBlock)sender).Foreground = Brushes.Black;
                ((TextBlock)sender).FontWeight = FontWeights.Regular;
            }
        }

        void tb_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsReadOnly)
            {
                ((TextBlock)sender).Foreground = Brushes.Blue;
                ((TextBlock)sender).FontWeight = FontWeights.Bold;
            }
        }

        void tb_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsReadOnly)
            switch (_nu.SelectedRepresentation)
            {
                case Gates.IOGates.AbstractNumeric.Representation.BINARY:
                    _nu.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.OCTAL;
                    break;
                case Gates.IOGates.AbstractNumeric.Representation.OCTAL:
                    _nu.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.DECIMAL;
                    break;
                case Gates.IOGates.AbstractNumeric.Representation.DECIMAL:
                    _nu.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL;
                    break;
                case Gates.IOGates.AbstractNumeric.Representation.HEXADECIMAL:
                    _nu.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.D2C;
                    break;
                case Gates.IOGates.AbstractNumeric.Representation.D2C:
                    if (_nu.Bits % 4 == 0)
                        _nu.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BCD;
                    else
                        _nu.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
                    break;
                case Gates.IOGates.AbstractNumeric.Representation.BCD:
                    _nu.SelectedRepresentation = Gates.IOGates.AbstractNumeric.Representation.BINARY;
                    break;

            }


        }

        public override Gate CreateUserInstance()
        {
            Numeric n;
            if (_nu is Gates.IOGates.NumericInput)
            {
                n = new Numeric(new Gates.IOGates.NumericInput(_nu.Bits));
                ((Gates.IOGates.NumericInput)n.AbGate).SelectedRepresentation = _nu.SelectedRepresentation;
                ((Gates.IOGates.NumericInput)n.AbGate).Value = _nu.Value;
            }
            else
            {
                n = new Numeric(new Gates.IOGates.NumericOutput(_nu.Bits));
                ((Gates.IOGates.NumericOutput)n.AbGate).SelectedRepresentation = _nu.SelectedRepresentation;
            }
            return n;
            

        }

        protected class RepConverter : IValueConverter
        {

            #region IValueConverter Members

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return value.ToString().Substring(0, 3);
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        
    }

    /// <summary>
    /// Class designed to provide a place holder for a numeric input
    /// and help create one.  If this is cloned (CreateUserInstance),
    /// a dialog asking the user to specify bits is presented.
    /// </summary>
    /// 
    // Green team notes:  This looks like it contains code to determine the user input
    public class DefaultNumericInput : Numeric
    {
        public DefaultNumericInput() : base(new Gates.IOGates.NumericInput(2)) { }

        public override Gate CreateUserInstance()
        {
            BitDialog bd = new BitDialog();
            bd.ShowDialog();
            if (bd.Bits != -1)
            {
                return new Numeric(new Gates.IOGates.NumericInput(bd.Bits));
            }
            else return null;
            

        }
    }

    /// <summary>
    /// Class designed to provide a place holder for a numeric output
    /// and help create one.  If this is cloned (CreateUserInstance),
    /// a dialog asking the user to specify bits is presented.
    /// </summary>
    class DefaultNumericOutput : Numeric
    {
        public DefaultNumericOutput() : base(new Gates.IOGates.NumericOutput(2)) { }

        public override Gate CreateUserInstance()
        {
            BitDialog bd = new BitDialog();
            bd.ShowDialog();
            if (bd.Bits != -1)
            {
                return new Numeric(new Gates.IOGates.NumericOutput(bd.Bits));
            }
            else return null;


        }
    }
}
