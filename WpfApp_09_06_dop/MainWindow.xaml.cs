using System.IO;
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
        private const string MutexName = "MyMutex";
        private const string FirstFileName = "random_numbers.txt";
        private const string SecondFileName = "prime_numbers.txt";
        private const string ThirdFileName = "prime_numbers_ending_with_7.txt";
        private const string ReportFileName = "report.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartProcessing_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => FirstThread());
            Task.Run(() => SecondThread());
            Task.Run(() => ThirdThread());
            Task.Run(() => FourthThread());
        }

        private void FirstThread()
        {
            using (var mutex = new Mutex(false, MutexName))
            {
                mutex.WaitOne();

                try
                {
                    Random random = new Random();
                    File.WriteAllText(FirstFileName, string.Join(Environment.NewLine, Enumerable.Range(1, 100).Select(_ => random.Next())));
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private void SecondThread()
        {
            using (var mutex = new Mutex(false, MutexName))
            {
                mutex.WaitOne();

                try
                {
                    string[] lines = File.ReadAllLines(FirstFileName);
                    var primeNumbers = lines.Where(line => IsPrime(int.Parse(line)));
                    File.WriteAllText(SecondFileName, string.Join(Environment.NewLine, primeNumbers));
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private void ThirdThread()
        {
            using (var mutex = new Mutex(false, MutexName))
            {
                mutex.WaitOne();

                try
                {
                    string[] lines = File.ReadAllLines(SecondFileName);
                    var primeNumbersEndingWith7 = lines.Where(line => IsPrime(int.Parse(line)) && line.EndsWith("7"));
                    File.WriteAllText(ThirdFileName, string.Join(Environment.NewLine, primeNumbersEndingWith7));
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private void FourthThread()
        {
            using (var mutex = new Mutex(false, MutexName))
            {
                mutex.WaitOne();

                try
                {
                    string report = $"First File - Count: {CountLines(FirstFileName)}, Size: {new FileInfo(FirstFileName).Length} bytes\n" +
                                    $"Second File - Count: {CountLines(SecondFileName)}, Size: {new FileInfo(SecondFileName).Length} bytes\n" +
                                    $"Third File - Count: {CountLines(ThirdFileName)}, Size: {new FileInfo(ThirdFileName).Length} bytes\n";

                    File.WriteAllText(ReportFileName, report);
                    Dispatcher.Invoke(() => ReportTextBox.Text = report);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private int CountLines(string fileName)
        {
            return File.ReadAllLines(fileName).Length;
        }

        private bool IsPrime(int number)
        {
            if (number <= 1)
                return false;
            if (number <= 3)
                return true;
            if (number % 2 == 0 || number % 3 == 0)
                return false;
            for (int i = 5; i * i <= number; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0)
                    return false;
            }
            return true;
        }
    }
}