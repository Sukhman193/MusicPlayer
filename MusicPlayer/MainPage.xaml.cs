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
        Button goBackButton;

        public MainPage()
        {
            this.InitializeComponent();
            retrieveData();
        }

        // This function will retrieve the data from the application at startup
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
        // The following function will create the necessary buttons from the data retrieved
        private void createButtons()
        {
            goBackButton = new Button
            {
                Content = "<-",
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Background = new SolidColorBrush(Windows.UI.Colors.Beige),
                Name = "goBack",
                Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom = 5 },
                Padding = new Thickness { Left = 10, Right = 10, Bottom = 10, Top = 10 },
                FocusVisualPrimaryBrush = new SolidColorBrush(Windows.UI.Colors.White),
                FocusVisualSecondaryBrush = new SolidColorBrush(Windows.UI.Colors.Blue),
                HorizontalAlignment = HorizontalAlignment.Center,
                IsEnabled = false
            };
            goBackButton.Click += GoBackButtonClick;
            menuStackPanel.Children.Add(goBackButton);


            Button button1 = new Button
            {
                Content = "Music",
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Background = new SolidColorBrush(Windows.UI.Colors.Beige),
                Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom= 5 },
                Padding = new Thickness { Left = 10, Right = 10, Bottom = 10, Top = 10 },
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
                    Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom = 5 },
                    Padding = new Thickness { Left = 10, Right = 10, Bottom = 10, Top = 10},
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
                FontSize = 30,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                Background = new SolidColorBrush(Windows.UI.Colors.DarkKhaki),
                Margin = new Thickness { Bottom = 5 },
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            addItem.Click += AddItem_Click;
            menuGrid.Children.Add(addItem);
            
        }


        // This function will trigger when the user clicks on the go back button
        private void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
            myWebView.GoBack();
            if (!myWebView.CanGoBack) 
            {
                goBackButton.IsEnabled = false; 
            }
        }

        // this function will trigger when the user selectrs a page to open
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

            addItemPanel.Visibility = Visibility.Collapsed;
            myWebView.Navigate(new Uri(htmlLink));
        }


        // This function will trigger when a webView Page starts loading
        private void myWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            progressRingWebView.Visibility = Visibility.Visible;
            myWebView.Visibility = Visibility.Collapsed;
            progressRingWebView.IsActive = true;
        }

        // This function will trigger when a webView Page ends loading
        private void myWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            progressRingWebView.Visibility = Visibility.Collapsed;
            myWebView.Visibility = Visibility.Visible;
            progressRingWebView.IsActive = false;

            // Check if the webView can go back
            if (myWebView.CanGoBack)
            {
                goBackButton.IsEnabled = true;
            }
        }


        // This function will trigger when the user decides to create a new object
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            myWebView.Visibility = Visibility.Collapsed;
            addItemPanel.Visibility = Visibility.Visible;
        }
        private async void saveData(string title, string htmlLink)
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;      // Put this with the other function
            string fileData = "\n" + title + "/-}/" + htmlLink;
            StorageFile myFile = await localFolder.CreateFileAsync("webViewLinks.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(myFile, fileData);
        }

        // This function will trigger when the user clicks on the Create new link button
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string _title = titleInput.Text;
            string _htmlLink = linkInput.Text;
            List<string> _titleList = new List<string>();

            foreach(Links links in webViewLinks)
            {
                _titleList.Add(links.GetTitle());
            }


            if(_title.Trim() == "" || _titleList.Contains(_title)){
                // create dialog box to say that we cannot use this title
                return;
            }

            if (!_htmlLink.StartsWith("http"))
            {
                // crate dialog box to say that we cannot use this link, should start with http
                return;
            }

            saveData(_title, _htmlLink);
            Links newLink = new Links(_title, _htmlLink);
            webViewLinks.Add(newLink);
            Button btn = new Button
            {
                Content = _title,
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Margin = new Thickness { Left = 0, Top = 5, Right = 0, Bottom = 5 },
                Padding = new Thickness { Left = 10, Right = 10, Bottom = 10, Top = 10 },
                Background = new SolidColorBrush(Windows.UI.Colors.Beige),
                FocusVisualPrimaryBrush = new SolidColorBrush(Windows.UI.Colors.White)
            };
            btn.Click += WebViewButtonClick;
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            menuStackPanel.Children.Add((btn));


        }

        private void WebViewButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Button webViewSelected = (Button)sender;

            string title = webViewSelected.Content.ToString();

            string urlLink = "";

            foreach(Links links in webViewLinks)
            {
                if(links.GetTitle() == title)
                {
                    urlLink = links.GetHtmlLink();
                }
                   
            }

            TextBlock textBlock = new TextBlock()
            {
                Text = "Edit the item",
                HorizontalAlignment =Windows.UI.Xaml.HorizontalAlignment.Center,
                FontFamily = new FontFamily("Cascadia Mono"),
                FontSize = 30,
                Margin = new Thickness { Bottom = 30 },
            };

            TextBox titleTextBox = new TextBox()
            {
                Text = title,
                PlaceholderText = "Enter the name of the title",
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                Margin = new Thickness { Bottom = 30, Top = 30 },
                FontSize = 30,
                FontFamily = new FontFamily("Cascadia Mono"),
            };

            TextBox urlTextBox = new TextBox()
            {
                Text = urlLink,
                PlaceholderText = "Enter the url",
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                Margin = new Thickness { Bottom = 30, Top = 30 },
                FontFamily = new FontFamily("Cascadia Mono"),
                FontSize = 30,

            };

            Button deleteButton = new Button()
            {
                Content = "Delete",
                Background = new SolidColorBrush(Windows.UI.Colors.Red),
            };

            Button makeChanges = new Button()
            {
                Content = "Submit Changes",

            };
            
            


            webViewSettings.Children.Add(textBlock);
            webViewSettings.Children.Add(titleTextBox);
            webViewSettings.Children.Add(urlTextBox);

        }
    }
}
