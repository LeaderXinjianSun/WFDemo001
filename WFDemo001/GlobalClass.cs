using HalconViewer;
using System.Runtime.InteropServices;

namespace WFDemo001
{
    public static class GlobalClass
    {
        public static ImageViewer barcodeImageViewer;
        [DllImport("halcon.dll")]
        public static extern void HIOCancelDraw();//退出画操作时，调用此函数
    }
}
