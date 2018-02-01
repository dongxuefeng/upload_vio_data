using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ehl.itgs.uploaddata.lib
{
    class ImageProc
    {
       public void img_Merge(int icount,params string[] simgfile)
        {
          
            System.Drawing.Image img;
            System.Drawing.Image simg; 
            int iw = 2048;
            int isubw = iw / 2;
            int ih = 1024;
            int isubh = ih / 2;
            if (icount == 2)
            {
                iw = 2048;
                ih = 768; ;
                isubw = iw / 2;
                isubh = ih;
            };

           img = new Bitmap(iw, ih);

           simg = Bitmap.FromFile(simgfile[0]);

           Graphics g = Graphics.FromImage(img);
           g.DrawImage(simg, new Rectangle(0, 0, iw / 2, ih / 2));
           img.Save("d:/test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
         /*
           Image im = this.BackgroundImage;
           MemoryStream ms=new MemoryStream();            
           im.Save(ms,System.Drawing.Imaging.ImageFormat.Jpeg);搜索
          */
        }
    }
}
