using Pixellation.Components.Editor;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Panels
{
    using Timer = System.Timers.Timer;

    /// <summary>
    /// Interaction logic for AnimationPlayer.xaml
    /// </summary>
    public partial class AnimationPlayer : UserControl
    {
        /// <summary>
        /// Timer for animation refresh.
        /// </summary>
        private Timer _dispatcherTimer = new Timer();

        /// <summary>
        /// Variable to handle frame drop.
        /// </summary>
        private Boolean _dispatcherTimerMonitorFlag = false;

        /// <summary>
        /// <see cref="AnimationPlayer"/> is loaded.
        /// </summary>
        private Boolean _loadedFlag = false;

        /// <summary>
        /// Canvas rectangle dimension.
        /// </summary>
        private Rect _canvasRect;

        /// <summary>
        /// Frame per second refresh(this parameter affect the "speed" of the rain).
        /// </summary>
        private int _framePerSecond = 30;

        /// <summary>
        /// Render current visualization for animation needs.
        /// </summary>
        private RenderTargetBitmap _renderTargetBitmap;

        /// <summary>
        /// Render current visualization for animation needs.
        /// </summary>
        private WriteableBitmap _wBitmap;

        /// <summary>
        /// Brush of the control background.
        /// </summary>
        private Brush _bgClearBrush = new SolidColorBrush(Colors.White);

        /// <summary>
        /// Frames to render.
        /// </summary>
        private List<DrawingFrame> _frames = new List<DrawingFrame>();

        /// <summary>
        /// If true, after all frames were played once the animation will stop.
        /// </summary>
        private bool _playOnceFlag = false;

        /// <summary>
        /// Index of current frame (private).
        /// </summary>
        private int _activeFrameIndex = 0;

        /// <summary>
        /// Index of current frame.
        /// </summary>
        private int ActiveFrameIndex
        {
            get => _activeFrameIndex;
            set
            {
                _activeFrameIndex = value % _frames.Count;
                if (_playOnceFlag && _activeFrameIndex == 0)
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Default constructor for <see cref="AnimationPlayer"/>.
        /// </summary>
        public AnimationPlayer()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            _dispatcherTimer.AutoReset = true;
            _dispatcherTimer.Elapsed += DispatcherTimerTick;

            // When the component is loaded  we need to make some calculation.
            Loaded += AnimationPlayerLoaded;
        }

        /// <summary>
        /// Method that occurs when the element is laid out, rendered, and ready for interaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimationPlayerLoaded(object sender, RoutedEventArgs e)
        {
            if (!_loadedFlag)
            {
                _loadedFlag = true;
                InitializeAnimationPlayer();
            }
        }

        /// <summary>
        /// Resized.
        /// </summary>
        public void DimensionChanged()
        {
            InitializeAnimationPlayer();
        }

        /// <summary>
        /// Inits variables.
        /// </summary>
        private void InitializeAnimationPlayer()
        {
            _canvasRect = new Rect(0, 0, myCanvas.ActualWidth, myCanvas.ActualHeight);

            _renderTargetBitmap = new RenderTargetBitmap((int)_canvasRect.Width, (int)_canvasRect.Height, 96, 96, PixelFormats.Pbgra32);
            
            _wBitmap = new WriteableBitmap(_renderTargetBitmap);
            myImage.Source = _wBitmap;

            myCanvas.Measure(new Size(_renderTargetBitmap.Width, _renderTargetBitmap.Height));
            myCanvas.Arrange(new Rect(new Size(_renderTargetBitmap.Width, _renderTargetBitmap.Height)));

            SetParameters(framePerSecond: _framePerSecond);
        }

        /// <summary>
        /// Method that occurs when the timer interval has elapsed. This method refresh the control to perform the animation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                // Synchronize on main thread.
                System.Timers.ElapsedEventHandler dt = DispatcherTimerTick;
                if (!_dispatcherTimerMonitorFlag)
                {
                    Dispatcher.Invoke(dt, sender, e);
                }
                return;
            }

            _dispatcherTimerMonitorFlag = true;

            DrawingVisual dv = RenderNextFrame();
            _renderTargetBitmap.Render(dv);

            // Using "_myImageBackground.Source = _renderTargetBitmap" would consume a lot of ram, so I use a _writeableBitmap to avoid ram increase at each refresh.

            _wBitmap.Lock();

            _renderTargetBitmap.CopyPixels(
                new Int32Rect(0, 0, _renderTargetBitmap.PixelWidth, _renderTargetBitmap.PixelHeight),
                _wBitmap.BackBuffer,
                _wBitmap.BackBufferStride * _wBitmap.PixelHeight,
                _wBitmap.BackBufferStride
            );

            _wBitmap.AddDirtyRect(
                new Int32Rect(0, 0, _renderTargetBitmap.PixelWidth, _renderTargetBitmap.PixelHeight)
            );

            _wBitmap.Unlock();

            _dispatcherTimerMonitorFlag = false;
        }

        /// <summary>
        /// Renders the next frame onto a <see cref="DrawingVisual"/>.
        /// </summary>
        /// <returns><see cref="DrawingVisual"/> with the rendered graphics.</returns>
        private DrawingVisual RenderNextFrame()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawRectangle(
                _bgClearBrush,
                null,
                _canvasRect
            );

            drawingContext.DrawImage(
                _frames[ActiveFrameIndex++].Bitmap,
                _canvasRect
            );

            drawingContext.Close();

            return drawingVisual;
        }

        /// <summary>
        /// Updates the given parameters.
        /// </summary>
        /// <param name="framePerSecond">Speed of the animation.</param>
        /// <param name="backgroundClearBrush">Used for background. Default is a white <see cref="SolidColorBrush"/>.</param>
        /// <param name="framesToPlay">Frames to play. Default is an empty list.</param>
        /// <param name="playOnce">Play every frame only once or loop animation until <see cref="Stop"/> is called.</param>
        public void SetParameters(int framePerSecond = 30, Brush backgroundClearBrush = null, List<DrawingFrame> framesToPlay = null, bool playOnce = false)
        {
            bool dispRunning = false;
            if (_dispatcherTimer.Enabled)
            {
                _dispatcherTimer.Stop();
                dispRunning = true;
            }

            if (framesToPlay != null)
            {
                _frames = framesToPlay;
            }

            _playOnceFlag = playOnce;

            if (backgroundClearBrush != null)
            {
                _bgClearBrush = backgroundClearBrush;
            }
            myCanvas.Background = _bgClearBrush.Clone();
            _bgClearBrush.Freeze();

            if (framePerSecond > 0)
            {
                _framePerSecond = framePerSecond;
            }
            else
            {
                if (_framePerSecond == 0)
                {
                    _framePerSecond = 30;
                }
            }

            _dispatcherTimer.Interval = 1000D / _framePerSecond;

            if (dispRunning)
            {
                _dispatcherTimer.Start();
            }
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        public void Start()
        {
            _dispatcherTimer.Start();
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop()
        {
            _dispatcherTimer.Stop();
        }
    }
}