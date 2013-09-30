using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Entities: Contains the list of hashtags for the tweet
/// Implemented IEnumberable so I can use the foreach for this class
/// </summary>
[DataContract]
public class Entities : IEnumerable
{
    [DataMember(Name = "hashtags")]
    public List<Hashtag> Hashtags { get; set; }

    public Entities()
    {
        Hashtags = new List<Hashtag>();
    }

    public void Add(Hashtag item)
    {
        Hashtags.Add(item);
    }

    public IEnumerator<Hashtag> GetEnumerator()
    {
        return Hashtags.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}