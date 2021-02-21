using Pixellation.Components.Editor.Memento;
using Pixellation.Models;
using Pixellation.Utils;
using Pixellation.Utils.MementoPattern;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class DrawingLayer : FrameworkElement, IOriginator<LayerMemento, IPixelEditorEventType>
    {
        #region Fields And Properties
        private PixelEditor _owner;

        private WriteableBitmap _bitmap;
        public WriteableBitmap Bitmap {
            get
            {
                return _bitmap;
            }
            set
            {
                if (value != null)
                {
                    _bitmap = value;
                }
            }
        }

        private string _name;
        public string LayerName {
            get { return _name; }
            set { _name = value; _owner.RefreshVisualsThenSignalUpdate(); }
        }

        public new double Opacity
        {
            get
            {
                if (Visible)
                {
                    return base.Opacity;
                }
                return base.Opacity;
            }
            set
            {
                base.Opacity = value;
            }
        }

        public int MagnifiedWidth => _bitmap.PixelWidth * _owner.Magnification;
        public int MagnifiedHeight => _bitmap.PixelHeight * _owner.Magnification;

        public bool Visible { get; set; }
        #endregion Fields And Properties

        #region Constructors, Init
        public DrawingLayer(PixelEditor owner, string layerName = "", bool visible = true, double opacity = 1.0)
        {
            InitBitmap(owner);
            if (layerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = layerName;
            }
            Visible = visible;
            Opacity = opacity;
        }

        public DrawingLayer(PixelEditor owner, WriteableBitmap bitmap, string layerName = "", bool visible = true, double opacity = 1.0)
        {
            InitBitmap(owner, bitmap);
            if (layerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = layerName;
            }
            Visible = visible;
            Opacity = opacity;
        }

        public DrawingLayer(PixelEditor owner, LayerModel model, bool visible = true)
        {
            InitBitmap(owner, model.LayerBitmap.ToWriteableBitmap(model.Width, model.Height, model.Stride));
            if (model.LayerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = model.LayerName;
            }
            Visible = visible;
            Opacity = model.Opacity;
        }

        public DrawingLayer(PixelEditor owner, LayerMemento mem)
        {
            InitBitmap(owner, mem.Bitmap);
            if (mem.LayerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = mem.LayerName;
            }
            Visible = mem.Visible;
            Opacity = mem.Opacity;
        }

        private void InitBitmap(PixelEditor owner, WriteableBitmap bitmap = null)
        {
            _owner = owner;
            if (bitmap != null)
            {
                _bitmap = bitmap;
            }
            else
            {
                _bitmap = BitmapFactory.New(_owner.PixelWidth, _owner.PixelHeight);
                _bitmap.Clear(Colors.Transparent);
            }
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }
        #endregion Constructors, Init

        #region Conversions, Cloning
        public override string ToString()
        {
            return LayerName;
        }

        public LayerModel ToLayerModel()
        {
            var src = _bitmap.ToImageSource();

            var width = _bitmap.PixelWidth;
            var height = _bitmap.PixelHeight;
            var stride = _bitmap.BackBufferStride;

            var bitmapData = new byte[height * stride];

            src.CopyPixels(bitmapData, stride, 0);

            return new LayerModel
            {
                LayerBitmap = bitmapData,
                LayerName = _name,
                Width = width,
                Height = height,
                Stride = stride,
                Opacity = Opacity
            };
        }

        public DrawingLayer Clone()
        {
            var bmp2 = _bitmap.Clone();
            return new DrawingLayer(_owner, bmp2, LayerName, Visible);
        }
        #endregion Conversions, Cloning

        #region Memento
        public void Restore(LayerMemento mem)
        {   
            if (mem.LayerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = mem.LayerName;
            }
            Visible = mem.Visible;
            Opacity = mem.Opacity;
            _bitmap = mem.Bitmap;
        }

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
        #endregion Memento

        #region Bitmap, Rendering
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var magnification = _owner.Magnification;
            var width = _bitmap.PixelWidth * magnification;
            var height = _bitmap.PixelHeight * magnification;

            dc.DrawImage(_bitmap, new Rect(0, 0, width, height));

            if (_owner.TiledModeOn)
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

        public void SetPixel(int x, int y, Color color) => _bitmap.SetPixel(x, y, color);

        public Color GetPixel(int x, int y) => _bitmap.GetPixel(x, y);

        public WriteableBitmap GetWriteableBitmap() => _bitmap.Clone();

        public WriteableBitmap GetWriteableBitmapWithAppliedOpacity()
        {
            var tmp = BitmapFactory.New(_bitmap.PixelWidth, _bitmap.PixelHeight);
            tmp.Clear(Colors.Transparent);

            for (int i = 0; i < tmp.Width; i++)
            {
                for (int j = 0; j < tmp.Height; j++)
                {
                    var c = _bitmap.GetPixel(i, j);
                    if (c.A == 0)
                    {
                        continue;
                    }
                    c.A = (byte)Math.Floor(Opacity * 255.0);
                    tmp.SetPixel(i, j, c);
                }
            }

            return tmp;
        }

        public void Mirror(bool horizontal = true)
        {
            if (horizontal)
            {
                _bitmap = _bitmap.Flip(WriteableBitmapExtensions.FlipMode.Horizontal);
            }
            else
            {
                _bitmap = _bitmap.Flip(WriteableBitmapExtensions.FlipMode.Vertical);
            }
        }

        public void Rotate(int angleInDegree) => _bitmap = _bitmap.Rotate(angleInDegree);

        public void Resize(int newWidth, int newHeight)
        {
            _bitmap = _bitmap.Resize(newWidth, newHeight, WriteableBitmapExtensions.Interpolation.NearestNeighbor);
        }
        #endregion Bitmap, Rendering
    }
}