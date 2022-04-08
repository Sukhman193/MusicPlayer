using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
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
        List<StorageFile> listofitems;
        Button goBackButton;
        Button button1;
        Button addItem;
        StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;      // Put this with the other function
        IReadOnlyList<StorageFile> files;


        public MainPage()
        {
            this.InitializeComponent();
            retrieveData();
            listofitems = new List<StorageFile>();
        }

        

        // Pick the music library using FileOpenPicker();
        private async void PickFilesButton_Click(object sender, RoutedEventArgs e)
        {
            
            FileOpenPicker openPicker = new FileOpenPicker();
            List<StorageFile> temporaryArr = new List<StorageFile>();
            
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            openPicker.FileTypeFilter.Add(".mp3");
            files = await openPicker.PickMultipleFilesAsync();
            
            if(files.Count > 0)
            {
                StringBuilder output = new StringBuilder("Picked Files:\n");
                foreach (StorageFile file in files)
                {
                    temporaryArr.Add(file);
                    String temp = file.Name;
                    temp = temp.ToString().Replace(".mp3", "");
                    output.Append(temp + "\n");
                    OutputList.Items.Add(temp);


                    listofitems.Add(file);
                }
                
                
            }
            
        }

        // Play the music
        private void listBoxSongs_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            int ind = OutputList.SelectedIndex;
            String n = OutputList.SelectedItem.ToString();
            /*for (int i = 0; i < listofitems.Count; i++)
            {
                if (listofitems[i].Equals(n))
                {
                    musicPlayerElement.Source = MediaSource.CreateFromStorageFile(listofitems[i]);
                }
            }*/
            musicPlayerElement.Source = MediaSource.CreateFromStorageFile(listofitems[ind]);




        }

       
        // This function will retrieve the data from the application at startup
        private async void retrieveData()
        {
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

        //Hex COlor
        public SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            return myBrush;
        }
        private void color_mode_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
            if(toggleSwitch != null)
            {
                if(toggleSwitch.IsOn == true)
                {
                    menuStackPanel.Children.Clear();
                    helloSukh.Background = GetSolidColorBrush("#FF0E1314");
                    menuGrid.Background = GetSolidColorBrush("#FF050401");
                    OutputList.Background = GetSolidColorBrush("#FF102521");
                    color_mode.Background = GetSolidColorBrush("#FF0E1314");
                    musicPlayerElement.Style = Application.Current.Resources["poster2"] as Style;
                    PickFilesButton.Style = Application.Current.Resources["submitButton2"] as Style;
                    color_mode.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                    putLinkHere.Foreground = new SolidColorBrush(Windows.UI.Colors.White);



                    createButtons2();
                }
                else
                {
                    menuStackPanel.Children.Clear();
                    helloSukh.Background = GetSolidColorBrush("#FFFDF0D5");
                    menuGrid.Background = new SolidColorBrush(Windows.UI.Colors.CornflowerBlue);
                    OutputList.Background = new SolidColorBrush(Windows.UI.Colors.White);
                    color_mode.Background = GetSolidColorBrush("#FFFDF0D5");
                    musicPlayerElement.Style = Application.Current.Resources["poster1"] as Style;
                    PickFilesButton.Style = Application.Current.Resources["submitButton"] as Style;
                    color_mode.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                    putLinkHere.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);

                    createButtons();
                }
            }
            
           

            /*var arr = menuStackPanel.Children.ToArray();
            for(int i = 0; i < arr.Length; i++)
            {
                
            }*/
        }
        private void createButtons2()
        {
            goBackButton = new Button
            {
                Content = "",
                Name = "goBack",
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                IsEnabled = false,
                Style = Application.Current.Resources["menuButtons2"] as Style,
            };
            goBackButton.Click += GoBackButtonClick;
            menuStackPanel.Children.Add(goBackButton);


            button1 = new Button
            {
                Content = "Music",
                Name = "Music",
                Style = Application.Current.Resources["menuButtons2"] as Style
            };
            button1.Click += Music_Click;
            menuStackPanel.Children.Add(button1);


            foreach (Links link in webViewLinks)
            {
                Button btn = new Button
                {
                    Content = link.GetTitle(),
                    Style = Application.Current.Resources["menuButtons2"] as Style
                };
                btn.Click += WebViewButtonClick;
                btn.RightTapped += WebViewButton_RightTapped;
                btn.HorizontalAlignment = HorizontalAlignment.Center;
                menuStackPanel.Children.Add((btn));
            }

            addItem = new Button
            {
                Content = "+",
                FontSize = 25,
                VerticalAlignment = VerticalAlignment.Bottom,
                Style = Application.Current.Resources["menuButtons2"] as Style
            };
            addItem.Click += AddItem_Click;
            menuGrid.Children.Add(addItem);

        }


        // The following function will create the necessary buttons from the data retrieved
        private void createButtons()
        {
            goBackButton = new Button
            {
                Content = "",
                Name = "goBack",
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                IsEnabled = false,
                Style = Application.Current.Resources["menuButtons"] as Style,
            };
            goBackButton.Click += GoBackButtonClick;
            menuStackPanel.Children.Add(goBackButton);

            
            button1 = new Button
            {
                Content = "Music",
                Name = "Music",
                Style = Application.Current.Resources["menuButtons"] as Style
            };
            button1.Click += Music_Click;
            menuStackPanel.Children.Add(button1);


            foreach (Links link in webViewLinks)
            {
                Button btn = new Button
                {
                    Content = link.GetTitle(),
                    Style = Application.Current.Resources["menuButtons"] as Style
                };
                btn.Click += WebViewButtonClick;
                btn.RightTapped += WebViewButton_RightTapped;
                btn.HorizontalAlignment = HorizontalAlignment.Center;
                menuStackPanel.Children.Add((btn));
            }

            addItem = new Button
            {
                Content = "+",
                FontSize = 25,
                VerticalAlignment = VerticalAlignment.Bottom,
                Style = Application.Current.Resources["menuButtons"] as Style
            };
            addItem.Click += AddItem_Click;
            menuGrid.Children.Add(addItem);
            
        }

        private void Music_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
            musicPanel.Visibility = Visibility.Visible;
            myWebView.Visibility = Visibility.Collapsed;
            addItemPanel.Visibility = Visibility.Collapsed;
            webViewSettings.Visibility = Visibility.Collapsed;
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
            webViewSettings.Children.Clear();
            musicPanel.Visibility = Visibility.Collapsed;
            webViewSettings.Visibility = Visibility.Collapsed;
            addItemPanel.Visibility = Visibility.Collapsed;
            myWebView.Visibility = Visibility.Visible;

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


        // This function will trigger when a webView Page starts loading
        private void myWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;
            myWebView.Visibility = Visibility.Collapsed;
            LoadingStoryBoard.Begin();
            LoadingStoryBoard.RepeatBehavior = Windows.UI.Xaml.Media.Animation.RepeatBehavior.Forever;

        }



        // This function will trigger when a webView Page ends loading
        private void myWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            webViewSettings.Children.Clear();
            ErrorMessage.Visibility = Visibility.Collapsed;
            webViewSettings.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Collapsed;
            LoadingStoryBoard.Stop();
            myWebView.Visibility = Visibility.Visible;

            // Check if the webView can go back
            if (myWebView.CanGoBack)
            {
                goBackButton.IsEnabled = true;
            }
        }


        // This function will trigger when the user decides to create a new object
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            titleInput.Text = "";
            linkInput.Text = "";
            webViewSettings.Children.Clear();
            ErrorMessage.Visibility = Visibility.Collapsed;
            musicPanel.Visibility = Visibility.Collapsed;
            webViewSettings.Visibility = Visibility.Collapsed;
            myWebView.Visibility = Visibility.Collapsed;
            addItemPanel.Visibility = Visibility.Visible;
        }


        private async void saveData(string title, string htmlLink)
        {
            string fileData = title + "/-}/" + htmlLink + "\n";
            StorageFile myFile = await localFolder.CreateFileAsync("webViewLinks.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(myFile, fileData);
        }



        // This function will trigger when the user clicks on the Create new link button
        private async void Create_Click(object sender, RoutedEventArgs e)
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

                ContentDialog contentDialog = new ContentDialog
                {
                    Title = "Title Unavailable ",
                    Content = "Cannot use the title entered",
                    CloseButtonText = "Okay"
                };
                await contentDialog.ShowAsync();
                return;
            }

            if (!_htmlLink.StartsWith("http"))
            {
                // crate dialog box to say that we cannot use this link, should start with http
                ContentDialog contentDialog = new ContentDialog
                {
                    Title = "Url not recognized",
                    Content = "Please make sure the url starts with http",
                    CloseButtonText = "Okay"
                };
                await contentDialog.ShowAsync();
                return;
            }

            saveData(_title, _htmlLink);
            Links newLink = new Links(_title, _htmlLink);
            webViewLinks.Add(newLink);
            if(color_mode.IsOn == true){
                Button btn = new Button
                {
                    Content = _title,

                    Style = Application.Current.Resources["menuButtons2"] as Style,
                };
                btn.Click += WebViewButtonClick;
                btn.IsRightTapEnabled = true;
                btn.RightTapped += WebViewButton_RightTapped;
                menuStackPanel.Children.Add((btn));
            }
            else
            {
                Button btn = new Button
                {
                    Content = _title,

                    Style = Application.Current.Resources["menuButtons"] as Style,
                };
                btn.Click += WebViewButtonClick;
                btn.IsRightTapEnabled = true;
                btn.RightTapped += WebViewButton_RightTapped;
                menuStackPanel.Children.Add((btn));
            }
            

        }



        private void WebViewButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Hide any open page
            webViewSettings.Children.Clear();
            ErrorMessage.Visibility = Visibility.Collapsed;
            musicPanel.Visibility = Visibility.Collapsed; 
            myWebView.Visibility = Visibility.Collapsed;
            addItemPanel.Visibility = Visibility.Collapsed;
            webViewSettings.Visibility = Visibility.Visible;


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
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                FontFamily = new FontFamily("Segoe Print"),
                FontSize = 30,
                Margin = new Thickness { Bottom = 30 },
            };

            TextBox titleTextBox = new TextBox()
            {
                Text = title,
                IsEnabled = false,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                Margin = new Thickness { Bottom = 30, Top = 30 },
                FontSize = 30,
                FontFamily = new FontFamily("Segoe Print"),
            };

            TextBox urlTextBox = new TextBox()
            {
                Text = urlLink,
                PlaceholderText = "Enter the url",
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                Margin = new Thickness { Bottom = 30, Top = 30 },
                FontFamily = new FontFamily("Segoe Print"),
                FontSize = 30,

            };

            Button deleteButton = new Button()
            {
                Content = "Delete",
                Background = new SolidColorBrush(Windows.UI.Colors.Red),
                Margin = new Thickness { Bottom = 30, Top = 30 },
                Style = Application.Current.Resources["submitButton"] as Style,
            };
            deleteButton.Click += DeleteButton_Click;

            Button makeChanges = new Button()
            {
                Content = "Submit Changes",
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                Style = Application.Current.Resources["submitButton"] as Style,
            };
            makeChanges.Click += MakeChanges_Click;
            
            
            webViewSettings.Children.Add(textBlock);
            webViewSettings.Children.Add(titleTextBox);
            webViewSettings.Children.Add(urlTextBox);
            webViewSettings.Children.Add(deleteButton);
            webViewSettings.Children.Add(makeChanges);
        }


        private async void MakeChanges_Click(object sender, RoutedEventArgs e)
        {

            Button button = (Button)sender;
            StackPanel stackPanel = (StackPanel)button.Parent;
            TextBox titleTextBox = (TextBox)stackPanel.Children[1];
            TextBox urlTextBox = (TextBox)(stackPanel.Children[2]);

            string title = titleTextBox.Text;
            string url = urlTextBox.Text;

            webViewLinks.Remove(webViewLinks.Find(l => l.GetTitle() == title));
            
            Links newalink = new Links(title, url);
            webViewLinks.Add(newalink);

            StorageFile myFile = await localFolder.CreateFileAsync("webViewLinks.txt", CreationCollisionOption.ReplaceExisting);

            foreach (Links links in webViewLinks)
            {
                saveData(links.GetTitle(), links.GetHtmlLink());
            }
        }



        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StackPanel stackPanel = (StackPanel)button.Parent;
            TextBox titleTextBox = (TextBox)stackPanel.Children[1];
            TextBox urlTextBox = (TextBox)(stackPanel.Children[2]);

            string title = titleTextBox.Text; 
            string url = urlTextBox.Text;


            webViewLinks.Remove(webViewLinks.Find(l => l.GetTitle() == title));
            
            StorageFile myFile = await localFolder.CreateFileAsync("webViewLinks.txt", CreationCollisionOption.ReplaceExisting);

            foreach(Links links in webViewLinks)
            {
                saveData(links.GetTitle(), links.GetHtmlLink());
            }

            webViewSettings.Visibility = Visibility.Collapsed;
            for (int i = 0; i < menuStackPanel.Children.Count; i++)
            {
                Button btn = (Button)menuStackPanel.Children[i];
                if(btn.Content.ToString() == title)
                {
                    menuStackPanel.Children.RemoveAt(i);
                }
            }
            
        }


        private void titleInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (linkInput.Text == "")
            {
                linkInput.Text = "https://www." + titleInput.Text + ".com/";
            }
        }

        private void myWebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Visible;
            ErrorMessage1.Begin();
            ErrorMessage1.RepeatBehavior = Windows.UI.Xaml.Media.Animation.RepeatBehavior.Forever;
            myWebView.Visibility = Visibility.Collapsed;
        }

        
    }
}
