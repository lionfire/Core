#define CENTERED // Jared
using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using ScottLogic.Util;

namespace LionFire.Avalon
{
    /// <summary>
    /// A pie piece shape
    /// </summary>
    public class PiePiece : Shape
    {
        //public PiePiece()
        //{
        //}

        //protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        //{
        //    base.OnRenderSizeChanged(sizeInfo);

        //}

        #region dependency properties

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The radius of this pie piece
        /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty PushOutProperty =
            DependencyProperty.Register("PushOut", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The distance to 'push' this pie piece out from the centre.
        /// </summary>
        public double PushOut
        {
            get { return (double)GetValue(PushOutProperty); }
            set { SetValue(PushOutProperty, value); }
        }

        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The inner radius of this pie piece
        /// </summary>
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty WedgeAngleProperty =
            DependencyProperty.Register("WedgeAngle", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The wedge angle of this pie piece in degrees
        /// </summary>
        public double WedgeAngle
        {
            get { return (double)GetValue(WedgeAngleProperty); }
            set
            {
                SetValue(WedgeAngleProperty, value);
                this.Percentage = (value / 360.0);
            }
        }

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, null, new CoerceValueCallback((d, o) => o)), new ValidateValueCallback((x) =>
            {
                //throw new Exception("Hello: " + (x == null ? "null" : x.ToString()));
                //System.Diagnostics.Trace.WriteLine(x == null ? "null" : x.ToString()); 
                return true;
            }));


        /// <summary>
        /// The rotation, in degrees, from the Y axis vector of this pie piece.
        /// </summary>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        public static readonly DependencyProperty CentreXProperty =
            DependencyProperty.Register("CentreX",
            typeof(double), typeof(PiePiece),

            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                null, new CoerceValueCallback((dobj, ob) =>
                {
                    if (ob as String != null)
                    {
                        return Convert.ToDouble((string)ob);
                    }
                    return ob;
                    //throw new Exception("HelloCoerce: " + (ob == null ? "null" : ob.ToString()));
                    //return ob;
                })),

                new ValidateValueCallback((x) =>
            {
                //throw new Exception("Hello: " + (x == null ? "null" : x.ToString()));
                //System.Diagnostics.Trace.WriteLine(x == null ? "null" : x.ToString());
                return true;
            }));

        /// <summary>
        /// The X coordinate of centre of the circle from which this pie piece is cut.
        /// </summary>
        public double CentreX
        {
            get { return (double)GetValue(CentreXProperty); }
            set { SetValue(CentreXProperty, value); }
        }

        public static readonly DependencyProperty CentreYProperty =
            DependencyProperty.Register("CentreY", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The Y coordinate of centre of the circle from which this pie piece is cut.
        /// </summary>
        public double CentreY
        {
            get { return (double)GetValue(CentreYProperty); }
            set { SetValue(CentreYProperty, value); }
        }

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// The percentage of a full pie that this piece occupies.
        /// </summary>
        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            private set { SetValue(PercentageProperty, value); }
        }

        public static readonly DependencyProperty PieceValueProperty =
            DependencyProperty.Register("PieceValue", typeof(double), typeof(PiePiece),
            new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// The value that this pie piece represents.
        /// </summary>
        public double PieceValue
        {
            get { return (double)GetValue(PieceValueProperty); }
            set { SetValue(PieceValueProperty, value); }
        }


        #endregion


        #region DrawStartLine
        // Jared Added

        /// <summary>
        /// DrawStartLine Dependency Property
        /// </summary>
        public static readonly DependencyProperty DrawStartLineProperty =
            DependencyProperty.Register("DrawStartLine", typeof(bool), typeof(PiePiece),
                new FrameworkPropertyMetadata((bool)true,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the DrawStartLine property. This dependency property 
        /// indicates whether the stroke is drawn at the end of the pie piece.
        /// </summary>
        public bool DrawStartLine
        {
            get { return (bool)GetValue(DrawStartLineProperty); }
            set { SetValue(DrawStartLineProperty, value); }
        }

        #endregion

        #region DrawEndLine

        /// <summary>
        /// DrawEndLine Dependency Property
        /// </summary>
        public static readonly DependencyProperty DrawEndLineProperty =
            DependencyProperty.Register("DrawEndLine", typeof(bool), typeof(PiePiece),
                new FrameworkPropertyMetadata((bool)true,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the DrawEndLine property. This dependency property 
        /// indicates whether the stroke is drawn at the end of the pie piece.
        /// </summary>
        public bool DrawEndLine
        {
            get { return (bool)GetValue(DrawEndLineProperty); }
            set { SetValue(DrawEndLineProperty, value); }
        }

        #endregion

        #region DrawInnerStroke

        /// <summary>
        /// DrawInnerStroke Dependency Property
        /// </summary>
        public static readonly DependencyProperty DrawInnerStrokeProperty =
            DependencyProperty.Register("DrawInnerStroke", typeof(bool), typeof(PiePiece),
                new FrameworkPropertyMetadata((bool)true));

        /// <summary>
        /// Gets or sets the DrawInnerStroke property. This dependency property 
        /// indicates whether the inner stroke is drawn.
        /// </summary>
        public bool DrawInnerStroke
        {
            get { return (bool)GetValue(DrawInnerStrokeProperty); }
            set { SetValue(DrawInnerStrokeProperty, value); }
        }

        #endregion

        #region SmoothStrokeJoin

        /// <summary>
        /// SmoothStrokeJoin Dependency Property
        /// </summary>
        public static readonly DependencyProperty SmoothStrokeJoinProperty =
            DependencyProperty.Register("SmoothStrokeJoin", typeof(bool), typeof(PiePiece),
                new FrameworkPropertyMetadata((bool)true,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the SmoothStrokeJoin property. This dependency property 
        /// indicates whether stroke joins are drawn smoothed.
        /// </summary>
        public bool SmoothStrokeJoin
        {
            get { return (bool)GetValue(SmoothStrokeJoinProperty); }
            set { SetValue(SmoothStrokeJoinProperty, value); }
        }

        #endregion

        #region CentreAngle

        /// <summary>
        /// CentreAngle Dependency Property
        /// </summary>
        public static readonly DependencyProperty CentreAngleProperty =
            DependencyProperty.Register("CentreAngle", typeof(double), typeof(PiePiece),
                new FrameworkPropertyMetadata((double)double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnCentreAngleChanged)));

        /// <summary>
        /// Gets or sets the CentreAngle property. This dependency property 
        /// indicates where the centre of the pie piece points.  If set (not NaN), it overrides RotationAngle
        /// </summary>
        public double CentreAngle
        {
            get { return (double)GetValue(CentreAngleProperty); }
            set { SetValue(CentreAngleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CentreAngle property.
        /// </summary>
        private static void OnCentreAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PiePiece target = (PiePiece)d;
            double oldCentreAngle = (double)e.OldValue;
            double newCentreAngle = target.CentreAngle;
            target.OnCentreAngleChanged(oldCentreAngle, newCentreAngle);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the CentreAngle property.
        /// </summary>
        protected virtual void OnCentreAngleChanged(double oldCentreAngle, double newCentreAngle)
        {
        }

        #endregion

        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    DrawGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        /// <summary>
        /// Draws the pie piece
        /// </summary>
        private void DrawGeometry(StreamGeometryContext context)
        {
#if CENTERED
            double CentreX = (this.ActualWidth / 2) + this.CentreX;
            double CentreY = (this.ActualHeight / 2) + this.CentreY;

            //startPoint = new Point(
            //    (this.ActualWidth / 2) + CentreX,
            //    (this.ActualHeight / 2) + CentreY);
#else
#endif
            Point startPoint = new Point(CentreX, CentreY);

            var RotationAngle = this.RotationAngle;

            if (!double.IsNaN(CentreAngle))
            {
                RotationAngle = CentreAngle - (WedgeAngle / 2);
            }

            Point innerArcStartPoint = Utils.ComputeCartesianCoordinate(RotationAngle, InnerRadius);
            innerArcStartPoint.Offset(CentreX, CentreY);

            Point innerArcEndPoint = Utils.ComputeCartesianCoordinate(RotationAngle + WedgeAngle, InnerRadius);
            innerArcEndPoint.Offset(CentreX, CentreY);

            Point outerArcStartPoint = Utils.ComputeCartesianCoordinate(RotationAngle, Radius);
            outerArcStartPoint.Offset(CentreX, CentreY);

            Point outerArcEndPoint = Utils.ComputeCartesianCoordinate(RotationAngle + WedgeAngle, Radius);
            outerArcEndPoint.Offset(CentreX, CentreY);

            bool largeArc = WedgeAngle > 180.0;

            if (PushOut > 0)
            {
                Point offset = Utils.ComputeCartesianCoordinate(RotationAngle + WedgeAngle / 2, PushOut);
                innerArcStartPoint.Offset(offset.X, offset.Y);
                innerArcEndPoint.Offset(offset.X, offset.Y);
                outerArcStartPoint.Offset(offset.X, offset.Y);
                outerArcEndPoint.Offset(offset.X, offset.Y);

            }

            Size outerArcSize = new Size(Radius, Radius);
            Size innerArcSize = new Size(InnerRadius, InnerRadius);

            context.BeginFigure(innerArcStartPoint, true, true);
            context.LineTo(outerArcStartPoint, DrawStartLine, SmoothStrokeJoin);
            context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, SmoothStrokeJoin);
            context.LineTo(innerArcEndPoint, DrawEndLine, SmoothStrokeJoin);
            context.ArcTo(innerArcStartPoint, innerArcSize, 0, largeArc, SweepDirection.Counterclockwise, DrawInnerStroke, SmoothStrokeJoin);
        }
    }

}
