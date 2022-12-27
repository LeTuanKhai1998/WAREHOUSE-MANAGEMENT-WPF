using QuanLyKho.Model;
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
    class SupplierEditViewModel : BaseViewModel
    {
        SqlConnection con;
        private ToastViewModel _toast = null;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private String _Title;
        public String Title { get => _Title; set { _Title = value; OnPropertyChanged(); } }
        private String _Error;
        public String Error { get => _Error; set { _Error = value; OnPropertyChanged(); } }
        private String _Id;
        public String Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        private Supplier _Supplier;
        public Supplier Supplier
        {
            get => _Supplier; set { _Supplier = value; OnPropertyChanged(); }

        }

        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // do time-consuming work here, calling ReportProgress as and when you can
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();

                string tex = string.Format("exec usp_Insert_Update_Supplier {0},N'{1}',N'{2}',N'{3}',N'{4}',N'{5}','{6}',N'{7}',{8}", 
                    Supplier.Id, Supplier.DisplayName, Supplier.Phone, Supplier.Address, Supplier.Email, Supplier.MoreInfo, Supplier.ContractDate, Supplier.Status, Supplier.IsVisible);
                SqlCommand cmd = new SqlCommand(tex, con);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("đã tồn tại"))
                {
                    string[] mess = ex.Message.ToString().Split('.');
                    Error = mess[mess.Length - 1];
                }
                else
                    Error = "Thao tác không thàng công!lỗi: " + ex.Message;

            }
            finally
            {
                con.Dispose();
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
        public SupplierEditViewModel() { }

        public SupplierEditViewModel(string title, Supplier _supplier)
        {

            this.Title = title;
            this.Supplier = _supplier;
            if (Supplier.Id == 0)
                Id = "Tự động sinh mã";
            else
                Id = Supplier.Id.ToString();
            _toast = new ToastViewModel(Corner.BottomLeft, 1, 140, 5);

            SaveCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {

                if (string.IsNullOrEmpty((string)Supplier.DisplayName) || string.IsNullOrWhiteSpace((string)Supplier.DisplayName) || Supplier.DisplayName.Length == 0)
                    _toast.ShowError((string)"Bạn chưa nhập tên nhà cung cấp!");
                else
                if (string.IsNullOrEmpty((string)Supplier.Phone) && string.IsNullOrEmpty((string)Supplier.Email))
                    _toast.ShowError((string)"bạn cần nhập số điện thoại hoặc email của nhà cung cấp");
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
                    progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff5349"));
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
                        _toast = new ToastViewModel(Corner.BottomRight, 2, 10, 20);
                        _toast.ShowSuccess("Thông tin nhà cung cấp được cật nhật thành công!");

                    }
                    
                }
            });
            MouseMoveWindowCommand = new RelayCommand<Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = p;
                var w = window as Window;
                if (w != null)
                {
                    w.DragMove();

                }
            });
            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });

        }
    }
}
