using System;
using System.IO;
using System.Net;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace PhotonColor
{
    public sealed partial class MainPage : Page
    {
        const string PHOTONDEVICEID = "390032000447343337373738";
        const string ACCESS_TOKEN = "b7b20dda00c7f11e1638a0fc6ee1519ee162f3f9";
        DispatcherTimer timer;
        SolidColorBrush currentColor;
        public MainPage()
        {
            this.InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object sender, object e)
        {
            if (WriteColor())
            {
                timer.Stop();
            }
        }
        private void ColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            if (!timer.IsEnabled)
            {
                WriteColor();
                timer.Start();
            }
        }
        private bool WriteColor()
        {
            SetRGB(colorPicker.SelectedColor);
            bool result = currentColor == colorPicker.SelectedColor;
            currentColor = colorPicker.SelectedColor;
            return result;
        }
        private async void SetRGB(SolidColorBrush rgb)
        {
            string url = String.Format("https://api.particle.io/v1/devices/{0}/setRGB?access_token={1}", PHOTONDEVICEID, ACCESS_TOKEN);
            var request = WebRequest.Create(url);
            var postData = "value=" + string.Format("{0},{1},{2}", rgb.Color.R, rgb.Color.G, rgb.Color.B);
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (var stream = await request.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }
            try
            {
                var response = await request.GetResponseAsync();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch { }
        }
    }
}
