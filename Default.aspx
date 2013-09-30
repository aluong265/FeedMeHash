<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Feed Me Hash</title>
    <link rel="Stylesheet" href="css/main.css" />
    <script type="text/javascript" src="js/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="js/jquery.watermark.min.js"></script>
    <script type="text/javascript" src="js/script.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="wrapper">
        <!-- Feed Hash, Sort, and Filters -->
        <asp:Panel ID="pnlHashFeed" CssClass="hash-feed-container" runat="server">
            <asp:PlaceHolder ID="phHashFeed" runat="server">
                <h1>Feed Me Hash</h1>
                <asp:TextBox ID="tbxHashtag" runat="server"></asp:TextBox>
                <asp:Button ID="btnFindTweets" Text="Find Tweets" OnClick="FindTweets_Click" runat="server" />
                <div class="clear"></div>

                <asp:Panel ID="pnlFilterSort" CssClass="filter-sort-container" Visible="false" runat="server">
                    <asp:TextBox ID="tbxFilter" runat="server"></asp:TextBox>
                    <div class="sort-container">
                        <label>Sort by:</label>
                        <asp:DropDownList ID="ddlSort" runat="server">
                            <asp:ListItem Value="aDate" Text="Date (Ascending)"></asp:ListItem>
                            <asp:ListItem Value="dDate" Text="Date (Descending)"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="clear"></div>
                </asp:Panel>
            </asp:PlaceHolder>

            <!-- Web Request Error Message Container -->
            <asp:PlaceHolder ID="phError" Visible="false" runat="server">
                <div class="error">
                    <asp:Literal ID="litWebReqError" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>
        </asp:Panel>

        <!-- Tweets Results -->
        <div class="tweet-results-container">
            <asp:Repeater ID="tweetRepeater" OnItemDataBound="tweetRepeater_ItemDataBound" runat="server">
                <ItemTemplate>
                    <div class="tweet-container">
                        <span class="title"><asp:Literal ID="litName" runat="server"></asp:Literal> (@<asp:Literal ID="litScreenName" runat="server"></asp:Literal>)</span>
                        <span class="date"><asp:Literal ID="litCreatedDate" runat="server"></asp:Literal></span>
                        <div class="tweet-content-container">
                            <span class="posts"><asp:Literal ID="litPost" runat="server"></asp:Literal></span>
                            <span class="hash-tags"><asp:Literal ID="litHashtags" runat="server"></asp:Literal><br /></span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    </form>
</body>
</html>