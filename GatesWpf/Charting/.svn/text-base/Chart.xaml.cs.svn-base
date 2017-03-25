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
using System.ComponentModel;
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
    /// An oscilloscope window for monitoring multiple lines.
    /// The constructor accepts a circuit and monitors it for changes.
    /// The display also updates automatically every second.
    /// </summary>
    public partial class Chart : Window
    {
        private Dictionary<Gates.AbstractGate, ChartLine> chartLines = new Dictionary<Gates.AbstractGate,ChartLine>();
        private Dictionary<Gates.AbstractGate, TextBlock> chartLabels = new Dictionary<Gates.AbstractGate, TextBlock>();
        private BackgroundWorker timer;
        private Gates.Circuit c;
        private DateTime begin;
        private DateTime beginPause;
        private TimeSpan paused;
        private bool lockRight = true;

        
        /// <summary>
        /// Construct a chart based on the given circuit.
        /// </summary>
        /// <param name="c"></param>
        public Chart(Gates.Circuit c)
        {
            InitializeComponent();
            begin = DateTime.Now;

            foreach (Gates.AbstractGate ag in c)
            {

                AddGate(ag);  

            }
            this.c = c;
            c.ListChanged += c_ListChanged;

            timer = new BackgroundWorker();
            timer.WorkerReportsProgress = true;
            timer.WorkerSupportsCancellation = true;
            timer.ProgressChanged += timer_ProgressChanged;
            timer.DoWork += timer_DoWork;

            paused = new TimeSpan();

            slZoom.Value = 7;
            timer.RunWorkerAsync();
            
        }

        private void c_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    AddGate(c[e.NewIndex]);

                    break;
                case ListChangedType.ItemDeleted:
                    Gates.AbstractGate toDelete = null;
                    foreach (Gates.AbstractGate ag in chartLines.Keys)
                    {
                        if (!c.Contains(ag))
                        {
                            toDelete = ag;
                        }
                    }
                    if (toDelete != null)
                    {
                     
                        chartLabels[toDelete].Foreground = Brushes.Gray;
                        chartLines[toDelete].Stroke = Brushes.Gray;
                        chartLines[toDelete].IsPaused = true;

                    }
                    break;
            }
        }

        private void SetLabel(Gates.AbstractGate ag)
        {
            string value = ag.Name;
            TextBlock lblName = chartLabels[ag];

            // I want to make sure the label fits within the area alloted.
            // If not, shorten it until it does, add ...
            FormattedText ft;

            string txtForLbl = value;
            ft = new FormattedText(txtForLbl, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
                new Typeface(lblName.FontFamily, lblName.FontStyle, lblName.FontWeight, lblName.FontStretch),
                lblName.FontSize, lblName.Foreground);
            bool modified = false;

            while (ft.Width > 70)
            {
                modified = true;
                txtForLbl = txtForLbl.Substring(0, txtForLbl.Length - 1);
                ft = new FormattedText(txtForLbl + "...", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
                new Typeface(lblName.FontFamily, lblName.FontStyle, lblName.FontWeight, lblName.FontStretch),
                lblName.FontSize, lblName.Foreground);
            }
            if (modified)
                txtForLbl += "...";

            lblName.Text = txtForLbl;
            chartLines[ag].ToolTip = value;
        }

        private void SetColor(Gates.AbstractGate ag)
        {
            ChartLine cl = chartLines[ag];
            if (ag.NumberOfInputs > 0)
                cl.Stroke = Brushes.Blue;
            else
                cl.Stroke = Brushes.Red;

            if (ag is Gates.IOGates.Clock)
                cl.Stroke = Brushes.Green;

            if (ag is Gates.IOGates.AbstractNumeric)
                if (ag.NumberOfInputs > 0)
                    cl.Stroke = Brushes.DarkBlue;
                else
                    cl.Stroke = Brushes.DarkRed;

            chartLabels[ag].Foreground = cl.Stroke;
        }
        
        private void AddGate(Gates.AbstractGate ag)
        {
            if (chartLines.ContainsKey(ag))
            {
                // re-activate previously deleted gate
                chartLines[ag].JumpToOffset(((btnPause.IsChecked.Value ? beginPause : DateTime.Now) - begin - paused).TotalSeconds);
                chartLines[ag].IsPaused = btnPause.IsChecked.Value;
                SetColor(ag);

            }
            else if (ag is Gates.IOGates.UserIO ||
                    ag is Gates.IOGates.Clock || 
                    ag is Gates.IOGates.AbstractNumeric)
                {
                    double max = 1;
                    if (ag is Gates.IOGates.AbstractNumeric)
                        max = ((Gates.IOGates.AbstractNumeric)ag).MaxValue;

                // if the view is paused, base the offset on the start of pause
                // rather than current time
                // adjust by when the view began and how long total pause time
                // which doesn't include current pause
                    ChartLine cl = new ChartLine(0, max,
                        ((btnPause.IsChecked.Value ? beginPause : DateTime.Now) - begin - paused).TotalSeconds);
                    cl.Height = 45;


                    

                    cl.IsPaused = btnPause.IsChecked.Value;
                    lbLines.Items.Add(cl);
                    cl.MajorLine = tickRuler.MajorLine;
                    chartLines[ag] = cl;
                    TextBlock tb = new TextBlock();
                    tb.Height = 15;
                    tb.Margin = new Thickness(5, 15, 0, 15);

                    lbLineLabels.Items.Add(tb);
                    chartLabels[ag] = tb;

                    SetColor(ag);
                    SetLabel(ag);

                    ag.PropertyChanged += ag_PropertyChanged;

                    slZoom_ValueChanged(null, null); // reset the log value for the new line
                }
        }

        private void ag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Gates.AbstractGate ag = sender as Gates.AbstractGate;
            
            DateTime dt = DateTime.Now;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.PropertyName == "Name")
                {
                    SetLabel(ag);
                }
                else
                {
                    if (chartLines[ag].Stroke != Brushes.Gray)
                        chartLines[ag].DataArrives(GetValue(ag), dt);
    
                }
            }));

            
        }

        private void timer_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!e.Cancel)
            {
                System.Threading.Thread.Sleep(1000);
                timer.ReportProgress(0, null);
            }

        }

        private double GetValue(Gates.AbstractGate ag)
        {
            if (ag is Gates.IOGates.UserInput || ag is Gates.IOGates.Clock)
                return ag.Output[0] ? 1 : 0;

            if (ag is Gates.IOGates.UserOutput)
                return ag[0] ? 1 : 0;

            if (ag is Gates.IOGates.AbstractNumeric)
                return ((Gates.IOGates.AbstractNumeric)ag).IntValue;
                

            return 0;
        }

        private void timer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            foreach (KeyValuePair<Gates.AbstractGate, ChartLine> cls in chartLines)
            {
                if (chartLines[cls.Key].Stroke != Brushes.Gray)
                    cls.Value.DataArrives(GetValue(cls.Key));
                
                cls.Value.GenLine();
            }
        }

        private void lbLines_ItemsReordered(object sender, RoutedEventArgs e)
        {

            int oidx;// = lbLines.OriginalItemIndex;
            int nidx; //= lbLines.SelectedIndex;
            if (sender == lbLines)
            {
                oidx = lbLines.OriginalItemIndex;
                nidx = lbLines.SelectedIndex;
            }
            else
            {
                oidx = lbLineLabels.OriginalItemIndex;
                nidx = lbLineLabels.SelectedIndex;
            }

            object o = lbLines.Items[oidx];

            lbLines.Items.RemoveAt(oidx);
            lbLines.Items.Insert(nidx, o);

            o = lbLineLabels.Items[oidx];

            lbLineLabels.Items.RemoveAt(oidx);
            lbLineLabels.Items.Insert(nidx, o);

            lbLines.SelectedIndex = -1;
            lbLineLabels.SelectedIndex = -1;
        }

        

        private void lbLines_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalOffset == 0 && !lockRight)
                return;

            if (lockRight && e.HorizontalChange >= 0)
            {
                lbLinesScroller.ScrollToRightEnd();
            } 
            
            if (e.Source == lbLinesScroller)
            {
                tickRuler.Offset = e.HorizontalOffset;
                lbLineLabelsScroller.ScrollToVerticalOffset(e.VerticalOffset);
                lockRight = Math.Abs(e.ExtentWidth - e.ViewportWidth - e.HorizontalOffset) < 5;
            }
            
            
            
        }

        
        private void slZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (tickRuler == null)
                return;

            double hoff = lbLinesScroller.HorizontalOffset / tickRuler.PixelsPerSecond;

            double logVal;
            double minv = Math.Log(0.03);
            double maxv = Math.Log(1000);
            double scale = (maxv - minv) / (slZoom.Maximum - slZoom.Minimum);
            logVal = Math.Exp(minv+scale*(slZoom.Value-slZoom.Minimum));
            

            
            tickRuler.PixelsPerSecond = 1.0 / logVal;



            foreach (ChartLine cls in chartLines.Values)
            {
                cls.MajorLine = tickRuler.MajorLine;
                cls.Scale = logVal;
            }

            
            

        }

        private void btnPause_Checked(object sender, RoutedEventArgs e)
        {
            if (btnPause.IsChecked.Value)
            {
                beginPause = DateTime.Now;
            }
            else
            {
                paused += (DateTime.Now - beginPause);
            }

            foreach (ChartLine cls in chartLines.Values)
                if (cls.Stroke != Brushes.Gray) // hack: this means it's deleted, don't mess with it
                    cls.IsPaused = btnPause.IsChecked.Value;

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            foreach (ChartLine cls in chartLines.Values)
                cls.Reset();

            if (btnPause.IsChecked.Value)
                beginPause = DateTime.Now;

            paused = new TimeSpan();

            lbLinesScroller.ScrollToLeftEnd();
            tickRuler.Offset = 0;
            lockRight = true;

        }

       
    }
}
