using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Drawing;
using QSB.Common;

/// <summary>
/// Summary description for ImageManager
/// </summary>
public class ImageManager
{
    private string _imagePath;

    public ImageManager(string pImagePath)
    {
        if (string.IsNullOrEmpty(pImagePath))
            throw new ArgumentNullException("Must supply valid Image Path");

        _imagePath = pImagePath;
        if (!System.IO.Directory.Exists(_imagePath))
            System.IO.Directory.CreateDirectory(_imagePath);
    }

    public string GetQuakeImageUrl(NameGraphic pGraphic)
    {
        string fileName = pGraphic.EncodedName + ".png";
        string path = System.IO.Path.Combine(_imagePath, fileName);

        if (!System.IO.File.Exists(path))
        {
            using (System.IO.FileStream file = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate))
            {
                System.Drawing.Image nameImage = pGraphic.GetNameImage();
                nameImage.Save(file, System.Drawing.Imaging.ImageFormat.Png);

                file.Flush();
                file.Close();
            }
        }

        return "Images/" + fileName;
    }
}
