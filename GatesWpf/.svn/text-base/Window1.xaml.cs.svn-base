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
using System.Reflection;
using System.Windows.Media.Animation;


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
    /// The main window, used for all purposes.  Instances may be made for
    /// editting and/or viewing ICs.
    /// </summary>
    public partial class Window1 : Window
    {

        public static string APP_TITLE;
        public static string APP_VERSION;
        public static string APP_COPYRIGHT;

        public static string LOAD_ON_START = "";

        private ICList icl;
        private string _filename;
        private EditLevel _myEditLevel;
        private ShadowBox sbZoom, sbSpeed, sbGates;
        private UIGates.IC _basedon = null;
        private UndoRedo.Transaction moves;
        private Line ICBounds_Line1, ICBounds_Line2;
        private Rectangle ICBounds;
        
        
        
        /// <summary>
        /// Replace the existing canvas with a new canvas, based on the IC given.
        /// This is similar to closing this window and replacing it with another window
        /// based on that IC instead.
        /// </summary>
        /// <param name="newgcic"></param>
        public void RefreshGateCanvas(UIGates.IC newgcic)
        {
            Grid1.Children.Remove(gateCanvas);

            gateCanvas.Circuit.Stop();

            gateCanvas = new GateCanvas(newgcic, icl);
            gateCanvas.UndoProvider = (UndoRedo.UndoManager)Resources["undoManager"];
            this.UnregisterName("gateCanvas");
            this.RegisterName("gateCanvas", gateCanvas);
            Grid1.Children.Insert(0 /*Grid1.Children.Count - 4*/,gateCanvas);
            Grid.SetColumn(gateCanvas, 1);
            Grid.SetRow(gateCanvas, 1);
            _basedon = newgcic;

            if (!string.IsNullOrEmpty(newgcic.AbGate.Name))
            {
                _filename = newgcic.AbGate.Name;
                spGates.ICName = newgcic.AbGate.Name;
                UpdateTitle();
            }

            if (MyEditLevel == EditLevel.FULL || MyEditLevel == EditLevel.EDIT)
            {
                // monitor the clipboard to provide cut/copy/paste visibility
                gateCanvas.selected.ListChanged += (s2, e2) =>
                {
                    btnCopy.IsEnabled = gateCanvas.selected.Count > 0;
                    btnCut.IsEnabled = gateCanvas.selected.Count > 0;
                    btnCopyAsImage.IsEnabled = gateCanvas.selected.Count > 0;
                };
            }

            if (_myEditLevel == EditLevel.FULL)
                gateCanvas.Circuit.Start();

            gateCanvas.SetCaptureICLChanges();

            gateCanvas.Zoom = slZoom.Value;

            gateCanvas.UpdateLayout();
            gateCanvas.UpdateWireConnections();

            
            
        }

        /// <summary>
        /// The "edit" permissions of a window
        /// </summary>
        public enum EditLevel
        {
            /// <summary>
            /// Full applies only to the main window.  Full indicates all application
            /// control.  Only one full window should exist.
            /// </summary>
            FULL, 

            /// <summary>
            /// Edit applies changes to be applied to the circuit, but not application-level
            /// operations like creating an IC or saving.
            /// </summary>
            EDIT, 

            /// <summary>
            /// View allows only observation of a circuit.
            /// </summary>
            VIEW
        }

        
        /// <summary>
        /// Gets the edit level of this window
        /// </summary>
        public EditLevel MyEditLevel
        {
            get
            {
                return _myEditLevel;
            }
        }


        #region Constructors
        protected Window1(EditLevel e)
        {
            InitializeComponent();
            _myEditLevel = e;

            EventDispatcher.BatchDispatcher = Dispatcher;

            gateCanvas.Circuit.Start();
            

            // Everybody gets zoom
            sbZoom = new ShadowBox();
            sbZoom.Margin = new Thickness(20);
            Grid1.Children.Remove(spZoom);
            sbZoom.Children.Add(spZoom);
            spZoom.Background = Brushes.Transparent;
            sbZoom.VerticalAlignment = VerticalAlignment.Top;
            sbZoom.HorizontalAlignment = HorizontalAlignment.Right;
            Grid1.Children.Add(sbZoom);
            Grid.SetColumn(sbZoom, 1);
            Grid.SetRow(sbZoom, 1);

            // everybody gets view keys
            this.PreviewKeyDown += new KeyEventHandler(Window1_View_KeyDown);

            Grid1.Children.Remove(spGates);
            if (e == EditLevel.FULL ||
                e == EditLevel.EDIT)
            {
                

                // delete for edit or full
                this.PreviewKeyDown += new KeyEventHandler(Window1_EditFull_KeyDown);

                this.PreviewKeyUp += (s2, e2) =>
                {
                    // add moves if needed
                    if (moves != null)
                        ((UndoRedo.UndoManager)Resources["undoManager"]).Add(moves);
                    
                    moves = null;
                };

                // drag/drop for edit or full
                DragDrop.DragDropHelper.ItemDropped += new EventHandler<DragDrop.DragDropEventArgs>(DragDropHelper_ItemDropped);

                // gates for edit or full

                sbGates = new ShadowBox();
                sbGates.Margin = new Thickness(20, 20, 20, 20);
                sbGates.Children.Add(spGates);
                spGates.Background = Brushes.Transparent;
                sbGates.VerticalAlignment = VerticalAlignment.Center;
                sbGates.HorizontalAlignment = HorizontalAlignment.Left;
                Grid1.Children.Add(sbGates);
                Grid.SetColumn(sbGates, 1);
                Grid.SetRow(sbGates, 1);

                // edit or full get undo and edit
                tbUndo.Visibility = Visibility.Visible;
                tbEdit.Visibility = Visibility.Visible;

                // monitor the clipboard to provide cut/copy/paste visibility
                gateCanvas.selected.ListChanged += (s2, e2) =>
                {
                    btnCopy.IsEnabled = gateCanvas.selected.Count > 0;
                    btnCut.IsEnabled = gateCanvas.selected.Count > 0;
                    btnCopyAsImage.IsEnabled = gateCanvas.selected.Count > 0;
                };
                this.Activated += (s2, e2) =>
                {
                    btnPaste.IsEnabled = Clipboard.ContainsData("IC");
                };
            } 


            Grid1.Children.Remove(spSpeed);
            if (e == EditLevel.FULL)
            {
                // speed only for the main window
                sbSpeed = new ShadowBox();
                sbSpeed.Margin = new Thickness(20, 20, 175, 20);
                sbSpeed.Children.Add(spSpeed);
                spSpeed.Background = Brushes.Transparent;
                sbSpeed.VerticalAlignment = VerticalAlignment.Top;
                sbSpeed.HorizontalAlignment = HorizontalAlignment.Right;
                Grid1.Children.Add(sbSpeed);
                Grid.SetColumn(sbSpeed, 1);
                Grid.SetRow(sbSpeed, 1);

                // otherwise the defaults mess it up when you open a new window
                slSpeed.ValueChanged += (sender2, e2) =>
                {
                    Gates.PropagationThread.SLEEP_TIME = (int)slSpeed.Value;
                };

                // full also gets file and ic
                tbFile.Visibility = Visibility.Visible;
                tbIC.Visibility = Visibility.Visible;
            }

            if (e == EditLevel.EDIT)
            {
                // can't edit the user gates in this view
                spGates.IsReadOnly = true;
            }

            this.Loaded += (sender2, e2) => 
            {
                ((UndoRedo.UndoManager)Resources["undoManager"]).SetSavePoint(); 
                UpdateTitle();
                lblAppTitle.Text = APP_TITLE;
                lblAppVersion.Text = APP_VERSION;
                lblAppCopyright.Text = APP_COPYRIGHT;

            };

            // green team: added if statement to determine if ctrl keys are pressed
            // then fires mouse wheel zoom event
            
                this.PreviewMouseWheel += (sender, e2) =>
                { 
                    if(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl ))
                    {
                        gateCanvas.UseZoomCenter = true;
                        double centerX = (Mouse.GetPosition(this).X + gateCanvas.GCScroller.HorizontalOffset) / gateCanvas.Zoom;
                        double centerY = (Mouse.GetPosition(this).Y + gateCanvas.GCScroller.VerticalOffset) / gateCanvas.Zoom;
                        gateCanvas.ZoomCenter = new Point(centerX, centerY);

                        if (e2.Delta > 0)
                            slZoom.Value += 0.1;
                        else
                            slZoom.Value -= 0.1;

                        e2.Handled = true;
                    }
                };

            

            ((UndoRedo.UndoManager)Resources["undoManager"]).PropertyChanged += (sender2, e2) =>
            {
                UpdateTitle(); // look for modified or not
            };

            InfoLine.GetInstance().PropertyChanged += InfoLine_PropertyChanged;
        }

        public Window1() : this(EditLevel.FULL)
        {



            AssemblyTitleAttribute title;
            AssemblyCopyrightAttribute copyright;
            Assembly aAssembly = Assembly.GetExecutingAssembly();
            
            
            title = (AssemblyTitleAttribute)
                    AssemblyTitleAttribute.GetCustomAttribute(
                aAssembly, typeof(AssemblyTitleAttribute));

            copyright = (AssemblyCopyrightAttribute)
                    AssemblyCopyrightAttribute.GetCustomAttribute(
                aAssembly, typeof(AssemblyCopyrightAttribute));
            APP_TITLE = title.Title;
            APP_VERSION = aAssembly.GetName().Version.ToString();
            APP_COPYRIGHT = copyright.Copyright;
            
            icl = new ICList();
            
            gateCanvas.ICL = icl;
            gateCanvas.UndoProvider = (UndoRedo.UndoManager)Resources["undoManager"];
            gateCanvas.SetCaptureICLChanges();
            spGates.ICList = icl;
            spGates.UndoProvider = (UndoRedo.UndoManager)Resources["undoManager"];

            this.Loaded += (s2, e2) => { Gates.IOGates.Clock.CalculatePrecession(); };

            this.Closing += new CancelEventHandler(Window1_Closing);

            if (!string.IsNullOrEmpty(LOAD_ON_START))
            {
                try
                {
                    CircuitXML cxml = new CircuitXML(icl);
                    RefreshGateCanvas(cxml.Load(LOAD_ON_START, icl.Add));

                    btnSave.IsEnabled = true;
                    _filename = LOAD_ON_START;
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load requested circuit, reason: " + ex.ToString());
                }

            }
            
        }

        /// <summary>
        /// Create a window based on a given IC and IC List.  Provide
        /// an edit level of either View or Edit as appropriate.
        /// </summary>
        /// <param name="IC"></param>
        /// <param name="useicl"></param>
        /// <param name="el"></param>
        public Window1(UIGates.IC IC, ICList useicl, EditLevel el)
            : this(el)
        {
            icl = useicl;

            gateCanvas.ICL = icl;
            gateCanvas.UndoProvider = (UndoRedo.UndoManager)Resources["undoManager"];
            spGates.ICList = icl;
            _filename = IC.AbGate.Name;


            this.Loaded += (sender, e) =>
            {
                RefreshGateCanvas(IC);

                spGates.ICName = IC.AbGate.Name;
                if (el == EditLevel.VIEW)
                    gateCanvas.IsReadOnly = true;

                Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        Activate();
                        Focus();

                    }));

            };

        }

        #endregion


        private void Window1_Closing(object sender, CancelEventArgs e)
        {
            // only for original full window
            e.Cancel = !QuerySave();
                
        }

        private void InfoLine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(InfoLine.GetInstance().CurrentInfoLine) || !this.IsActive)
            {
                lblInfoLine.Visibility = Visibility.Collapsed;
                spAppInfo.Visibility = Visibility.Visible;

            }
            else
            {
                lblInfoLine.Text = InfoLine.GetInstance().CurrentInfoLine;
                lblInfoLine.Visibility = Visibility.Visible;
                spAppInfo.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the IC this window is based on, if any.
        /// </summary>
        public UIGates.IC BasedOn
        {
            get
            {
                return _basedon;
            }
        }

        /// <summary>
        /// Creates an IC from this window's circuit.
        /// This just calls CreateIC in the appropriate gate canvas.
        /// </summary>
        /// <returns></returns>
        public UIGates.IC GetIC()
        {
            return gateCanvas.CreateIC();
        }



        private void KeyRotateGates(int degree)
        {
            foreach (Gate g in gateCanvas.selected)
            {
                double origin = ((RotateTransform)g.RenderTransform).Angle;
                double dest = origin + degree;
                gateCanvas.UndoProvider.Add(new UndoRedo.RotateGate(g, gateCanvas, origin, dest));

                ((RotateTransform)g.RenderTransform).Angle = dest;
                ((GateLocation)g.Tag).Angle = dest;
            }
            gateCanvas.UpdateWireConnections();
        }

        private void KeyMoveGates(int dx, int dy)
        {
            foreach (Gate g in gateCanvas.selected)
            {
                moves.Add(new UndoRedo.MoveGate(g, gateCanvas, new Point(g.Margin.Left, g.Margin.Top), new Point(g.Margin.Left + dx, g.Margin.Top + dy)));
                g.Margin = new Thickness(g.Margin.Left + dx, g.Margin.Top + dy, 0, 0);
                ((GateLocation)g.Tag).X += dx;
                ((GateLocation)g.Tag).Y += dy;
            }
            gateCanvas.UpdateWireConnections();
        }

        private void Window1_View_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.P)
                {
                    btnPrint_Click(sender, e);
                }

                // zoom controls
                if (e.Key == Key.D0 || e.Key == Key.NumPad0)
                {
                    btnFitToScreen_Click(sender, e);
                }
                if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                {
                    btnActualSize_Click(sender, e);
                }
                if (e.Key == Key.Add || e.Key == Key.OemPlus)
                {
                    slZoom.Value += 0.1;
                }
                if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                {
                    slZoom.Value -= 0.1;
                }
            }
        }

        private void Window1_EditFull_KeyDown(object sender, KeyEventArgs e)
        {
            // delete all selected gates
            if (e.Key == Key.Delete)
            {
                gateCanvas.UndoProvider.Add(gateCanvas.DeleteSelectedGates());
            }

            if (e.Key == Key.Escape)
            {
                DragDrop.DragDropHelper.Cancel();
            }

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // the control shortcuts
                // because our toolbar are image not text
                if (e.Key == Key.N && MyEditLevel == EditLevel.FULL)
                {
                    btnNew_Click(sender, e);
                }

                if (e.Key == Key.O && MyEditLevel == EditLevel.FULL)
                {
                    btnOpen_Click(sender, e);
                }

                if (e.Key == Key.S && MyEditLevel == EditLevel.FULL)
                {
                    if (btnSave.IsEnabled)
                        btnSave_Click(sender, e);
                    else
                        btnSave_As_Click(sender, e);
                }

                if (e.Key == Key.X && btnCut.IsEnabled)
                {
                    btnCut_Click(sender, e);
                }
                if (e.Key == Key.C && btnCopy.IsEnabled)
                {
                    btnCopy_Click(sender, e);
                }
                if (e.Key == Key.V && btnPaste.IsEnabled)
                {
                    btnPaste_Click(sender, e);
                }

                if (e.Key == Key.Z && btnUndo.IsEnabled)
                {
                    btnUndo_Click(sender, e);
                }
                if (e.Key == Key.Y && btnRedo.IsEnabled)
                {
                    btnRedo_Click(sender, e);
                }
                


                if (!gateCanvas.EndUserMode)
                {
                    if (e.Key == Key.A)
                    {
                        gateCanvas.SelectAll();
                    }

                    // gate rotation
                    if (e.Key == Key.Right)
                    {
                        KeyRotateGates(90);
                    }
                    if (e.Key == Key.Left)
                    {
                        KeyRotateGates(-90);
                    }
                }

            }
            else
            { // not using ctrl

                if (!gateCanvas.EndUserMode)
                {
                    if ((e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.Down || e.Key == Key.Up) && gateCanvas.selected.Count > 0 && moves == null)
                    {
                        moves = new UndoRedo.Transaction("Move " +
                                    (gateCanvas.selected.Count == 1 ?
                                    "Gate" : gateCanvas.selected.Count.ToString() + " Gates"));
                    }

                    // moving gates
                    if (e.Key == Key.Right)
                    {
                        KeyMoveGates(1, 0);
                    }
                    if (e.Key == Key.Left)
                    {
                        KeyMoveGates(-1, 0);
                    }
                    if (e.Key == Key.Up)
                    {
                        KeyMoveGates(0, -1);
                    }
                    if (e.Key == Key.Down)
                    {
                        KeyMoveGates(0, 1);
                    }
                }
            }

        }

        

      

        private void DragDropHelper_ItemDropped(object sender, GatesWpf.DragDrop.DragDropEventArgs e)
        {
            if (e.DropTarget.IsDescendantOf(gateCanvas) && this.IsActive)
            {
                Gate newgate = null;

                newgate = ((Gate)e.Content).CreateUserInstance();

                gateCanvas.AddGate(newgate, new GateLocation(gateCanvas.GetNearestSnapTo(gateCanvas.TranslateScrolledPoint(e.Position))));


              
                gateCanvas.UpdateLayout();
                gateCanvas.UpdateWireConnections();

                UndoRedo.AddGate ag = new GatesWpf.UndoRedo.AddGate(gateCanvas, newgate);
               
                    if (gateCanvas.UndoProvider != null)
                        gateCanvas.UndoProvider.Add(ag);
                
                
            }
        }

        

        private void btnCreateIC_Click(object sender, RoutedEventArgs e)
        {
            

            UIGates.IC nic = gateCanvas.CreateIC(icl.GenerateAvailableName("Untitled"), GateCanvas.SELECTED_GATES.SELECTED_IF_TWO_OR_MORE);

            icl.Add(nic);
            // can't call seteditname on nic directly
            // because it doesn't exist in the selector
            // because the selector makes an instance
            // so have the selector redirect the request

            // bg delay is due to animation effect time
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s2, e2) => { System.Threading.Thread.Sleep(500); };
            bg.RunWorkerCompleted += (s2, e2) => 
            {
                spGates.SetEditName(nic.AbGate.Name);
            };
            bg.RunWorkerAsync();

            ((UndoRedo.UndoManager)Resources["undoManager"]).Add(new UndoRedo.CreateIC(icl, nic));


        }
       
       




        #region Circuit File Operations

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            CircuitXML cxml = new CircuitXML(icl);
            cxml.Save(_filename, gateCanvas);

            ((UndoRedo.UndoManager)Resources["undoManager"]).SetSavePoint();
            
        }

        private void btnSave_As_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".gcg";
            dlg.Filter = "Gate Circuit Groups (.gcg)|*.gcg";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                CircuitXML cxml = new CircuitXML(icl);
                try
                {
                    cxml.Save(dlg.FileName, gateCanvas);
                    _filename = dlg.FileName;
                    btnSave.IsEnabled = true;
                    UpdateTitle();

                    ((UndoRedo.UndoManager)Resources["undoManager"]).SetSavePoint();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to save circuit as requested: " + ex.ToString());
                }
            }
        }


        private void btnImportIC_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ic";
            dlg.Filter = "IC (.ic)|*.ic|All Files (*.*)|*.*";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                CircuitXML cxml = new CircuitXML(icl);
                try
                {
                    cxml.Load(dlg.FileName, icl.Add);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load circuit as requested: " + ex.ToString());
                }

            }
        }

        
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (!QuerySave())
                return;

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".gcg";
            dlg.Filter = "Gate Circuit Groups (.gcg)|*.gcg|All Files|*.*";
            bool? result = dlg.ShowDialog();
            
            if (result == true)
            {
                btnUserMode.IsChecked = false;

                foreach (Window w in Application.Current.Windows)
                {
                    if (w != this)
                        w.Close();
                }
                gateCanvas.ClearSelection();

                icl.Clear();
                CircuitXML cxml = new CircuitXML(icl);

                try
                {
                    slZoom.Value = 1;
                    RefreshGateCanvas(cxml.Load(dlg.FileName, icl.Add));



                    btnSave.IsEnabled = true;
                    _filename = dlg.FileName;
                    UpdateTitle();

                    ((UndoRedo.UndoManager)Resources["undoManager"]).Clear();
                    ((UndoRedo.UndoManager)Resources["undoManager"]).SetSavePoint();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load circuit as requested: " + ex.ToString());
                }
                
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {

            if (!QuerySave())
                return;

            btnUserMode.IsChecked = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w != this)
                    w.Close();
            }

            gateCanvas.ClearSelection();
            slZoom.Value = 1;
            btnSave.IsEnabled = false;
            _filename = "";
            UpdateTitle();
            icl.Clear();
            ((UndoRedo.UndoManager)Resources["undoManager"]).Clear();
            ((UndoRedo.UndoManager)Resources["undoManager"]).SetSavePoint();
            RefreshGateCanvas(new UIGates.IC(new Gates.IC(new Gates.Circuit(), new Gates.IOGates.UserInput[0], new Gates.IOGates.UserOutput[0], ""), new Gate.TerminalID[0]));
            

        }

        private bool QuerySave()
        {
            if (!((UndoRedo.UndoManager)Resources["undoManager"]).IsAtSavePoint)
            {
                SaveClose sc = new SaveClose(String.IsNullOrEmpty(_filename) ? "[Untitled]" : _filename);
                sc.ShowDialog();
                switch (sc.Selected)
                {
                    case SaveClose.Result.CANCEL:
                        return false;
                    case SaveClose.Result.DONT_SAVE:
                        return true;
                    case SaveClose.Result.SAVE:
                        if (btnSave.IsEnabled)
                            btnSave_Click(null, null);
                        else
                            btnSave_As_Click(null, null);
                        return true;
                    }
            }
            return true;

        }

        #endregion

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.ClearSelection();
            gateCanvas.UndoProvider.Undo();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.ClearSelection();
            gateCanvas.UndoProvider.Redo();
        }

        private void btnClearUndos_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.UndoProvider.Clear();
            GC.Collect();
        }


        private void UpdateTitle()
        {
            StringBuilder ttl = new StringBuilder();

            if (String.IsNullOrEmpty(_filename))
            {
                ttl.Append("[Untitled]");
            }
            else
            {
                ttl.Append(_filename.Substring(_filename.LastIndexOf(@"\") + 1));
            }

            if (!((UndoRedo.UndoManager)Resources["undoManager"]).IsAtSavePoint)
            {
                ttl.Append(" (Modified) ");
            }
            ttl.Append(" - ");

            switch (_myEditLevel)
            {
                case EditLevel.VIEW:
                    ttl.Append("View IC - ");
                    break;
                case EditLevel.EDIT:
                    ttl.Append("View/Edit IC - ");
                    break;
            }

            

            ttl.Append(APP_TITLE);

            Title = ttl.ToString();

        }

        #region Circuit Zoom Operations

        private void slZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            gateCanvas.Zoom = slZoom.Value;
        }

        private void AnimateZoom(double dest, Point? zoomCenter)
        {
            if (dest < slZoom.Minimum)
                dest = slZoom.Minimum;

            if (dest > slZoom.Maximum)
                dest = slZoom.Maximum;

            PointAnimation pa = null;
            
            if (zoomCenter.HasValue) 
            {
                gateCanvas.SetZoomCenter();
                pa = new PointAnimation(gateCanvas.ZoomCenter, zoomCenter.Value, new Duration(new TimeSpan(0, 0, 1)));
                
                pa.AccelerationRatio = 0.2;
                pa.DecelerationRatio = 0.2;

            }
            DoubleAnimation da = new DoubleAnimation(dest, new Duration(new TimeSpan(0, 0, 1)));
            da.AccelerationRatio = 0.2;
            da.DecelerationRatio = 0.2;

            Storyboard sb = new Storyboard();
            sb.Children.Add(da);
            if (pa != null)
            {
                sb.Children.Add(pa);
                Storyboard.SetTarget(pa, gateCanvas);
                Storyboard.SetTargetProperty(pa, new PropertyPath(GateCanvas.ZoomCenterProperty));
                gateCanvas.UseZoomCenter = true;

            }
            Storyboard.SetTarget(da, slZoom);
            Storyboard.SetTargetProperty(da, new PropertyPath(Slider.ValueProperty));
            sb.FillBehavior = FillBehavior.Stop;

            
            sb.Begin();

            BackgroundWorker finishani = new BackgroundWorker();
            finishani.DoWork += (sender2, e2) =>
            {
                System.Threading.Thread.Sleep(900);
            };
            finishani.RunWorkerCompleted += (sender2, e2) =>
            {
                if (zoomCenter.HasValue)
                    gateCanvas.ZoomCenter = zoomCenter.Value;

                slZoom.Value = dest;
            };
            finishani.RunWorkerAsync();

            
        }

        private void btnActualSize_Click(object sender, RoutedEventArgs e)
        {
            AnimateZoom(1, null);
        }

        private void btnFitToScreen_Click(object sender, RoutedEventArgs e)
        {
            Rect bounds;
            bounds = gateCanvas.GetBounds(64, gateCanvas.selected.Count > 1);
            

            double minx = bounds.Left;
            double miny = bounds.Top;
            double maxx = bounds.Right;
            double maxy = bounds.Bottom;

            double wid =  gateCanvas.ActualWidth / (maxx - minx);
            double hei = gateCanvas.ActualHeight / (maxy - miny);
            
            
            AnimateZoom(Math.Min(wid, hei), new Point(minx + (maxx - minx) / 2.0,
                miny + (maxy - miny) / 2.0));

            BackgroundWorker resetzc = new BackgroundWorker();
            resetzc.DoWork += (sender2, e2) =>
            {
                System.Threading.Thread.Sleep(1500);
            };
            resetzc.RunWorkerCompleted += (sender2, e2) =>
            {
                //gateCanvas.UseZoomCenter = false;
            };
            resetzc.RunWorkerAsync();


        }
        #endregion


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        
        #region CopyPaste
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (gateCanvas.selected.Count > 0) {
                UIGates.IC ic = gateCanvas.CreateIC("(clipboard)", GateCanvas.SELECTED_GATES.SELECTED);
                CircuitXML cx = new CircuitXML(icl);
                DataObject clipobj = new DataObject();
                Clipboard.SetData("IC", cx.CreateCircuitXML(ic).ToString());
                btnPaste.IsEnabled = true;
            }
        }

        private void btnCopyAsImage_Click(object sender, RoutedEventArgs e)
        {
            if (gateCanvas.selected.Count > 0)
            {
                UIGates.IC ic = gateCanvas.CreateIC("(clipboard)", GateCanvas.SELECTED_GATES.SELECTED);
                
                
                GateCanvas tmp = new GateCanvas(ic, icl);
                tmp.Width = tmp.GetBounds(0, false).Width;
                tmp.Height = tmp.GetBounds(0, false).Height;
                BackgroundWorker bg = new BackgroundWorker();
                bg.DoWork += (s2, e2) =>
                {
                    System.Threading.Thread.Sleep(500); // prop time
                    // don't use wait on propagation because if there is a loop
                    // it will loop forever
                };
                bg.RunWorkerCompleted += (s2, e2) =>
                {
                    tmp.Mute = true;
                    tmp.UpdateLayout();
                    tmp.UpdateWireConnections();
                    Clipboard.SetImage(tmp.CreateImage());
                    Grid1.Children.Remove(tmp);
                };
                
                Grid1.Children.Add(tmp);
                bg.RunWorkerAsync();
                
            }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsData("IC"))
            {
                string xml = Clipboard.GetData("IC") as string;
                CircuitXML cx = new CircuitXML(icl);
                try
                {
                    UIGates.IC ic = cx.LoadCircuit(System.Xml.Linq.XElement.Parse(xml));
                    gateCanvas.PasteIC(ic);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to complete paste; maybe you deleted a needed IC?");
                }
            }
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            btnCopy_Click(sender, e); // do a copy
            gateCanvas.UndoProvider.Add(gateCanvas.DeleteSelectedGates());
        }

        #endregion
        
        private void btnFlatten_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.Flatten();
        }


       
        
        private void btnShowHideToolbars_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sbSpeed != null)
                Fader.AnimateOpacity(0, sbGates, sbSpeed, sbZoom);
            else
                Fader.AnimateOpacity(0, sbGates, sbZoom);

           
        }

        private void btnShowHideToolbars_Checked(object sender, RoutedEventArgs e)
        {

            if (sbGates == null) return; // occurs during load, not ready yet

            if (sbSpeed != null)
                Fader.AnimateOpacity(1, sbGates, sbSpeed, sbZoom);
            else
                Fader.AnimateOpacity(1, sbGates, sbZoom);

           

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new System.Windows.Controls.PrintDialog();

            if (printDlg.ShowDialog() == true)
            {

                gateCanvas.Print(printDlg);

             
            }


        }

        private void btnSaveAsImage_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Image (.png)|*.png|JPEG Image (.jpg)|*.jpg|Bitmap Image (.bmp)|*.bmp";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                BitmapEncoder be = null;
                switch (dlg.FilterIndex)
                {
                    case 0:
                        be = new PngBitmapEncoder();
                        break;
                    case 1:
                        be = new JpegBitmapEncoder();
                        break;
                    case 2:
                        be = new BmpBitmapEncoder();
                        break;
                }
                gateCanvas.Mute = true;
                be.Frames.Add(BitmapFrame.Create(gateCanvas.CreateImage()));
                gateCanvas.Mute = false;
                try
                {
                    System.IO.FileStream fs = System.IO.File.Create(dlg.FileName);
                    be.Save(fs);
                    fs.Close();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to save image as requested: "+ ex.ToString());
                }
            }
        }

        private void btnAlignTopLeft_Click(object sender, RoutedEventArgs e)
        {
            gateCanvas.AlignUpperLeft();
        }

        private void btnChart_Click(object sender, RoutedEventArgs e)
        {
            Charting.Chart chrt = new Charting.Chart(gateCanvas.Circuit);
            chrt.Show();
        }

        private void btnShowTrueFalse_Checked(object sender, RoutedEventArgs e)
        {
            gateCanvas.ShowTrueFalse = true;
        }

        private void btnShowTrueFalse_Unchecked(object sender, RoutedEventArgs e)
        {
            gateCanvas.ShowTrueFalse = false;
        }

        private void btnUserMode_Checked(object sender, RoutedEventArgs e)
        {
            btnShowHideToolbars.IsChecked = false;
            btnShowHideToolbars.IsEnabled = false;
            gateCanvas.EndUserMode = true;
            
        }

        private void btnUserMode_Unchecked(object sender, RoutedEventArgs e)
        {
            btnShowHideToolbars.IsChecked = true;
            btnShowHideToolbars.IsEnabled = true;
            gateCanvas.EndUserMode = false;
            
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            ICBuilder x = new ICBuilder(gateCanvas.Circuit, g =>
            {
                Gate fg = gateCanvas.FindGate(g);
                return new GateLocation()
                {
                    X = fg.Margin.Left,
                    Y = fg.Margin.Top,
                    Width = fg.Width,
                    Height = fg.Height,
                    Angle = ((RotateTransform)fg.RenderTransform).Angle
                };

            });
            var pRect = x.createCenter();
           

            var adj_pRect = pRect;

            ICBounds_Line1 = new Line();
            

            ICBounds_Line1.Stroke = System.Windows.Media.Brushes.Green;
            ICBounds_Line1.Opacity = 0.5;
            ICBounds_Line1.X1 = adj_pRect.Left;
            ICBounds_Line1.Y1 = adj_pRect.Top;
            ICBounds_Line1.X2 = adj_pRect.Right;
            ICBounds_Line1.Y2 = adj_pRect.Bottom;
            ICBounds_Line1.StrokeThickness = 2;
            gateCanvas.GC.Children.Add(ICBounds_Line1);


            ICBounds_Line2 = new Line();

            ICBounds_Line2.Stroke = System.Windows.Media.Brushes.Green;
            ICBounds_Line2.Opacity = 0.5;
            ICBounds_Line2.X1 = adj_pRect.Right;
            ICBounds_Line2.Y1 = adj_pRect.Top;
            ICBounds_Line2.X2 = adj_pRect.Left;
            ICBounds_Line2.Y2 = adj_pRect.Bottom;

            ICBounds_Line2.StrokeThickness = 2;
            gateCanvas.GC.Children.Add(ICBounds_Line2);

            ICBounds = new Rectangle();
            ICBounds.Stroke = System.Windows.Media.Brushes.Green;
            ICBounds.Opacity = 0.5;
            ICBounds.Width = adj_pRect.Width;
            ICBounds.Height = adj_pRect.Height;
            ICBounds.Margin = new Thickness() { Left = adj_pRect.Left, Top = adj_pRect.Top };
            ICBounds.StrokeThickness = 2;
            gateCanvas.GC.Children.Add(ICBounds);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            gateCanvas.GC.Children.Remove(ICBounds_Line1);
            gateCanvas.GC.Children.Remove(ICBounds_Line2);
            gateCanvas.GC.Children.Remove(ICBounds);
            
        }

        

        

        
    }
}


