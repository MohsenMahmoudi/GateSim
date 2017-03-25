using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Controls;

// FROM http://blogs.claritycon.com/blogs/lee_roth/archive/2009/03/31/generic-wpf-drag-and-drop-adorner.aspx


namespace GatesWpf.DragDrop
{
    public class DragDropHelper
    {
        #region Private Members
        private Point _initialMousePosition;
        private Point _delta;
        private Point _scrollTarget;
        private UIElement _dropTarget;
        private Rect _dropBoundingBox;
        private bool _mouseCaptured;
        private object _draggedData;

        private Window _topWindow;
        private Canvas _adornerLayer;
        private DragDropAdornerBase _adorner;

        private static DragDropHelper instance;
        private static DragDropHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DragDropHelper();
                }
                return instance;
            }
        }
        #endregion

        #region Attached Properties
        public static bool GetIsDragSource(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragSourceProperty);
        }
        public static void SetIsDragSource(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragSourceProperty, value);
        }
        public static UIElement GetDragDropControl(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(DragDropControlProperty);
        }
        public static void SetDragDropControl(DependencyObject obj, UIElement value)
        {
            obj.SetValue(DragDropControlProperty, value);
        }
        public static string GetDropTarget(DependencyObject obj)
        {
            return (string)obj.GetValue(DropTargetProperty);
        }
        public static void SetDropTarget(DependencyObject obj, string value)
        {
            obj.SetValue(DropTargetProperty, value);
        }
        public static string GetAdornerLayer(DependencyObject obj)
        {
            return (string)obj.GetValue(AdornerLayerProperty);
        }
        public static void SetAdornerLayer(DependencyObject obj, string value)
        {
            obj.SetValue(AdornerLayerProperty, value);
        }

        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDropHelper), new UIPropertyMetadata(false, IsDragSourceChanged));
       
        public static readonly DependencyProperty DragDropControlProperty =
            DependencyProperty.RegisterAttached("DragDropControl", typeof(UIElement), typeof(DragDropHelper), new UIPropertyMetadata(null));
        
        public static readonly DependencyProperty AdornerLayerProperty =
             DependencyProperty.RegisterAttached("AdornerLayer", typeof(string), typeof(DragDropHelper), new UIPropertyMetadata(null));

        public static readonly DependencyProperty DropTargetProperty =
            DependencyProperty.RegisterAttached("DropTarget", typeof(string), typeof(DragDropHelper), new UIPropertyMetadata(string.Empty));

        

        private static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var dragSource = obj as UIElement;
            if (dragSource != null)
            {
                if (Object.Equals(e.NewValue, true))
                {
                    dragSource.PreviewMouseLeftButtonDown += Instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp += Instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove += Instance.DragSource_PreviewMouseMove;
                }
                else
                {
                    dragSource.PreviewMouseLeftButtonDown -= Instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp -= Instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove -= Instance.DragSource_PreviewMouseMove;
                }
            }
        }

      

        
        #endregion

        #region Utilities
        public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
        {
            while (visual != null && !ancestorType.IsInstanceOfType(visual))
            {
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }
            return visual as FrameworkElement;
        }

        public static bool IsMovementBigEnough(Point initialMousePosition, Point currentPosition)
        {
            return (Math.Abs(currentPosition.X - initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(currentPosition.Y - initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance);
        }
        #endregion

        #region Drag Handlers
        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Visual visual = e.OriginalSource as Visual;
                _topWindow = (Window)DragDropHelper.FindAncestor(typeof(Window), visual);
                
                if (!_topWindow.IsActive)
                    return;

                _initialMousePosition = e.GetPosition(_topWindow);

                string adornerLayerName = GetAdornerLayer(sender as DependencyObject);
                _adornerLayer = (Canvas)_topWindow.FindName(adornerLayerName);

                string dropTargetName = GetDropTarget(sender as DependencyObject);
                _dropTarget = (UIElement)_topWindow.FindName(dropTargetName);

                _draggedData = (sender as FrameworkElement).DataContext;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception in DragDropHelper: " + exc.InnerException.ToString());
            }
        }

        // Drag = mouse down + move by a certain amount
        private void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                _draggedData = null;

            if (!_mouseCaptured && _draggedData != null)
            {
                // Only drag when user moved the mouse by a reasonable amount.
                if (DragDropHelper.IsMovementBigEnough(_initialMousePosition, e.GetPosition(_topWindow)))
                {
                    try
                    {
                        _adorner = (DragDropAdornerBase)GetDragDropControl(sender as DependencyObject);
                        _adorner.DataContext = _draggedData;
                        _adorner.Opacity = 0.7;

                        _adornerLayer.Visibility = Visibility.Visible;
                    
                        _adornerLayer.Children.Add(_adorner);
                        _mouseCaptured = Mouse.Capture(_adorner);

                        Canvas.SetLeft(_adorner, _initialMousePosition.X - 20);
                        Canvas.SetTop(_adorner, _initialMousePosition.Y - 15);

                        _adornerLayer.PreviewMouseMove += new MouseEventHandler(_adorner_MouseMove);
                        _adornerLayer.PreviewMouseUp += new MouseButtonEventHandler(_adorner_MouseUp);
                    }
                    catch (Exception) 
                    { 
// bummer
                    }
                }
            }
        }

        private void _adorner_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(_topWindow);
            //currentPoint.X = currentPoint.X - 20;
            //currentPoint.Y = currentPoint.Y - 15;

            _delta = new Point(_initialMousePosition.X - currentPoint.X, _initialMousePosition.Y - currentPoint.Y);
            _scrollTarget = new Point(_initialMousePosition.X - _delta.X, _initialMousePosition.Y - _delta.Y);

            Canvas.SetLeft(_adorner, _scrollTarget.X);
            Canvas.SetTop(_adorner, _scrollTarget.Y);

            _adorner.AdornerDropState = DropState.CannotDrop;

            if (_dropTarget != null)
            {
                try
                {
                    GeneralTransform t = _dropTarget.TransformToVisual(_adornerLayer);
                    _dropBoundingBox = t.TransformBounds(new Rect(0, 0, _dropTarget.RenderSize.Width, _dropTarget.RenderSize.Height));
                    //Unnamed team edit: droptarget is the layer where gates are dropped. Boundingbox is the limits.
                    GateCanvas gc = (GateCanvas)_dropTarget;

                    //Unnamed team edit: conditions for checking the adorner against the boundries.  Call scroller if true.
                    if (currentPoint.X > _dropBoundingBox.Right - 50)
                    {
                        ((GateCanvas)_dropTarget).GCScroller.ScrollToHorizontalOffset(
                            ((GateCanvas)_dropTarget).GCScroller.HorizontalOffset + 10);
                        //Unnamed team edit: turn off the size canvas method in order to resize the canvas, to increase scroll area.
                        gc.DisableSizeCanvas = true;
                        //increase scroll area.
                        gc.GC.Width += 5;
                    }
                    if (currentPoint.X < _dropBoundingBox.Left + 50)
                    {
                        ((GateCanvas)_dropTarget).GCScroller.ScrollToHorizontalOffset(
                            ((GateCanvas)_dropTarget).GCScroller.HorizontalOffset - 10);
                    }
                    if (currentPoint.Y < _dropBoundingBox.Top + 50)
                    {
                        ((GateCanvas)_dropTarget).GCScroller.ScrollToVerticalOffset(
                            ((GateCanvas)_dropTarget).GCScroller.VerticalOffset - 10);
                    }

                    if (currentPoint.Y > _dropBoundingBox.Bottom - 50)
                    {
                        ((GateCanvas)_dropTarget).GCScroller.ScrollToVerticalOffset(
                            ((GateCanvas)_dropTarget).GCScroller.VerticalOffset + 10);
                        gc.DisableSizeCanvas = true;
                        gc.GC.Height += 5;
                    }
                    if (e.GetPosition(_adornerLayer).X > _dropBoundingBox.Left &&
                        e.GetPosition(_adornerLayer).X < _dropBoundingBox.Right &&
                        e.GetPosition(_adornerLayer).Y > _dropBoundingBox.Top &&
                        e.GetPosition(_adornerLayer).Y < _dropBoundingBox.Bottom)
                    {
                        _adorner.AdornerDropState = DropState.CanDrop;
                    }
                }
                catch (Exception) { }
            }
        }

        public static void Cancel()
        {
            if (Instance._adorner != null)
            {
                Instance._adorner.AdornerDropState = DropState.CannotDrop;
                Instance._adorner_MouseUp(Instance, null);
            }
        }

        private void _adorner_MouseUp(object sender, MouseEventArgs e)
        {
            //unnamed team edit:  added code to turn the sizecanvas() method back on, and call it.
            //needed to reset the canvas if the adorner causes scrolling.
            ((GateCanvas)_dropTarget).DisableSizeCanvas = false;
            ((GateCanvas)_dropTarget).GCScroller.UpdateLayout();
            switch (_adorner.AdornerDropState)
            {
                case DropState.CanDrop:
                    try
                    {
                        ((Storyboard)_adorner.Resources["canDrop"]).Completed += (s, args) =>
                        {
                            _adornerLayer.Children.Clear();
                            _adornerLayer.Visibility = Visibility.Collapsed;
                        };
                        ((Storyboard)_adorner.Resources["canDrop"]).Begin(_adorner);

                        // Added position in drop target
                        Point pos = e.GetPosition((IInputElement)_dropTarget);
                        if (ItemDropped != null)
                            ItemDropped(_adorner, new DragDropEventArgs(_draggedData, pos, _dropTarget));
                    }
                    catch (Exception)
                    { }
                    break;
                case DropState.CannotDrop:
                    try
                    {
                        Storyboard sb = _adorner.Resources["cannotDrop"] as Storyboard;
                        DoubleAnimation aniX = sb.Children[0] as DoubleAnimation;
                        aniX.To = _delta.X;
                        DoubleAnimation aniY = sb.Children[1] as DoubleAnimation;
                        aniY.To = _delta.Y;
                        sb.Completed += (s, args) =>
                        {
                            _adornerLayer.Children.Clear();
                            _adornerLayer.Visibility = Visibility.Collapsed;
                        };
                        sb.Begin(_adorner);
                    }
                    catch (Exception) { }
                    break;
            }
            
            _draggedData = null;
            _adornerLayer.PreviewMouseMove -= new MouseEventHandler(_adorner_MouseMove);
            _adornerLayer.PreviewMouseUp -= new MouseButtonEventHandler(_adorner_MouseUp);

            if (_adorner != null)
            {
                _adorner.ReleaseMouseCapture();
            }
            _adorner = null;
            _mouseCaptured = false;
        }

        private void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _draggedData = null;
            _mouseCaptured = false;

            if (_adorner != null)
            {
                _adorner.ReleaseMouseCapture();
            }
        }      

        #endregion

        #region Events
        public static event EventHandler<DragDropEventArgs> ItemDropped;
        #endregion
    }

    public class DragDropEventArgs : EventArgs
    {
        public object Content;
        public Point Position;
        public UIElement DropTarget;
        public DragDropEventArgs() { }
        public DragDropEventArgs(object content, Point pos, UIElement dropTarget)
        {
            Content = content;
            Position = pos;
            DropTarget = dropTarget;
        }
    }

   
}
