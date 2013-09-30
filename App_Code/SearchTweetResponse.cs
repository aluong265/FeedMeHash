using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

/// <summary>
/// SearchTweetResponse: Object for the whole JSON response from Twitter
/// Only need the list of Tweets
/// Implements IEnumberable so I can use the foreach for this class and pass the object into the repeater
/// </summary>
[DataContract]
public class SearchTweetResponse : IEnumerable
{
    [DataMember(Name = "statuses")]
    public List<Tweet> Tweets { get; set; }

    public SearchTweetResponse()
    {
        Tweets = new List<Tweet>();
    }

    public void Add(Tweet item)
    {
        Tweets.Add(item);
    }

    public IEnumerator<Tweet> GetEnumerator()
    {
        return Tweets.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}