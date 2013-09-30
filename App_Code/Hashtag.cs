using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Hashtag: hashtag information from the tweet
/// </summary>
[DataContract]
public class Hashtag
{
    [DataMember(Name = "text")]
    public string TagName { get; set; }
}