using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class _Default : System.Web.UI.Page 
{
    // Refer to LeathalChicken's post which contains working code on returning results from the Twitter API
    // Look at the code where she said she managed to figure out how it worked
    // She also mentioned how to pass in extra query params to the Twitter API to narrow down the results
    // https://dev.twitter.com/discussions/15206

    // Refer to my documentation on more details about this project

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FindTweets_Click(Object sender, EventArgs e)
    {
        // %23 is the url encoding for @, which is required for the query search
        var query = "%23" + tbxHashtag.Text;

        // Url for searching tweets only
        var url = "https://api.twitter.com/1.1/search/tweets.json";
        
        // Find Tweets, if not tweets are returned, then something has happened with the request or code
        if (FindTweets(url, query))
        {
            // Change the style of feed hash container on postback
            String cssClass = "hash-feed-container-results";
            if (!HasClass(cssClass, pnlHashFeed))
            {
                AddClass(cssClass, pnlHashFeed);
                pnlFilterSort.Visible = true;
            }
        }
        else
        {
            // Display a generic error message
            litWebReqError.Text = "An error has occurred. Please try again later.";
            phHashFeed.Visible = false;
            phError.Visible = true;
        }
    }

    protected Boolean FindTweets(String resource_url, String q)
    {
        // oauth application keys
        var oauth_token = "1909688060-o5PeClchGXZZhrlq9NvHsMCiCnhzEyX2vDVPbKG";
        var oauth_token_secret = "nb8cY3kx2JMCgN85MRfSO3OgT9ZgJNfuPYEhFPdo3s";
        var oauth_consumer_key = "cSi9xDJd0SXDx0G2NzUXA";
        var oauth_consumer_secret = "EhwRvxIDkWcxuCbdlviUoH5uk0snemb4Qo7CANx4olg";

        // oauth implementation details
        var oauth_version = "1.0";
        var oauth_signature_method = "HMAC-SHA1";

        // unique request details
        var oauth_nonce = Convert.ToBase64String(
            new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
        var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

        // Please check with the twitter AuthTool to get format of the oauth signature
        // It needs to be in that exact order if you were to add extra parameters
        // create oauth signature
        var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                        "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&q={6}";

        var baseString = string.Format(baseFormat,
                                    oauth_consumer_key,
                                    oauth_nonce,
                                    oauth_signature_method,
                                    oauth_timestamp,
                                    oauth_token,
                                    oauth_version,
                                    Uri.EscapeDataString(q)
                                    );

        baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

        var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                                "&", Uri.EscapeDataString(oauth_token_secret));

        String oauth_signature;
        using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
        {
            oauth_signature = Convert.ToBase64String(
                hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
        }

        // create the request header
        var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                           "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                           "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                           "oauth_version=\"{6}\"";

        var authHeader = string.Format(headerFormat,
                                Uri.EscapeDataString(oauth_nonce),
                                Uri.EscapeDataString(oauth_signature_method),
                                Uri.EscapeDataString(oauth_timestamp),
                                Uri.EscapeDataString(oauth_consumer_key),
                                Uri.EscapeDataString(oauth_token),
                                Uri.EscapeDataString(oauth_signature),
                                Uri.EscapeDataString(oauth_version)
                        );

        ServicePointManager.Expect100Continue = false;

        // make the request
        var queryParams = "q=" + Uri.EscapeDataString(q);
        resource_url += "?" + queryParams;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
        request.Headers.Add("Authorization", authHeader);
        request.Method = "GET";
        request.ContentType = "application/x-www-form-urlencoded";

        try
        {
            using (var response = request.GetResponse())
            {
                var reader = new StreamReader(response.GetResponseStream());
                var objText = reader.ReadToEnd();

                // Deserialize JSON into C# Object
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(SearchTweetResponse));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(objText));
                SearchTweetResponse tweetList = (SearchTweetResponse)ser.ReadObject(ms);

                // Filter out a hashtag from the results
                // It only filters out 1 hashtag instead of multiple hashtags
                String filter = tbxFilter.Text;
                if (!String.IsNullOrEmpty(filter))
                {
                    List<Tweet> filteredTweetList = new List<Tweet>();
                    foreach (Tweet tweet in tweetList)
                    {
                        if (!HashtagExists(tweet.Entities.Hashtags, filter))
                        {
                            filteredTweetList.Add(tweet);
                        }
                    }
                    tweetList.Tweets = filteredTweetList;
                }

                // After the filters has been applied, populate the results
                tweetRepeater.DataSource = tweetList;
                tweetRepeater.DataBind();

                // Find Tweets has been a success
                return true;
            }
        }
        catch (WebException ex)
        {
            Console.WriteLine(ex.Status);
            if (ex.Response != null)
            {
                // can use ex.Response.Status, .StatusDescription
                if (ex.Response.ContentLength != 0)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            Console.WriteLine(reader.ReadToEnd());
                        }
                    }
                }
            }
        }
        return false;
    }

    // Populate Tweet
    protected void tweetRepeater_ItemDataBound(Object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Tweet tweet = (Tweet)e.Item.DataItem;
            ((Literal)e.Item.FindControl("litName")).Text = tweet.User.Name;
            ((Literal)e.Item.FindControl("litScreenName")).Text = tweet.User.UserName;
            ((Literal)e.Item.FindControl("litPost")).Text = tweet.Post;
            
            // Format hashtags
            List<String> hashtagList = new List<String>();
            foreach (Hashtag hashtag in tweet.Entities.Hashtags)
            {
                hashtagList.Add("#" + hashtag.TagName);
            }
            ((Literal)e.Item.FindControl("litHashtags")).Text = String.Join(" ", hashtagList.ToArray());
            
            // Twitter has a certain datetime format
            // Decided to convert and display it in another format
            DateTime createdDate = ParseTwitterTime(tweet.CreatedDate);
            ((Literal)e.Item.FindControl("litCreatedDate")).Text = createdDate.ToShortDateString();
        }
    }

    // Add CssClass to WebControl
    protected void AddClass(String cssClass, WebControl control)
    {
        List<String> cssClassList = new List<string>(control.CssClass.Split(' '));
        if (!cssClassList.Contains(cssClass))
        {
            cssClassList.Add(cssClass);
            control.CssClass = String.Join(" ", cssClassList.ToArray());
        }
    }

    // Check if CssClass exists in WebControl
    protected Boolean HasClass(String cssClass, WebControl control)
    {
        List<String> cssClassList = new List<string>(control.CssClass.Split(' '));
        return cssClassList.Contains(cssClass);
    }

    // Remove CssClass from WebControl
    protected void RemoveClass(String cssClass, WebControl control)
    {
        List<String> cssClassList = new List<string>(control.CssClass.Split(' '));
        cssClassList.Remove(cssClass);
        control.CssClass = String.Join(" ", cssClassList.ToArray());
    }

    // Convert Twitter DateTime String into C# DateTime Object
    protected DateTime ParseTwitterTime(String date)
    {
        const string format = "ddd MMM dd HH:mm:ss zzzz yyyy";
        return  DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
    }

    // Check if Hashtag exist Tweet
    protected Boolean HashtagExists(List<Hashtag> hashtags, String filter)
    {
        List<String> hashtagsList = new List<String>();
        foreach (Hashtag hashtag in hashtags)
        {
            hashtagsList.Add(hashtag.TagName);
        }
        return hashtagsList.Contains(filter);
    }
}