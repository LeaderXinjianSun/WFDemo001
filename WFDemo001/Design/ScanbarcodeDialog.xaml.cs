using System.Windows;

namespace WFDemo001.Design
{
    /// <summary>
    /// ScanbarcodeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ScanbarcodeDialog : Window
    {
        public ScanbarcodeDialog()
        {
            InitializeComponent();
            GlobalClass.barcodeImageViewer = CameraImageViewer;
        }
    }
}
