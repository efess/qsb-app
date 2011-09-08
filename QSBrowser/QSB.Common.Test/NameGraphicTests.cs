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
using NUnit.Framework;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace QSB.Common.Test
{
    [TestFixture]
    public class NameGraphicTests
    {
        [Test]
        public void TestApplyToCharmap()
        {
            foreach (var strFile in Directory.GetFiles(@"C:\bin\uniserver_install\UniServer\www\charmaps\backup"))
            {
                Image charMap = Bitmap.FromFile(strFile);
                var destination = new Bitmap(charMap.Width, charMap.Height);

                Graphics gName = Graphics.FromImage(destination);

                ImageAttributes ia = new ImageAttributes();

                // Adjust coloration in image
                ColorMatrix cm = new ColorMatrix();
                cm.Matrix00 = cm.Matrix11 = cm.Matrix22 = 1.3f; // Adjust contrast
                cm.Matrix33 = cm.Matrix44 = 1;
                cm.Matrix40 = cm.Matrix41 = cm.Matrix42 = 0.2f; // Adjust brightness

                ia.SetColorMatrix(cm);

                gName.DrawImage(charMap, new Rectangle(0, 0, charMap.Width, charMap.Height), 0, 0, charMap.Width, charMap.Height, GraphicsUnit.Pixel, ia);


                destination.MakeTransparent(destination.GetPixel(1, 1));

                var fileName = Path.GetFileName(strFile);
                var directory = Path.Combine(Path.GetDirectoryName(strFile), "output");

                System.IO.FileStream file = new System.IO.FileStream(Path.Combine(directory, fileName)
                    , System.IO.FileMode.OpenOrCreate);

                destination.Save(file, System.Drawing.Imaging.ImageFormat.Png);

                file.Flush();
                file.Close();
            }


        }
    }
}
