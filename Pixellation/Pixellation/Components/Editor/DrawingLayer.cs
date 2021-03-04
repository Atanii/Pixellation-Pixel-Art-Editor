using Pixellation.Components.Editor.Memento;
using Pixellation.Components.Event;
using Pixellation.Interfaces;
using Pixellation.MementoPattern;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    /// <summary>
    /// Class representing a layer of surface for drawing.
    /// </summary>
    public class DrawingLayer : FrameworkElement, IOriginator<LayerMemento, IPixelEditorEventType>
    {
        #region Fields And Properties

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid LayerGuid { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// String form of <see cref="LayerGuid"/>.
        /// </summary>
        public string Id => LayerGuid.ToString();

        /// <summary>
        /// Owner editor of the layer.
        /// </summary>
        private readonly IDrawingHelper _owner;

        private WriteableBitmap _bitmap;

        /// <summary>
        /// Bitmap for drawing.
        /// </summary>
        public WriteableBitmap Bitmap
        {
            get => _bitmap;
            set
            {
                if (value != null)
                {
                    _bitmap = value;
                    InvalidateVisual();
                    PropertyUpdated?.Invoke();
                }
            }
        }

        private string _name;

        /// <summary>
        /// Name of the layer shown on the UI.
        /// </summary>
        public string LayerName
        {
            get => _name;
            set
            {
                _name = value;
                PropertyUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Opacity of the layer.
        /// </summary>
        public new double Opacity
        {
            get
            {
                if (Visible)
                {
                    return base.Opacity;
                }
                return 0d;
            }
            set
            {
                base.Opacity = value;
                InvalidateVisual();
                PropertyUpdated?.Invoke();
            }
        }

        private bool _visible = true;

        /// <summary>
        /// Indicates if visible or hidden.
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                InvalidateVisual();
                PropertyUpdated?.Invoke();
            }
        }

        private bool _tiledModeEnabled = Properties.Settings.Default.DefaultTiledModeEnabled;

        /// <summary>
        /// Indicates if tiled mode rendering is enabled.
        /// </summary>
        public bool TiledModeEnabled
        {
            get => _tiledModeEnabled;
            set
            {
                _tiledModeEnabled = value;
                InvalidateVisual();
            }
        }

        private float _tiledModeOpacity = Properties.Settings.Default.DefaultTiledOpacity;

        /// <summary>
        /// Tile opacity for tiled mdoe.
        /// </summary>
        public float TiledModeOpacity
        {
            get => _tiledModeOpacity;
            set
            {
                _tiledModeOpacity = value;
                InvalidateVisual();
            }
        }

        #endregion Fields And Properties

        #region Events

        /// <summary>
        /// Indicates propertyupdate like renaming or opacity change.
        /// </summary>
        public static event LayerEventHandler PropertyUpdated;

        #endregion Events

        #region Constructors, Init

        /// <summary>
        /// Inits layer.
        /// </summary>
        /// <param name="owner">Owner editor.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="visible">Is the layer visible or not.</param>
        /// <param name="opacity">Opacity of the layer.</param>
        public DrawingLayer(IDrawingHelper owner, string layerName = "", bool visible = true, double opacity = 1.0)
        {
            _owner = owner;
            InitBitmap();
            LayerName = layerName == "" ? "Layer-" + (new DateTime()).Ticks : layerName;
            Visible = visible;
            Opacity = opacity;
        }

        /// <summary>
        /// Inits layer.
        /// </summary>
        /// <param name="owner">Owner editor.</param>
        /// <param name="bitmap">Bitmap for drawing on.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="visible">Is the layer visible or not.</param>
        /// <param name="opacity">Opacity of the layer.</param>
        public DrawingLayer(IDrawingHelper owner, WriteableBitmap bitmap, string layerName = "", bool visible = true, double opacity = 1.0)
        {
            _owner = owner;
            InitBitmap(bitmap);
            LayerName = layerName == "" ? "Layer-" + (new DateTime()).Ticks : layerName;
            Visible = visible;
            Opacity = opacity;
        }

        /// <summary>
        /// Inits layer from a memento.
        /// </summary>
        /// <param name="owner">Owner editor.</param>
        /// <param name="mem">Saved state to create from.</param>
        public DrawingLayer(IDrawingHelper owner, LayerMemento mem)
        {
            _owner = owner;
            InitBitmap(mem.Bitmap);
            LayerName = mem.LayerName;
            Visible = mem.Visible;
            Opacity = mem.Opacity;
            LayerGuid = mem.LayerGuid;
        }

        /// <summary>
        /// Inits the underlying bitmap.
        /// </summary>
        /// <param name="bitmap">Init from this bitmap if provided.</param>
        private void InitBitmap(WriteableBitmap bitmap = null)
        {
            _bitmap = bitmap ?? BitmapFactory.New(_owner.PixelWidth, _owner.PixelHeight);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }

        #endregion Constructors, Init

        #region Conversions, Cloning

        /// <summary>
        /// Represents the layer as a string.
        /// </summary>
        /// <returns>Name of this layer.</returns>
        public override string ToString() => LayerName;

        /// <summary>
        /// Creates a copy of this <see cref="DrawingLayer"/>.
        /// </summary>
        /// <param name="deep">Copy exact same LayerName if set to true.</param>
        /// <param name="sameName">Copy will have the same name.</param>
        /// <returns>Created copy.</returns>
        public DrawingLayer Clone(bool deep = false, bool sameName = false)
        {
            var bmp2 = _bitmap.Clone();
            if (deep)
            {
                return new DrawingLayer(_owner, bmp2, LayerName, Visible, Opacity);
            }
            if (sameName)
            {
                return new DrawingLayer(_owner, bmp2, LayerName, Visible, Opacity);
            }
            string newName;
            if (int.TryParse(LayerName, out int val))
            {
                newName = (++val).ToString();
            }
            else if (int.TryParse(LayerName.Split('_')[^1], out int val2))
            {
                newName = LayerName.Split('_')[0] + '_' + (++val2).ToString();
            }
            else
            {
                newName = LayerName + "_1";
            }
            return new DrawingLayer(_owner, bmp2, newName, Visible, Opacity);
        }

        #endregion Conversions, Cloning

        #region Memento

        /// <summary>
        /// Restore layer state from memento.
        /// </summary>
        /// <param name="mem">Saved state.</param>
        public void Restore(LayerMemento mem)
        {
            LayerName = mem.LayerName == "" ? "Layer-" + (new DateTime()).Ticks : mem.LayerName;
            Visible = mem.Visible;
            Opacity = mem.Opacity;
            _bitmap = mem.Bitmap;
            LayerGuid = mem.LayerGuid;
        }

        /// <summary>
        /// Generates a memento representing the current state of the layer.
        /// </summary>
        /// <param name="mTypeValue">Value of <see cref="IPixelEditorEventType"/>.</param>
        /// <returns>Resulting memento.</returns>
        public LayerMemento GetMemento(int mTypeValue)
        {
            return new LayerMemento
            (
                _owner,
                mTypeValue,
                _owner.GetIndex(this),
                this
            );
        }

        /// <summary>
        /// Saves the state of layer for possible undo.
        /// </summary>
        /// <param name="mTypeValue">Value of <see cref="IPixelEditorEventType"/>.</param>
        public void SaveState(int mTypeValue)
        {
            _owner.SaveState(mTypeValue, _owner.ActiveLayerIndex);
        }

        #endregion Memento

        #region Bitmap, Rendering

        /// <summary>
        /// Renders <see cref="Bitmap"/> (with tiled mode if enabled) if the layer is visible.
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (!Visible)
            {
                return;
            }

            var width = _owner.MagnifiedWidth;
            var height = _owner.MagnifiedHeight;

            dc.DrawImage(_bitmap, new Rect(0, 0, width, height));

            if (_owner.TiledModeEnabled)
            {
                var temp = Bitmap.Clone();
                temp.BlitRender(temp, false, _owner.TiledOpacity);
                for (int x = -5 * width; x <= 5 * width; x += width)
                {
                    for (int y = -5 * height; y <= 5 * height; y += height)
                    {
                        if (x == 0 && y == 0)
                            continue;
                        dc.DrawImage(temp, new Rect(x, y, width, height));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the copy of <see cref="Bitmap"/> without applying layer opacity.
        /// </summary>
        /// <returns></returns>
        public WriteableBitmap GetWriteableBitmap() => _bitmap.Clone();

        /// <summary>
        /// Applies the <see cref="Opacity"/> to the copy of the underlying bitmap then returns the it.
        /// Does not affect original bitmap!
        /// </summary>
        /// <returns><see cref="Bitmap"/> with applied layer opacity.</returns>
        public WriteableBitmap GetWriteableBitmapWithAppliedOpacity()
        {
            var temp = _bitmap.Clone();
            temp.BlitRender(temp, false, (float)Opacity);
            return temp;
        }

        /// <summary>
        /// Mirrors the layer.
        /// </summary>
        /// <param name="horizontal">Horizontal or vertical mirroring.</param>
        public void Mirror(bool horizontal = true)
        {
            _bitmap = _bitmap.Flip(
                horizontal ?
                WriteableBitmapExtensions.FlipMode.Horizontal :
                WriteableBitmapExtensions.FlipMode.Vertical
            );
        }

        /// <summary>
        /// Rotates the layer by the given degree.
        /// </summary>
        /// <param name="angleInDegree"></param>
        public void Rotate(int angleInDegree)
        {
            _bitmap = _bitmap.Rotate(angleInDegree);
        }

        /// <summary>
        /// Resizes the layer.
        /// </summary>
        /// <param name="newWidth">New width in pixels.</param>
        /// <param name="newHeight">New height in pixels.</param>
        public void Resize(int newWidth, int newHeight)
        {
            _bitmap = _bitmap.Resize(
                newWidth, newHeight,
                WriteableBitmapExtensions.Interpolation.NearestNeighbor
            );
        }

        /// <summary>
        /// Clears the layer leaving only transparent pixels.
        /// </summary>
        public void Clear()
        {
            _bitmap.Clear(Colors.Transparent);
        }

        #endregion Bitmap, Rendering
    }
}