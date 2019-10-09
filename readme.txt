Version: ?? 
Contact: Efess on QuakeOne.com or email efessel@gmail.com

(Certain information might be obsolete since this was first created in 2009)

QSBrowser contains all the sourcecode needed to query a set of servers for status and player information, store that information, and retreive it for reporting to either the Web or Quake clients. The code contained here is the same that is used at servers.quakeone.com.

The solution and projects were created with Visual Studio 2010

This package contains:

readme.txt - this file
CreateDeployBinaries.bat - Compiles website, and creates a directory with pages and binaries
gpl.txt - The GPL License
QSBrowser.sln - The solution file for this project

Libraries - Directory which contains third party Dll's (NHibernate, FluentNHibernate, SQLite) referenced by the QSData module

Contained projects:

================
----QSB.App----
================

QSB.App - Console application which performs harvesting and writes to the database

================
----QSB.Common---- 
================

QSB.Common - Contains common methods and enums common to all projects

==============
----QSB.Data---- 
==============

QSB.Data - Data Layer which exposes common CRUD operations. NHibernate (https://www.hibernate.org/) is used to access the database, and FluentNHibernate (http://fluentnhibernate.org/) is used to create the mappings between the .net Objects and Table entities in code. This project is simple, the one class DataStore contains all common methods for all data operations.

==================
----QSB.Server---- 
==================

QSB.Server - Contains all logic which keeps Server and Player information up to date in cache. The one class ServerManager is where all the logic is performed to perform the queries and update the internal cache.

===============================
----QSB.GameServerInterface---- 
===============================

QSB.GameServerInterface - Provides all game specific server query functions and funnels this information into common objects.

======================
----QServerBrowser---- 
======================


Example web pages which makes use of the various functions. There are provided classes which are intialized as static and are used application wide for all users to access. The only piece missing is a css file, you'll have to provide your own for your own specific formatting.
 
*** IMPORTANT *** You must use a generated MachineKey. Here is the generator I used: http://www.aspnetresources.com/tools/keycreator.asp in order for thew ViewState to persist between Application pool restarts. One has been created in the provided web.config, but creating your own should be your first order of business.

In order to Query/Update the database manually, use a Sqlite management program. I recommend SqlIte Administrator found here: http://sqliteadmin.orbmu2k.de/ 

Status.cs class is a is a Debug utility which outputs debug strings or exceptions to a Text file. 

ImageManager.cs manages player name images as they appear in game.  

QueryThread.cs manages a thread which executes the server queries at a defined interval. 

Default.aspx page shows a summery of all servers that have recently been queryied, and shows players in game if any. This page uses the MS UpdatePanel control, which uses AJAX to update the client at a defined interval.

Login.aspx provides a simple name/password prompt which allows the user to optionally authenticate to access protected pages.

ServerList.aspx is an admin protected page that lists all servers in the database, and allows Create/Update/Delete of these servers.

UpdateServer.aspx is a popup page which contains a form which allows the user to manipulate the properties of a server.

NQServers.apsx is a page accessed by Quake Clients that provides client specific output streams of server information. This page should be used if Quake Clients are expected to connect and download server information.

Monitor.aspx is a page used to monitor the status of the server browser. Ouputs the status buffer as well in order to diagnose any issues.



