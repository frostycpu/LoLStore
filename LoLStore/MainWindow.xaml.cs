using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RtmpSharp.Net;

namespace LoLStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal RtmpClient RtmpClient;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if(!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            RegionCB.ItemsSource = ServerInfo.ALL;
            RegionCB.SelectedIndex = 0;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginQueueDiag diag = new LoginQueueDiag(this);
            diag.Top = Top + (Height - diag.Height) / 2;
            diag.Left = Left + (Width - diag.Width) / 2;
            bool? result=diag.ShowDialog();
            if (!result??false)
            {
                switch(diag.FailReason)
                {
                    case "invalid_credentials":
                        StatusBlock.Text = "Wrong username or password";
                        break;
                    default:
                        StatusBlock.Text = diag.FailReason;
                        break;
                }
            }
            else
            {
                LoginGrid.Visibility = Visibility.Hidden;
                MainGrid.Visibility = Visibility.Visible;
            }
        }

        private async void OpenStoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (RtmpClient == null) return;
            string link = await RtmpClient.InvokeAsync<string>("my-rtmps","loginService","getStoreUrl",new object[0]);
            System.Diagnostics.Process.Start(link);
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (RtmpClient != null && !RtmpClient.IsDisconnected)
                await RtmpClient.LogoutAsync();
        }
    }
}
