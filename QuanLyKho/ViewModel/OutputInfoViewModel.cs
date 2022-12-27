using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    class OutputInfoViewModel : BaseViewModel
    {
        SqlConnection con;

        private ToastViewModel _toast = null;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private Output _Output;
        public Output Output { get => _Output; set { _Output = value; OnPropertyChanged(); } }


        private ObservableCollection<Model.OutputInfoView> _List;
        public ObservableCollection<Model.OutputInfoView> List { get => _List; set { _List = value; OnPropertyChanged(); } }
        private double? _TotalFinal;
        public double? TotalFinal { get => _TotalFinal; set { _TotalFinal = value; OnPropertyChanged(); } }

        private double? _TotalQuantity = 0;
        public double? TotalQuantity { get => _TotalQuantity; set { _TotalQuantity = value; OnPropertyChanged(); } }
        private String _Error;
        public String Error { get => _Error; set { _Error = value; OnPropertyChanged(); } }
        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }




        private void DoWorkCancel(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;


            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                string s = string.Format("exec usp_Update_Status_Output N'{0}',N'{1}'",Output.Id,"Đã hủy");
                SqlCommand cmd = new SqlCommand(s, con);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) {
                Error = "Thao tác không thàng công!lỗi: " + ex.Message;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }


        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // do time-consuming work here, calling ReportProgress as and when you can
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                string sql = string.Format("exec usp_Update_Note_Output N'{0}',N'{1}'", Output.Id, Output.Note);
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Error = "Thao tác không thàng công!lỗi: " + ex.Message;
            }
            finally
            {
                con.Close();
                con.Dispose();

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
            //var window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(); //change MainWindow to the type of the window that you want to close
            //if (window != null)
            //    window.Close();
        }

        public OutputInfoViewModel()
        {

        }
        public OutputInfoViewModel(Output _out)
        {
            this.Output = _out;
            _toast = new ToastViewModel(Corner.BottomRight, 1, 10, 100);
            SaveCommand = new RelayCommand<Window>(p => true, p =>
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
                progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#8a2be2"));
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
                    _toast.ShowSuccess("Thông tin hóa đơn được cật nhật thành công!");

                }

            });

            CancelCommand = new RelayCommand<Window>(p =>
            {
                if (Output.Status.Contains("hủy")) return false;
                return true;
            }, p =>
            {

                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Hủy hóa đơn bán hàng";
                editViewModel.Content = "Bạn có chắc chắn muốn hủy hóa đơn " + Output.Id + " không?";
                editView.ShowDialog();
                if (editViewModel.Result == true)
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += new DoWorkEventHandler(DoWorkCancel);
                    worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
                    worker.RunWorkerAsync();
                    progressBarViewModel = new ProgressBarViewModel();
                    progressBarView = new ProgressBarView();
                    progressBarView.DataContext = progressBarViewModel;
                    progressBarViewModel.Title = "Đang xử lý, vui lòng chờ...";
                    progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#8a2be2"));
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
                        _toast.ShowSuccess("Thông tin hóa đơn được cật nhật thành công!");


                    }
                }
            });
        }
    }
}
