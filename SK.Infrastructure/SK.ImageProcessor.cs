using Microsoft.Extensions.Configuration;
using SK.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;

namespace SK.Infrastructure
{
  public class ImageProcessor
  {
    public ImageProcessor()
    {

    }

    public void ExifRotate(Stream imgStream, Stream resultStream)
    {
      const int exifOrientationID = 0x112; //274

      var img = Image.FromStream(imgStream);

      if (img.PropertyIdList.Contains(exifOrientationID))
      {
        var prop = img.GetPropertyItem(exifOrientationID);
        int val = BitConverter.ToUInt16(prop.Value, 0);
        var rot = RotateFlipType.RotateNoneFlipNone;

        if (val == 3 || val == 4)
          rot = RotateFlipType.Rotate180FlipNone;
        else if (val == 5 || val == 6)
          rot = RotateFlipType.Rotate90FlipNone;
        else if (val == 7 || val == 8)
          rot = RotateFlipType.Rotate270FlipNone;

        if (val == 2 || val == 4 || val == 5 || val == 7)
          rot |= RotateFlipType.RotateNoneFlipX;

        if (rot != RotateFlipType.RotateNoneFlipNone)
          img.RotateFlip(rot);
      }

      img.Save(resultStream, ImageFormat.Jpeg);
    }
  }
}
