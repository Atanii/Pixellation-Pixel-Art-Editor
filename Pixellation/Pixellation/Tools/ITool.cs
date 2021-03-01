﻿using Pixellation.Components.Editor;
using System.Drawing;
using System.Windows.Input;

namespace Pixellation.Tools
{
    /// <summary>
    /// Interface for all drawing tools used in Pixellation.
    /// </summary>
    public interface ITool
    {
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
        public void SetAllDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer, MirrorModeStates mirrorModeState = MirrorModeStates.OFF, ToolThickness thickness = ToolThickness.NORMAL);

        /// <summary>
        /// Cursor for the drawing tool.
        /// </summary>
        public Cursor ToolCursor { get; }

        /// <summary>
        /// Sets editor magnification.
        /// </summary>
        /// <param name="magnification"></param>
        public void SetMagnification(int magnification);

        /// <summary>
        /// Sets color for drawing.
        /// </summary>
        /// <param name="c">Color to set.</param>
        public void SetDrawColor(Color c);

        public void OnMouseMove(MouseEventArgs e);

        public void OnMouseDown(MouseButtonEventArgs e);

        public void OnMouseUp(MouseButtonEventArgs e);

        public void OnKeyDown(KeyEventArgs e);

        /// <summary>
        /// Resets drawing tool.
        /// </summary>
        public void Reset();

        /// <summary>
        /// Gets an instance with the specified instancekey.
        /// </summary>
        /// <param name="key">Instancekey.</param>
        /// <returns>Actual multiton instance.</returns>
        public ITool GetInstanceByKey(string key);

        /// <summary>
        /// Color used for drawing.
        /// </summary>
        public System.Windows.Media.Color ToolColor { get; set; }

        /// <summary>
        /// Mirror mode state for the drawing tool.
        /// </summary>
        public MirrorModeStates MirrorMode { get; set; }

        /// <summary>
        /// Thickness used with the drawing tool.
        /// </summary>
        public ToolThickness Thickness { get; set; }

        /// <summary>
        /// Indicates if tool is used as an eraser.
        /// </summary>
        public bool EraserModeOn { get; set; }
    }
}