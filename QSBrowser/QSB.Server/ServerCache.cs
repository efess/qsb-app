/*
Copyright 2009-2011 Joe Lukacovic

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
using System.Collections;
using QSB.GameServerInterface;
using QSB.Data.TableObject;

namespace QSB.Server
{
    /// <summary>
    /// Uses the Singleton pattern. Use the Cache property to access.
    /// </summary>
    public class ServerCache : IEnumerable
    {
        private static ServerCache _CacheInstance;
        private static object _synchLockObject = new object();

        private Hashtable _Cache;
        private ServerCache()
        {
            _Cache = new Hashtable();
        }

        public static ServerCache Cache
        {
            get
            {
                if (_CacheInstance == null)
                {
                    lock (_synchLockObject)
                    {
                        if (_CacheInstance == null)
                        {
                            _CacheInstance = new ServerCache();
                        }
                    }
                }

                return _CacheInstance;
            }
        }

        public void AddSnapshot(int pServerId, ServerActivity pSnapshot)
        {
            if (_Cache.ContainsKey(pServerId))
                _Cache[pServerId] = pSnapshot;
            else
                _Cache.Add(pServerId, pSnapshot);
        }
        public void Clear()
        {
            _Cache.Clear();
        }
        public ServerActivity this[int pServerId]
        {
            get
            {
                if (_Cache.ContainsKey(pServerId))
                    return _Cache[pServerId] as ServerActivity;
                else
                    return null;
            }
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (DictionaryEntry dictEntry in _Cache)
            {
                yield return dictEntry.Value as ServerActivity;
            }
        }

        #endregion
    }
}
