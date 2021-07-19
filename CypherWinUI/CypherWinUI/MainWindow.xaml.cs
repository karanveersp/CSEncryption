using CypherWinUI.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CypherWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>
        {
            { "notes", typeof(NotesPage) },
            { "pastebin", typeof(PastebinPage) },
            { "settings", typeof(SettingsPage) }
        };

        /// <summary>
        /// Navigation item click event handler that initiates the page transition in the frame.
        /// </summary>
        private void TopNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                TopNav_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                TopNav_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="navItemTag"></param>
        /// <param name="transitionInfo"></param>
        private void TopNav_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            
            var _page = _pages.GetValueOrDefault(navItemTag);

            // Get the page type before navigation so you can prevent duplicate entries in
            // the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void TopNav_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationView navView = sender as NavigationView;
            navView.SelectedItem = _pages["pastebin"];
            ContentFrame.Navigate(_pages["pastebin"]);
        }
    }
}
