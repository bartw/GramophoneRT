using BeeWee.DiscogsRT.Models;
using Utils;

namespace Models
{
    public class MyRelease
    {
        public MyRelease()
        {

        }

        public MyRelease(Release release)
        {
            Id = release.id;
            Artist = TextHelpers.GetArtistsString(release.artists);
            Format = TextHelpers.GetFormatsString(release.formats);
            Label = TextHelpers.GetLabelsString(release.labels);
            ResourceUrl = release.resource_url;
            Thumb = release.thumb;
            Title = release.title;
            Year = release.year;
        }

        public int Id { get; set; }
        public string Artist { get; set; }
        public string Format { get; set; }
        public string Label { get; set; }
        public string ResourceUrl { get; set; }
        public string Thumb { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
    }
}
