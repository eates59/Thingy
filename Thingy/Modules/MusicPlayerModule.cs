﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Thingy.Infrastructure;
using Thingy.MusicPlayerCore;
using Thingy.MusicPlayerCore.Formats;

namespace Thingy.Modules
{
    public class MusicPlayerModule : ModuleBase
    {
        private IAudioEngine _audioEngine;

        public override string ModuleName
        {
            get { return "Music Player"; }
        }

        public override ImageSource Icon
        {
            get { return new BitmapImage(new Uri("pack://application:,,,/Thingy.Resources;component/Icons/icons8-boombox-96.png")); }
        }

        public override string Category
        {
            get { return ModuleCategories.Utilities; }
        }

        public override UserControl RunModule()
        {
            var view = new Views.MusicPlayer.MusicPlayer();
            view.DataContext = new ViewModels.MusicPlayer.MusicPlayerViewModel(view, App.Instance, _audioEngine);
            return view;
        }

        public override bool IsSingleInstance
        {
            get { return true; }
        }

        public override bool CanLoad
        {
            get
            {
                if (_audioEngine == null)
                    _audioEngine = App.IoCContainer.ResolveSingleton<IAudioEngine>();

                return _audioEngine.OutputDevices.Count > 0;
            }
        }

        public override IEnumerable<string> SupportedExtensions
        {
            get
            {
                IExtensionProvider provider = new ExtensionProvider();
                return provider.AllSupportedFormats;
            }
        }
    }
}
