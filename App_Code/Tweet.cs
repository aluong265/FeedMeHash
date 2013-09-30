using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Tweet: Grab Information on Tweet
/// Below are the information needed
/// Can add more properties if needed
/// </summary>
[DataContract]
public class Tweet
{
    [DataMember(Name = "text")]
    public string Post { get; set; }

    [DataMember(Name = "entities")]
    public Entities Entities { get; set; }

    [DataMember(Name = "user")]
    public User User { get; set; }

    [DataMember(Name = "created_at")]
    public String CreatedDate { get; set; }
}