using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Controls
{
    [TemplatePart(Name = "SearchButton", Type = typeof(Button))]
    public sealed class SearchTextBox : TextBox
    {
        public static readonly DependencyProperty SearchButtonCommandProperty = DependencyProperty.Register("SearchButtonCommand", typeof(ICommand), typeof(SearchTextBox), new PropertyMetadata(null, OnSearchButtonCommandChanged));

        private Button _searchButton;

        public ICommand SearchButtonCommand
        {
            get { return (ICommand)GetValue(SearchButtonCommandProperty); }
            set { SetValue(SearchButtonCommandProperty, value); }
        }

        public SearchTextBox()
        {
            this.DefaultStyleKey = typeof(SearchTextBox);
            this.KeyDown += SearchTextBox_KeyDown;
            this.GotFocus += SearchTextBox_GotFocus;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _searchButton = this.GetTemplateChild("SearchButton") as Button;

            if (_searchButton != null)
            {
                _searchButton.Click += button_Click;
            }
        }

        private static void OnSearchButtonCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (SearchTextBox)sender;
            self.SearchButtonCommand = (ICommand)e.NewValue;
        }

        private void SearchTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Search();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            this.Visibility = Visibility.Collapsed;

            SearchButtonCommand.Execute(null);

            this.Visibility = Visibility.Visible;
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.SelectAll();
        }
    }
}
