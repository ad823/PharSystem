using Basic;
using H_Pannel_lib;
using MinasA6DLL;
using MyUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLUI;
using HIS_DB_Lib;

namespace FADC
{
    public partial class Main_Form : Form
    {
        public static List<medPicClass> medPicClasses = new List<medPicClass>();
        public static List<Image> Function_取得藥品圖片(string Code)
        {
            bool IsValidImage(Image img)
            {
                return img != null && img.Width > 0 && img.Height > 0;
            }

            List<medPicClass> medPicClasse_buf = medPicClasses.Where(temp => temp.藥碼 == Code).ToList();
            List<Image> images = new List<Image>();

            if (medPicClasse_buf.Count == 0)
            {
                medPicClass medPicClass = new medPicClass();
                List<Image> loadedImages = medPicClass.get_images_by_code(Main_Form.API_Server, Code);

                medPicClass.藥碼 = Code;

                if (loadedImages != null)
                {
                    if (loadedImages.Count > 0 && IsValidImage(loadedImages[0]))
                    {
                        medPicClass.Image_0 = loadedImages[0];
                        images.Add(loadedImages[0]);
                    }
                    if (loadedImages.Count > 1 && IsValidImage(loadedImages[1]))
                    {
                        medPicClass.Image_1 = loadedImages[1];
                        images.Add(loadedImages[1]);
                    }
                }

                medPicClasses.Add(medPicClass);
                return images;
            }
            else
            {
                var cached = medPicClasse_buf[0];
                if (IsValidImage(cached.Image_0)) images.Add(cached.Image_0);
                if (IsValidImage(cached.Image_1)) images.Add(cached.Image_1);
                return images;
            }
        }

       
    }
}
