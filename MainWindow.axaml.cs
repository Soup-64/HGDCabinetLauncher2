using Avalonia.Collections;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Controls;
using System;
using Avalonia.Platform.Storage;

namespace HGDCabinetLauncher
{
    public partial class MainWindow : Window
    {
        private readonly AvaloniaList<ListBoxItem> _uiList = []; //list for populating the ui with

        //instance of the GameFinder class for indexing metafiles and running detected games
        private readonly GameFinder _finder = new();

        public MainWindow()
        {
            this.InitializeComponent();
#if !DEBUG
         this.WindowState = WindowState.FullScreen;
#endif
        }

        //update interface when new item is selected
        private void UiList_OnSelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            //fix crash condition if selection is changed before the control is done loading
            //also don't do anything if the list is empty
            if (uiList is null || uiList.SelectedIndex == -1) return;           

            name.Text = _finder.GameList[uiList.SelectedIndex].Name;
            desc.Text = "Description:\n" + _finder.GameList[uiList.SelectedIndex].Desc;
            ver.Text = "Version:\n" + _finder.GameList[uiList.SelectedIndex].Version;
            authors.Text = "Author(s):\n" + _finder.GameList[uiList.SelectedIndex].Authors;
            //set data for link opening stuff via click event
            qrImage.Tag = _finder.GameList[uiList.SelectedIndex].Link;

            //keep tabs on functions like setting images and generating qr codes that may fail

            this.gameImg.Source = _finder.GameList[uiList.SelectedIndex].gameImage;
            this.qrImage.Source = _finder.GameList[uiList.SelectedIndex].qrImage;
        }

        //build new avalonia list for listbox once it's loaded in
        private void UiList_OnLoaded(object? sender, RoutedEventArgs? e)
        {
            Console.WriteLine("loaded...");
            foreach (var gameData in _finder.GameList)
            {
                _uiList.Add(new() { Content = gameData.Name });
            }

            //uiList.items = _uiList;
            uiList.ItemsSource = _uiList;
            uiList.SelectedIndex = 0; //forces a selection so focus is on the listbox
            //force selection on startup because these run in a different order for some awful reason
            UiList_OnSelectionChanged(null, null);
        }

        //play the game
        private void ButtonPlay(object? sender, RoutedEventArgs e)
        {
            if (_finder.GetRunning()) return;
            _finder.PlayGame(uiList.SelectedIndex, this);
        }

        //handling input for moving along the list of games in the interface
        private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (_finder.GetRunning()) return;
            //because I hate how this toolkit handles
            //focus since focusing the listbox is not the same as focusing the listbox items
            switch (e.Key)
            {
                case Key.W:
                    if (uiList.SelectedIndex > 0)
                    {
                        this.uiList.SelectedIndex -= 1;
                    }
                    break;
                case Key.S:
                    if (uiList.SelectedIndex < uiList.ItemCount - 1)
                    {
                        this.uiList.SelectedIndex += 1;
                    }
                    break;
                default:
                    //only default to any key if no modifiers are pressed
                    if (e.Key is >= Key.A and <= Key.Z)
                    {
                        _finder.PlayGame(uiList.SelectedIndex, this);
                        //this.WindowState = WindowState.Minimized;
                    }
                    break;
            }
        }
    }
}