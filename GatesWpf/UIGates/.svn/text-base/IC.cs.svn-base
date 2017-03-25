using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Globalization;
using System.Windows;


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
    /// An integrated circuit.  In addition to the IC itself, this class maintains
    /// location information for all gates within the IC (in the form of location "hints")
    /// as well as sequencing input and output terminals on the four sides.
    /// If allowed, the user may interactively rename the IC.
    /// </summary>
    public class IC : Gate
    {

        public Dictionary<Gates.AbstractGate, GateLocation> locationHints;

        /// <summary>
        /// Given a map of old gate to new gate, update the location hints to reflect the change.
        /// </summary>
        /// <param name="replacements"></param>
        public void UpdateLocationHints(Dictionary<Gates.AbstractGate, Gates.AbstractGate> replacements) 
        {
            foreach (KeyValuePair<Gates.AbstractGate, Gates.AbstractGate> replacement in replacements)
            {
                if (locationHints.ContainsKey(replacement.Key)) 
                {
                    if (replacement.Value != null)
                        locationHints.Add(replacement.Value, locationHints[replacement.Key]);
                    locationHints.Remove(replacement.Key);
                }
            }

        }

        private TextBox nm;
        private Rectangle r;
        private void ResizeDueToName()
        {
            FormattedText ft = new FormattedText(_gate.Name, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
               new Typeface(nm.FontFamily, nm.FontStyle, nm.FontWeight, nm.FontStretch),
               nm.FontSize, nm.Foreground);

            // I would assume 40 b/c that is the total below
            // for diff between label width and gate width
            // but it is not enough
            // hacking it
            this.Width = Math.Max(this.Width, ft.Width + 50);

            r.Width = this.Width - 24;
            r.Height = this.Height - 34;

            nm.Width = this.Width - 40;
            nm.Height = 24;
        }

        public IC(Gates.IC gate, TerminalID[] termsid)
            : base(gate, termsid)
        {
            
            locationHints = new Dictionary<Gates.AbstractGate, GateLocation>();

            // may need to length the gate symbol
            // to accomodate the label
           
            
            // load the terminal tooltips
            foreach (TerminalID tid in _termsid)
            {
                Gates.AbstractGate ab;
                if (tid.isInput)
                    ab = gate.Inputs[tid.ID];
                else
                    ab = gate.Outputs[tid.ID];
                tid.t.ToolTip = ab.Name;
            }
           


            r = new Rectangle();
            r.Margin = new System.Windows.Thickness(12,17,12,17);
            r.Width = this.Width - 24;
            r.Height = this.Height - 34;
            r.Stroke = Brushes.Black;
            r.StrokeThickness = 2;
            r.Fill = Brushes.White;
            myCanvas.Children.Add(r);

            
            nm = new TextBox();
            nm.Text = gate.Name;
            nm.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            nm.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            nm.TextAlignment = TextAlignment.Center;
            nm.Margin = new System.Windows.Thickness(20, this.Height / 2 - 12, 20, this.Height / 2 - 12);
            nm.Width = this.Width - 40;
            nm.Height = 24;
            nm.BorderThickness = new Thickness(0);
            IsReadOnly = true;
            myCanvas.Children.Add(nm);

            nm.LostFocus += new RoutedEventHandler(nm_LostFocus);
            nm.KeyDown += new System.Windows.Input.KeyEventHandler(nm_KeyDown);
            ResizeDueToName();
        }

        void nm_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                this.Focus(); // take away from textbox
            }
        }

        public delegate void ICNameChangeEventHandler(object sender, string newName);
        public event ICNameChangeEventHandler ICNameChanged;

        public delegate void PreviewICNameChangeEventHandler(object sender, string newName, ref bool cancel);
        public event PreviewICNameChangeEventHandler PreviewICNameChanged;

        void nm_LostFocus(object sender, RoutedEventArgs e)
        {

            if (nm.Text != AbGate.Name )
            {
                bool cancel = false;
                if (PreviewICNameChanged != null)
                    PreviewICNameChanged(this, nm.Text, ref cancel);

                if (cancel)
                    nm.Text = AbGate.Name;

                if (!cancel && ICNameChanged != null)
                    ICNameChanged(this, nm.Text);
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return nm.IsReadOnly;
            }
            set
            {
                base.IsReadOnly = value;
                if (nm != null)
                    nm.IsReadOnly = value;
            }
        }

        public void SetEditName()
        {
            IsReadOnly = false;
            Focusable = true;
            nm.SelectAll();
            BringIntoView();
            nm.Focus();
            System.Windows.Input.Keyboard.Focus(nm);

        }


        

        private TerminalID[] CloneTerminals()
        {
            // duplicate the term ids because they have individual
            // refernece to actual terminal
            TerminalID[] ntid = new TerminalID[_termsid.Length];
            for (int i = 0; i < ntid.Length; i++)
                ntid[i] = new TerminalID(_termsid[i].isInput, _termsid[i].ID, _termsid[i].pos);

            return ntid;
        }

        private void DuplicateLocationHinting(UIGates.IC target)
        {
            // duplicate the location hinting
            foreach (KeyValuePair<Gates.AbstractGate, GateLocation> hint in locationHints)
            {
                int cidx = ((Gates.IC)_gate).Circuit.IndexOf(hint.Key);
                target.locationHints.Add( ((Gates.IC)target.AbGate).Circuit[cidx], hint.Value);
            }
        }

        /// <summary>
        /// The "template" here is used as a visual template, that is, a descriptor of
        /// how the terminals should be laid out and a provided of location hints.
        /// The IC becomes the actual circuit within the return value.
        /// Accordingly, it is assumed that the template will be of the same
        /// "type" as the IC.
        /// </summary>
        /// <param name="ic"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static IC CreateFromTemplate(Gates.IC ic, IC template)
        {
            UIGates.IC nuic = new UIGates.IC( ic, template.CloneTerminals());
            template.DuplicateLocationHinting(nuic);
            return nuic;
            
        }

        /// <summary>
        /// Clone the IC with terminals and location hinting.  The IC may optionally
        /// be renamed in this cloning process.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IC CreateUserInstance(string name)
        {
            Gates.IC nic = (Gates.IC)((Gates.IC)_gate).Clone(name);

            // duplicate the term ids because they have individual
            // refernece to actual terminal
            IC nuic = new IC(nic, CloneTerminals());

            DuplicateLocationHinting(nuic);

            return nuic;
        }

        /// <summary>
        /// Clone the IC with terminals and location hinting.
        /// </summary>
        /// <returns></returns>
        public override Gate CreateUserInstance()
        {
            return CreateUserInstance(_gate.Name);
        }
    }
}
