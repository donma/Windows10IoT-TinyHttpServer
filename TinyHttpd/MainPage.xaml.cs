using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TinyHttpd
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btnReadHtml_Click(object sender, RoutedEventArgs e)
        {
            string file = @"Assets\html\no2cloud.com.html";
            var c=System.IO.File.ReadAllText(file);

            Windows.UI.Popups.MessageDialog a = new Windows.UI.Popups.MessageDialog(c);

            a.ShowAsync();

        }

        private async void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            TinyHttpdServer ths=new TinyHttpdServer(5978);
            ths.Start();
       

        }
    }
}
