﻿using Pixellation.Components.Editor;
using Pixellation.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for PreviewPanel.xaml
    /// </summary>
    public partial class PreviewPanel : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Event used for one- and twoway databinding.
        /// Marks change regarding one of the properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private static event EventHandler<DependencyPropertyChangedEventArgs> IFrameProviderUpdated;

        public IFrameProvider FrameProvider
        {
            get => (IFrameProvider)GetValue(FrameProviderProperty);
            set {
                SetValue(FrameProviderProperty, value);
                OnPropertyChanged();
            }
        }
        public static readonly DependencyProperty FrameProviderProperty =
         DependencyProperty.Register("FrameProvider", typeof(IFrameProvider), typeof(PreviewPanel), new FrameworkPropertyMetadata(
             default,
             (s, e) => { IFrameProviderUpdated?.Invoke(s, e); }
        ));

        private PreviewMode _pMode;
        public PreviewMode PMode
        {
            get => _pMode;
            set {
                _pMode = value;
                OnPropertyChanged();
            }
        }

        public PreviewPanel()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            IFrameProviderUpdated += (s, e) =>
            {
                OnPropertyChanged(nameof(FrameProvider));
            };

            //PixelEditor.FrameListChanged += (s, a) => { InvalidateVisual(); };
            //PixelEditor.LayerListChanged += (s, a) => { InvalidateVisual(); };

            //PixelEditor.RaiseImageUpdatedEvent += (s, a) => { InvalidateVisual(); };

            //DrawingLayer.OnUpdated += () => { InvalidateVisual(); };
            //DrawingFrame.OnUpdated += () => { InvalidateVisual(); };
        }

        /// <summary>
        /// Used as change notification for one- and twoway binding with <see cref="DependencyProperty"/> objects.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void RbAllClick(object sender, RoutedEventArgs e)
        {
            PMode = PreviewMode.FRAMES;
        }

        private void RbFrameClick(object sender, RoutedEventArgs e)
        {
            PMode = PreviewMode.FRAME;
        }

        private void RbLayerClick(object sender, RoutedEventArgs e)
        {
            PMode = PreviewMode.LAYER;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            dPreview.OnionModeEnabled = !dPreview.OnionModeEnabled;
        }
    }
}