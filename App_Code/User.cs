using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// User: User information on the tweet
/// </summary>
[DataContract]
public class User
{
    [DataMember(Name = "screen_name")]
    public string UserName { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }
}