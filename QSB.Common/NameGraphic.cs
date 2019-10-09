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
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace QSB.Common
{
    public class NameGraphic
    {
        public byte[] NameBytes { get; private set; }
        public Game GameType { get; private set; }
        public string EncodedName { get; private set; }

        public NameGraphic(Game pGame, byte[] pNameBytes)
        {
            GameType = pGame;
            NameBytes = pNameBytes;
            EncodedName = Convert.ToBase64String(pNameBytes).Replace('+', '_').Replace('/', '_').Replace("=", "");
            GetNameImage();
        }

        private Bitmap GetGameCharMap()
        {
            switch (GameType)
            {
                case Game.NetQuake:
                    return CharacterMaps.NetQuakeCharMap;
                case Game.QuakeWorld:
                    return CharacterMaps.NetQuakeCharMap;
                case Game.Quake2:
                    return CharacterMaps.Quake2CharMap;
                case Game.Quake3:
                    return null;
                case Game.Quake4:
                    return null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates an image containing player's name with a transparent background
        /// </summary>
        /// <returns>Image of Player's Name</returns>
        public virtual Image GetNameImage()
        {
            Bitmap charMap = GetGameCharMap();
            if (charMap == null)
                throw new NullReferenceException("No character map available to create name from");

            float vPx = charMap.Height / 16f;
            float hPx = charMap.Width / 16f;

            // Scale Horizontal size to match 12px vertical
            float scale = 12f / vPx;
            vPx = 12f;  
            hPx = scale * hPx;

            Bitmap nameImage = new Bitmap((int)(hPx * NameBytes.Length), (int)vPx);

            if (NameBytes.Length == 0)
                return nameImage;

            Graphics gName = Graphics.FromImage(nameImage);

            for (int i = 0; i < NameBytes.Length; i++)
            {
                byte nameByte = NameBytes[i];

                int vImgOffset = (nameByte >> 4);
                int hImgOffset = nameByte & 0x0F;

                // Set Clipping region in name
                gName.Clip = new System.Drawing.Region(new RectangleF(i * hPx, 0, hPx, vPx));
                gName.DrawImage(charMap, (i * hPx) - (hImgOffset * hPx), (-1) * (vImgOffset * vPx), charMap.Width * scale, charMap.Height * scale);
            }

            // Reset clip
            gName.Clip = new System.Drawing.Region(new Rectangle(0, 0, nameImage.Width, nameImage.Height));

            // Copy image so we can wipe original, and repaint w/ transparent color
            Bitmap bmp = new Bitmap(nameImage);

            // Set transparency attribute the first pixel in the character map
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorKey(charMap.GetPixel(0, 0), charMap.GetPixel(0, 0));

            // Adjust coloration in image
            ColorMatrix cm = new ColorMatrix();
            cm.Matrix00 = cm.Matrix11 = cm.Matrix22 = 1.3f; // Adjust contrast
            cm.Matrix33 = cm.Matrix44 = 1;
            cm.Matrix40 = cm.Matrix41 = cm.Matrix42 = 0.2f; // Adjust brightness
            
            ia.SetColorMatrix(cm);

            // Clear canvas in order to re-paint with Transparency
            gName.Clear(Color.Transparent);
            gName.DrawImage(bmp, new Rectangle(0, 0, nameImage.Width, nameImage.Height), 0, 0, nameImage.Width, nameImage.Height, GraphicsUnit.Pixel, ia);

            // FOR DEBUG
            //System.IO.FileStream file = new System.IO.FileStream("C:\\temp\\try" + (DateTime.Now.Ticks % 25435).ToString() + ".png", System.IO.FileMode.OpenOrCreate);

            //nameImage.Save(file, System.Drawing.Imaging.ImageFormat.Png);
            //file.Flush();
            //file.Close();

            // Clean up
            gName.Dispose();
            bmp.Dispose();

            return nameImage;
        }
    }

}
