using Pixellation.Models;
using Pixellation.Utils;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Base class for all select-copy-paste-cut tools used in Pixellation.
    /// </summary>
    /// <typeparam name="T">Type of inheriting class. Needed for the multiton pattern.</typeparam>
    internal abstract class BaseSelectionTool<T> : BaseMultitonTool<T> where T : class, ITool
    {
        protected IntPoint p0;
        protected IntPoint p1;
        protected IntPoint p1prev;

        private readonly static Color _selectionFillColour = Properties.Settings.Default.DefaultSelectionFillColor;

        protected IntRect _selectionArea = new IntRect();
        protected IntRect _copyArea = new IntRect();

        protected WriteableBitmap _copySrc;

        private bool _creating = false;
        private bool _dragging = false;
        private bool _click = false;

        protected BaseSelectionTool() : base()
        {
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            p0 = e.GetPosition(_previewLayer).DivideByIntAsIntPoint(_magnification);
            p1prev = p0;

            if (!_selectionArea.Contains(p0))
            {
                if (_copySrc != null)
                {
                    _copySrc.Clear();
                }

                _selectionArea.X = p0.X;
                _selectionArea.Y = p0.Y;
                _selectionArea.Width = 0;
                _selectionArea.Height = 0;

                _creating = true;
                _dragging = false;
                _click = true;
            }
            else
            {
                _dragging = true;
                _creating = false;
            }
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            _creating = false;
            if (_click)
            {
                _dragging = false;
                Reset();
            }
        }

        public override void OnMouseMove(MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _click = false;

                p1 = e.GetPosition(_previewLayer).DivideByIntAsIntPoint(_magnification);

                if (_creating)
                {
                    var diff = (p1 - p0);

                    if (diff.X < 0)
                    {
                        _selectionArea.X = p1.X;
                    }
                    if (diff.Y < 0)
                    {
                        _selectionArea.Y = p1.Y;
                    }

                    _selectionArea.Width = Math.Abs(diff.X);
                    _selectionArea.Height = Math.Abs(diff.Y);

                    var x1 = _selectionArea.X;
                    var y1 = _selectionArea.Y;
                    var x2 = _selectionArea.Right;
                    var y2 = _selectionArea.Bottom;

                    _previewDrawSurface.Clear();
                    DrawSelection(x1, y1, x2, y2, _selectionFillColour, _previewDrawSurface);
                }

                if (_dragging && !_creating)
                {
                    var diff = p1 - p1prev;

                    var tmpX = _selectionArea.X + diff.X;
                    var tmpY = _selectionArea.Y + diff.Y;
                    var tmpX2 = tmpX + _selectionArea.Width;
                    var tmpY2 = tmpY + _selectionArea.Height;

                    if (tmpX < 0)
                    {
                        tmpX = 0;
                        tmpX2 = tmpX + _selectionArea.Width;
                    }
                    else if (tmpX2 >= _surfaceWidth)
                    {
                        tmpX2 = _surfaceWidth;
                        tmpX = tmpX2 - _selectionArea.Width;
                    }

                    if (tmpY < 0)
                    {
                        tmpY = 0;
                        tmpY2 = tmpY + _selectionArea.Height;
                    }
                    else if (tmpY2 >= _surfaceHeight)
                    {
                        tmpY2 = _surfaceHeight;
                        tmpY = tmpY2 - _selectionArea.Height;
                    }

                    _selectionArea.X = tmpX;
                    _selectionArea.Y = tmpY;

                    var x1 = _selectionArea.X;
                    var y1 = _selectionArea.Y;
                    var x2 = tmpX2;
                    var y2 = tmpY2;

                    _previewDrawSurface.Clear();
                    DrawSelection(x1, y1, x2, y2, _selectionFillColour, _previewDrawSurface);
                }

                p1prev = p1;
            }
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.X)
            {
                _copyArea.X = _selectionArea.X;
                _copyArea.Y = _selectionArea.Y;
                _copyArea.Width = _selectionArea.Width;
                _copyArea.Height = _selectionArea.Height;
                _copySrc = _drawSurface.Clone();

                var x1 = _selectionArea.X;
                var y1 = _selectionArea.Y;
                var x2 = _selectionArea.Right;
                var y2 = _selectionArea.Bottom;

                CutClear(x1, y1, x2, y2, Colors.Transparent, _drawSurface);
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.C)
            {
                _copyArea.X = _selectionArea.X;
                _copyArea.Y = _selectionArea.Y;
                _copyArea.Width = _selectionArea.Width;
                _copyArea.Height = _selectionArea.Height;
                _copySrc = _drawSurface.Clone();
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V)
            {
                var dest = new Rect(_selectionArea.X, _selectionArea.Y, _selectionArea.Width, _selectionArea.Height);
                var cpy = new Rect(_copyArea.X, _copyArea.Y, _copyArea.Width, _copyArea.Height);
                _drawSurface.Blit(dest, _copySrc, cpy, WriteableBitmapExtensions.BlendMode.Alpha);
            }
        }

        /// <summary>
        /// Resets the selection and copy area.
        /// </summary>
        public override void Reset()
        {
            _selectionArea = new IntRect();

            _copyArea = new IntRect();

            if (_copySrc != null)
            {
                _copySrc.Clear();
            }

            _previewDrawSurface.Clear();
        }

        /// <summary>
        /// Cuts the selected area.
        /// </summary>
        /// <param name="x1">X component of first point.</param>
        /// <param name="y1">Y component of first point.</param>
        /// <param name="x2">X component of second point.</param>
        /// <param name="y2">Y component of second point.</param>
        /// <param name="c">Color for indicating selection area.</param>
        /// <param name="surface">Surface to represent the sleection area on.</param>
        protected abstract void CutClear(int x1, int y1, int x2, int y2, Color c, WriteableBitmap surface);

        /// <summary>
        /// Draws the selection area.
        /// </summary>
        /// <param name="x1">X component of first point.</param>
        /// <param name="y1">Y component of first point.</param>
        /// <param name="x2">X component of second point.</param>
        /// <param name="y2">Y component of second point.</param>
        /// <param name="c">Color for indicating selection area.</param>
        /// <param name="surface">Surface to represent the sleection area on.</param>
        protected abstract void DrawSelection(int x1, int y1, int x2, int y2, Color c, WriteableBitmap surface);
    }
}
