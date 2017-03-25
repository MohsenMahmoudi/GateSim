//---------------------------------------------------------------------
// <copyright file="ReorderableListBox.cs" company="SiSoft">
//     Copyright (c) SiSoft. Authorised use to reproduce, modify
//     or transmit in any form for commercial or non-commercial purposes
//     is granted subject to the retention of all notices of copyright.
//     The software is licensed “as-is.” You bear the risk of using it. 
//     SiSoft implies no express warranties, guarantees or conditions by
//     the use of this sample.
// </copyright>
// <summary>
//    Reorderable WPF List Box.
// </summary>
//---------------------------------------------------------------------
namespace GatesWpf.DragDrop
{
    #region Directives
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    #endregion

    /// <summary>
    /// ReorderableListBox, subclasses ListBox
    /// </summary>
    public class ReorderableListBox : ListBox
    {
        #region Public Static Members
        /// <summary>
        /// ItemsReorderedEvent
        /// </summary>
        public static readonly RoutedEvent ItemsReorderedEvent;
        #endregion

        #region Protected Members
        /// <summary>
        /// dragStartPoint
        /// </summary>
        protected Point dragStartPoint;

        /// <summary>
        /// dragStarted
        /// </summary>
        protected bool dragging;

        /// <summary>
        /// dragItemSelected
        /// </summary>
        protected bool dragItemSelected;

        /// <summary>
        /// adornerLayer
        /// </summary>
        protected AdornerLayer adornerLayer;

        /// <summary>
        /// overlayElement
        /// </summary>
        protected DropPreviewAdorner overlayElement;

        /// <summary>
        /// originalItemIndex
        /// </summary>
        protected int originalItemIndex;


        #endregion               

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="ReorderableListBox"/> class.
        /// </summary>
        static ReorderableListBox()
        {
            ItemsReorderedEvent = EventManager.RegisterRoutedEvent("ItemsReordered", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ReorderableListBox));
        }
        #endregion

        #region Event Elements
        /// <summary>
        /// ItemsReordered
        /// </summary>
        public event RoutedEventHandler ItemsReordered
        {
            add { this.AddHandler(ItemsReorderedEvent, value); }
            remove { this.RemoveHandler(ItemsReorderedEvent, value); }
        }
        #endregion        

        #region Public Properties

        /// <summary>
        /// OriginalItemIndex
        /// </summary>
        public int OriginalItemIndex
        {
            get { return this.originalItemIndex; }
            set { this.originalItemIndex = value; }
        }

        /// <summary>
        /// Gets the adorner layer.
        /// </summary>
        /// <value>The adorner layer.</value>
        public AdornerLayer AdornerLayer
        {
            get
            {
                if (this.adornerLayer != null)
                {
                    return this.adornerLayer;
                }
                else
                {
                    this.adornerLayer = AdornerLayer.GetAdornerLayer((Visual)this);
                    return AdornerLayer;
                }
            }
        }

        #endregion       

        #region Protected Methods
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"></see> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"></see> is set to true internally.
        /// </summary>
        /// <param name="e">Arguments of the event.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.OnPreviewMouseLeftButtonDown);
            this.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.OnPreviewMouseLeftButtonUp);
            this.SelectionChanged += new SelectionChangedEventHandler(this.OnSelectionChanged);
            this.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.OnPreviewMouseMove);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the SelectionChanged event of the ReorderableListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dragging && !this.dragItemSelected)
            {
                this.dragItemSelected = true;
                this.originalItemIndex = this.SelectedIndex;

                ListBoxItem listBoxItem = (ListBoxItem)ItemContainerGenerator.ContainerFromIndex(SelectedIndex);

                this.overlayElement = new DropPreviewAdorner((UIElement)this, listBoxItem);

                this.overlayElement.Visibility = Visibility.Collapsed;
                this.AdornerLayer.Add(this.overlayElement);
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the ReorderableListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.dragStartPoint = e.GetPosition(this);
                this.dragging = true;
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonUp event of the ReorderableListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.dragging && this.dragItemSelected)
            {
                this.dragging = false;
                this.dragItemSelected = false;

                this.adornerLayer.Remove(this.overlayElement);

                object originalItem = this.Items[this.originalItemIndex];

                RoutedEventArgs routedEventArgs = new RoutedEventArgs(ItemsReorderedEvent, this);
                this.RaiseEvent(routedEventArgs);
            }
        }

        /// <summary>
        /// Handles the PreviewMouseMove event of the ReorderableListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && this.dragging)
            {
                if (this.overlayElement != null)
                {
                    this.overlayElement.Visibility = Visibility.Visible;
                    Point currentPosition = (Point)e.GetPosition((IInputElement)this);
                    this.overlayElement.LeftOffset = currentPosition.X;
                    this.overlayElement.TopOffset = currentPosition.Y;
                }
            }
        }
        #endregion
    }
}
