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

namespace GatesWpf
{
    /// <summary>
    /// A panel for user gate selection via drag/drop.  Two main parts:
    /// The top parts are the basic, compound, and I/O gates; all the gates
    /// provided internally and fixed within the program.  The bottom part
    /// is user defined gates (ICs), which may change.  To facilitate this,
    /// an IC List should be provided and will be monitored for changes.
    /// Users can also initiate changes to an IC by double-clicking.
    /// </summary>
    public partial class GateSelector : UserControl
    {


        private UndoRedo.UndoManager undoProvider;
        private bool _ro = false;
        private string _icname;
        private ICList icl;
        private const double MAX_SIZE = 100;


        /// <summary>
        /// If an undo manager is provided, changes to ICs will be undoable.
        /// </summary>
        public UndoRedo.UndoManager UndoProvider
        {
            set
            {
                undoProvider = value;
            }
        }

        
        /// <summary>
        /// Set read only to disable editting of user ICs
        /// </summary>
        public bool IsReadOnly
        {
            set
            {
                foreach (Gate g in spGates.Children)
                {
                    g.IsReadOnly = value;
                    g.ContextMenu.IsEnabled = !value;
                    SetInfoLine(g as UIGates.IC);
                }
                _ro = value;

            }
        }

        private void SetInfoLine(UIGates.IC ic)
        {
            string inf = "Left-drag to place";
            
            if (!_ro)
                inf += ", double-click to edit, type to rename";
            InfoLine.SetInfo(ic, inf);
        }
        
        /// <summary>
        /// If this selector is being used in a window that is editting an IC,
        /// you don't want them to put that IC into itself, or another in such a
        /// way as to create a recursive loop.  Setting the IC name of the IC being
        /// editted, if any, disables ICs that would create loops.
        /// </summary>
        public string ICName
        {
            set
            {
                _icname = value;
                foreach (UIGates.IC ic in spGates.Children)
                {
                    if (((Gates.IC)ic.AbGate).DeepIncludes(value))
                        ic.Visibility = Visibility.Collapsed;
                    else
                        ic.Visibility = Visibility.Visible;
                }
            }
        }

        
        /// <summary>
        /// The IC List is the provider of ICs for the user gates area, and also
        /// where changes are sent.
        /// </summary>
        public ICList ICList
        {
            set
            {
                if (icl != null)
                {
                    icl.ListChanged -= new System.ComponentModel.ListChangedEventHandler(icl_ListChanged);
                    icl.ChangeIC -= new ICList.ChangeICEventHandler(icl_ChangeIC);
                    spGates.Children.Clear();
                }
                icl = value;
                foreach (UIGates.IC nic in icl)
                    AddDragDropGate( (UIGates.IC)nic.CreateUserInstance());

                icl.ListChanged += new System.ComponentModel.ListChangedEventHandler(icl_ListChanged);
                icl.ChangeIC += new ICList.ChangeICEventHandler(icl_ChangeIC);
            }
        }

        private void icl_ChangeIC(object sender, ICList.ChangeICEventArgs e)
        {
            int idx = -1;
            for (int i = 0; i < spGates.Children.Count; i++)
                if (((UIGates.IC)spGates.Children[i]).AbGate.Name == e.original.AbGate.Name)
                    idx = i;
            spGates.Children.RemoveAt(idx);
            
            if (e.newic != null)
                AddDragDropGate(idx, (UIGates.IC)e.newic.CreateUserInstance());
        }

        private void icl_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            // we have to clone these because there could be multiple selectors
            // operating off of a single master ICList
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    AddDragDropGate(e.NewIndex, (UIGates.IC)icl[e.NewIndex].CreateUserInstance());
                    break;
                case ListChangedType.ItemChanged:
                    // replace the gate as needed
                    spGates.Children.RemoveAt(e.NewIndex);
                    AddDragDropGate(e.NewIndex, (UIGates.IC)icl[e.NewIndex].CreateUserInstance());
                    break;
                case ListChangedType.Reset:
                    ICList = icl;
                    break;
            }
        }

        public GateSelector()
        {
            InitializeComponent();

            tbAnd.DataContext = new UIGates.And();
            tbNand.DataContext = new UIGates.Nand();
            tbNot.DataContext = new UIGates.Not();
            tbOr.DataContext = new UIGates.Or();
            tbNor.DataContext = new UIGates.Nor();
            tbXor.DataContext = new UIGates.Xor();
            tbXnor.DataContext = new UIGates.Xnor();
            tbBuffer.DataContext = new UIGates.Buffer();

            tbUserInput.DataContext = new UIGates.UserInput();
            tbUserOutput.DataContext = new UIGates.UserOutput();
            tbNumericOutput.DataContext = new UIGates.DefaultNumericOutput();
            tbNumericInput.DataContext = new UIGates.DefaultNumericInput();
            tbClock.DataContext = new UIGates.Clock();
            tbComment.DataContext = new UIGates.Comment();

            tbUserInput.IsReadOnly = true;
            tbUserOutput.IsReadOnly = true;
            tbNumericInput.IsReadOnly = true;
            tbNumericOutput.IsReadOnly = true;
            tbClock.IsReadOnly = true;
            tbComment.IsReadOnly = true;
            
           
        }

        /// <summary>
        /// Find the given named user IC and set it to edit name mode
        /// </summary>
        /// <param name="icname"></param>
        public void SetEditName(string icname)
        {
            foreach (UIGates.IC g in spGates.Children)
            {
                if (g.AbGate.Name == icname && g.Visibility == Visibility.Visible)
                {
                    g.SetEditName();
                }
            }
        }

        

        private void AddDragDropGate(int pos, UIGates.IC g)
        {
            g.DataContext = g.CreateUserInstance();
            

            DragDrop.DragDropHelper.SetIsDragSource(g, true);
            DragDrop.DragDropHelper.SetDragDropControl(g, new DragDrop.GateDragDropAdorner());
            DragDrop.DragDropHelper.SetDropTarget(g, "gateCanvas");
            DragDrop.DragDropHelper.SetAdornerLayer(g, "adornerLayer");


            g.PreviewICNameChanged += (object sender2, string newname, ref bool cancel) =>
            {
                if (newname == "")
                    cancel = true;

                foreach (Gate g2 in icl)
                {
                    if (newname == g2.AbGate.Name)
                        cancel = true;
                }
            };

            g.ICNameChanged += (sender2, newname) =>
            {
                UIGates.IC oic = icl.GetIC((g.AbGate.Name));
                UIGates.IC nic = g.CreateUserInstance(newname);
                icl[icl.IndexOf(oic)] = nic;
                if (undoProvider != null)
                    undoProvider.Add(new UndoRedo.ReplaceIC(icl, oic, nic));

            };

            ScaleTransform st = new ScaleTransform();
            st.CenterX = g.Width / 2.0;
            st.CenterY = g.Height / 2.0;
            double fac = 1.0;
            if (g.Width > MAX_SIZE)
                fac = Math.Min(MAX_SIZE / g.Width, fac);

            if (g.Height > MAX_SIZE)
                fac = Math.Min(MAX_SIZE / g.Height, fac);
            st.ScaleY = fac;
            st.ScaleX = fac;
            g.LayoutTransform = st;


            g.ContextMenu = new ContextMenu();
            MenuItem exp = new MenuItem();
            exp.Header = "Export...";
            exp.Click += (sender2, e2) =>
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".ic";
                dlg.Filter = "IC (.ic)|*.ic";
                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    CircuitXML cxml = new CircuitXML(icl);
                    try
                    {
                        cxml.Save(dlg.FileName, g);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to save IC: " + ex.ToString());
                    }
                }
            };
            g.ContextMenu.Items.Add(exp);
            MenuItem del = new MenuItem();
            del.Header = "Delete";
            del.Click += (sender2, e2) =>
            {
                if (MessageBox.Show("All instances of this IC in all circuits will be removed.  This operation cannot be undone.  Proceed?", "Danger Zone", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    UIGates.IC todel = icl.GetIC(g.AbGate.Name);

                    icl.Remove(todel);
                    if (undoProvider != null)
                        undoProvider.Clear();
                }
            };
            g.ContextMenu.Items.Add(del);
            MenuItem hid = new MenuItem();
            hid.Header = "Hide";
            hid.Click += (sender2, e2) =>
            {
                g.Visibility = Visibility.Collapsed;
            };
            //g.ContextMenu.Items.Add(hid);

            spGates.Children.Insert(pos, g);
            g.MouseDoubleClick += new MouseButtonEventHandler(g_MouseDoubleClick);
            expUserGates.IsExpanded = true;
            g.BringIntoView();
            g.IsReadOnly = _ro;
            g.ContextMenu.IsEnabled = !_ro;


            if (!string.IsNullOrEmpty(_icname))
                if (((Gates.IC)g.AbGate).DeepIncludes(_icname))
                    g.Visibility = Visibility.Collapsed;

            ((Gates.IC)g.AbGate).Circuit.Start();

            SetInfoLine(g);
            
        }


        private void AddDragDropGate(UIGates.IC g)
        {
            AddDragDropGate(spGates.Children.Count, g);
        }

        
        private void g_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s2, e2) => { System.Threading.Thread.Sleep(500); };
            bg.RunWorkerCompleted += (s2, e2) => { DragDrop.DragDropHelper.Cancel(); };

            if (_ro)
                return;

            foreach (Window w in Application.Current.Windows)
            {
                if (((Window1)w).BasedOn != null &&
                    ((Window1)w).MyEditLevel == Window1.EditLevel.EDIT &&
                    ((Window1)w).BasedOn.AbGate.Name == ((UIGates.IC)sender).AbGate.Name)
                {
                    w.Activate();
                    return;
                }
            }

            UIGates.IC template = ((UIGates.IC)sender).CreateUserInstance() as UIGates.IC;
            ((Gates.IC)template.AbGate).Circuit.Start();
            Window1 icw = new Window1(template, icl, Window1.EditLevel.EDIT);


            icw.Show();
            icw.Closing += (s2, e2) =>
            {
                try
                {

                    
                    // only replace gates if changes made
                    if (!icw.gateCanvas.UndoProvider.IsAtSavePoint)
                    {
                        UIGates.IC oic = icl.GetIC(icw.BasedOn.AbGate.Name);
                        UIGates.IC nic = icw.GetIC();

                        // check for recursion
                        // can bypass the selector if you are sneaky
                        foreach (Gates.AbstractGate ag in ((Gates.IC)nic.AbGate).Circuit)
                        {
                            if (ag is Gates.IC)
                            {
                                if (((Gates.IC)ag).DeepIncludes(((UIGates.IC)nic).AbGate.Name)) 
                                {
                                    MessageBox.Show("Recursive circuit detected");
                                    return;
                                }
                            }
                        }

                        // check for decreased inputs
                        // can't undo
                        if (oic.AbGate.NumberOfInputs > nic.AbGate.NumberOfInputs)
                        {
                            if (MessageBox.Show("Reducing the number of inputs will affect all instances of this IC in all circuits.  This operation cannot be undone. Proceed?", "Danger Zone", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
                                return;


                        }

                        icl[icl.IndexOf(oic)] = nic;
                        if (undoProvider != null)
                            undoProvider.Add(new UndoRedo.ReplaceIC(icl, oic, nic));

                    }

                }
                catch (Exception) { } // can fail if this IC has been removed

                ((Gates.IC)template.AbGate).Circuit.Stop();

            };

            icl.ChangeIC += (s2, e2) =>
            {
                if (e2.original.AbGate.Name == ((UIGates.IC)sender).AbGate.Name)
                {
                    if (e2.newic == null)
                    {
                        icw.Close();
                    }
                    else
                    {
                        // find the gate being edited
                        foreach (UIGates.IC g in spGates.Children)
                            if (g.AbGate.Name == e2.newic.AbGate.Name)
                                icw.RefreshGateCanvas(g.CreateUserInstance() as UIGates.IC);
                    }
                }
            };
            
            

        }



        
    }

    public class ExpanderHeightConverter : IMultiValueConverter
    {

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result = 1.0;
            foreach (object v in values)
                if (v is double)
                    result *= (double)v;

            result += 22; // minimum height to show header

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
