using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

public static class ScreenshotHelper
{
    /// <summary>
    /// 對指定控制項截圖，並自動依時間存檔。
    /// </summary>
    public static string CaptureControlWithTimestamp(Control ctrl)
    {
        try
        {
            // 建立資料夾
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "screenshots");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            // 檔名 yyyyMMdd_HHmmss.png
            string filename = $"UI_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string filepath = Path.Combine(folder, filename);

            // 擷取控制項畫面
            Bitmap bmp = new Bitmap(ctrl.Width, ctrl.Height);
            ctrl.DrawToBitmap(bmp, new Rectangle(0, 0, ctrl.Width, ctrl.Height));

            // 存檔
            bmp.Save(filepath, ImageFormat.Png);
            bmp.Dispose();

            return filepath;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"截圖失敗：{ex.Message}");
            return null;
        }
    }
}
