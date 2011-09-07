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
using System.Text;
using System.IO;
using QSB.Common;
using System;

namespace QSB.Common
{
    /// <summary>
    /// 
    /// Summary description for Status
    /// </summary>
    public class Status
    {
        private const string STATUS_LINE_DELIMINTER = "<-ST->";
        private const int STATUSWRITE_RETRY_MAX = 3;

        string[] _statusBuffer;
        bool _bufferFull = false;
        object _statusBufferLock;
        string _savePath;
        bool _useDebugFile;

        public string GetStatus()
        {
            StringBuilder sb = new StringBuilder();

            lock (_statusBufferLock)
            {
                for (int i = _statusBuffer.Length - 1; i >= 0; i--)
                {
                    if (_statusBuffer[i] == null) continue;
                    sb.AppendLine(_statusBuffer[i]);
                }
            }

            return sb.ToString();
        }

        public void AddStatus(string pStatus)
        {
            string statusText = STATUS_LINE_DELIMINTER
                + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")
                + " -> " + pStatus;

            try
            {
                lock (_statusBufferLock)
                {
                    if (_useDebugFile)
                        WriteDebugFile(statusText);

                    if (!_bufferFull)
                    {
                        for (int i = 0; i < _statusBuffer.Length; i++)
                        {
                            if (_statusBuffer[i] == null)
                            {
                                _statusBuffer[i] = statusText; return;
                            }
                        }
                        _bufferFull = true;
                    }

                    for (int i = 1; i < _statusBuffer.Length; i++)
                    {
                        _statusBuffer[i - 1] = _statusBuffer[i];
                    }
                    _statusBuffer[_statusBuffer.Length - 1] = statusText;
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// DO NOT call this in an existing lock or you'll deadlock! der.
        /// </summary>
        private void WriteDebugFile(string pText)
        {
            int retryCount = 0;
            bool writeError = false;
            string strToWrite = pText + Environment.NewLine;
            FileStream fs = null;

            // Retry logic
            do
            {
                if (retryCount > 0)
                    System.Threading.Thread.Sleep(500);

                writeError = false;
                try
                {
                    fs = new FileStream(
                        Path.Combine(_savePath, DebugFileName), FileMode.Append);
                }
                catch (Exception ex)
                {
                    writeError = true;
                }

            } while (retryCount++ < STATUSWRITE_RETRY_MAX && writeError);

            Streamhelper.WriteStringToStream(fs, strToWrite);
            fs.Flush();
            fs.Close();
        }

        public Status(int pStatusCount, string pDebugPath)
        {
            _savePath = pDebugPath;
            _statusBufferLock = new object();
            _statusBuffer = new string[pStatusCount];
            _useDebugFile = true;
            if (!Directory.Exists(_savePath))
                Directory.CreateDirectory(_savePath);
        }

        private string DebugFileName
        {
            get { return DateTime.UtcNow.ToString("MM_dd_yyyy") + "____DEBUG.TXT"; }
        }
    }
}