using Pixellation.Models;
using Pixellation.Utils;
using Pixellation.Utils.UndoRedo;
using System;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class DrawingLayer : DrawingSurface, IPreviewable, IOriginator<DrawingLayer.Memento>
    {
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

        public bool Visible { get; set; }

        public event EventHandler RaiseImageUpdatedEvent;

        public class Memento : IMemento
        {
            private readonly MementoType _mType;
            private readonly IOriginatorHandler<Memento> _originator;

            public int LayerIndex { get; private set; }
            public string LayerName { get; private set; }
            public double Opacity { get; private set; }
            public bool Visible { get; private set; }
            public WriteableBitmap Bitmap { get; private set; }

            public Memento(IOriginatorHandler<Memento> originator, MementoType type, int layerIndex, DrawingLayer original)
            {
                _originator = originator;
                _mType = type;

                LayerIndex = layerIndex;

                LayerName = original.LayerName;
                Opacity = original.Opacity;
                Visible = original.Visible;
                Bitmap = original._bitmap.Clone();
            }

            public IMemento Restore()
            {
                return _originator.HandleRestore(this);
            }

            public MementoType GetMementoType() => _mType;
        }

        public DrawingLayer(PixelEditor owner, string layerName = "", bool visible = true) : base(owner)
        {
            if (layerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = layerName;
            }
            Visible = visible;
        }

        public DrawingLayer(PixelEditor owner, WriteableBitmap bitmap, string layerName = "", bool visible = true) : base(owner, bitmap)
        {
            if (layerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = layerName;
            }
            Visible = visible;
        }

        public DrawingLayer(PixelEditor owner, LayerModel model, bool visible = true) : base(
            owner,
            model.LayerBitmap.ToWriteableBitmap(model.Width, model.Height, model.Stride)
        ) {
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

        public DrawingLayer(PixelEditor owner, Memento mem) : base(
            owner,
            mem.Bitmap
        )
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
        }

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

        public void Restore(Memento mem)
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
            SetBitmap(mem.Bitmap);
        }

        public Memento GetMemento(MementoType mType)
        {
            return new Memento
            (
                _owner,
                mType,
                _owner.GetIndex(this),
                this
            );
        }
    }
}