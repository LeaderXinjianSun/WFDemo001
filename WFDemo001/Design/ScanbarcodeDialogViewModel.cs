using HalconDotNet;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewROI;
using WFDemo001.Data.Factory;
using WFDemo001.Design.Models;
using WFDemo001.MvvmHelper;

namespace WFDemo001.Design
{
    public class ScanbarcodeDialogViewModel : BindableBase
    {
        #region 变量
        private readonly IDbContextFactory dbContextFactory;
        private VisionRec1 rec1 = new VisionRec1();
        #endregion
        #region 属性
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }
        private int cameraExposureTime;
        public int CameraExposureTime
        {
            get { return cameraExposureTime; }
            set { SetProperty(ref cameraExposureTime, value); }
        }
        private bool isDrawing;
        public bool IsDrawing
        {
            get { return isDrawing; }
            set { SetProperty(ref isDrawing, value); }
        }
        #region halcon
        private HImage cameraImage0;
        public HImage CameraImage0
        {
            get { return cameraImage0; }
            set { SetProperty(ref cameraImage0, value); }
        }
        private HObject cameraAppendHObject0;
        public HObject CameraAppendHObject0
        {
            get { return cameraAppendHObject0; }
            set { SetProperty(ref cameraAppendHObject0, value); }
        }
        private Tuple<string, object> cameraGCStyle0;
        public Tuple<string, object> CameraGCStyle0
        {
            get { return cameraGCStyle0; }
            set { SetProperty(ref cameraGCStyle0, value); }
        }
        private ObservableCollection<ROI> cameraROIList0 = new ObservableCollection<ROI>();
        public ObservableCollection<ROI> CameraROIList0
        {
            get { return cameraROIList0; }
            set { SetProperty(ref cameraROIList0, value); }
        }

        private HMsgEntry cameraAppendHMessage0;
        public HMsgEntry CameraAppendHMessage0
        {
            get { return cameraAppendHMessage0; }
            set { SetProperty(ref cameraAppendHMessage0, value); }
        }
        #endregion
        #endregion
        #region 方法
        private ICommand textBoxLostFocusCommand;
        public ICommand TextBoxLostFocusCommand
        {
            get
            {
                return textBoxLostFocusCommand;
            }
            set
            {
                textBoxLostFocusCommand = value;
            }
        }
        private ICommand openImageCommand;

        public ICommand OpenImageCommand
        {
            get { return openImageCommand; }
            set { openImageCommand = value; }
        }
        private ICommand loadedCommand;

        public ICommand LoadedCommand
        {
            get { return loadedCommand; }
            set { loadedCommand = value; }
        }
        private ICommand createRec1RegionCommand;

        public ICommand CreateRec1RegionCommand
        {
            get { return createRec1RegionCommand; }
            set { createRec1RegionCommand = value; }
        }

        private ICommand closedCommand;

        public ICommand ClosedCommand
        {
            get { return closedCommand; }
            set { closedCommand = value; }
        }
        private ICommand scanBarcodeCommand;

        public ICommand ScanBarcodeCommand
        {
            get { return scanBarcodeCommand; }
            set { scanBarcodeCommand = value; }
        }

        private void TextBoxLostFocusCommandExcute(object obj)
        {
            try
            {
                using (var mdb = dbContextFactory.Create())
                {
                    switch (obj.ToString())
                    {
                        case "CameraExposureTime":
                            {
                                var param = mdb.Params.FirstOrDefault(x => x.Name == "CameraExposureTime");
                                if (param != null)
                                {
                                    param.Value = CameraExposureTime.ToString();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    var changes = mdb.ChangeTracker.Entries().Count();
                    if (changes > 0)
                    {
                        mdb.SaveChanges();
                    }
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
         
        }
        private void OpenImageCommandExcute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图片文件|*.jpg;*.png;*.bmp;*.tif;*.tiff";
            if (openFileDialog.ShowDialog() == true)
            {
                CameraImage0 = new HImage(openFileDialog.FileName);
            }
        }
        private void LoadedCommandExcute()
        {
            LoadParamFromDb();
        }
        private void ClosedCommandExcute()
        {
            if (IsDrawing)
            {
                GlobalClass.HIOCancelDraw();
            }
            CameraImage0 = null;
        }
        private void CreateRec1RegionCommandExcute()
        {
            if (CameraImage0 != null)
            {
                if (MessageBox.Show("确认重新画范围吗？", "信息", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    
                    IsBusy = true;
                    try
                    {
                        CameraAppendHMessage0 = null;
                        CameraAppendHObject0 = null;
                        IsDrawing = true;
                        var roi = (ROIRectangle1)GlobalClass.barcodeImageViewer.DrawROI(ROI.ROI_TYPE_RECTANGLE1);
                        IsDrawing = false;
                        var _rec1 = roi.getRegion();

                        HOperatorSet.RegionFeatures(_rec1, "row1", out var row0);
                        HOperatorSet.RegionFeatures(_rec1, "column1", out var column0);
                        HOperatorSet.RegionFeatures(_rec1, "row2", out var row1);
                        HOperatorSet.RegionFeatures(_rec1, "column2", out var column1);

                        rec1.Row0 = Math.Round(row0.D, 1);
                        rec1.Column0 = Math.Round(column0.D, 1);
                        rec1.Row1 = Math.Round(row1.D, 1);
                        rec1.Column1 = Math.Round(column1.D, 1);

                        using (var mdb = dbContextFactory.Create())
                        {
                            var param = mdb.Params.FirstOrDefault(x => x.Name == "BarcodeRec1");
                            if (param != null)
                            {
                                param.Value = JsonConvert.SerializeObject(rec1);
                            }
                            var changes = mdb.ChangeTracker.Entries().Count();
                            if (changes > 0)
                            {
                                mdb.SaveChanges();
                            }
                        }

                        CameraGCStyle0 = new Tuple<string, object>("DrawMode", "margin");
                        CameraGCStyle0 = new Tuple<string, object>("Color", "yellow");
                        CameraGCStyle0 = new Tuple<string, object>("LineWidth", 5);

                        CameraAppendHObject0 = _rec1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                    IsBusy = false;
                }
            }
            else
            {
                MessageBox.Show("图片不能为空", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private async void ScanBarcodeCommandExcute()
        {
            if (CameraImage0 != null)
            {
                IsBusy = true;
                CameraAppendHObject0 = null;
                CameraAppendHMessage0 = null;
                await Task.Delay(500);
                HOperatorSet.CreateDataCode2dModel("QR Code", new HTuple(), new HTuple(), out var DataCodeHandle);
                HOperatorSet.SetDataCode2dParam(DataCodeHandle, "polarity", "any");
                HOperatorSet.SetDataCode2dParam(DataCodeHandle, "timeout", 200);
                HOperatorSet.SetDataCode2dParam(DataCodeHandle, "string_encoding", "locale");
                CameraGCStyle0 = new Tuple<string, object>("Color", "cornflower blue");
                HOperatorSet.GenRectangle1(out var _rec1, rec1.Row0, rec1.Column0, rec1.Row1, rec1.Column1);
               
                HOperatorSet.ReduceDomain(CameraImage0, _rec1, out var imageReduced);
                CameraAppendHObject0 = _rec1;
                HOperatorSet.FindDataCode2d(imageReduced, out var symbolXLD1, DataCodeHandle, new HTuple(), new HTuple(), out var ResultHandles, out var DecodedDataStrings);
                HOperatorSet.ClearDataCode2dModel(DataCodeHandle);
                if (DecodedDataStrings.TupleLength() == 0)
                {
                    HOperatorSet.CreateDataCode2dModel("QR Code", new HTuple(), new HTuple(), out var DataCodeHandle1);
                    HOperatorSet.FindDataCode2d(imageReduced, out symbolXLD1, DataCodeHandle1, "train", "all", out ResultHandles, out DecodedDataStrings);
                    if (DecodedDataStrings.TupleLength() > 0)
                    {
                        var trainfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ecc200_trained_model.dcm");
                        HOperatorSet.WriteDataCode2dModel(DataCodeHandle1, trainfile);
                    
                        MessageBox.Show("扫码训练成功", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    HOperatorSet.ClearDataCode2dModel(DataCodeHandle1);
                }
                if (DecodedDataStrings.TupleLength() > 0)
                {
                    CameraGCStyle0 = new Tuple<string, object>("DrawMode", "margin");
                    CameraGCStyle0 = new Tuple<string, object>("LineWidth", 5);
                    CameraGCStyle0 = new Tuple<string, object>("Color", "green");
                    CameraAppendHObject0 = symbolXLD1;
                    string bar = DecodedDataStrings.TupleSelect(0).S;

                    CameraAppendHMessage0 = new HMsgEntry(bar, 10, 10, "magenta", "window", "box", "false", 36, "mono", "true", "false");
                }
                else
                {
                    CameraAppendHMessage0 = new HMsgEntry($"NG", 10, 10, "red", "window", "box", "false", 36, "mono", "true", "false");
                }
               
                IsBusy = false;
            }
            else
            {
                MessageBox.Show("图片不能为空", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion
        #region 构造函数
        public ScanbarcodeDialogViewModel()
        {
            dbContextFactory = new DbContextFactory();
            TextBoxLostFocusCommand = new RelayCommand<object>(TextBoxLostFocusCommandExcute);
            OpenImageCommand = new RelayCommand(OpenImageCommandExcute);
            LoadedCommand = new RelayCommand(LoadedCommandExcute);
            ClosedCommand = new RelayCommand(ClosedCommandExcute);
            CreateRec1RegionCommand = new RelayCommand(CreateRec1RegionCommandExcute);
            ScanBarcodeCommand = new RelayCommand(ScanBarcodeCommandExcute);
        }
        #endregion
        #region 功能函数
        private void LoadParamFromDb()
        {
            try
            {
                using (var mdb = dbContextFactory.Create())
                {
                    var param = mdb.Params.FirstOrDefault(x => x.Name == "CameraExposureTime");
                    if (param != null)
                    {
                        CameraExposureTime = int.Parse(param.Value);
                    }
                    else
                    {
                        CameraExposureTime = 1000;
                        mdb.Params.Add(new Data.Models.Param()
                        {
                            Name = "CameraExposureTime",
                            Value = CameraExposureTime.ToString(),
                            Type = typeof(int).Name,
                        });
                    }
                    param = mdb.Params.FirstOrDefault(x => x.Name == "BarcodeRec1");
                    if (param != null)
                    {
                        rec1 = JsonConvert.DeserializeObject<VisionRec1>(param.Value);
                    }
                    else
                    {
                        rec1 = new VisionRec1() 
                        {
                            Row0 = 1,
                            Column0 = 1,
                            Row1 = 100,
                            Column1 = 100,
                        };
                        mdb.Params.Add(new Data.Models.Param()
                        {
                            Name = "BarcodeRec1",
                            Value = JsonConvert.SerializeObject(rec1),
                            Type = typeof(VisionRec1).Name,
                        });
                    }
                    var changes = mdb.ChangeTracker.Entries().Count();
                    if (changes > 0)
                    {
                        mdb.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
