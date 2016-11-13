﻿using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Boxify
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        /// <summary>
        /// The page that allows the user to log in
        /// </summary>
        public LoginPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
        }

        /// <summary>
        /// When the user navigates to this page
        /// </summary>
        /// <param name="e">The navigation event arguments</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Back button in title bar
            Frame rootFrame = Window.Current.Content as Frame;

            string myPages = "";
            foreach (PageStackEntry page in rootFrame.BackStack)
            {
                myPages += page.SourcePageType.ToString() + "\n";
            }

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            
            this.webView.Navigate(RequestHandler.getAuthorizationUri());
        }

        /// <summary>
        /// When the webview loads a new page
        /// </summary>
        /// <param name="sender">The webview browser</param>
        /// <param name="args">The webview navigation starting event arguments</param>
        private async void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.AbsoluteUri.StartsWith(RequestHandler.callbackUrl))
            {
                this.webView.Visibility = Visibility.Collapsed;
                WwwFormUrlDecoder queryParams = new WwwFormUrlDecoder(args.Uri.Query);
                try
                {
                    await RequestHandler.getTokens(queryParams.GetFirstValueByName("code"));
                    this.Frame.Navigate(typeof(MainPage), null);
                }
                catch (ArgumentException)
                {

                }

            }
        }

        /// <summary>
        /// User navigates back from this page
        /// </summary>
        /// <param name="sender">The login page</param>
        /// <param name="e">The back requested event arguments</param>
        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }
    }
}
