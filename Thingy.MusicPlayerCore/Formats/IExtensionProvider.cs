﻿using System.Collections.Generic;

namespace Thingy.MusicPlayerCore.Formats
{
    public enum FormatKind
    {
        Stream,
        Tracker,
        Midi,
        Playlist
    }

    public interface IExtensionProvider
    {
        string PlalistsFilterString { get; }
        string AllFormatsFilterString { get; }
        IEnumerable<string> AllSupportedFormats { get; }
        bool IsMatchForFormat(FormatKind formatKind, string file);
    }
}
