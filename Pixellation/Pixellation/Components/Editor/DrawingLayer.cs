﻿using Pixellation.Components.Editor.Memento;
using Pixellation.Interfaces;
using Pixellation.MementoPattern;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class DrawingLayer : FrameworkElement, IOriginator<LayerMemento, IPixelEditorEventType>
    {
        #region Fields And Properties

        public Guid LayerGuid { get; private set; } = Guid.NewGuid();
        public string Id => LayerGuid.ToString();

        private readonly IDrawingHelper _owner;

        private WriteableBitmap _bitmap;

        public WriteableBitmap Bitmap
        {
            get => _bitmap;
            set
            {
                if (value != null)
                {
                    _bitmap = value;
                    InvalidateVisual();
                    OnUpdated?.Invoke();
                }
            }
        }

        private string _name;

        public string LayerName
        {
            get => _name;
            set
            {
                _name = value;
                OnUpdated?.Invoke();
            }
        }

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
                OnUpdated?.Invoke();
            }
        }

        private bool _visible = true;

        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                InvalidateVisual();
            }
        }

        private bool _tiledModeOn = Properties.Settings.Default.DefaultTiledModeOn;

        public bool TiledModeOn
        {
            get => _tiledModeOn;
            set
            {
                _tiledModeOn = value;
                InvalidateVisual();
            }
        }

        private float _tiledModeOpacity = Properties.Settings.Default.DefaultTiledOpacity;

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

        public static event LayerEventHandler OnUpdated;

        #endregion Events

        #region Constructors, Init

        public DrawingLayer(IDrawingHelper owner, string layerName = "", bool visible = true, double opacity = 1.0)
        {
            _owner = owner;
            InitBitmap();
            LayerName = layerName == "" ? "Layer-" + (new DateTime()).Ticks : layerName;
            Visible = visible;
            Opacity = opacity;
        }

        public DrawingLayer(IDrawingHelper owner, WriteableBitmap bitmap, string layerName = "", bool visible = true, double opacity = 1.0)
        {
            _owner = owner;
            InitBitmap(bitmap);
            LayerName = layerName == "" ? "Layer-" + (new DateTime()).Ticks : layerName;
            Visible = visible;
            Opacity = opacity;
        }

        public DrawingLayer(IDrawingHelper owner, LayerMemento mem)
        {
            _owner = owner;
            InitBitmap(mem.Bitmap);
            LayerName = mem.LayerName;
            Visible = mem.Visible;
            Opacity = mem.Opacity;
        }

        private void InitBitmap(WriteableBitmap bitmap = null)
        {
            _bitmap = bitmap ?? BitmapFactory.New(_owner.PixelWidth, _owner.PixelHeight);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }

        #endregion Constructors, Init

        #region Conversions, Cloning

        public override string ToString() => LayerName;

        /// <summary>
        /// Creates a copy of this <see cref="DrawingLayer"/>.
        /// </summary>
        /// <param name="deep">Copy exact same LayerName if set to true.</param>
        /// <returns>Created copy.</returns>
        public DrawingLayer Clone(bool deep = false)
        {
            var bmp2 = _bitmap.Clone();
            if (deep)
            {
                return new DrawingLayer(_owner, bmp2, LayerName, Visible, Opacity);
            }
            return new DrawingLayer(_owner, bmp2, LayerName + "_copy", Visible, Opacity);
        }

        #endregion Conversions, Cloning

        #region Memento

        public void Restore(LayerMemento mem)
        {
            LayerName = mem.LayerName == "" ? "Layer-" + (new DateTime()).Ticks : mem.LayerName;
            Visible = mem.Visible;
            Opacity = mem.Opacity;
            _bitmap = mem.Bitmap;
            LayerGuid = mem.LayerGuid;
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

        public void SaveState(int mTypeValue)
        {
            _owner.SaveState(mTypeValue, _owner.ActiveLayerIndex);
        }

        #endregion Memento

        #region Bitmap, Rendering

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
            _bitmap = _bitmap.Flip(
                horizontal ?
                WriteableBitmapExtensions.FlipMode.Horizontal :
                WriteableBitmapExtensions.FlipMode.Vertical
            );
        }

        public void Rotate(int angleInDegree)
        {
            _bitmap = _bitmap.Rotate(angleInDegree);
        }

        public void Resize(int newWidth, int newHeight)
        {
            _bitmap = _bitmap.Resize(
                newWidth, newHeight,
                WriteableBitmapExtensions.Interpolation.NearestNeighbor
            );
        }

        public void Clear()
        {
            _bitmap.Clear(Colors.Transparent);
        }

        #endregion Bitmap, Rendering
    }
}