using ClosedXML.Excel;
using QuanLyKho.Properties;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    public class ExportViewModel : BaseViewModel
    {
        SqlConnection con;
        private ToastViewModel _toast = null;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private DataTable data = new DataTable(Settings.Default.DataTableName, Settings.Default.DataTableNamespace);
        private readonly string tempDir = Settings.Default.TemporaryDirectory;
        private readonly string templateFile = Settings.Default.TempateFilePath;
        private ObservableCollection<Object> _List;
        public ObservableCollection<Object> List { get { return _List; } set { _List = value; OnPropertyChanged(); } }

        private String _Error;
        public String Error { get => _Error; set { _Error = value; OnPropertyChanged(); } }
        private String _Title;
        public String Title { get => _Title; set { _Title = value; OnPropertyChanged(); } }
        private String _path;
        public String path { get => _path; set { _path = value; OnPropertyChanged(); } }
      
        private bool _IsChecked = true;
        public bool IsChecked { get => _IsChecked; set { _IsChecked = value; OnPropertyChanged(); } }



        public ICommand ExportCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand ShowSaveFileDialog { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }


        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;



            ////Exporting to Excel

            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //    path = PathFolder + FileName;
            //}
            

            //Codes for the Closed XML
            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(data, "Hàng hóa");
                    wb.SaveAs(path);
                }
            }
            catch (Exception ex)
            {
                Error = "Thao tác không thành công!, lỗi: " + ex.Message;
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //this. = e.ProgressPercentage;
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // If you need to do anything opn completion
            progressBarView.Close();

        }

        public ExportViewModel(DataTable _data,string _path,string color)
        {
            this.data = _data;
            this.path = "outputExcel\\" + _path +"_"+DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + ".xlsx"; 
             
            _toast = new ToastViewModel(Corner.BottomCenter, 1, 0, 100);
            ExportCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path) || path.Length == 0)
                    _toast.ShowError("Đường dẫn không được để trống!");
                else
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += new DoWorkEventHandler(DoWork);
                    worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
                    worker.RunWorkerAsync();
                    progressBarViewModel = new ProgressBarViewModel();
                    progressBarView = new ProgressBarView();
                    progressBarView.DataContext = progressBarViewModel;
                    progressBarViewModel.Title = "Đang xử lý, vui lòng chờ...";
                    progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
                    progressBarView.ShowDialog();

                    if (!string.IsNullOrEmpty(Error))
                    {
                        _toast.ShowError(Error);
                        Error = "";
                    }
                    else
                    {
                        p.Close();
                        _toast = null;
                        _toast = new ToastViewModel(Corner.BottomRight, 2, 5, 10);
                        _toast.ShowSuccess("Xuất file thành công!");
                        // If checkbox is checked, show XLSX file in Microsoft Excel.
                        if (this.IsChecked == true)
                            System.Diagnostics.Process.Start(path);

                    }

                }


            });

            ShowSaveFileDialog = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {

                SaveFileDialog open = new SaveFileDialog()
                {
                    FileName = path,
                    DefaultExt = "*.xlsx",
                    Filter = "Excel Workbook (.xlsx)|*.xlsx"
                };

                if (open.ShowDialog() == DialogResult.OK)
                {
                    path = open.FileName;
              
                }
            });

            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });
            MouseMoveWindowCommand = new RelayCommand<Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = p;
                var w = window as Window;
                if (w != null)
                {
                    w.DragMove();

                }
            });

        }



    }
}
