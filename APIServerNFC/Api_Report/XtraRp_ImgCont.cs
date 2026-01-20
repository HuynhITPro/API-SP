using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;

namespace APIServerNFC.Api_Report
{
    public partial class XtraRp_ImgCont : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraRp_ImgCont()
        {
            InitializeComponent();
        }
        private byte[] ResizeImageFromUrl(string url, int width = 200)
        {
            try
            {
                using var client = new HttpClient();
                var imageBytes = client.GetByteArrayAsync(url).Result;

                using var ms = new MemoryStream(imageBytes);
                using var image = System.Drawing.Image.FromStream(ms);

                // Giữ tỷ lệ gốc
                double ratio = (double)width / image.Width;
                int height = (int)(image.Height * ratio);

                using var resized = new System.Drawing.Bitmap(image, new Size(width, height));
                using var output = new MemoryStream();
                resized.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);
                return output.ToArray();
            }
            catch
            {
                return null;
            }
        }

        private void xrPictureBox1_BeforePrint(object sender, CancelEventArgs e)
        {
            var pictureBox = sender as XRPictureBox;
            //var url = GetCurrentColumnValue("UrlRoot")?.ToString();

            if (!string.IsNullOrEmpty(urlrootcrr))
            {
                var imgBytes = ResizeImageFromUrl(urlrootcrr, 250); // bạn có thể chỉnh lại width
                if (imgBytes != null)
                {
                    using var ms = new MemoryStream(imgBytes);
                    pictureBox.Image = System.Drawing.Image.FromStream(ms); // ✅ dùng Image
                   // pictureBox.ImageSource = DevExpress.XtraPrinting.Drawing.ImageSource.FromStream(ms);
                }
            }
        }
        string urlrootcrr = "";
        private void xrLabel2_BeforePrint(object sender, CancelEventArgs e)
        {
            urlrootcrr = xrLabel2.Text;
        }
    }
}
