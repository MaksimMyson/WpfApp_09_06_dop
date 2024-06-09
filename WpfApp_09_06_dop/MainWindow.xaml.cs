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
        // Створюємо семафор з максимум трьома дозволами
        private Semaphore semaphore = new Semaphore(3, 3);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartThreads_Click(object sender, RoutedEventArgs e)
        {
            OutputText.Text = ""; // Очищаємо вміст текстового блоку перед початком

            // Створюємо та запускаємо 10 потоків
            for (int i = 0; i < 10; i++)
            {
                Thread thread = new Thread(ThreadWork);
                thread.Start(i);
            }
        }

        private void ThreadWork(object id)
        {
            // Очікуємо на доступ до семафора
            semaphore.WaitOne();

            try
            {
                // Відображаємо ідентифікатор потоку
                AppendText($"Thread {id} started.");

                // Генеруємо та виводимо набір випадкових чисел
                Random random = new Random();
                for (int i = 0; i < 5; i++)
                {
                    AppendText($"Thread {id} random number: {random.Next(100)}");
                    Thread.Sleep(100); // Затримка для імітації роботи
                }

                AppendText($"Thread {id} finished.");
            }
            finally
            {
                // Звільняємо семафор
                semaphore.Release();
            }
        }

        private void AppendText(string text)
        {
            // Метод для додавання тексту до вмісту текстового блоку з врахуванням множинного потоку
            Dispatcher.Invoke(() =>
            {
                OutputText.Text += text + "\n";
            });
        }
    }
}