using Pixellation.Components.Editor;
using Pixellation.Components.Event;
using Pixellation.Models;
using Pixellation.Utils;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace Pixellation.Tools
{
    using MColor = System.Windows.Media.Color;

    /// <summary>
    /// Base class for all drawing tools used in Pixellation.
    /// </summary>
    public abstract class BaseTool
    {
        /// <summary>
        /// Cursor for the drawing tool.
        /// </summary>
        public abstract Cursor ToolCursor { get; }

        /// <summary>
        /// Editor magnification.
        /// </summary>
        protected static int _magnification;

        /// <summary>
        /// Draw area width.
        /// </summary>
        protected static int _surfaceWidth;

        /// <summary>
        /// Draw area height.
        /// </summary>
        protected static int _surfaceHeight;

        /// <summary>
        /// Layer to draw on.
        /// </summary>
        protected static DrawingLayer _layer;

        protected static WriteableBitmap _drawSurface;

        /// <summary>
        /// Layer for drawing preview.
        /// </summary>
        protected static DrawingLayer _previewLayer;

        protected static WriteableBitmap _previewDrawSurface;

        public delegate void ToolEventHandler(ToolEventArgs args);

        public static event ToolEventHandler RaiseToolEvent;

        private static bool _isMementoLocked = false;

        private MColor _toolColor;

        /// <summary>
        /// Drawing color for tool.
        /// </summary>
        public MColor ToolColor
        {
            get => EraserModeOn ? MColor.FromArgb(0, 0, 0, 0) : _toolColor;
            set => _toolColor = value;
        }

        protected MColor PointerColor = MColor.FromArgb(100, 50, 50, 50);

        /// <summary>
        /// Indicates if tool is used as an eraser.
        /// </summary>
        public bool EraserModeOn { get; set; } = false;

        /// <summary>
        /// Mirror mode state for the drawing tool.
        /// </summary>
        public MirrorModeStates MirrorMode { get; set; }

        /// <summary>
        /// Thickness used with the drawing tool.
        /// </summary>
        public ToolThickness Thickness { get; set; }

        /// <summary>
        /// Is compatible with different thickness settings?
        /// </summary>
        public virtual bool ThicknessCompatible { get; } = true;

        /// <summary>
        /// Is compatible with different mirror mode settings?
        /// </summary>
        public virtual bool MirrorModeCompatible { get; } = true;

        /// <summary>
        /// Is compatible with erasermorde?
        /// </summary>
        public virtual bool EraserModeCompatible { get; } = true;

        protected bool _showPointerFlag = true;

        protected BaseTool()
        {
            ToolColor = MColor.FromRgb(0, 0, 0);
        }

        protected void OnRaiseToolEvent(ToolEventArgs e)
        {
            RaiseToolEvent?.Invoke(e);
        }

        /// <summary>
        /// Sets the parameters for drawing.
        /// </summary>
        /// <param name="magnification">Editor magnification.</param>
        /// <param name="pixelWidth">Editable area width in pixels.</param>
        /// <param name="pixelHeight">Editable area height in pixels.</param>
        /// <param name="ds">Layer to draw on.</param>
        /// <param name="previewLayer">Layer to preview drawing on.</param>
        /// <param name="mirrorModeState">Mirror mode state for drawing.</param>
        /// <param name="thickness">Line thickness for drawing.</param>
        public void SetAllDrawingCircumstances(
            int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer, MirrorModeStates mirrorModeState = MirrorModeStates.OFF, ToolThickness thickness = ToolThickness.NORMAL)
        {
            _magnification = magnification;
            _surfaceWidth = pixelWidth;
            _surfaceHeight = pixelHeight;
            _layer = ds;
            _drawSurface = ds.Bitmap;
            _previewLayer = previewLayer;
            _previewDrawSurface = _previewLayer.Bitmap;
            MirrorMode = mirrorModeState;
            Thickness = thickness;
        }

        public void SetMagnification(int magnification)
        {
            _magnification = magnification;
        }

        public void SetDrawColor(Color c)
        {
            ToolColor = c.ToMediaColor();
        }

        protected static bool IsMouseDown(MouseEventArgs e) => e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed;

        public virtual void OnMouseDown(MouseEventArgs e)
        {
            return;
        }

        public virtual void OnMouseUp(MouseEventArgs e)
        {
            return;
        }

        /// <summary>
        /// Shows mousepointer with applied thickness in a form of a greyed area.
        /// Should be called before OnMouseMove to make sure clearing preview area won't conflict.
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseMoveTraceWithPointer(MouseEventArgs e)
        {
            void showPointer(IntPoint p)
            {
                if (ThicknessCompatible)
                {
                    SetPixelWithThickness(_previewDrawSurface, p.X, p.Y, PointerColor, Thickness);
                }
                else
                {
                    SetPixelWithThickness(_previewDrawSurface, p.X, p.Y, PointerColor, ToolThickness.NORMAL);
                }
            }

            if (!_showPointerFlag)
            {
                return;
            }

            _previewDrawSurface.Clear();

            var p = e.GetPosition(_previewLayer).DivideByIntAsIntPoint(_magnification);

            showPointer(p);
            if (MirrorModeCompatible && MirrorMode != MirrorModeStates.OFF)
            {
                p = Mirr(p);
                showPointer(p);
            }
        }

        public virtual void OnMouseMove(MouseEventArgs e)
        {
            return;
        }

        public virtual void OnKeyDown(KeyEventArgs e)
        {
            return;
        }

        public virtual void Reset()
        {
            return;
        }

        /// <summary>
        /// Clean not applied drawn content.
        /// </summary>
        public virtual void Clean()
        {
            _previewDrawSurface.Clear();
        }

        /// <summary>
        /// Saves the state of the drawing area for possible undo.
        /// </summary>
        /// <param name="lockMemento">Indicates is save should be locked.</param>
        protected static void SaveLayerMemento(bool lockMemento = false)
        {
            if (!_isMementoLocked)
            {
                _layer.SaveState(IPixelEditorEventType.LAYER_PIXELS_CHANGED);
            }
            if (lockMemento)
            {
                _isMementoLocked = true;
            }
        }

        /// <summary>
        /// Unlocks saving.
        /// </summary>
        protected static void UnlockMemento()
        {
            _isMementoLocked = false;
        }

        public static bool OutOfBounds(IntPoint p) => p.X < 0 || p.X >= _surfaceWidth || p.Y < 0 || p.Y >= _surfaceHeight;

        public static int Min(int a, int b) => a <= b ? a : b;

        public static int Max(int a, int b) => a >= b ? a : b;

        protected IntPoint MirrH(IntPoint p)
        {
            p.Y = _surfaceHeight - p.Y;
            return p;
        }

        protected IntPoint MirrV(IntPoint p)
        {
            p.X = _surfaceWidth - p.X;
            return p;
        }

        /// <summary>
        /// Mirrors the given point according to the current <see cref="MirrorModeStates"/>.
        /// In case MirrorMode is not turned on, it returns the point without any change.
        /// </summary>
        /// <param name="p">Point to mirror.</param>
        /// <returns>Mirrored point.</returns>
        protected IntPoint Mirr(IntPoint p)
        {
            switch (MirrorMode)
            {
                case MirrorModeStates.OFF:
                    break;

                case MirrorModeStates.VERTICAL:
                    p = MirrV(p);
                    break;

                case MirrorModeStates.HORIZONTAL:
                    p = MirrH(p);
                    break;

                default:
                    break;
            }
            return p;
        }

        /// <summary>
        /// Draws one unit of draw area (default is 1px).
        /// Based on the given thickness it can affect the surrounding pixels making a bigger draw "unit".
        /// </summary>
        /// <param name="bmp">Bitmap to draw on.</param>
        /// <param name="x0">X component of coordinate.</param>
        /// <param name="y0">Y component of coordinate.</param>
        /// <param name="c">Color to draw with.</param>
        /// <param name="thickness">Thickness to use.</param>
        public static void SetPixelWithThickness(WriteableBitmap bmp, int x0, int y0, MColor c, ToolThickness thickness)
        {
            if (x0 >= 0 && y0 >= 0 && x0 < bmp.PixelWidth && y0 < bmp.PixelHeight)
                bmp.SetPixel(x0, y0, c);

            if (thickness > ToolThickness.NORMAL)
            {
                if ((x0 - 1) >= 0 && y0 >= 0 && (x0 - 1) < bmp.PixelWidth && y0 < bmp.PixelHeight)
                    bmp.SetPixel(x0 - 1, y0, c);
                if ((x0 - 1) >= 0 && (y0 - 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                    bmp.SetPixel(x0 - 1, y0 - 1, c);
                if (x0 >= 0 && (y0 - 1) >= 0 && x0 < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                    bmp.SetPixel(x0, y0 - 1, c);

                if (thickness > ToolThickness.MEDIUM)
                {
                    if ((x0 + 1) >= 0 && (y0 + 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                        bmp.SetPixel(x0 + 1, y0 + 1, c);
                    if ((x0 + 1) >= 0 && (y0 - 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                        bmp.SetPixel(x0 + 1, y0 - 1, c);
                    if ((x0 - 1) >= 0 && (y0 + 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                        bmp.SetPixel(x0 - 1, y0 + 1, c);
                    if ((x0 + 1) >= 0 && y0 >= 0 && (x0 + 1) < bmp.PixelWidth && y0 < bmp.PixelHeight)
                        bmp.SetPixel(x0 + 1, y0, c);
                    if (x0 >= 0 && (y0 + 1) >= 0 && x0 < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                        bmp.SetPixel(x0, y0 + 1, c);
                }
            }
        }

        /// <summary>
        /// Gets a cursor from the resource.
        /// </summary>
        /// <param name="cursorFileName">Name of the cursor file with the extension.</param>
        /// <returns>Resulting object.</returns>
        protected static Cursor GetCursorFromResource(string cursorFileName)
        {
            StreamResourceInfo cursorResStream = Application.GetResourceStream(
                new Uri($"pack://application:,,,/Resources/cursors/{cursorFileName}", UriKind.RelativeOrAbsolute)
            );
            return new Cursor(cursorResStream.Stream);
        }
    }
}