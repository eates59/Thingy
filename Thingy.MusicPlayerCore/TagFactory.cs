﻿using AppLib.Common.Extensions;
using System;
using System.Windows.Media.Imaging;
using Thingy.MusicPlayerCore.DataObjects;
using Thingy.Resources;

namespace Thingy.MusicPlayerCore
{
    public static class TagFactory
    {
        public static TagInformation CreateTagInfoFromFile(string fileName)
        {
            try
            {
                TagLib.File tags = TagLib.File.Create(fileName);
                var year = tags.Tag.Year.ToString();
                var album = tags.Tag.Album;
                var title = tags.Tag.Title;
                var artist = "";
                if (tags.Tag.Performers != null && tags.Tag.Performers.Length != 0)
                {
                    artist = tags.Tag.Performers[0];
                }
                return new TagInformation
                {
                    Artist = artist,
                    Album = album,
                    Cover = GetCover(tags),
                    Title = title,
                    Year = year,
                    FileName = System.IO.Path.GetFileName(fileName)
                };

            }
            catch (Exception)
            {
                var unknown = TagInformation.Unknown;
                unknown.FileName = System.IO.Path.GetFileName(fileName);
                return unknown;
            }
        }

        public static TagInformation CreateTagInfoForNetStream(string filename, string title, string artist = null)
        {
            return new TagInformation
            {
                FileName = filename,
                Artist = artist,
                Title = title,
                Year = DateTime.Now.Year.ToString(),
                Album = "Internet stream",
            };
        }

        public static TagInformation CreateTagInfoForCD(int drive, int track)
        {
            var title = CollectionExtensions.GetValueOrFallback(CDInfoProvider.CdData, $"TITLE{track + 1}", "Unknown song");
            var artist = CollectionExtensions.GetValueOrFallback(CDInfoProvider.CdData, $"PERFORMER{track + 1}", "Unknown artist");
            var album = CollectionExtensions.GetValueOrFallback(CDInfoProvider.CdData, $"PERFORMER{0}", "Unknown");
            album += " - " + CollectionExtensions.GetValueOrFallback(CDInfoProvider.CdData, $"TITLE{0}", "");
            return new TagInformation
            {
                FileName = $"CD track {track + 1}, on Drive: {drive}",
                Title = title,
                Artist = artist,
                Year = "unknown",
                Album = album
            };
        }

        private static BitmapImage GetCover(TagLib.File tags)
        {
            if (tags.Tag.Pictures.Length > 0)
            {
                var picture = tags.Tag.Pictures[0].Data;
                using (var ms = new System.IO.MemoryStream(picture.Data))
                {
                    BitmapImage ret = new BitmapImage();
                    ret.BeginInit();
                    ret.StreamSource = ms;
                    ret.DecodePixelWidth = 300;
                    ret.CacheOption = BitmapCacheOption.OnLoad;
                    ret.EndInit();
                    return ret;
                }
            }
            return new BitmapImage(ResourceLocator.GetIcon(IconCategories.Big, "icons8-audio-wave-540.png"));
        }
    }
}