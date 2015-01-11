using BeeWee.DiscogsRT.Models;
using System.Collections.Generic;
using Utils;

namespace Models
{
    public class MyCollectionItem
    {
        public MyCollectionItem()
        {

        }

        public MyCollectionItem(CollectionItem collectionItem)
        {
            Id = collectionItem.id;
            InstanceIds = new List<int>();
            InstanceIds.Add(collectionItem.instance_id);
            Rating = collectionItem.rating;
            Artist = TextHelpers.GetArtistsString(collectionItem.basic_information.artists);
            Format = TextHelpers.GetFormatsString(collectionItem.basic_information.formats);
            Label = TextHelpers.GetLabelsString(collectionItem.basic_information.labels);
            ResourceUrl = collectionItem.basic_information.resource_url;
            Thumb = collectionItem.basic_information.thumb;
            Title = collectionItem.basic_information.title;
            Year = collectionItem.basic_information.year;
        }

        public int Id { get; set; }
        public List<int> InstanceIds { get; set; }
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
