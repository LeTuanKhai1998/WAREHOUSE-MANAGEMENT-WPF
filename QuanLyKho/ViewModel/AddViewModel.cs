using QuanLyKho.View;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    public class AddViewModel : BaseViewModel
    {
        SqlConnection con;
        private ToastViewModel _toast = null;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private String _NameTable;
        public String NameTable { get => _NameTable; set { _NameTable = value; OnPropertyChanged(); } }
        private int _Id;
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        private String _Error;
        public String Error { get => _Error; set { _Error = value; OnPropertyChanged(); } }
        private String _Title;
        public String Title { get => _Title; set { _Title = value; OnPropertyChanged(); } }
        private String _Name;
        public String Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        private String _NameTitle;
        public String NameTitle { get => _NameTitle; set { _NameTitle = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // do time-consuming work here, calling ReportProgress as and when you can
            try
            {
                if (Id == null)
                    Id = 0;

                string tex = string.Format("usp_Insert_Update_{0}  {1},N'{2}'", NameTable, Id, Name);
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand(tex, con);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("đã tồn tại"))
                {
                    string[] mess = ex.Message.ToString().Split('.');
                    //_toast.ShowError(mess[mess.Length - 1]);
                    Error = mess[mess.Length - 1];
                }
                else
                    Error = "Thao tác không thàng công!lỗi: " + ex.Message;
            }
            finally
            {
                con.Close();

            }
            //Thread.Sleep(3000);

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

        public AddViewModel()
        {

            _toast = new ToastViewModel(Corner.BottomCenter, 1, 0, 100);
            SaveCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name) || Name.Length == 0)
                    _toast.ShowError("Dữ liệu không được để trống!");
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
                    progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00b3b3"));
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
                        _toast = new ToastViewModel(Corner.BottomCenter, 2, 0, 10);
                        if (Id == 0 || Id == null)
                            _toast.ShowSuccess("Thêm thành công!");
                        else
                            _toast.ShowSuccess("Sửa thành công!");

                    }

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
