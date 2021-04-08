using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace OperativniSistemiApp
{
    public sealed partial class SecondExample : Page
    {
        const int NumValues = 5;
        StackPanel[] panels;
        int[] values;
        SemaphoreSlim[] semaphores = new SemaphoreSlim[]
        {
            new SemaphoreSlim(1),
            new SemaphoreSlim(1),
            new SemaphoreSlim(1)
        };
        //SemaphoreSlim semaphore = new SemaphoreSlim(1);

        public SecondExample()
        {
            this.InitializeComponent();
            panels = new StackPanel[] { panel1, panel2, panel3 };
            values = new int[panels.Length];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < NumValues; ++i)
            {
                /*await semaphore.WaitAsync();
                try
                {*/
                //for (int j = 0; j < panels.Length; ++j)
                Parallel.For(0, panels.Length, async j =>
                {
                    await semaphores[j].WaitAsync();
                    try
                    {
                        int value = values[j];
                        //await Task.Delay(500);
                        await Task.Run(() => LongExecution());
                        values[j] = ++value;
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            panels[j].Children.Add(new TextBlock() { Text = values[j].ToString() })
                        );
                    }
                    finally
                    {
                        semaphores[j].Release();
                    }
                });
                /*}
                finally
                {
                    semaphore.Release();
                }*/
            }
        }

        private void LongExecution()
        {
            Task.Delay(500).GetAwaiter().GetResult();
        }
    }
}
