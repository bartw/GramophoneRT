using BeeWee.DiscogsRT.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public static class TextHelpers
    {
        public static string GetArtistsString(List<Artist> artists)
        {
            if (artists != null && artists.Count > 0)
            {
                var artistNames = (from artist in artists
                                   where !string.IsNullOrWhiteSpace(artist.name)
                                   select artist.name).ToArray();

                return string.Join(", ", artistNames);
            }

            return "";
        }

        public static string GetFormatsString(List<Format> formats)
        {
            if (formats != null && formats.Count > 0)
            {
                var formatStrings = new List<string>();

                foreach (var format in formats)
                {
                    var formatString = GetFormatString(format);

                    if (!string.IsNullOrWhiteSpace(formatString))
                    {
                        formatStrings.Add(formatString);
                    }
                }

                return string.Join("; ", formatStrings.ToArray());
            }

            return "";
        }

        public static string GetLabelsString(List<Label> labels)
        {
            if (labels != null && labels.Count > 0)
            {
                var labelStrings = new List<string>();

                foreach (var label in labels)
                {
                    var labelString = GetLabelString(label);

                    if (!string.IsNullOrWhiteSpace(labelString))
                    {
                        labelStrings.Add(labelString);
                    }
                }

                return string.Join("; ", labelStrings.ToArray());
            }

            return "";
        }

        private static string GetFormatString(Format format)
        {
            if (format != null)
            {
                StringBuilder builder = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(format.qty))
                {
                    builder.AppendFormat("{0} × ", format.qty);
                }

                builder.Append(format.name);

                if (format.descriptions != null && format.descriptions.Count > 0)
                {
                    foreach (var description in format.descriptions)
                    {
                        builder.AppendFormat(", {0}", description);
                    }
                }

                return builder.ToString();
            }
            else
            {
                return "";
            }
        }

        private static string GetLabelString(Label label)
        {
            if (label != null)
            {
                StringBuilder builder = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(label.name))
                {
                    builder.AppendFormat("{0}: ", label.name);
                }

                if (!string.IsNullOrWhiteSpace(label.name))
                {
                    builder.Append(label.catno);
                }

                return builder.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
