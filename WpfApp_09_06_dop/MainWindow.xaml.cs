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
        private const string SemaphoreName = "MyAppSemaphore";
        private Semaphore semaphore;

        public MainWindow()
        {
            InitializeComponent();

            // Try to create a new semaphore. If it already exists, it means another instance is running.
            bool createdNew;
            semaphore = new Semaphore(3, 3, SemaphoreName, out createdNew);

            if (!createdNew)
            {
                // Another instance is running. Show message and close this instance.
                MessageBox.Show("Another instance of this application is already running.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Release the semaphore when the window is closed.
            semaphore.Release();
            base.OnClosed(e);
        }
    }
}