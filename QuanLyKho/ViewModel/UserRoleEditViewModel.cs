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

    class UserRoleEditViewModel : BaseViewModel
    {
        SqlConnection con;
        private ToastViewModel _toast = null;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;

        public bool[] Role { set; get; }
        private UserRole _UserRoles;
        public UserRole UserRoles { get => _UserRoles; set { _UserRoles = value; OnPropertyChanged(); } }

        private String _Error;
        public String Error { get => _Error; set { _Error = value; OnPropertyChanged(); } }
        private String _Title;
        public String Title { get => _Title; set { _Title = value; OnPropertyChanged(); } }
        private bool _Objects;
        public bool Objects
        {
            get { return this._Objects; }
            set
            {
                this._Objects = value;
                this.OnPropertyChanged();
                Role[0] = value;

            }
        }

        private bool _Inputs;
        public bool Inputs
        {
            get { return this._Inputs; }
            set
            {
                this._Inputs = value;
                this.OnPropertyChanged();
                Role[1] = value;
            }
        }

        private bool _Outputs;
        public bool Outputs
        {
            get { return this._Outputs; }
            set
            {
                this._Outputs = value;
                this.OnPropertyChanged();
                Role[2] = value;
            }
        }

        private bool _Suppliers;
        public bool Suppliers
        {
            get { return this._Suppliers; }
            set
            {
                this._Suppliers = value;
                this.OnPropertyChanged();
                Role[3] = value;
            }
        }

        private bool _Customers;
        public bool Customers
        {
            get { return this._Customers; }
            set
            {
                this._Customers = value;
                this.OnPropertyChanged();
                Role[4] = value;
            }
        }

        private bool _Users;
        public bool Users
        {
            get { return this._Users; }
            set
            {
                this._Users = value;
                this.OnPropertyChanged();
                Role[5] = value;
            }
        }
        private bool _CPUs;
        public bool CPUs
        {
            get { return this._CPUs; }
            set
            {
                this._CPUs = value;
                this.OnPropertyChanged();
                Role[6] = value;
            }
        }

        private bool _UserRole;
        public bool UserRole
        {
            get { return this._UserRole; }
            set
            {
                this._UserRole = value;
                this.OnPropertyChanged();
                Role[7] = value;
            }
        }

        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }
        
        private string RolePermiss()
        {
            string permiss = "";
            for (int i = 0; i < 8; i++)
            {
                if (Role[i] == true)
                    permiss += (i + 1);
            }
            return permiss;
        }

        private void CheckValue()
        {
            for (int i = 0; i < 8; i++)
                if (UserRoles.RolePermision.Contains((i + 1).ToString()))
                    switch (i + 1)
                    {
                        case 1: Objects = true; break;
                        case 2: Inputs = true; break;
                        case 3: Outputs = true; break;
                        case 4: Suppliers = true; break;
                        case 5: Customers = true; break;
                        case 6: Users = true; break;
                        case 7: CPUs = true; break;
                        case 8: UserRole = true; break;

                    }
        }
        private bool CheckSave()
        {
            return (Objects == false && Inputs == false && Outputs == false && Customers == false && Suppliers == false && Users == false && CPUs == false && UserRole == false) ? true : false;
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // do time-consuming work here, calling ReportProgress as and when you can
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                string sqlQuery = string.Format("exec usp_Insert_UserRole {0},N'{1}',N'{2}'", UserRoles.Id,UserRoles.DisplayName, RolePermiss());

                SqlCommand cmd = new SqlCommand(sqlQuery, con);
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
                //_toast.ShowError("Thao tác không thàng công! lỗi: " + ex.Message);
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
            //var window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(); //change MainWindow to the type of the window that you want to close
            //if (window != null)
            //    window.Close();
        }

        public UserRoleEditViewModel() { }
        public UserRoleEditViewModel(UserRole _userRole)
        {
            this.UserRoles = _userRole;
            Role = new bool[9];
            CheckValue();

            _toast = new ToastViewModel(Corner.BottomLeft, 1, 140, 5);
            



            SaveCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (string.IsNullOrEmpty(UserRoles.DisplayName) || string.IsNullOrWhiteSpace(UserRoles.DisplayName)) _toast.ShowError("Tên không được để trống!");
                else if (CheckSave() == true) _toast.ShowError("Bạn cần chọn ít nhất 1 quyền!");
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
                        _toast.ShowSuccess("Thông tin vai trò được cật nhật thành công!");

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
