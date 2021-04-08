using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using OperativniSistemiApp.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace OperativniSistemiApp
{
    public sealed partial class MainPage : Page
    {
        SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (resultText.Text == "Done.")
            {
                await semaphore.WaitAsync();
                try
                {
                    coolButton.IsEnabled = false;
                    string text = "Not done.";
                    await Task.Run(() =>
                    {
                        Task.Delay(5000).GetAwaiter().GetResult();
                        text = "Really not done.";
                    });
                    resultText.Text = text;
                    coolButton.IsEnabled = true;
                }
                finally
                {
                    semaphore?.Release();
                }
            }
            else
                resultText.Text = "Done.";
        }

        private async void BadButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                for (int i = 0; i <= 10; ++i)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => progressBar.Value = i);
                    Task.Delay(1000).GetAwaiter().GetResult();
                }
            });
        }

        private async void ListButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                CustomControl progressBar = null;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        () =>
                        {
                            progressBar = new CustomControl() { ProgressValue = 10, Margin = new Thickness(5) };
                            stackPanel.Children.Add(progressBar);
                        });
                for (int i = 0; i <= 10; ++i)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => progressBar.ProgressValue = i);
                    await Task.Delay(1000);
                }
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => stackPanel.Children.Remove(progressBar));
            });
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            downloadButton.IsEnabled = false;

            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads,
                SuggestedFileName = "New Document"
            };
            savePicker.FileTypeChoices.Add("HTML", new List<string>() { ".htm" });
            StorageFile storageFile = await savePicker.PickSaveFileAsync();
            if (storageFile == null)
            {
                downloadButton.IsEnabled = true;
                return;
            }

            progressRing.Visibility = Visibility.Visible;
            string html;
            using (HttpClient client = new HttpClient())
                html = await client.GetStringAsync(urlText.Text);
            progressRing.Visibility = Visibility.Collapsed;


            await FileIO.WriteTextAsync(storageFile, html);
            //File.WriteAllText("C:\\test.html", html, System.Text.Encoding.UTF8);
            MessageDialog dialog = new MessageDialog("Your file was saved.", "Success.");
            await dialog.ShowAsync();

            downloadButton.IsEnabled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SecondExample));
        }
    }
}
