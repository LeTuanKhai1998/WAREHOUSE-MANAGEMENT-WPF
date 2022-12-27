using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    public class UserViewModel : BaseViewModel
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataSet ds, ds2;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private ToastViewModel _toast = null;
        private User _UserSeleted;
        public User UserSeleted { get => _UserSeleted; set { _UserSeleted = value; OnPropertyChanged(); } }


        private ObservableCollection<UserRole> _UserRole;
        public ObservableCollection<UserRole> UserRole { get => _UserRole; set { _UserRole = value; OnPropertyChanged(); } }

        private ObservableCollection<User> _List;
        public ObservableCollection<User> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        public ICollectionView ItemsView { get { return CollectionViewSource.GetDefaultView(List); } }



        //Lọc dữ liệu
        private bool Filter(object item)
        {
            User sup = item as User;

            bool[] kq = new bool[4];
            int i = 0;

            if (string.IsNullOrEmpty(Search) && RadioStatusAll == true) return true;

            if (!string.IsNullOrEmpty(Search))
            {
                if (sup.DisplayName.ToUpper().Contains(Search.ToUpper()) || sup.Id.ToString().ToUpper().Contains(Search.ToUpper()))
                {
                    kq[0] = true;
                }
                i++;
            }

            if (RadioStatusAll == false)
            {
                if (RadioBusiness == true)
                {
                    if (sup.Status.ToUpper().Contains("ĐANG")) kq[2] = true;

                }
                else if (RadioStop == true)
                    if (sup.Status.ToUpper().Contains("NGỪNG")) kq[3] = true;
                i++;
            }



            int dem = 0;
            foreach (bool b in kq)
                if (b == true) dem++;
            if (i == dem && i != 0) return true;
            return false;

        }
        private bool? _AllSelected = false;
        public bool? AllSelected
        {
            get { return _AllSelected; }
            set
            {
                _AllSelected = value;
                if (List != null)
                    SelectAll(value, List);
                OnPropertyChanged();
            }
        }

        private void SelectAll(bool? select, ObservableCollection<User> models)
        {
            if (select == false || select == true)
                foreach (var model in models)
                {
                    model.IsSelected = (bool)select;
                }

        }

        private User _SelectedItem;
        public User SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (value != null)
                {
                    if (UserSeleted != value)
                        RowDetailsVisible = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;

                    else
                   if (RowDetailsVisible == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                        RowDetailsVisible = DataGridRowDetailsVisibilityMode.Collapsed;
                    else
                        RowDetailsVisible = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                    UserSeleted = value;
                    _SelectedItem = null;

                }




            }
        }


        private string _Search;
        public string Search
        {
            get { return _Search; }
            set
            {
                _Search = value;
                OnPropertyChanged();
                ItemsView.Refresh(); // required    
            }
        }

        private bool _RadioStatusAll;
        public bool RadioStatusAll
        {
            get { return this._RadioStatusAll; }
            set
            {
                this._RadioStatusAll = value;
                this.OnPropertyChanged("RadioStatusAll");
                if (List != null)
                    ItemsView.Refresh();
            }
        }
        private bool _RadioBusiness;
        public bool RadioBusiness
        {
            get { return this._RadioBusiness; }
            set
            {
                this._RadioBusiness = value;
                this.OnPropertyChanged("RadioBusiness");
                ItemsView.Refresh();
            }
        }
        private bool _RadioStop;
        public bool RadioStop
        {
            get { return this._RadioStop; }
            set
            {
                this._RadioStop = value;
                this.OnPropertyChanged("RadioStop");
                ItemsView.Refresh();
            }
        }

        private int? _CountSelected;
        public int? CountSelected { get { return _CountSelected; } set { _CountSelected = value; OnPropertyChanged(); } }


        private Visibility _Stop = Visibility.Visible;
        public Visibility Stop { get => _Stop; set { _Stop = value; OnPropertyChanged(); } }
        private Visibility _Business = Visibility.Visible;
        public Visibility Business { get => _Business; set { _Business = value; OnPropertyChanged(); } }

        private DataGridRowDetailsVisibilityMode _rowDetailsVisible = DataGridRowDetailsVisibilityMode.Collapsed;
        public DataGridRowDetailsVisibilityMode RowDetailsVisible { get { return _rowDetailsVisible; } set { _rowDetailsVisible = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand DeleteOneCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand BusinessCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ExportCommand { get; set; }

        public UserViewModel(int x)
        {

        }

        User kiemTraChonXoa()
        {
            if (List != null)
                return List.FirstOrDefault(p => p.IsSelected == true);
            return null;
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
        }
        public UserViewModel()
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
            progressBarViewModel.Title = "Đang tải danh sách người dùng, vui lòng chờ...";
            progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#8a2be2"));
            progressBarView.ShowDialog();


            _toast = new ToastViewModel(Corner.BottomRight, 4, 10, 20);

            //loadData();
            RadioStatusAll = true;
            SelectedItem = null;
            if (List != null)
            {
                this.ItemsView.Filter = Filter;
            }

            AddCommand = new RelayCommand<User>(p => { return true; }, p =>
                        {
                            UserEditViewModel editViewModel = new UserEditViewModel();
                            UserEditView editView = new UserEditView();
                            editView.DataContext = editViewModel;
                            editViewModel.Title = "Thêm người dùng";
                            editViewModel.RadioMale = true;
                            User sup = new User
                            {
                                Id = 0,
                                DisplayName = "",
                                BirthDay = null,
                                Sex = "Nam",
                                Address = "",
                                Phone = "",
                                Email = "",
                                MoreInfo = "",
                                UserName = "",
                                Password = "",
                                UserRole = null,
                                Status = "Đang hoạt động",
                                IsVisible = 0,
                            };

                            editViewModel.User = sup;

                            editView.ShowDialog();
                            List.Clear();
                            UserRole.Clear();
                            loadData();

                        });
            EditCommand = new RelayCommand<User>(p => { return true; }, p =>
                         {
                             if (UserSeleted != null)
                             {
                                 UserEditViewModel editViewModel = new UserEditViewModel();
                                 UserEditView editView = new UserEditView();
                                 editView.DataContext = editViewModel;
                                 editViewModel.Title = "Cật nhật người dùng";
                                 editViewModel.SelectedUserRole = UserSeleted.UserRole;
                                 User sup = new User
                                 {
                                     Id = UserSeleted.Id,
                                     DisplayName = UserSeleted.DisplayName,
                                     BirthDay = UserSeleted.BirthDay,
                                     Sex = UserSeleted.Sex,
                                     Phone = UserSeleted.Phone,
                                     Address = UserSeleted.Address,
                                     Email = UserSeleted.Email,
                                     MoreInfo = UserSeleted.MoreInfo,
                                     UserName = UserSeleted.UserName,
                                     Password = UserSeleted.Password,
                                     IdRole = UserSeleted.IdRole,
                                     Status = UserSeleted.Status,
                                     IsVisible = UserSeleted.IsVisible,
                                     UserRole = UserSeleted.UserRole

                                 };
                                 editViewModel.User = sup;
                                 if (sup.Sex.Contains("Nam"))
                                     editViewModel.RadioMale = true;
                                 else
                                     editViewModel.RadioFeMale = true;
                                 editView.ShowDialog();
                                 List.Clear();
                                 UserRole.Clear();
                                 loadData();
                             }
                         });
            DeleteCommand = new RelayCommand<User>((p) =>
            {

                if (kiemTraChonXoa() == null)
                {
                    CountSelected = null;
                    AllSelected = false;
                    return false;
                }
                else
                {
                    CountSelected = List.Count(x => x.IsSelected == true);
                    if (List.Count(x => x.IsSelected == false) == 0)
                        AllSelected = true;
                    else
                        AllSelected = null;
                }
                return true;
            }, (p) =>
            {
                User objXoa = kiemTraChonXoa();

                string tex = "";
                int count = List.Count(x => x.IsSelected == true);
                if (count == 1)
                    tex = "Hệ thống sẽ xóa hoàn toàn người dùng " + objXoa.DisplayName + " nhưng vẫn giữ thông tin người dùng trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";
                else
                    tex = "Hệ thống sẽ xóa hoàn toàn " + count + " người dùng bạn đã chọn nhưng vẫn giữ thông tin người dùng trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";

                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Xóa người dùng";
                editViewModel.Content = tex;
                editView.ShowDialog();
                if (editViewModel.Result == true)
                    try
                    {
                        SqlConnection con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        foreach (User o in List)
                            if (o.IsSelected == true)
                            {
                                string texSql = "exec usp_Delete_User " + o.Id;
                                cmd = new SqlCommand(texSql, con);
                                cmd.ExecuteNonQuery();
                            }
                        List.Clear();
                        UserRole.Clear();
                        loadData();
                        _toast.ShowSuccess("Xóa thành công danh sách người dùng đã chọn!");
                    }
                    catch (TransactionAbortedException ex)
                    {
                        // Log error       
                        _toast.ShowError("Thao tác không thành công!, lỗi: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();

                    }
            });

            DeleteOneCommand = new RelayCommand<User>((p) =>
            {
                return true;
            }, (p) =>
            {

                string tex = "Hệ thống sẽ xóa hoàn toàn người dùng " + UserSeleted.DisplayName + " nhưng vẫn giữ thông tin người dùng trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";


                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Xóa người dùng";
                editViewModel.Content = tex;
                editView.ShowDialog();
                if (editViewModel.Result == true)
                    try
                    {
                        SqlConnection con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();

                        string texSql = "exec usp_Delete_User " + UserSeleted.Id;
                        cmd = new SqlCommand(texSql, con);
                        cmd.ExecuteNonQuery();

                        List.Clear();
                        UserRole.Clear();
                        loadData();
                        _toast.ShowSuccess("Xóa thành công danh sách người dùng đã chọn!");
                    }
                    catch (TransactionAbortedException ex)
                    {
                        // Log error       
                        _toast.ShowError("Thao tác không thành công!, lỗi: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();

                    }
            });
            BusinessCommand = new RelayCommand<User>(p =>
            {
                if (UserSeleted != null)
                    if (UserSeleted.Status.ToUpper().Contains("ĐANG"))
                    {
                        Business = Visibility.Collapsed;
                        Stop = Visibility.Visible;
                        return false;
                    }
                    else
                    {
                        Stop = Visibility.Collapsed;
                        Business = Visibility.Visible;

                        return true;
                    }
                return true;
            }, p =>
            {
                if (UserSeleted != null)
                    try
                    {
                        con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        string sql = string.Format("exec usp_Update_Status_User {0},N'Đang hoạt động'", UserSeleted.Id);
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        _toast.ShowSuccess(string.Format("Cật nhật trạng thái người dùng {0} thành công!", UserSeleted.DisplayName));
                    }
                    catch (SqlException ex)
                    {
                        _toast.ShowError("Thao tác không thành công!, lỗi: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                        List.Clear();
                        UserRole.Clear();
                        loadData();
                    }
            });
            StopCommand = new RelayCommand<User>(p =>
            {
                if (UserSeleted != null)
                    if (UserSeleted.Status.ToUpper().Contains("ĐANG"))
                        return true;
                return false;
            }, p =>
            {
                if (UserSeleted != null)
                    try
                    {
                        con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        string sql = string.Format("exec usp_Update_Status_User {0},N'Ngừng hoạt động'", UserSeleted.Id);
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        _toast.ShowSuccess(string.Format("Cật nhật trạng thái người dùng {0} thành công!", UserSeleted.DisplayName));



                    }
                    catch (SqlException ex)
                    {
                        _toast.ShowError("Thao tác không thành công!, lỗi: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                        List.Clear();
                        UserRole.Clear();
                        loadData();
                    }
            });
            RefreshCommand = new RelayCommand<User>((p) =>
            {
                return true;
            }, (p) =>
            {

                RadioStatusAll = true;
                Search = null;
            });
            ExportCommand = new RelayCommand<Supplier>((p) =>
            {
                if (List != null && List.ToArray().Length > 0)
                    return true;
                else return false;
            }, (p) =>
            {
                try
                {
                    var dataSet = new DataSet();
                    var dataTable = new DataTable();
                    dataSet.Tables.Add(dataTable);

                    // we assume that the properties of DataSourceVM are the columns of the table
                    // you can also provide the type via the second parameter

                    dataTable.Columns.Add("Tài khoản");
                    dataTable.Columns.Add("Tên người dùng");
                    dataTable.Columns.Add("Vai trò");
                    dataTable.Columns.Add("Ngày sinh");
                    dataTable.Columns.Add("Giới tính");
                    dataTable.Columns.Add("Địa chỉ");
                    dataTable.Columns.Add("Điện thoại");
                    dataTable.Columns.Add("Email");
                    dataTable.Columns.Add("Thông tin khác");
                    dataTable.Columns.Add("Trạng thái");


                    foreach (var user in List)
                    {
                        var newRow = dataTable.NewRow();

                        // fill the properties into the cells
                        newRow["Tài khoản"] = user.UserName;
                        newRow["Tên người dùng"] = user.DisplayName;
                        newRow["Vai trò"] = user.UserRole == null ? "" : user.UserRole.DisplayName;
                        newRow["Ngày sinh"] = user.BirthDay;
                        newRow["Giới tính"] = user.Sex;
                        newRow["Địa chỉ"] = user.Address;
                        newRow["Điện thoại"] = user.Phone;
                        newRow["Email"] = user.Email;
                        newRow["Thông tin khác"] = user.MoreInfo;
                        newRow["Trạng thái"] = user.Status;
                        dataTable.Rows.Add(newRow);
                    }
                    // Do excel export

                    ExportViewModel editViewModel = new ExportViewModel(dataTable, "DanhSachNguoiDung", "#8a2be2");
                    ExportView editView = new ExportView();
                    editView.DataContext = editViewModel;
                    editViewModel.Title = "Xuất file danh sách người dùng";
                    editView.ShowDialog();
                }
                catch (Exception e1)
                {
                    _toast.ShowError("Thao tác không thành công!, lỗi: " + e1.Message);
                }


            });
        }


        private void loadData()
        {


            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                cmd = new SqlCommand("exec usp_View_User", con);
                SqlCommand cmd2 = new SqlCommand("select * from uv_View_UserRole", con);
                adapter = new SqlDataAdapter(cmd);
                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                ds = new DataSet();
                ds2 = new DataSet();
                adapter.Fill(ds);
                adapter2.Fill(ds2);
                if (List == null)
                    List = new ObservableCollection<User>();
                else
                    List.Clear();
                if (UserRole == null)
                    UserRole = new ObservableCollection<UserRole>();
                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    UserRole role = new UserRole
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString(),
                        RolePermision = dr[2].ToString()
                    };
                    UserRole.Add(role);
                }

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(dr[12].ToString()) == 0)
                    {
                        User user = new User
                        {
                            Id = Convert.ToInt32(dr[0].ToString()),
                            DisplayName = dr[1].ToString(),
                            BirthDay = DBNull.Value.Equals(dr[2]) ? null : (DateTime?)Convert.ToDateTime(dr[2].ToString()),
                            Sex = dr[3].ToString(),
                            Address = dr[4].ToString(),
                            Phone = dr[5].ToString(),
                            Email = dr[6].ToString(),
                            MoreInfo = dr[7].ToString(),
                            UserName = dr[8].ToString(),
                            Password = dr[9].ToString(),
                            IdRole = Convert.ToInt32(dr[10].ToString()),
                            Status = dr[11].ToString(),
                            IsVisible = Convert.ToInt32(dr[12].ToString()),
                            IsSelected = false,
                            UserRole = UserRole.FirstOrDefault(x => x.Id == Convert.ToInt32(dr[10].ToString()))
                        };
                        List.Add(user);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ds = null;
                adapter.Dispose();
                con.Close();
                con.Dispose();
            }
        }

    }
}