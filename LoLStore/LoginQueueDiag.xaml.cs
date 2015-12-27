using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RtmpSharp.IO;
using RtmpSharp.Messaging;
using RtmpSharp.Net;

namespace LoLStore
{
    /// <summary>
    /// Interaction logic for LoginQueueDiag.xaml
    /// </summary>
    public partial class LoginQueueDiag : Window
    {
        MainWindow parent;
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        internal string FailReason;
        public LoginQueueDiag(MainWindow parent)
        {
            this.parent = parent;
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null)
                e.Cancel = true;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServerInfo region = parent?.RegionCB.SelectedItem as ServerInfo;
            if (region == null) return;
            WebClient client = new WebClient();
            client.BaseAddress = region.LoginQueue;
            string payload = $"user={parent?.UsernameTB.Text},password={parent?.PasswordTB.Password}";
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            string response;
            Dictionary<string, object> robj = null;
            try
            {
                response = await client.UploadStringTaskAsync("login-queue/rest/queue/authenticate", "payload=" + HttpUtility.UrlEncode(payload));
                robj = serializer.Deserialize<Dictionary<string, object>>(response);
            }
            catch (WebException ex)
            {
                var str = ex.Response.GetResponseStream();
                byte[] b = new byte[str.Length];
                str.Read(b, 0, b.Length);
                response = System.Text.Encoding.UTF8.GetString(b);
                robj = serializer.Deserialize<Dictionary<string, object>>(response);
            }
            if ((string)robj["status"] == "FAILED")
            {
                FailReason = robj["reason"] as string;
                DialogResult = false;

                Close();
                return;
            }
            else if (!robj.ContainsKey("token"))
            {
                int node = (int)robj["node"];
                string champ = (string)robj["champ"];
                int rate = (int)robj["rate"];
                int delay = (int)robj["delay"];

                int id = 0;
                int current = 0;
                object[] tickers = (object[])robj["tickers"];
                foreach(object o in tickers)
                {
                    var ticker=o as Dictionary<string, object>;
                    if ((int)ticker["node"] == node)
                    {
                        id = (int)ticker["id"];
                        current = (int)ticker["current"];
                        break;
                    }
                }
                SubStatusBlock.Text = "Login Queue position: " + (id - current);
                while(id-current>rate)
                {
                    Thread.Sleep(delay);
                    response = await client.DownloadStringTaskAsync("login-queue/rest/queue/ticker/" + champ);
                    robj = serializer.Deserialize<Dictionary<string, object>>(response);
                    current = int.Parse((string)robj[node.ToString()]);
                    SubStatusBlock.Text = "Login Queue position: " + Math.Max(1, id - current);
                }
                do
                {
                    Thread.Sleep(100);
                    response = await client.DownloadStringTaskAsync("login-queue/rest/queue/authToken/" + parent.UsernameTB.Text.ToLowerInvariant());
                    robj = serializer.Deserialize<Dictionary<string, object>>(response);
                } while (!robj.ContainsKey("token"));
            }
            string authToken = (string)robj["token"];
            string ip = null;
            try
            {
                client.BaseAddress = "";
                response = await client.DownloadStringTaskAsync("http://ll.leagueoflegends.com/services/connection_info");
                robj = serializer.Deserialize<Dictionary<string, object>>(response);
                ip = robj["ip_address"] as string;
            }
            catch(WebException ex)
            {
                //ignore
                ip = null;
            }
            if (ip == null)
                ip = "127.0.0.1";
            AsObject obj = new AsObject("com.riotgames.platform.login.AuthenticationCredentials")
            {
                ["username"] = parent.UsernameTB.Text,
                ["password"] = parent.PasswordTB.Password,
                ["authToken"] = authToken,
                ["clientVersion"] = "5.24.asd",
                ["ipAddress"] = ip,
                ["locale"] = "en_US",
                ["domain"] = "lolclient.lol.riotgames.com",
                ["operatingSystem"] = "Wingdoze",
                ["securityAnswer"] = null,
                ["oldPassword"] = null,
                ["partnerCredentials"] = null,
            };
            RtmpClient rtmp = parent.RtmpClient = new RtmpClient(new Uri("rtmps://" + region.Server+":2099"), new SerializationContext(), ObjectEncoding.Amf3);
            AsObject result = await rtmp.ConnectAsync();
            try
            {
                result = await rtmp.InvokeAsync<AsObject>("my-rtmps", "loginService", "login", obj);
            }
            catch (InvocationException ex)
            {
                obj["clientVersion"] = ((ex.RootCause as AsObject)["substitutionArguments"] as object[])[1];
                result = await rtmp.InvokeAsync<AsObject>("my-rtmps", "loginService", "login", obj);
            }
            string stoken = (string)result["token"];
            await rtmp.LoginAsync(parent.UsernameTB.Text, stoken);

            DialogResult = true;
            Close();

        }
    }
}
