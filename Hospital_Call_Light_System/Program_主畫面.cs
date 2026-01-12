using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyUI;
using Basic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using hcls_DB_Lib;
using NPOI.SS.Formula.Functions;

namespace Hospital_Call_Light_System
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        int 叫號台01_號碼 = 0;
        int 叫號台02_號碼 = 0;
        MyTimerBasic MyTimerBasic_第一台_圖片計時 = new MyTimerBasic(1000);
        MyTimerBasic MyTimerBasic_第二台_圖片計時 = new MyTimerBasic(1000);
        int 第一台_圖片索引 = 1;
        int 第二台_圖片索引 = 1;
        List<object[]> list_叫號內容設定 = new List<object[]>();
        List<object[]> list_樣式設定 = new List<object[]>();
        List<object[]> list_公告設定 = new List<object[]>();
        private void Program_主畫面_Init()
        {
            this.button_第一台號碼輸入.Click += Button_第一台號碼輸入_Click;
            this.button_第二台號碼輸入.Click += Button_第二台號碼輸入_Click;
            this.plC_RJ_Button_刷新螢幕.MouseDownEvent += PlC_RJ_Button_刷新螢幕_MouseDownEvent;
            this.plC_RJ_Button_檢查按鈕.MouseDownEvent += PlC_RJ_Button_檢查按鈕_MouseDownEvent;
            this.plC_RJ_Button_刷新音效.MouseDownEvent += PlC_RJ_Button_刷新音效_MouseDownEvent;
       
            Dialog_小叫號台.EnrterNum01Event += Dialog_小叫號台_EnrterNum01Event;
            Dialog_小叫號台.EnrterNum02Event += Dialog_小叫號台_EnrterNum02Event;
            this.plC_UI_Init.Add_Method(Program_主畫面);
        }
        private void Program_主畫面()
        {
            sub_Program_刷新螢幕();
            sub_Program_刷新音效();
            sub_Program_資料刷新();
        }
        private void Function_主畫面_叫號音效輸出(int num)
        {
            List<object[]> list_value = this.sqL_DataGridView_參數.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_參數.Name, "音效");
            if (list_value.Count == 0)
            {
                object[] value = new object[new enum_參數().GetLength()];
                value[(int)enum_參數.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_參數.Name] = "音效";
                value[(int)enum_參數.Value] = $"領藥號 {num} 以前可以領藥";
                this.sqL_DataGridView_參數.SQL_AddRow(value, false);
            }
            else
            {
                object[] value = list_value[0];
                value[(int)enum_參數.Name] = "音效";
                value[(int)enum_參數.Value] = $"領藥號 {num} 以前可以領藥";
                this.sqL_DataGridView_參數.SQL_ReplaceExtra(value, false);
            }

            System.Media.SoundPlayer sp = null;

            if (this.myConfigClass.本地音效)
            {
                try
                {
                    voice.Speak($"領藥號 {num} 以前可以領藥");
                    //sp = new System.Media.SoundPlayer(".//RING.wav");
                    //sp.Stop();

                    //sp.Play();

                }
                finally
                {
                    if (sp != null) sp.Dispose();
                }
            }
        }
        private void Function_主畫面_資料刷新()
        {
            list_叫號內容設定 = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
            list_樣式設定 = this.sqL_DataGridView_樣式設定.SQL_GetAllRows(false);
            list_公告設定 = this.sqL_DataGridView_公告設定.SQL_GetAllRows(false);
        }

        /// <summary>
        /// 產生一個包含標題文字的 Bitmap 影像，並可設定背景與外框。
        /// 文字會在指定區域內水平與垂直置中顯示。
        /// </summary>
        /// <param name="titleText">要繪製的標題文字。</param>
        /// <param name="titleFont">標題文字所使用的字型。</param>
        /// <param name="titleSize">整體影像大小 (寬度與高度)。</param>
        /// <param name="titleTextWidth">標題文字可使用的寬度範圍。</param>
        /// <param name="titleFontColor">標題文字顏色。</param>
        /// <param name="titleBackColor">影像背景顏色。</param>
        /// <returns>
        /// 回傳產生的 <see cref="Bitmap"/> 物件。  
        /// 若產生失敗，則回傳 <c>null</c>。
        /// </returns>
        private Bitmap GetTextBitmap(string titleText, Font titleFont, Size titleSize, int titleTextWidth, Color titleFontColor, Color titleBackColor)
        {
            return GetTextBitmap(titleText, titleFont, titleSize, titleTextWidth, titleFontColor, titleBackColor, titleBackColor, 0, 0);
        }
        /// <summary>
        /// 產生一個包含標題文字的 Bitmap 影像，並可設定背景、內框(padding)、圓角。  
        /// - 中文：逐字平均分配並置中。  
        /// - 英文：單字內字母間距與單字間距自動計算，整體壓在 titleTextWidth 內置中。  
        /// </summary>
        private Bitmap GetTextBitmap(
       string titleText,
       Font titleFont,
       Size titleSize,
       int titleTextWidth,
       Color titleFontColor,
       Color titleBackColor,    // 整體背景色
       Color contentBackColor,  // 文字框背景色
       int padding,             // 內框 padding
       int borderRadius,
       float letterRatio = 0.4f,
       float wordRatio = 0.6f)
        {
            try
            {
                Bitmap bitmap = new Bitmap(titleSize.Width, titleSize.Height);
                using (Graphics g_bmp = Graphics.FromImage(bitmap))
                {
                    g_bmp.SmoothingMode = SmoothingMode.AntiAlias;
                    g_bmp.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g_bmp.CompositingQuality = CompositingQuality.HighQuality;
                    g_bmp.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    // === 整體背景 ===
                    using (SolidBrush backBrush = new SolidBrush(titleBackColor))
                        g_bmp.FillRectangle(backBrush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

                    // === 文字框區域 ===
                    Rectangle contentRect = new Rectangle(
                        padding,
                        padding,
                        bitmap.Width - padding * 2,
                        bitmap.Height - padding * 2
                    );

                    using (GraphicsPath path = new GraphicsPath())
                    {
                        if (borderRadius > 0)
                        {
                            int r = borderRadius;
                            path.AddArc(contentRect.Left, contentRect.Top, r * 2, r * 2, 180, 90);
                            path.AddArc(contentRect.Right - r * 2, contentRect.Top, r * 2, r * 2, 270, 90);
                            path.AddArc(contentRect.Right - r * 2, contentRect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                            path.AddArc(contentRect.Left, contentRect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                            path.CloseFigure();
                        }
                        else
                        {
                            path.AddRectangle(contentRect);
                        }

                        // 填滿框背景
                        using (SolidBrush contentBrush = new SolidBrush(contentBackColor))
                            g_bmp.FillPath(contentBrush, path);
                    }

                    // === 繪製文字 ===
                    int xStart = contentRect.Left + (contentRect.Width - titleTextWidth) / 2;
                    int yCenter = contentRect.Top + contentRect.Height / 2;

              

                    bool hasEnglish = Regex.IsMatch(titleText, "[A-Za-z]");

                    if (hasEnglish)
                    {
                        using (Brush textBrush = new SolidBrush(titleFontColor))
                        {
                            SizeF size = g_bmp.MeasureString(titleText, titleFont);
                            float x = xStart + (titleTextWidth - size.Width) / 2;
                            float y = yCenter - size.Height / 2;

                            g_bmp.DrawString(titleText, titleFont, textBrush, x, y);
                        }
                    }
                    else
                    {
                        int charCount = titleText.Length;
                        float cellWidth = (float)titleTextWidth / charCount;
                        float curX = xStart;

                        // 取得字型度量
                        FontFamily ff = titleFont.FontFamily;
                        float emHeight = ff.GetEmHeight(titleFont.Style);
                        float ascent = ff.GetCellAscent(titleFont.Style);
                        float lineSpacing = ff.GetLineSpacing(titleFont.Style);

                        float ascentPixel = titleFont.Size * ascent / emHeight;
                        float lineSpacingPixel = titleFont.Size * lineSpacing / emHeight;

                        // ✅ 使用 lineSpacing 計算 baseline
                        float baseline = yCenter + (lineSpacingPixel / 2 - ascentPixel);

                     
       


                        using (Brush textBrush = new SolidBrush(titleFontColor))
                        {
                            foreach (char c in titleText)
                            {
                                string s = c.ToString();
                                SizeF size = g_bmp.MeasureString(s, titleFont, PointF.Empty, StringFormat.GenericTypographic);

                                float x = curX + (cellWidth - size.Width) / 2;
                                float y = baseline - ascentPixel / 2;  // baseline 減去 ascent

                                g_bmp.DrawString(s, titleFont, textBrush, x, y, StringFormat.GenericTypographic);
                                curX += cellWidth;
                            }
                        }
                    }


                }
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private Bitmap DrawNormalText(string text, Font font, Size size, Color textColor, Color backColor)
        {
            return DrawNormalText(text, font, size, textColor, backColor, backColor, 0, 0);
        }
        /// <summary>
        /// 一般繪製文字 (不平均分配，依字型自然排版)
        /// </summary>
        private Bitmap DrawNormalText(string text, Font font, Size size, Color textColor, Color backColor, Color contentBackColor, int padding, int borderRadius)
        {
            try
            {
                Bitmap bmp = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    // === 整體背景 ===
                    using (SolidBrush backBrush = new SolidBrush(backColor))
                        g.FillRectangle(backBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));

                    // === 文字框區域 ===
                    Rectangle contentRect = new Rectangle(
                        padding,
                        padding,
                        bmp.Width - padding * 2,
                        bmp.Height - padding * 2
                    );

                    using (GraphicsPath path = new GraphicsPath())
                    {
                        if (borderRadius > 0)
                        {
                            int r = borderRadius;
                            path.AddArc(contentRect.Left, contentRect.Top, r * 2, r * 2, 180, 90);
                            path.AddArc(contentRect.Right - r * 2, contentRect.Top, r * 2, r * 2, 270, 90);
                            path.AddArc(contentRect.Right - r * 2, contentRect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                            path.AddArc(contentRect.Left, contentRect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                            path.CloseFigure();
                        }
                        else
                        {
                            path.AddRectangle(contentRect);
                        }

                        // 填滿框背景
                        using (SolidBrush contentBrush = new SolidBrush(contentBackColor))
                            g.FillPath(contentBrush, path);
                    }

                    // === 量測文字大小 ===
                    SizeF textSize = g.MeasureString(text, font);

                    // 置中繪製
                    float x = contentRect.Left + (contentRect.Width - textSize.Width) / 2f;
                    float y = contentRect.Top + (contentRect.Height - textSize.Height) / 2f;

                    using (SolidBrush textBrush = new SolidBrush(textColor))
                        g.DrawString(text, font, textBrush, x, y);
                }

                return bmp;
            }
            catch
            {
                return null;
            }
        }



        #region Event

        private void PlC_RJ_Button_刷新音效_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.myConfigClass.全局音效)
            {
                System.Media.SoundPlayer sp = null;
                List<object[]> list_value = this.sqL_DataGridView_參數.SQL_GetAllRows(false);
                list_value = list_value.GetRows((int)enum_參數.Name, "音效");
                if (list_value.Count > 0)
                {
                    object[] value = list_value[0];
                  

                    if (value[(int)enum_參數.Value].ObjectToString().StringIsEmpty())
                    {
                        try
                        {
                            string text = value[(int)enum_參數.Value].ObjectToString();
                            voice.Speak($"{text}");
                            sp = new System.Media.SoundPlayer(".//RING.wav");
                            sp.Stop();

                            sp.Play();

                        }
                        finally
                        {
                            if (sp != null) sp.Dispose();
                        }
                    }

                    value[(int)enum_參數.Name] = "音效";
                    value[(int)enum_參數.Value] = "";
                    this.sqL_DataGridView_參數.SQL_ReplaceExtra(value, false);
                }
            }
        }
        private void PlC_RJ_Button_刷新螢幕_MouseDownEvent(MouseEventArgs mevent)
        {
            Bitmap bitmap_標題_0 = null;
            Bitmap bitmap_英文標題_0 = null;
            Bitmap bitmap_叫號_0 = null;
            Bitmap bitmap_叫號備註_0 = null;
            Bitmap bitmap_標題_1 = null;
            Bitmap bitmap_英文標題_1 = null;
            Bitmap bitmap_叫號_1 = null;
            Bitmap bitmap_叫號備註_1 = null;
            Bitmap bitmap_圖片_0 = null;
            Bitmap bitmap_圖片_1 = null;
            try
            {
                using (Graphics g = this.panel_叫號.CreateGraphics())
                {

                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    string 一號台名稱 = "";
                    string 二號台名稱 = "";
                    string 公告名稱 = "";
                    
                    this.Invoke(new Action(delegate 
                    {
                        一號台名稱 = comboBox_一號台名稱.Text;
                        二號台名稱 = comboBox_二號台名稱.Text;
                        公告名稱 = comboBox_公告名稱.Text;
                    }));
                    int width = this.panel_叫號.Width;
                    int height = this.panel_叫號.Height;
                  
                    List<object[]> list_叫號內容設定_buf = new List<object[]>();
                    List<object[]> list_樣式設定_buf = new List<object[]>();
                    List<object[]> list_公告設定_buf = new List<object[]>();
                    if(公告名稱 != "無")
                    {
                        list_公告設定_buf = list_公告設定.GetRows((int)enum_公告設定.名稱, 公告名稱);
                    }
                    int 公告高度 = 0;
                    if(list_公告設定_buf.Count > 0)
                    {
                        公告高度 = list_公告設定_buf[0][(int)enum_公告設定.高度].StringToInt32();
                    }
                    bool flag_RING_1 = false;
                    bool flag_RING_2 = false;
                    for (int i = 0; i < list_樣式設定.Count; i++)
                    {
                        if (i == 0)
                        {
                            list_叫號內容設定_buf = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 一號台名稱);
                            if (list_叫號內容設定_buf.Count == 0) continue;
                            list_樣式設定_buf = list_樣式設定.GetRows((int)enum_sytle_setting.代碼, list_叫號內容設定_buf[0][(int)enum_叫號內容設定.樣式代碼].ObjectToString());
                            if (list_樣式設定_buf.Count == 0) continue;
                        }
                        if (i == 1)
                        {
                            list_叫號內容設定_buf = list_叫號內容設定.GetRows((int)enum_叫號內容設定.名稱, 二號台名稱);
                            if (list_叫號內容設定_buf.Count == 0) continue;
                            list_樣式設定_buf = list_樣式設定.GetRows((int)enum_sytle_setting.代碼, list_叫號內容設定_buf[0][(int)enum_叫號內容設定.樣式代碼].ObjectToString());
                            if (list_樣式設定_buf.Count == 0) continue;
                        }
                        if (list_叫號內容設定_buf.Count == 0) continue;
                        int panel_width = (width - 0) / 2;
                        int temp = 0;
                        if (!radioButton_一號台_不顯示.Checked) temp++;
                        if (!radioButton_二號台_不顯示.Checked) temp++;
                        if (temp == 1) panel_width = (width - 0) / 1;

                        styleSettingClass styleSetting = list_樣式設定_buf[0].ObjectToStyleSetting();

                        string titleName = list_叫號內容設定_buf[0][(int)enum_叫號內容設定.名稱].ObjectToString();
                        string engTitleName = list_叫號內容設定_buf[0][(int)enum_叫號內容設定.英文名].ObjectToString();
                        string callNote = list_叫號內容設定_buf[0][(int)enum_叫號內容設定.叫號備註].ObjectToString();


                        Font titleFont = styleSetting.Title.TitleFont.ToFont();
                        int titleTextWidth = styleSetting.Title.TitleTextWidth.StringToInt32();
                        Color titleFontColor = styleSetting.Title.TitleFontColor.ToColor();
                        Color titleBackColor = styleSetting.Title.TitleBackColor.ToColor();
                        int titleHeight = styleSetting.Title.TitleHeight.StringToInt32();

                        Font engTitleFont = styleSetting.Title.EngTitleFont.ToFont();
                        int engTitleHeight = styleSetting.Title.EngTitleHeight.StringToInt32();

                        Font callFont = styleSetting.Call.CallFont.ToFont();
                        int callTextWidth = styleSetting.Call.CallTextWidth.StringToInt32();
                        Color callFontColor = styleSetting.Call.CallFontColor.ToColor();
                        Color callBackColor = styleSetting.Call.CallBackColor.ToColor();                 
                        Font callNoteFont = styleSetting.Call.CallNoteFont.ToFont();
                        int callNoteHeight = styleSetting.Call.CallNoteHeight.StringToInt32();
                        if (callNote.StringIsEmpty()) callNoteHeight = 0;
                        int callHeight = styleSetting.CaculateCallHeight(height, 公告高度 + callNoteHeight);

                        int callBorderRadius = styleSetting.Call.CallBorderRadius.StringToInt32();
                        int callMargin = styleSetting.Call.CallMargin.StringToInt32();

                        if (i == 0 && radioButton_一號台_號碼.Checked)
                        {
                            if (叫號台01_號碼 != list_叫號內容設定_buf[0][(int)enum_叫號內容設定.號碼].StringToInt32())
                            {
                                flag_RING_1 = true;
                            }
                            叫號台01_號碼 = list_叫號內容設定_buf[0][(int)enum_叫號內容設定.號碼].StringToInt32();
                            bitmap_標題_0 = GetTextBitmap(titleName, titleFont, new Size(panel_width, titleHeight), titleTextWidth, titleFontColor, titleBackColor);
                            bitmap_英文標題_0 = GetTextBitmap(engTitleName, engTitleFont, new Size(panel_width, engTitleHeight), titleTextWidth, titleFontColor, titleBackColor);
                            bitmap_叫號_0 = GetTextBitmap(叫號台01_號碼.ToString("0000"), callFont, new Size(panel_width, callHeight), callTextWidth, callFontColor, titleBackColor, callBackColor , callMargin, callBorderRadius);
                            if (叫號台01_號碼 == 0)
                            {
                                bitmap_叫號_0 = null;
                                if (bitmap_標題_0 != null) bitmap_叫號備註_0 = GetTextBitmap(callNote, callNoteFont, new Size(panel_width, height - bitmap_標題_0.Height - bitmap_英文標題_0.Height), callTextWidth, callFontColor, titleBackColor, callBackColor, callMargin, callBorderRadius);
                            }
                            else
                            {
                                if (bitmap_標題_0 != null) bitmap_叫號備註_0 = DrawNormalText(callNote, callNoteFont, new Size(panel_width, callNoteHeight), titleFontColor, titleBackColor);

                            }
                        }
                        if (i == 1 && radioButton_二號台_號碼.Checked)
                        {
                            if (叫號台02_號碼 != list_叫號內容設定_buf[0][(int)enum_叫號內容設定.號碼].StringToInt32())
                            {
                                flag_RING_2 = true;
                            }

                            叫號台02_號碼 = list_叫號內容設定_buf[0][(int)enum_叫號內容設定.號碼].StringToInt32();
                            bitmap_標題_1 = GetTextBitmap(titleName, titleFont, new Size(panel_width, titleHeight), titleTextWidth, titleFontColor, titleBackColor);
                            bitmap_英文標題_1 = GetTextBitmap(engTitleName, engTitleFont, new Size(panel_width, engTitleHeight), titleTextWidth, titleFontColor, titleBackColor);
                            bitmap_叫號_1 = GetTextBitmap(叫號台02_號碼.ToString("0000"), callFont, new Size(panel_width, callHeight), callTextWidth, callFontColor, titleBackColor, callBackColor, callMargin, callBorderRadius);
                            if (叫號台02_號碼 == 0)
                            {
                                bitmap_叫號_1 = null;
                                if (bitmap_標題_1 != null) bitmap_叫號備註_1 = GetTextBitmap(callNote, callNoteFont, new Size(panel_width, height - bitmap_標題_1.Height - bitmap_英文標題_1.Height), callTextWidth, callFontColor, titleBackColor, callBackColor, callMargin, callBorderRadius);
                            }
                            else
                            {
                                if (bitmap_標題_1 != null) bitmap_叫號備註_1 = DrawNormalText(callNote, callNoteFont, new Size(panel_width, callNoteHeight), titleFontColor, titleBackColor);
                            }
                        }

                        if (i == 0 && radioButton_一號台_圖片.Checked)
                        {
                            if(MyTimerBasic_第一台_圖片計時.IsTimeOut())
                            {
                                if (第一台_圖片索引 > 9) 第一台_圖片索引 = 1;
                                int temp0 = Function_取得一號台圖片選取();
                                if(temp0.GetBit(第一台_圖片索引 - 1))
                                {
                                    int 停留秒數 = 0;
                                    using (Bitmap bitmap = Function_圖片上傳_取得指定代碼圖片(第一台_圖片索引.ToString(), ref 停留秒數))
                                    {
                                        if(bitmap != null)
                                        {
                                            bitmap_圖片_0 = bitmap.ScaleImage(panel_width, height - 公告高度);       
                                        }
                                        
                                    }
                                    MyTimerBasic_第一台_圖片計時.TickStop();
                                    MyTimerBasic_第一台_圖片計時.StartTickTime(停留秒數 * 1000);
                                }
                                第一台_圖片索引++;


                            }                         
                        }
                        if (i == 0 && radioButton_二號台_圖片.Checked)
                        {
                            if (MyTimerBasic_第二台_圖片計時.IsTimeOut())
                            {
                                if (第二台_圖片索引 > 9) 第二台_圖片索引 = 1;
                                int temp0 = Function_取得二號台圖片選取();
                                if (temp0.GetBit(第二台_圖片索引 - 1))
                                {
                                    int 停留秒數 = 0;
                                    using (Bitmap bitmap = Function_圖片上傳_取得指定代碼圖片(第二台_圖片索引.ToString(), ref 停留秒數))
                                    {
                                        if (bitmap != null)
                                        {
                                            bitmap_圖片_1 = bitmap.ScaleImage(panel_width, height - 公告高度);
                                        }

                                    }
                                    MyTimerBasic_第二台_圖片計時.TickStop();
                                    MyTimerBasic_第二台_圖片計時.StartTickTime(停留秒數 * 1000);
                                }
                                第二台_圖片索引++;


                            }
                        }
                    }
                    int tota_width = 0;
                    if (radioButton_一號台_不顯示.Checked == false)
                    {
                        if (bitmap_標題_0 != null) tota_width += bitmap_標題_0.Width;
                        if (radioButton_一號台_圖片.Checked) tota_width += width / 2;
                    }
                    if (radioButton_二號台_不顯示.Checked == false)
                    {
                        if (bitmap_標題_1 != null) tota_width += bitmap_標題_1.Width;
                        if (radioButton_二號台_圖片.Checked) tota_width += width / 2;
                    }
              
               

                    int posx = (width - tota_width) / 2;
                    if (bitmap_標題_0 != null && radioButton_一號台_號碼.Checked)
                    {
                        g.DrawImage(bitmap_標題_0, new PointF(posx, 0));
                        g.DrawImage(bitmap_英文標題_0, new PointF(posx, bitmap_標題_0.Height));
                        if (bitmap_叫號_0 != null) g.DrawImage(bitmap_叫號_0, new PointF(posx, bitmap_標題_0.Height + bitmap_英文標題_0.Height));
                        g.DrawImage(bitmap_叫號備註_0, new PointF(posx, bitmap_標題_0.Height + bitmap_英文標題_0.Height + (bitmap_叫號_0 == null ? 0 : bitmap_叫號_0.Height)));
                    }


                    if (bitmap_標題_1 != null && radioButton_二號台_號碼.Checked)
                    {
                        g.DrawImage(bitmap_標題_1, new PointF(posx + bitmap_標題_1.Width, 0));
                        g.DrawImage(bitmap_英文標題_1, new PointF(posx + bitmap_標題_1.Width, bitmap_標題_1.Height));
                        if (bitmap_叫號_1 != null) g.DrawImage(bitmap_叫號_1, new PointF(posx + bitmap_標題_1.Width, bitmap_標題_1.Height + bitmap_英文標題_1.Height));
                        g.DrawImage(bitmap_叫號備註_1, new PointF(posx + bitmap_標題_1.Width, bitmap_標題_1.Height + bitmap_英文標題_1.Height + (bitmap_叫號_1 == null ? 0 : bitmap_叫號_1.Height)));
                    }
                    if (bitmap_圖片_0 != null && radioButton_一號台_圖片.Checked)
                    {
                        g.DrawImage(bitmap_圖片_0, new PointF(0, 0));
                    }
                    if (bitmap_圖片_1 != null && radioButton_二號台_圖片.Checked)
                    {
                        tota_width = 0;
                        if (radioButton_二號台_圖片.Checked) tota_width += width / 2;
                        g.DrawImage(bitmap_圖片_1, new PointF(tota_width, 0));
                    }
                    if (list_公告設定_buf.Count > 0)
                    {
                        string text = list_公告設定_buf[0][(int)enum_公告設定.內容].ObjectToString();
                        Font font = list_公告設定_buf[0][(int)enum_公告設定.字體].ObjectToString().ToFont();
                        Color textColor = list_公告設定_buf[0][(int)enum_公告設定.字體顏色].ObjectToString().ToColor();
                        Color backColor = list_公告設定_buf[0][(int)enum_公告設定.背景顏色].ObjectToString().ToColor();
                        int speed = list_公告設定_buf[0][(int)enum_公告設定.移動速度].StringToInt32();
                        speed =  speed * 1;
                        Marquee.SetConfig(text, font, textColor, backColor, speed, width, 公告高度);
                        using (Bitmap bitmap = Marquee.GetBitmap())
                        {
                            g.DrawImage(bitmap, new PointF(0,height - 公告高度));
                        }
                    }
                    //叫號聲音
                    if (flag_RING_1 || flag_RING_2)
                    {
                        Dialog_小叫號台.Refresh(叫號台01_號碼, 叫號台02_號碼);
                        if (flag_RING_1) Function_主畫面_叫號音效輸出(叫號台01_號碼);
                        else if(flag_RING_2)Function_主畫面_叫號音效輸出(叫號台02_號碼);

                    }


                }
            }
            catch
            {

            }
            finally
            {
                if (bitmap_標題_0 != null) bitmap_標題_0.Dispose();
                if (bitmap_英文標題_0 != null) bitmap_英文標題_0.Dispose();
                if (bitmap_叫號_0 != null) bitmap_叫號_0.Dispose();
                if (bitmap_叫號備註_0 != null) bitmap_叫號備註_0.Dispose();
                if (bitmap_標題_1 != null) bitmap_標題_1.Dispose();
                if (bitmap_英文標題_1 != null) bitmap_英文標題_1.Dispose();
                if (bitmap_叫號_1 != null) bitmap_叫號_1.Dispose();
                if (bitmap_叫號備註_1 != null) bitmap_叫號備註_1.Dispose();
                if (bitmap_圖片_0 != null) bitmap_圖片_0.Dispose();
                if (bitmap_圖片_1 != null) bitmap_圖片_1.Dispose();


            }

        }
        private void PlC_RJ_Button_檢查按鈕_MouseDownEvent(MouseEventArgs mevent)
        {

        }
        private void Button_第一台號碼輸入_Click(object sender, EventArgs e)
        {
            if (radioButton_一號台_號碼.Checked == false)
            {
                MyMessageBox.ShowDialog("第一台目前非叫號模式!");
                return;
            }
            string 機台名稱 = this.comboBox_一號台名稱.Text;
            List<object[]> list_value = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
            if(list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("找無資料!");
                return;
            }
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
            list_value[0][(int)enum_叫號內容設定.號碼] = dialog_NumPannel.Value.ToString("0000");
            this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_value, false);
        }
        private void Button_第二台號碼輸入_Click(object sender, EventArgs e)
        {
            if(radioButton_二號台_號碼.Checked == false)
            {
                MyMessageBox.ShowDialog("第二台目前非叫號模式!");
                return;
            }
            string 機台名稱 = this.comboBox_二號台名稱.Text;
            List<object[]> list_value = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("找無資料!");
                return;
            }
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
            list_value[0][(int)enum_叫號內容設定.號碼] = dialog_NumPannel.Value.ToString("0000");
            this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_value, false);
        }
        private void PlC_RJ_Button_全螢幕顯示_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_螢幕選擇 dialog_螢幕選擇 = new Dialog_螢幕選擇();

            this.Invoke(new Action(delegate
            {
                if (dialog_螢幕選擇.ShowDialog() == DialogResult.Yes)
                {

                    try
                    {
                        Dialog_小叫號台.ShowForm();
                        Dialog_小叫號台.Refresh(叫號台01_號碼, 叫號台02_號碼);


                        Basic.Screen.FullScreen(this.FindForm(), dialog_螢幕選擇.Value, true);
                        Basic.Screen.FullScreen(this.FindForm(), dialog_螢幕選擇.Value, false);
                        List<Task> taskList = new List<Task>();
                        taskList.Add(Task.Run(() =>
                        {

                            System.Threading.Thread.Sleep(500);
                            this.Invoke(new Action(delegate
                            {
                                Basic.Screen.FullScreen(this.FindForm(), dialog_螢幕選擇.Value, true);
                            }));
                        }));


                        panel_Main.Visible = false;
                        Application.DoEvents();

                        this.全螢幕 = true;
                    }
                    catch
                    {
                        Basic.Screen.FullScreen(this.FindForm(), 0, false);
                        MyMessageBox.ShowDialog("找無此螢幕!");
                    }
                }
            }));


        }
        private void Dialog_小叫號台_EnrterNum02Event()
        {
            if (radioButton_二號台_號碼.Checked == false)
            {
                MyMessageBox.ShowDialog("第二台目前非叫號模式!");
                return;
            }
            string 機台名稱 = this.comboBox_二號台名稱.Text;
            List<object[]> list_value = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("找無資料!");
                return;
            }
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
            list_value[0][(int)enum_叫號內容設定.號碼] = dialog_NumPannel.Value.ToString("0000");
            this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_value, false);
        }
        private void Dialog_小叫號台_EnrterNum01Event()
        {
            if (radioButton_一號台_號碼.Checked == false)
            {
                MyMessageBox.ShowDialog("第一台目前非叫號模式!");
                return;
            }
            string 機台名稱 = this.comboBox_一號台名稱.Text;
            List<object[]> list_value = this.sqL_DataGridView_叫號內容設定.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_叫號內容設定.名稱, 機台名稱);
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("找無資料!");
                return;
            }
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
            list_value[0][(int)enum_叫號內容設定.號碼] = dialog_NumPannel.Value.ToString("0000");
            this.sqL_DataGridView_叫號內容設定.SQL_ReplaceExtra(list_value, false);
        }
        #endregion

        #region PLC_刷新螢幕
        PLC_Device PLC_Device_刷新螢幕 = new PLC_Device("");
        PLC_Device PLC_Device_刷新螢幕_OK = new PLC_Device("");
        Task Task_刷新螢幕;
        MyTimer MyTimer_刷新螢幕_結束延遲 = new MyTimer();
        int cnt_Program_刷新螢幕 = 65534;
        void sub_Program_刷新螢幕()
        {
            if (plC_ScreenPage_Main.PageText == "主畫面") PLC_Device_刷新螢幕.Bool = true;
            if (cnt_Program_刷新螢幕 == 65534)
            {
                this.MyTimer_刷新螢幕_結束延遲.StartTickTime(100);
                PLC_Device_刷新螢幕.SetComment("PLC_刷新螢幕");
                PLC_Device_刷新螢幕_OK.SetComment("PLC_刷新螢幕_OK");
                PLC_Device_刷新螢幕.Bool = false;
                cnt_Program_刷新螢幕 = 65535;
            }
            if (cnt_Program_刷新螢幕 == 65535) cnt_Program_刷新螢幕 = 1;
            if (cnt_Program_刷新螢幕 == 1) cnt_Program_刷新螢幕_檢查按下(ref cnt_Program_刷新螢幕);
            if (cnt_Program_刷新螢幕 == 2) cnt_Program_刷新螢幕_初始化(ref cnt_Program_刷新螢幕);
            if (cnt_Program_刷新螢幕 == 3) cnt_Program_刷新螢幕 = 65500;
            if (cnt_Program_刷新螢幕 > 1) cnt_Program_刷新螢幕_檢查放開(ref cnt_Program_刷新螢幕);

            if (cnt_Program_刷新螢幕 == 65500)
            {
                this.MyTimer_刷新螢幕_結束延遲.TickStop();
                this.MyTimer_刷新螢幕_結束延遲.StartTickTime(100);
                PLC_Device_刷新螢幕.Bool = false;
                PLC_Device_刷新螢幕_OK.Bool = false;
                cnt_Program_刷新螢幕 = 65535;
            }
        }
        void cnt_Program_刷新螢幕_檢查按下(ref int cnt)
        {
            if (PLC_Device_刷新螢幕.Bool) cnt++;
        }
        void cnt_Program_刷新螢幕_檢查放開(ref int cnt)
        {
            if (!PLC_Device_刷新螢幕.Bool) cnt = 65500;
        }
        void cnt_Program_刷新螢幕_初始化(ref int cnt)
        {
            if (this.MyTimer_刷新螢幕_結束延遲.IsTimeOut())
            {
                if (Task_刷新螢幕 == null)
                {
                    Task_刷新螢幕 = new Task(new Action(delegate { PlC_RJ_Button_刷新螢幕_MouseDownEvent(null); }));
                }
                if (Task_刷新螢幕.Status == TaskStatus.RanToCompletion)
                {
                    Task_刷新螢幕 = new Task(new Action(delegate { PlC_RJ_Button_刷新螢幕_MouseDownEvent(null); }));
                }
                if (Task_刷新螢幕.Status == TaskStatus.Created)
                {
                    Task_刷新螢幕.Start();
                }
                cnt++;
            }
        }







        #endregion
        #region PLC_刷新音效
        PLC_Device PLC_Device_刷新音效 = new PLC_Device("");
        PLC_Device PLC_Device_刷新音效_OK = new PLC_Device("");
        Task Task_刷新音效;
        MyTimer MyTimer_刷新音效_結束延遲 = new MyTimer();
        int cnt_Program_刷新音效 = 65534;
        void sub_Program_刷新音效()
        {
            if (plC_ScreenPage_Main.PageText == "主畫面") PLC_Device_刷新音效.Bool = true;
            if (cnt_Program_刷新音效 == 65534)
            {
                this.MyTimer_刷新音效_結束延遲.StartTickTime(500);
                PLC_Device_刷新音效.SetComment("PLC_刷新音效");
                PLC_Device_刷新音效_OK.SetComment("PLC_刷新音效_OK");
                PLC_Device_刷新音效.Bool = false;
                cnt_Program_刷新音效 = 65535;
            }
            if (cnt_Program_刷新音效 == 65535) cnt_Program_刷新音效 = 1;
            if (cnt_Program_刷新音效 == 1) cnt_Program_刷新音效_檢查按下(ref cnt_Program_刷新音效);
            if (cnt_Program_刷新音效 == 2) cnt_Program_刷新音效_初始化(ref cnt_Program_刷新音效);
            if (cnt_Program_刷新音效 == 3) cnt_Program_刷新音效 = 65500;
            if (cnt_Program_刷新音效 > 1) cnt_Program_刷新音效_檢查放開(ref cnt_Program_刷新音效);

            if (cnt_Program_刷新音效 == 65500)
            {
                this.MyTimer_刷新音效_結束延遲.TickStop();
                this.MyTimer_刷新音效_結束延遲.StartTickTime(500);
                PLC_Device_刷新音效.Bool = false;
                PLC_Device_刷新音效_OK.Bool = false;
                cnt_Program_刷新音效 = 65535;
            }
        }
        void cnt_Program_刷新音效_檢查按下(ref int cnt)
        {
            if (PLC_Device_刷新音效.Bool) cnt++;
        }
        void cnt_Program_刷新音效_檢查放開(ref int cnt)
        {
            if (!PLC_Device_刷新音效.Bool) cnt = 65500;
        }
        void cnt_Program_刷新音效_初始化(ref int cnt)
        {
            if (this.MyTimer_刷新音效_結束延遲.IsTimeOut())
            {
                if (Task_刷新音效 == null)
                {
                    Task_刷新音效 = new Task(new Action(delegate { PlC_RJ_Button_刷新音效_MouseDownEvent(null); }));
                }
                if (Task_刷新音效.Status == TaskStatus.RanToCompletion)
                {
                    Task_刷新音效 = new Task(new Action(delegate { PlC_RJ_Button_刷新音效_MouseDownEvent(null); }));
                }
                if (Task_刷新音效.Status == TaskStatus.Created)
                {
                    Task_刷新音效.Start();
                }
                cnt++;
            }
        }







        #endregion
        #region PLC_資料刷新
        PLC_Device PLC_Device_資料刷新 = new PLC_Device("");
        PLC_Device PLC_Device_資料刷新_OK = new PLC_Device("");
        Task Task_資料刷新;
        MyTimer MyTimer_資料刷新_結束延遲 = new MyTimer();
        int cnt_Program_資料刷新 = 65534;
        void sub_Program_資料刷新()
        {
            if (plC_ScreenPage_Main.PageText == "主畫面") PLC_Device_資料刷新.Bool = true;
            if (cnt_Program_資料刷新 == 65534)
            {
                this.MyTimer_資料刷新_結束延遲.StartTickTime(500);
                PLC_Device_資料刷新.SetComment("PLC_資料刷新");
                PLC_Device_資料刷新_OK.SetComment("PLC_資料刷新_OK");
                PLC_Device_資料刷新.Bool = false;
                cnt_Program_資料刷新 = 65535;
            }
            if (cnt_Program_資料刷新 == 65535) cnt_Program_資料刷新 = 1;
            if (cnt_Program_資料刷新 == 1) cnt_Program_資料刷新_檢查按下(ref cnt_Program_資料刷新);
            if (cnt_Program_資料刷新 == 2) cnt_Program_資料刷新_初始化(ref cnt_Program_資料刷新);
            if (cnt_Program_資料刷新 == 3) cnt_Program_資料刷新 = 65500;
            if (cnt_Program_資料刷新 > 1) cnt_Program_資料刷新_檢查放開(ref cnt_Program_資料刷新);

            if (cnt_Program_資料刷新 == 65500)
            {
                this.MyTimer_資料刷新_結束延遲.TickStop();
                this.MyTimer_資料刷新_結束延遲.StartTickTime(500);
                PLC_Device_資料刷新.Bool = false;
                PLC_Device_資料刷新_OK.Bool = false;
                cnt_Program_資料刷新 = 65535;
            }
        }
        void cnt_Program_資料刷新_檢查按下(ref int cnt)
        {
            if (PLC_Device_資料刷新.Bool) cnt++;
        }
        void cnt_Program_資料刷新_檢查放開(ref int cnt)
        {
            if (!PLC_Device_資料刷新.Bool) cnt = 65500;
        }
        void cnt_Program_資料刷新_初始化(ref int cnt)
        {
            if (this.MyTimer_資料刷新_結束延遲.IsTimeOut())
            {
                if (Task_資料刷新 == null)
                {
                    Task_資料刷新 = new Task(new Action(delegate { Function_主畫面_資料刷新(); }));
                }
                if (Task_資料刷新.Status == TaskStatus.RanToCompletion)
                {
                    Task_資料刷新 = new Task(new Action(delegate { Function_主畫面_資料刷新(); }));
                }
                if (Task_資料刷新.Status == TaskStatus.Created)
                {
                    Task_資料刷新.Start();
                }
                cnt++;
            }
        }







        #endregion
        public class Marquee
        {
            private static string text = "";
            private static Font font = new Font("微軟正黑體", 16);
            private static Color textColor = Color.Black;
            private static Color bgColor = Color.LightGray;
            private static int speed = 20;
            private static int width = 1920;
            private static int height = 40;
       
            private static int position_x = 0;

            public static string Text { get => text; set => text = value; }
            public static Font Font { get => font; set => font = value; }
            public static Color TextColor { get => textColor; set => textColor = value; }
            public static Color BgColor { get => bgColor; set => bgColor = value; }
            public static int Speed { get => speed; set => speed = value; }
            public static int Width { get => width; set => width = value; }
            public static int Height { get => height; set => height = value; }

            public static void SetConfig(string text, Font font, Color textColor, Color bgColor, int speed, int width, int height)
            {
                Text = text;
                Font = font;
                TextColor = textColor;
                BgColor = bgColor;
                Speed = speed;
                Width = width;
                Height = height;
            }
            public static Bitmap GetBitmap()
            {
                Bitmap bitmap = new Bitmap(Width, Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                graphics.Clear(BgColor);
                SizeF textSize = graphics.MeasureString(Text, Font);

                if (position_x + textSize.Width < 0)
                {
                    position_x = Width;
                }

                int y = (int)((Height - textSize.Height) / 2);
                graphics.Clear(BgColor);
                graphics.DrawString(Text, Font, new SolidBrush(TextColor), position_x, y);


                position_x -= Speed; // 調整速度

             

                graphics.Dispose();

                return bitmap;
            }
        }
       
    }
}
