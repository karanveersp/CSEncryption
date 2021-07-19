using CypherWinUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CypherWinUI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PastebinPage : Page
    {

        private PastebinViewModel viewModel { get; set; }

        public PastebinPage()
        {
            this.InitializeComponent();
            this.viewModel = new PastebinViewModel();
        }

        /// <summary>
        /// Event handler for toggled state. Sets the property 
        /// in the view model with the new state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            if (toggle != null)
            {
                viewModel.IsEncryptMode = toggle.IsOn;
            }
        }

        private void KeyInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.Key = KeyInput.Password;
        }


        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.Key))
            {
                KeyStatusText.Text = "Key cannot be empty";
                return;
            }
            else if (viewModel.Key.Length < 8)
            {
                KeyStatusText.Text = "Key must have 8 or more characters";
                return;
            }
            else if (string.IsNullOrEmpty(viewModel.TextContent))
            {
                KeyStatusText.Text = "No text found in input box";
                return;
            }

            if (viewModel.IsEncryptMode == true)
            {
                Output.Text = AESLib.AES.Encrypt(viewModel.TextContent, viewModel.Key)
                    .IfNone(() => "Encoding Error");
            }
            else
            {
                Output.Text = AESLib.AES.Decrypt(viewModel.TextContent, viewModel.Key)
                    .IfNone(() => "Decoding Error");
            }
            KeyStatusText.Text = string.Empty;

        }

        private async void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog confirm = new ContentDialog
            {
                Title = "Confirm",
                Content = "Clear all fields?",
                CloseButtonText = "No",
                PrimaryButtonText = "Yes",
                XamlRoot = Content.XamlRoot
            };

            ContentDialogResult result = await confirm.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // clear all fields.
                viewModel.Key = string.Empty;
                KeyInput.Password = string.Empty;
                viewModel.TextContent = string.Empty;
                Output.Text = string.Empty;
                ModeToggle.IsOn = true;
                KeyStatusText.Text = string.Empty;
            }
            else
            {
                // do nothing.
            }
        }
    }
}
