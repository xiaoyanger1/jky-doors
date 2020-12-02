using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using text.doors.Default;

namespace text.doors.Detection
{
    public partial class TakePhotos : Form
    {
        private static Young.Core.Logger.ILog Logger = Young.Core.Logger.LoggerManager.Current();
        private string _code = "";
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        public TakePhotos(string code)
        {
            _code = code;
            InitializeComponent();
            try
            {
                // 枚举所有视频输入设备
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                foreach (FilterInfo device in videoDevices)
                {
                    tscbxCameras.Items.Add(device.Name);
                }

                tscbxCameras.SelectedIndex = 0;

            }
            catch (ApplicationException)
            {
                tscbxCameras.Items.Add("No local capture devices");
                videoDevices = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[tscbxCameras.SelectedIndex].MonikerString);
            videoSource.DesiredFrameSize = new System.Drawing.Size(250, 150);
            videoSource.DesiredFrameRate = 1;
            videoSource.VideoResolution = videoSource.VideoCapabilities[1];
            videoSourcePlayer.VideoSource = videoSource;
            videoSourcePlayer.Width = 500;
            videoSourcePlayer.Height = 300;
            videoSourcePlayer.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            videoSourcePlayer.SignalToStop();
            videoSourcePlayer.WaitForStop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (videoSourcePlayer.IsRunning)
                {
                    BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                    videoSourcePlayer.GetCurrentVideoFrame().GetHbitmap(),
                                    IntPtr.Zero,
                                     Int32Rect.Empty,
                                    BitmapSizeOptions.FromEmptyOptions());
                    PngBitmapEncoder pE = new PngBitmapEncoder();
                    pE.Frames.Add(BitmapFrame.Create(bitmapSource));
                    string picName = GetImagePath() + "\\" + _code + DateTime.Now.ToString("MMddhhmm") + ".jpg";

                    DefaultBase.ImagesName = picName;
                    if (File.Exists(picName))
                    {
                        File.Delete(picName);
                    }
                    using (Stream stream = File.Create(picName))
                    {
                        pE.Save(stream);
                    }
                    MessageBox.Show("照片保存至PersonImg文件夹下！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    //拍照完成后关摄像头并刷新同时关窗体
                    if (videoSourcePlayer != null && videoSourcePlayer.IsRunning)
                    {
                        videoSourcePlayer.SignalToStop();
                        videoSourcePlayer.WaitForStop();
                    }
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("摄像头异常！" + ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                Logger.Error(ex);
            }
        }


        private string GetImagePath()
        {
            string personImgPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)
                         + Path.DirectorySeparatorChar.ToString() + "PersonImg";
            if (!Directory.Exists(personImgPath))
            {
                Directory.CreateDirectory(personImgPath);
            }

            return personImgPath;
        }
    }
}
