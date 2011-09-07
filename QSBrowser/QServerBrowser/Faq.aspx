<%@ Page Language="C#" MasterPageFile="~/QSBMaster.master" AutoEventWireup="true" CodeFile="Faq.aspx.cs" Inherits="Faq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Main" Runat="Server">

<p>
Q: How do you identify players?
A: Purely by player name right now. I will be implmenting a way for a player to mark their IP address here so it'll identify you by your IP (Server must provide your IP) regardless of name.
</p>
<p>
Q: Where are stats like Efficiency, Kill/Death ratio, etc?
A: I can only report on what little information I can retrieve from the server. Right now this is limited to who's playing, their score, and their current color.
</p>
<p>
Q: How does it determine when a match has ended?
A: There's a few triggers I look out for: Players leaving, More then one player's score drastically dropping, or a Map change.
</p>
<p>
Q: Why isn't my match listed?
A: If the server went down for any period of time, your match information could have been lost.
</p>
<p>
Q: Why is my color white in a match? I was on team blue!
A: In games like clan arena, your color goes to white when you die. The color shown in the match is the average color that you had durring a match. If your color shows as white, then that means that the majority of the match, you were white.
</p>

</asp:Content>
