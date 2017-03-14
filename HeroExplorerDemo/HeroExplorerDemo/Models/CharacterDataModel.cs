using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HeroExplorerDemo.Models
{
    [DataContract]
    public class Thumbnail
    {
        [DataMember] public string path { get; set; }
        [DataMember] public string extension { get; set; }
    }

    [DataContract]
    public class ComicSummary
    {
        [DataMember] public string resourceURI { get; set; }
        [DataMember] public string name { get; set; }
    }

    [DataContract]
    public class Comics
    {
        [DataMember] public int available { get; set; }
        [DataMember] public string collectionURI { get; set; }
        [DataMember] public List<ComicSummary> items { get; set; }
        [DataMember] public int returned { get; set; }
    }

    [DataContract]
    public class SeriesSummary
    {
        [DataMember] public string resourceURI { get; set; }
        [DataMember] public string name { get; set; }
    }

    [DataContract]
    public class Series
    {
        [DataMember] public int available { get; set; }
        [DataMember] public string collectionURI { get; set; }
        [DataMember] public List<SeriesSummary> items { get; set; }
        [DataMember] public int returned { get; set; }
    }

    [DataContract]
    public class StorySummary
    {
        [DataMember] public string resourceURI { get; set; }
        [DataMember] public string name { get; set; }
        [DataMember] public string type { get; set; }
    }

    [DataContract]
    public class Stories
    {
        [DataMember] public int available { get; set; }
        [DataMember] public string collectionURI { get; set; }
        [DataMember] public List<StorySummary> items { get; set; }
        [DataMember] public int returned { get; set; }
    }

    [DataContract]
    public class EventSummary
    {
        [DataMember] public string resourceURI { get; set; }
        [DataMember] public string name { get; set; }
    }

    [DataContract]
    public class Events
    {
        [DataMember] public int available { get; set; }
        [DataMember] public string collectionURI { get; set; }
        [DataMember] public List<EventSummary> items { get; set; }
        [DataMember] public int returned { get; set; }
    }

    [DataContract]
    public class Url
    {
        [DataMember] public string type { get; set; }
        [DataMember] public string url { get; set; }
    }

    [DataContract]
    public class Character
    {
        [DataMember] public int id { get; set; }
        [DataMember] public string name { get; set; }
        [DataMember] public string description { get; set; }
        [DataMember] public string modified { get; set; }
        [DataMember] public Thumbnail thumbnail { get; set; }
        [DataMember] public string resourceURI { get; set; }
        [DataMember] public Comics comics { get; set; }
        [DataMember] public Series series { get; set; }
        [DataMember] public Stories stories { get; set; }
        [DataMember] public Events events { get; set; }
        [DataMember] public List<Url> urls { get; set; }
    }

    [DataContract]
    public class CharacterDataContainer
    {
        [DataMember] public int offset { get; set; }
        [DataMember] public int limit { get; set; }
        [DataMember] public int total { get; set; }
        [DataMember] public int count { get; set; }
        [DataMember] public List<Character> results { get; set; }
    }

    [DataContract]
    public class CharacterDataModel
    {
        [DataMember] public int code { get; set; }
        [DataMember] public string status { get; set; }
        [DataMember] public string copyright { get; set; }
        [DataMember] public string attributionText { get; set; }
        [DataMember] public string attributionHTML { get; set; }
        [DataMember] public string etag { get; set; }
        [DataMember] public CharacterDataContainer data { get; set; }
    }
}
