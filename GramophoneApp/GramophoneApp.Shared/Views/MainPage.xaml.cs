using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GramophoneApp.Views
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

        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var pin = await ExtractPin(sender as WebView);

            if (!string.IsNullOrEmpty(pin))
            {
                Messenger.Default.Send<string>(pin);
            }
        }

        private async Task<string> ExtractPin(WebView webView)
        {
            if (webView != null)
            {
                string html = await webView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });

                if (!string.IsNullOrWhiteSpace(html))
                {
                    string startNeedle = "auth_success_verify_code";
                    string endNeedle = "</pre>";
                    int startIndex = html.IndexOf(startNeedle);
                    if (startIndex > 0)
                    {
                        int endIndex = html.IndexOf(endNeedle, startIndex);
                        int startSubstring = startIndex + startNeedle.Length + 2;

                        var pin = html.Substring(startSubstring, endIndex - startSubstring);

                        if (!string.IsNullOrWhiteSpace(pin))
                        {
                            return pin;
                        }
                    }
                }
            }

            return null;
        }
    }
}
