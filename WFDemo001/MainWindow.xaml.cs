using System.Windows;
using WFDemo001.Design;

namespace WFDemo001
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanbarcodeDialog scanbarcodeDialog = new ScanbarcodeDialog();
            scanbarcodeDialog.ShowDialog();
        }
    }
}
