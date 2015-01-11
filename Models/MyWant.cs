using BeeWee.DiscogsRT.Models;
using Utils;

namespace Models
{
    public class MyWant
    {
        public MyWant()
        {
        }

        public MyWant(Want want)
        {
            Id = want.id;
            Notes = want.notes;
            Rating = want.rating;
            Artist = TextHelpers.GetArtistsString(want.basic_information.artists);
            Format = TextHelpers.GetFormatsString(want.basic_information.formats);
            Label = TextHelpers.GetLabelsString(want.basic_information.labels);
            ResourceUrl = want.basic_information.resource_url;
            Thumb = want.basic_information.thumb;
            Title = want.basic_information.title;
            Year = want.basic_information.year;
        }

        public int Id { get; set; }
        public string Notes { get; set; }
        public int Rating { get; set; }
        public string Artist { get; set; }
        public string Format { get; set; }
        public string Label { get; set; }
        public string ResourceUrl { get; set; }
        public string Thumb { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
    }
}
