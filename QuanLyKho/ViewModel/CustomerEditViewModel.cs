using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    class CustomerEditViewModel : BaseViewModel
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
        private Customer _Customer;
        public Customer Customer
        {
            get => _Customer; set { _Customer = value; OnPropertyChanged(); }

        }

        private bool _RadioMale;
        public bool RadioMale
        {
            get { return this._RadioMale; }
            set
            {
                this._RadioMale = value;
                this.OnPropertyChanged("RadioMale");
                if (Customer != null)
                    Customer.Sex = "Nam";
            }
        }
        private bool _RadioFeMale;
        public bool RadioFeMale
        {
            get { return this._RadioFeMale; }
            set
            {
                this._RadioFeMale = value;
                this.OnPropertyChanged("RadioFeMale");
                if (Customer != null)
                    Customer.Sex = "Nữ";
            }
        }


        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand AddImageCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // do time-consuming work here, calling ReportProgress as and when you can
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                string brithday = "", linkImage = "";
                if (Customer.BirthDay == null)
                    brithday = "null";
                else
                    brithday = "'" + Customer.BirthDay.ToString() + "'";
                if (Customer.LinkImage.Contains("IMG_Holder.png"))
                    linkImage = "null";
                else
                {
                    string[] link = Customer.LinkImage.Split('\\');
                    linkImage = link[link.Length - 1];
                }
                string sql = string.Format("exec usp_Insert_Update_Customer {0},N'{1}',{2},N'{3}',N'{4}',N'{5}',N'{6}',N'{7}','{8}',N'{9}',{10},N'{11}'", Customer.Id, Customer.DisplayName, brithday, Customer.Sex, Customer.Address, Customer.Phone, Customer.Email, Customer.MoreInfo, Customer.ContractDate, Customer.Status, Customer.IsVisible, linkImage);
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                //if (Customer.Id == 0)
                //    _toast.Show("Thêm khách hàng thành công!");
                //else
                //    _toast.Show("Sửa khách hàng thành công!");

            }
            catch (SqlException ex)
            {
                //if (ex.Message.Contains("đã tồn tại"))
                //{
                //    string[] mess = ex.Message.ToString().Split('.');
                //    Error = mess[mess.Length - 1];
                //}
                //else
                Error = "Thao tác không thàng công!";
                //+ ex.Message;
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
        public CustomerEditViewModel() { }
        public CustomerEditViewModel(Customer _customer)
        {
            this.Customer = _customer;
            if (Customer.Id == 0)
                Id = "Tự động sinh mã";
            else
                Id = Customer.Id.ToString();
            _toast = new ToastViewModel(Corner.BottomLeft, 1, 245, 5);

            AddImageCommand = new RelayCommand<Model.Object>((p) => true,
                (p) =>
                {
                    OpenFileDialog open = new OpenFileDialog();
                    open.Filter = "(*.jpg;*jpeg,*bmp;*png) | *.jpg;*.jpeg;*bmp;*png";
                    if (open.ShowDialog() == DialogResult.OK)
                    {
                        Customer.LinkImage = open.FileName;
                    }

                });

            SaveCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {


                if (string.IsNullOrEmpty((string)Customer.DisplayName) || string.IsNullOrWhiteSpace((string)Customer.DisplayName) || Customer.DisplayName.Length == 0)
                    _toast.ShowError((string)"Bạn chưa nhập tên Khách hàng!");

                else
                if (string.IsNullOrEmpty((string)Customer.Phone) && string.IsNullOrEmpty((string)Customer.Email))
                    _toast.ShowError((string)"bạn cần nhập số điện thoại hoặc email của khách hàng");

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
                        _toast.ShowSuccess("Thông tin khách hàng được cật nhật thành công!");

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
