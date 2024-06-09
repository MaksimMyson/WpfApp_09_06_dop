using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_09_06_dop
{
    public partial class MainWindow : Window
    {
        private static Mutex mutex = new Mutex();
        private static bool isFirstThreadFinished = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Thread firstThread = new Thread(PrintAscending);
            Thread secondThread = new Thread(PrintDescending);

            firstThread.Start();
            secondThread.Start();
        }

        private void PrintAscending()
        {
            mutex.WaitOne();
            try
            {
                for (int i = 0; i <= 20; i++)
                {
                    Dispatcher.Invoke(() => AscendingTextBlock.Text += i + "\n");
                    Thread.Sleep(100); // Для демонстрації роботи потоків
                }
                isFirstThreadFinished = true;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void PrintDescending()
        {
            while (!isFirstThreadFinished)
            {
                Thread.Sleep(100); // Очікування завершення першого потоку
            }

            mutex.WaitOne();
            try
            {
                for (int i = 10; i >= 0; i--)
                {
                    Dispatcher.Invoke(() => DescendingTextBlock.Text += i + "\n");
                    Thread.Sleep(100); // Для демонстрації роботи потоків
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}