//---------------------------------------------------------------------
// <copyright file="DropPreviewAdorner.cs" company="SiSoft">
//     Copyright (c) SiSoft. Authorised use to reproduce, modify
//     or transmit in any form for commercial or non-commercial purposes
//     is granted subject to the retention of all notices of copyright.
//     The software is licensed “as-is.” You bear the risk of using it. 
//     SiSoft implies no express warranties, guarantees or conditions by
//     the use of this sample.
// </copyright>
// <summary>
//    Converted and modified from the original DropPreviewAdorner writen
//    by Marcelo:
//    http://blogs.msdn.com/marcelolr/archive/2006/03/03/543301.aspx
// </summary>
//---------------------------------------------------------------------
namespace GatesWpf.DragDrop
{
    #region Directives
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Documents;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    #endregion

    /// <summary>
    /// DropPreviewAdorner
    /// </summary>
    public class DropPreviewAdorner : Adorner
    {
        #region Private Members
        /// <summary>
        /// child
        /// </summary>
        private Rectangle child;

        /// <summary>
        /// leftOffset
        /// </summary>
        private double leftOffset;

        /// <summary>
        /// topOffset
        /// </summary>
        private double topOffset;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DropPreviewAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="adorningElement">The adorning element.</param>
        public DropPreviewAdorner(UIElement adornedElement, UIElement adorningElement)
            : base(adornedElement)
        {
            VisualBrush brush = new VisualBrush(adorningElement);
            this.child = new Rectangle();
            this.child.Width = adorningElement.RenderSize.Width;
            this.child.Height = adorningElement.RenderSize.Height;
            this.child.Fill = brush;
            this.child.IsHitTestVisible = false;
            System.Windows.Media.Animation.DoubleAnimation animation;
            animation = new System.Windows.Media.Animation.DoubleAnimation(0.3, 1, new Duration(TimeSpan.FromSeconds(1)));
            animation.AutoReverse = true;
            animation.RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever;
            brush.BeginAnimation(System.Windows.Media.Brush.OpacityProperty, animation);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the left offset.
        /// </summary>
        /// <value>The left offset.</value>
        public double LeftOffset
        {
            get
            {
                return this.leftOffset;
            }

            set
            {
                this.leftOffset = value;
                this.UpdatePosition();
            }
        }

        /// <summary>
        /// Gets or sets the top offset.
        /// </summary>
        /// <value>The top offset.</value>
        public double TopOffset
        {
            get
            {
                return this.topOffset;
            }

            set
            {
                this.topOffset = value;
                this.UpdatePosition();
            }
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>Returns either zero (no child elements) or one (one child element).</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a <see cref="T:System.Windows.Media.Transform"></see> for the adorner, based on the transform that is currently applied to the adorned element.
        /// </summary>
        /// <param name="transform">The transform that is currently applied to the adorned element.</param>
        /// <returns>A transform to apply to the adorner.</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.LeftOffset, this.TopOffset));
            return result;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"></see> object representing the amount of layout space needed by the adorner.
        /// </returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        /// <summary>
        /// When implemented in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see>-derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"></see>, and returns a child at the specified index from that element's collection of child elements. However, in this override, the only valid index is zero.
        /// </summary>
        /// <param name="index">Zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception should be raised.
        /// </returns>
        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            return this.child;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the position.
        /// </summary>
        private void UpdatePosition()
        {
            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Update(AdornedElement);
            }
        }
        #endregion        
    }
}