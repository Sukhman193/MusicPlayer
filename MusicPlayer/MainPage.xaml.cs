using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MusicPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<Links> webViewLinks;

        public MainPage()
        {
            this.InitializeComponent();
            retrieveData();
        }

        private async void retrieveData()
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile myFile = await localFolder.CreateFileAsync("webViewLinks.txt", CreationCollisionOption.OpenIfExists);
            IList<string> allLinkList = await FileIO.ReadLinesAsync(myFile);
            webViewLinks = new List<Links>();  
            foreach (string link in allLinkList)
            {
                string[] stringSplit = link.Split("/-}/");
                Links currentLink = new Links(stringSplit[0], stringSplit[1]);
                webViewLinks.Add(currentLink);
            }
            createButtons();
        }


        private void createButtons()
        {
            Button button = new Button();
            button.Content = "<-";
            button.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            button.Background = new SolidColorBrush(Windows.UI.Colors.Beige);
            button.Name = "goBack";
            button.FocusVisualPrimaryBrush = new SolidColorBrush(Windows.UI.Colors.White);
            button.FocusVisualSecondaryBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
            
            button.HorizontalAlignment = HorizontalAlignment.Center;
            menuStackPanel.Children.Add(button);

            Button button1 = new Button
            {
                Content = "Music",
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Background = new SolidColorBrush(Windows.UI.Colors.Beige),
                FocusVisualPrimaryBrush = new SolidColorBrush(Windows.UI.Colors.White),
                Name = "Music",
                HorizontalAlignment = HorizontalAlignment.Center
            };
            menuStackPanel.Children.Add(button1);

            foreach (Links link in webViewLinks)
            {
                Button btn = new Button
                {
                    Content = link.GetTitle(),
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                    Background = new SolidColorBrush(Windows.UI.Colors.Beige),
                    FocusVisualPrimaryBrush = new SolidColorBrush(Windows.UI.Colors.White)
                };
                btn.Click += WebViewButtonClick;
                btn.HorizontalAlignment = HorizontalAlignment.Center;
                menuStackPanel.Children.Add((btn));
            }

            Button addItem = new Button
            {
                Content = "+",
                FontSize = 40,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                Background = new SolidColorBrush(Windows.UI.Colors.DarkKhaki),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            menuStackPanel.Children.Add(addItem);
            
        }
        

        private void WebViewButtonClick(object sender, RoutedEventArgs e)
        {

            Button button = (Button)sender;
            string title = (string)button.Content;
            string htmlLink = "";

            foreach(Links link in webViewLinks)
            {
                if (title == link.GetTitle())
                    htmlLink = link.GetHtmlLink();
            }

            myWebView.Navigate(new Uri(htmlLink));
        }
    }
}
