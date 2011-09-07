/*
Copyright 2009-2010 Joe Lukacovic

This file is part of QSBrowser.

QSBrowser is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

QSBrowser is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with QSBrowser.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSB.GameServerInterface
{
    /// <summary>
    /// Exception thrown when there's an error parsing the response from a server
    /// </summary>
    public class ServerQueryParseException : Exception
    {
        public ServerQueryParseException(Exception pInnerException) : base(pInnerException.Message, pInnerException) { }
    }
    /// <summary>
    /// Exception thrown when a serverinfo request has timed out
    /// </summary>
    public class ServerNotRespondingException : Exception
    {
        public ServerNotRespondingException(Exception pInnerException) : base(pInnerException.Message, pInnerException) { }
    }
    /// <summary>
    /// Exception thrown when a route to a server could nto be found
    /// </summary>
    public class ServerNotFoundException : Exception
    {
        public ServerNotFoundException(Exception pInnerException) : base(pInnerException.Message, pInnerException) { }
    }
}
