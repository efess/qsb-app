<%@ Page Language="C#" MasterPageFile="~/QSBMaster.master" AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Main" Runat="Server">
<p class="blockParagraph">What is this? <b>ANOTHER</b> server browser? Aren't there already enough of these? Don't we already know all the servers?</p>
<p class="blockParagraph">Sure, you can browse servers here, this site will tell you which servers are online and which are currently active. 
What this site does differently, however, is record the information being gathered from the servers. There's a bunch of useful information that can
be reported when querying a server's historical data, and that's the purpose of this site. To record, and provide additional information
which may be useful to the average player. </p>
<p class="blockParagraph">This is an ASP.NET site which runs on Apache using the mod_mono plugin. The information harvesting server is written using .NET framework 3.5 and runs on top of Mono using MySql as a data warehouse. JQuery is used to help out with the client side scripting, and ajax is also heavily used in order to update your view in realtime. Instead of using qSTAT, I have created my own implentation of a server query utility--
partly for fun--mostly in order to support any kind of data which might be provided in the future just in case server programmers decide
to extend the query data set.</p>
<p class="blockParagraph">I've made the core of this site fully <b>Open Source</b> under the GPL. It can be downloaded <a href="QSBrowser.zip">here</a>(Edit- this is very much outdated. I'll post a SVN repository soon).</p>
<p class="blockParagraph">This site will only work with newer browsers. It has been tested with Google Chrome, Internet Explorer 7, and Firefox 2/3. Anything older, and you
will not be able to enjoy the interactive capabilities of this site.</p>
<p class="blockParagraph">If you wish to contact me, you can catch me on the <a href="http://www.quakeone.com">QuakeOne.com</a> forums, or email me: efess at quakeone.com</p>
</asp:Content>
