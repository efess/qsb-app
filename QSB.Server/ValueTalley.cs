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

namespace QSB.Server
{
    /// <summary>
    /// This class is used to keep track of the number of times a value is used
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueTally<T>
    {
        private const int MAXIMUM_SIZE = 100;
        
        Dictionary<T, int> _valueDictionary;
        public ValueTally()
        {
            _valueDictionary = new Dictionary<T, int>();
        }

        public void AddValue(T pValue)
        {
            if (!_valueDictionary.ContainsKey(pValue))
                _valueDictionary[pValue] = 1;
            else
            {
                if (_valueDictionary.Count < MAXIMUM_SIZE
                    && _valueDictionary[pValue] < MAXIMUM_SIZE)
                {
                    _valueDictionary[pValue]++;
                }
            }
        }

        public T MostUsed
        {
            get
            {
                KeyValuePair<T, int> mostUsed = new KeyValuePair<T,int>();
                foreach (KeyValuePair<T, int> keyValue in _valueDictionary)
                {
                    if (keyValue.Value > mostUsed.Value)
                        mostUsed = keyValue;
                }
                return mostUsed.Key;
            }
        }
    }
}
