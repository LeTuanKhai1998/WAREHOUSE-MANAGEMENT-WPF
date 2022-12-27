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
    class UserRoleViewModel : BaseViewModel
    {
        SqlConnection con;
        private ToastViewModel _toast = null;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;

        public bool[] Role { set; get; }
        private ObservableCollection<UserRole> _List;
        public ObservableCollection<UserRole> List { get => _List; set { _List = value; OnPropertyChanged(); } }


        private UserRole _SelectedUserRole;
        public UserRole SelectedUserRole
        {
            get => _SelectedUserRole;
            set
            {
                if (_SelectedUserRole != value)
                    CheckFalse();
                _SelectedUserRole = value;
                OnPropertyChanged();
                if (value != null)
                {
                    for (int i = 0; i < 8; i++)
                        if (value.RolePermision.Contains((i + 1).ToString()))
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


            }
        }


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

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SaveCommand { get; set; }
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

        private void CheckFalse()
        {
            Objects = false; Inputs = false; Outputs = false; Customers = false; Suppliers = false; Users = false; CPUs = false; UserRole = false;
        }
        private bool CheckSave()
        {
            return (Objects == false && Inputs == false && Outputs == false && Customers == false && Suppliers == false && Users == false && CPUs == false && UserRole == false) ? true : false;
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // do time-consuming work here, calling ReportProgress as and when you can
            loadData();
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

        public UserRoleViewModel()
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
            progressBarViewModel.Title = "Đang tải danh sách vai trò, vui lòng chờ...";
            progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#8a2be2"));
            progressBarView.ShowDialog();

            _toast = new ToastViewModel(Corner.BottomLeft, 1, 140, 5);
            Role = new bool[9];

            AddCommand = new RelayCommand<UserRole>(p => { return true; }, p =>
            {
                UserRole user = new UserRole
                {
                    Id = 0,
                    DisplayName = "",
                    RolePermision = "",
                };
                UserRoleEditViewModel editViewModel = new UserRoleEditViewModel(user);
                UserRoleEditView editView = new UserRoleEditView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Thêm vai trò";
                editView.ShowDialog();
                loadData();

            });
            EditCommand = new RelayCommand<User>(p => { return true; }, p =>
            {
                if (SelectedUserRole != null)
                {
                    UserRoleEditViewModel editViewModel = new UserRoleEditViewModel(SelectedUserRole);
                    UserRoleEditView editView = new UserRoleEditView();
                    editView.DataContext = editViewModel;
                    editViewModel.Title = "Cật nhật vai trò";
                    editView.ShowDialog();
                    List.Clear();
                    loadData();
                }
            });

            SaveCommand = new RelayCommand<Window>((p) =>
            {

                if (SelectedUserRole == null || CheckSave() == true) return false;
                else
                    return true;
            }, (p) =>
            {
                if (CheckSave() == true) _toast.ShowError("Bạn cần chọn ít nhất 1 quyền!");
                else
                    try
                    {
                        con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        string sql = string.Format("exec usp_Update_RolePermision_UserRole {0},N'{1}'", SelectedUserRole.Id, RolePermiss());
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        _toast.ShowSuccess(string.Format("Cật nhật phân quyền người dùng {0} thành công!", SelectedUserRole.DisplayName));



                    }
                    catch (SqlException ex)
                    {
                        _toast.ShowError("Thao tác không thành công!, lỗi: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                        loadData();
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
                if (List == null)
                    List = new ObservableCollection<UserRole>();
                else
                    List.Clear();


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    UserRole user = new UserRole
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString(),
                        RolePermision = dr[2].ToString()
                    };
                    List.Add(user);
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
