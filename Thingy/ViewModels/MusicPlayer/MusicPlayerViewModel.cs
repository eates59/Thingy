﻿using AppLib.MVVM;
using System;
using System.Windows;
using Thingy.MusicPlayerCore;
using Thingy.MusicPlayerCore.Formats;
using Thingy.Views.Interfaces;

namespace Thingy.ViewModels.MusicPlayer
{
    public class MusicPlayerViewModel: ViewModel<IMusicPlayer>, IDisposable
    {
        private IAudioEngine _audioEngine;
        private PlayListViewModel _playlist;
        private IExtensionProvider _extensions;


        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand SeekFwdCommand { get; private set; }
        public DelegateCommand SeekBackCommand { get; private set; }

        public DelegateCommand NextTrackCommand { get; private set; }
        public DelegateCommand PrevousTrackCommand { get; private set; }

        public DelegateCommand DragStartedCommand { get; private set; }
        public DelegateCommand<double> DragCompletedCommand { get; private set; }

        public IAudioEngine AudioEngine
        {
            get { return _audioEngine; }
            set
            {
                bool subscribe = false;
                if (value == null &&_audioEngine != null)
                {
                    _audioEngine.SongFinishedEvent -= songFinished;
                    _audioEngine.Dispose();
                }
                else if (value != null)
                {
                    subscribe = true;
                }
                SetValue(ref _audioEngine, value);
                if (subscribe)
                {
                    _audioEngine.SongFinishedEvent += songFinished;
                }
            }
        }

        private void songFinished(object sender, RoutedEventArgs e)
        {
            if (Playlist.IsPossibleToChangeTrack(1))
            {
                Playlist.CurrentIndex = Playlist.CurrentIndex + 1;
                _audioEngine.Load(Playlist.CurrrentFile);
                _audioEngine.Play();
            }
            else
            {
                _audioEngine.Stop();
            }
        }

        public PlayListViewModel Playlist
        {
            get { return _playlist; }
            set { SetValue(ref _playlist, value); }
        }

        public MusicPlayerViewModel(IMusicPlayer view, IApplication app, IAudioEngine engine) : base(view)
        {
            AudioEngine = engine;
            Playlist = new PlayListViewModel(app);
            _extensions = new ExtensionProvider();
            OpenFileCommand = Command.ToCommand(OpenFile);
            PlayCommand = Command.ToCommand(Play);
            PauseCommand = Command.ToCommand(Pause);
            SeekBackCommand = Command.ToCommand(SeekBack);
            SeekFwdCommand = Command.ToCommand(SeekFwd);
            DragStartedCommand = Command.ToCommand(DragStarted);
            DragCompletedCommand = Command.ToCommand<double>(DragCompleted);
            PrevousTrackCommand = Command.ToCommand(PrevousTrack);
            NextTrackCommand = Command.ToCommand(NextTrack);
        }

        private void DragCompleted(double obj)
        {
            AudioEngine.Position = obj;
            AudioEngine.Seeking = false;
        }

        private void DragStarted()
        {
            AudioEngine.Seeking = true;
        }


        private void SeekBack()
        {
            _audioEngine.Seeking = true;
            double position = _audioEngine.Position;
            _audioEngine.Position = position - 5;
            _audioEngine.Seeking = false;
        }

        private void SeekFwd()
        {
            _audioEngine.Seeking = true;
            double position = _audioEngine.Position;
            _audioEngine.Position = position + 5;
            _audioEngine.Seeking = false;
        }

        private void PrevousTrack()
        {
            if (Playlist.IsPossibleToChangeTrack(-1))
            {
                Playlist.CurrentIndex = Playlist.CurrentIndex - 1;
                _audioEngine.Load(Playlist.CurrrentFile);
                _audioEngine.Play();
            }
        }

        private void NextTrack()
        {
            if (Playlist.IsPossibleToChangeTrack(1))
            {
                Playlist.CurrentIndex = Playlist.CurrentIndex + 1;
                _audioEngine.Load(Playlist.CurrrentFile);
                _audioEngine.Play();
            }
        }

        private async void OpenFile()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = _extensions.AllFormatsAndPlaylistsFilterString;
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (ofd.FileNames.Length > 1)
                {
                    foreach (var file in ofd.FileNames)
                    {
                        var format = _extensions.GetFormatKind(file);
                        if (format == FormatKind.Playlist)
                            await Playlist.DoOpenList(file, true);
                        else if (format == FormatKind.Stream)
                            Playlist.Playlist.Add(file);
                    }
                    View.SwithToTab(MusicPlayerTabs.Playlist);
                }
                else
                {
                    var format = _extensions.GetFormatKind(ofd.FileName);
                    if (format == FormatKind.Playlist)
                    {
                        await Playlist.DoOpenList(ofd.FileName, true);
                        View.SwithToTab(MusicPlayerTabs.Playlist);
                    }
                    else if (format == FormatKind.Stream)
                    {
                        Playlist.Playlist.Add(ofd.FileName);
                        if (Playlist.Playlist.Count == 1)
                        {
                            _audioEngine.Load(Playlist.Playlist[0]);
                            _audioEngine.Play();
                            View.SwithToTab(MusicPlayerTabs.Player);
                        }
                    }
                }
            }
        }

        private void Pause()
        {
            _audioEngine.Pause();
        }

        private void Play()
        {
            _audioEngine.Play();
        }

        public void Dispose()
        {
            if (AudioEngine != null)
            {
                AudioEngine.Dispose();
                AudioEngine = null;
            }
        }
    }
}
