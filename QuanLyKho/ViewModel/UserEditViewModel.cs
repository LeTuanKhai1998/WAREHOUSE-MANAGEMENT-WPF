using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    class UserEditViewModel : BaseViewModel
    {
        SqlConnection con;
        private ToastViewModel _toast = null;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private String _Title;
        public String Title { get => _Title; set { _Title = value; OnPropertyChanged(); } }


        private String _PasswordAgain;
        public String PasswordAgain { get => _PasswordAgain; set { _PasswordAgain = value; OnPropertyChanged(); } }

        private String _Password;
        public String Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        private String _Error;
        public String Error { get => _Error; set { _Error = value; OnPropertyChanged(); } }

        private User _User;
        public User User
        {
            get => _User; set { _User = value; OnPropertyChanged(); }

        }

        private ObservableCollection<UserRole> _UserRoles;
        public ObservableCollection<UserRole> UserRoles { get => _UserRoles; set { _UserRoles = value; OnPropertyChanged(); } }
        private Visibility _RoleVisible = Visibility.Visible;
        public Visibility RoleVisible { get => _RoleVisible; set { _RoleVisible = value; OnPropertyChanged(); } }



        private UserRole _SelectedUserRole;
        public UserRole SelectedUserRole
        {
            get => _SelectedUserRole;
            set
            {
                _SelectedUserRole = value;
                OnPropertyChanged();
                if (value != null && User != null)
                {
                    User.IdRole = value.Id;
                    User.UserRole = value;
                }

            }
        }


        private bool _RadioMale;
        public bool RadioMale
        {
            get { return this._RadioMale; }
            set
            {
                this._RadioMale = value;
                this.OnPropertyChanged("RadioMale");
                if (User != null)
                    User.Sex = "Nam";
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
                if (User != null)
                    User.Sex = "Nữ";
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
                string brithday = "", sex = "Nam";
                if (User.BirthDay == null)
                    brithday = "null";
                else
                    brithday = "'" + User.BirthDay.ToString() + "'";
                if (string.IsNullOrEmpty(Password) || string.IsNullOrWhiteSpace(Password))
                    Password = User.Password;
                else Password = LoginViewModel.MD5Hash(LoginViewModel.Base64Encode(Password));
                if (RadioFeMale == true)
                    sex = "Nữ";

                string sqlQuery = string.Format("exec usp_Insert_Update_User {0},N'{1}',{2},N'{3}',N'{4}','{5}',N'{6}',N'{7}',N'{8}',N'{9}',{10},N'{11}',{12}", User.Id, User.DisplayName, brithday, sex, User.Address, User.Phone, User.Email, User.MoreInfo, User.UserName, Password, User.IdRole, User.Status, User.IsVisible);


                Password = PasswordAgain;
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                cmd.ExecuteNonQuery();

                //p.Close();
                //_toast = null;
                //_toast = new ToastViewModel(Corner.BottomRight, 2, 10, 20);
                //if (User.Id == 0)
                //    _toast.ShowSuccess("Thêm người dùng thành công!");
                //else
                //    _toast.ShowSuccess("Sửa người dùng thành công!");

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

        public UserEditViewModel()
        {
            
            _toast = new ToastViewModel(Corner.BottomLeft, 1, 140, 5);
            loadData();


            SaveCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {



                if (string.IsNullOrEmpty((string)User.DisplayName) || string.IsNullOrWhiteSpace((string)User.DisplayName) || User.DisplayName.Length == 0)
                {
                    //_toast.Show((string)"Bạn chưa nhập tên người dùng!");
                    _toast.ShowError("Bạn chưa nhập tên người dùng!");

                    return;
                }
                if (User.Id == 0)
                {
                    if (string.IsNullOrEmpty((string)User.UserName) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(PasswordAgain))
                    {
                        _toast.ShowError("Bạn hãy nhập đầy đủ thông tin tên tài khoản,mật khẩu và gõ lại mật khẩu trước khi thêm!!");
                        return;
                    }

                }


                if (!string.IsNullOrEmpty(Password))
                    if (PasswordAgain != Password)
                    {
                        _toast.ShowError((string)"Gõ lại mật khẩu không chính xác!");
                        return;
                    }
                if (SelectedUserRole == null)
                    _toast.ShowError("bạn cần chọn vai trò của người dùng");
                else
                if (string.IsNullOrEmpty((string)User.Phone) && string.IsNullOrEmpty((string)User.Email))
                    _toast.ShowError("bạn cần nhập số điện thoại hoặc email của người dùng");

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
                        _toast.ShowSuccess("Thông tin người dùng được cật nhật thành công!");

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


        private void loadData()
        {
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from uv_View_UserRole", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (UserRoles == null)
                    UserRoles = new ObservableCollection<UserRole>();
                else
                    UserRoles.Clear();


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    UserRole user = new UserRole
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString(),
                        RolePermision = dr[2].ToString()
                    };
                    UserRoles.Add(user);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }
}
