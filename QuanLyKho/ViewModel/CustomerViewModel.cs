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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    public class CustomerViewModel : BaseViewModel
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter, adapter2, adapter3;
        DataSet ds, ds2, ds3;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private ToastViewModel _toast = null;
        public static String path = Constants.CustomerImagesPath;
        private ObservableCollection<Model.Output> _Output;
        public ObservableCollection<Model.Output> Output { get => _Output; set { _Output = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.Output> _ListOutput;
        public ObservableCollection<Model.Output> ListOutput { get => _ListOutput; set { _ListOutput = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.User> _User;
        public ObservableCollection<Model.User> User { get => _User; set { _User = value; OnPropertyChanged(); } }

        private ObservableCollection<Customer> _List;
        public ObservableCollection<Customer> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        public ICollectionView ItemsView { get { return CollectionViewSource.GetDefaultView(List); } }




        //Lọc dữ liệu
        private bool Filter(object item)
        {
            Customer sup = item as Customer;

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
                SelectAll(value, List);
                OnPropertyChanged();
            }
        }

        private void SelectAll(bool? select, ObservableCollection<Customer> models)
        {
            if (select == false || select == true)
                foreach (var model in models)
                {
                    model.IsSelected = (bool)select;
                }

        }

        private Customer _SelectedItem;
        public Customer SelectedItem
        {
            get => _SelectedItem;
            set
            {

                _SelectedItem = value;
                OnPropertyChanged();

                if (value != null)
                {
                    AddListOutput(value);
                    //CustomerInfoViewModel editViewModel = new CustomerInfoViewModel(value);
                    //CustomerViewInfoWindow editView = new CustomerViewInfoWindow();
                    //editView.DataContext = editViewModel;
                    //editViewModel.List = ListOutput;
                    //editViewModel.ListCustomer = List;
                    //editView.ShowDialog();
                    //_SelectedItem = null;
                    //List.Clear();
                    //Output.Clear();
                    //User.Clear();
                    //loadData();
                }


            }
        }

        void AddListOutput(Customer item)
        {
            if (item != null)
            {
                if (ListOutput == null)
                    ListOutput = new ObservableCollection<Output>();
                else
                    ListOutput.Clear();
                foreach (Output i in Output)
                    if (i.IdCustomer == item.Id && i.Status.ToUpper().Contains("Hoàn thành".ToUpper()))
                    {
                        Output Output = new Output
                        {
                            Id = i.Id,
                            IdUser = i.IdUser,
                            IdCustomer = i.IdCustomer,
                            DateOutput = i.DateOutput,
                            Discount = i.Discount,
                            Payment = i.Payment,
                            TotalPrice = i.TotalPrice,
                            Note = i.Note,
                            Status = i.Status,
                            Customer = i.Customer,
                            User = i.User
                        };

                        ListOutput.Add(Output);
                    }
            }

        }

        private int? _CountSelected;
        public int? CountSelected { get { return _CountSelected; } set { _CountSelected = value; OnPropertyChanged(); } }
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
        private Visibility _Stop = Visibility.Visible;
        public Visibility Stop { get => _Stop; set { _Stop = value; OnPropertyChanged(); } }
        private Visibility _Business = Visibility.Visible;
        public Visibility Business { get => _Business; set { _Business = value; OnPropertyChanged(); } }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand DeleteOneCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand BusinessCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ExportCommand { get; set; }


        public CustomerViewModel(int x)
        {

        }


        Customer kiemTraChonXoa()
        {
            return List.FirstOrDefault(p => p.IsSelected == true);
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
        public CustomerViewModel()
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
            progressBarViewModel.Title = "Đang tải danh sách khách hàng, vui lòng chờ...";
            progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff5349"));
            progressBarView.ShowDialog();


            _toast = new ToastViewModel(Corner.BottomRight, 4, 10, 20);
            //loadData();
            RadioStatusAll = true;
            SelectedItem = null;
            if (List != null)
            {
                this.ItemsView.Filter = Filter;
            }
            AddCommand = new RelayCommand<Customer>(
                         p => { return true; },
                         p =>
                         {
                             Customer sup = new Customer
                             {
                                 Id = 0,
                                 DisplayName = "",
                                 BirthDay = null,
                                 Sex = "Nam",
                                 Address = "",
                                 Phone = "",
                                 Email = "",
                                 MoreInfo = "",
                                 ContractDate = DateTime.Now,
                                 Status = "Đang hoạt động",
                                 IsVisible = 0,
                                 LinkImage = path + "IMG_Holder.png"
                             };
                             CustomerEditViewModel editViewModel = new CustomerEditViewModel(sup);
                             CustomerEditView editView = new CustomerEditView();
                             editView.DataContext = editViewModel;
                             editViewModel.Title = "Thêm khách hàng";
                             editViewModel.RadioMale = true;
                             editView.ShowDialog();
                             List.Clear();
                             Output.Clear();
                             User.Clear();
                             loadData();

                         });




            EditCommand = new RelayCommand<Customer>(
                           p => { return true; },
                           p =>
                           {
                               CustomerEditViewModel editViewModel = new CustomerEditViewModel(SelectedItem);
                               CustomerEditView editView = new CustomerEditView();
                               editView.DataContext = editViewModel;
                               editViewModel.Title = "Cật nhật nhà cung cấp";
                               if (SelectedItem.Sex.Contains("Nam"))
                                   editViewModel.RadioMale = true;
                               else
                                   editViewModel.RadioFeMale = true;
                               editView.ShowDialog();
                               List.Clear();
                               Output.Clear();
                               User.Clear();
                               loadData();
                           }
            );








            DeleteCommand = new RelayCommand<Category>((p) =>
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
                Customer objXoa = kiemTraChonXoa();

                string tex = "";
                int count = List.Count(x => x.IsSelected == true);
                if (count == 1)
                    tex = "Hệ thống sẽ xóa hoàn toàn khách hàng " + objXoa.DisplayName + " nhưng vẫn giữ thông tin khách hàng trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";
                else
                    tex = "Hệ thống sẽ xóa hoàn toàn " + count + " khách hàng bạn đã chọn nhưng vẫn giữ thông tin khách hàng trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";

                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Xóa khách hàng";
                editViewModel.Content = tex;
                editView.ShowDialog();
                if (editViewModel.Result == true)
                    try
                    {
                        SqlConnection con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        foreach (Customer o in List)
                            if (o.IsSelected == true)
                            {
                                string texSql = "exec usp_Delete_Customer " + o.Id;
                                cmd = new SqlCommand(texSql, con);
                                cmd.ExecuteNonQuery();
                            }

                        List.Clear();
                        Output.Clear();
                        User.Clear();
                        loadData();
                        _toast.ShowSuccess("Đã xóa thành công danh sách khách hàng đã chọn!");

                    }
                    catch (TransactionAbortedException ex)
                    {
                        // Log error       
                        _toast.ShowError("Thao tác không thành công! lỗi: " + ex.Message);

                    }
                    finally
                    {
                        con.Close();
                    }
            });

            DeleteOneCommand = new RelayCommand<Category>((p) =>
            {
                return true;
            }, (p) =>
            {

                string tex = "Hệ thống sẽ xóa hoàn toàn khách hàng " + SelectedItem.DisplayName + " nhưng vẫn giữ thông tin khách hàng trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";

                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Xóa khách hàng";
                editViewModel.Content = tex;
                editView.ShowDialog();
                if (editViewModel.Result == true)
                    try
                    {
                        SqlConnection con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        string texSql = "exec usp_Delete_Customer " + SelectedItem.Id;
                        cmd = new SqlCommand(texSql, con);
                        cmd.ExecuteNonQuery();

                        string name = SelectedItem.DisplayName;


                        List.Clear();
                        Output.Clear();
                        User.Clear();
                        loadData();
                        _toast.ShowSuccess(string.Format("Đã xóa khách hàng {0} thành công!", name));

                    }
                    catch (TransactionAbortedException ex)
                    {
                        // Log error       
                        _toast.ShowError("Thao tác không thành công! lỗi: " + ex.Message);

                    }
                    finally
                    {
                        con.Close();
                    }
            });

            BusinessCommand = new RelayCommand<User>(p =>
            {
                if (SelectedItem != null)
                    if (SelectedItem.Status.ToUpper().Contains("ĐANG"))
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
                if (SelectedItem != null)
                    try
                    {
                        con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        string sql = string.Format("exec usp_Update_Status_Customer {0},N'Đang hoạt động'", SelectedItem.Id);
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        _toast.ShowSuccess(string.Format("Cật nhật trạng thái nhà cung cấp {0} thành công!", SelectedItem.DisplayName));
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
                        Output.Clear();
                        User.Clear();
                        loadData();
                    }
            });
            StopCommand = new RelayCommand<User>(p =>
            {
                if (SelectedItem != null)
                    if (SelectedItem.Status.ToUpper().Contains("ĐANG"))
                        return true;
                return false;
            }, p =>
            {
                if (SelectedItem != null)
                    try
                    {
                        con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        string sql = string.Format("exec usp_Update_Status_Customer {0},N'Ngừng hoạt động'", SelectedItem.Id);
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        _toast.ShowSuccess(string.Format("Cật nhật trạng thái khách hàng {0} thành công!", SelectedItem.DisplayName));
                    }
                    catch (SqlException ex)
                    {
                        _toast.ShowError("Thao tác không tành công!, lỗi: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                        List.Clear();
                        List.Clear();
                        Output.Clear();
                        User.Clear();
                        loadData();
                    }
            });
            RefreshCommand = new RelayCommand<Customer>((p) =>
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

                    dataTable.Columns.Add("Mã khách hàng");
                    dataTable.Columns.Add("Tên khách hàng");
                    dataTable.Columns.Add("Ngày sinh");
                    dataTable.Columns.Add("Giới tính");
                    dataTable.Columns.Add("Địa chỉ");
                    dataTable.Columns.Add("Điện thoại");
                    dataTable.Columns.Add("Email");
                    dataTable.Columns.Add("Ngày liên hệ");
                    dataTable.Columns.Add("Thông tin khác");
                    dataTable.Columns.Add("Trạng thái");


                    foreach (var cus in List)
                    {
                        var newRow = dataTable.NewRow();

                        // fill the properties into the cells
                        newRow["Mã khách hàng"] = cus.Id;
                        newRow["Tên khách hàng"] = cus.DisplayName;
                        newRow["Ngày sinh"] = cus.BirthDay.ToString();
                        newRow["Giới tính"] = cus.Sex;
                        newRow["Địa chỉ"] = cus.Address;
                        newRow["Điện thoại"] = cus.Phone;
                        newRow["Email"] = cus.Email;
                        newRow["Ngày liên hệ"] = cus.ContractDate;
                        newRow["Thông tin khác"] = cus.MoreInfo;
                        newRow["Trạng thái"] = cus.Status;
                        dataTable.Rows.Add(newRow);
                    }

                    // Do excel export

                    ExportViewModel editViewModel = new ExportViewModel(dataTable, "DanhSachKhachHang", "#ff5349");
                    ExportView editView = new ExportView();
                    editView.DataContext = editViewModel;
                    editViewModel.Title = "Xuất file danh sách khách hàng";
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
                cmd = new SqlCommand("exec usp_View_Customer", con);
                SqlCommand cmd2 = new SqlCommand("select * from uv_View_User", con);
                SqlCommand cmd3 = new SqlCommand("select * from uv_View_Output", con);
                //cmd = new SqlCommand("exec usp_View_Customer", con);
                //SqlCommand cmd2 = new SqlCommand("exec usp_View_User", con);
                //SqlCommand cmd3 = new SqlCommand("exec usp_View_Output", con);
                adapter = new SqlDataAdapter(cmd);
                adapter2 = new SqlDataAdapter(cmd2);
                adapter3 = new SqlDataAdapter(cmd3);
                ds = new DataSet();
                ds2 = new DataSet();
                ds3 = new DataSet();
                adapter.Fill(ds);
                adapter2.Fill(ds2);
                adapter3.Fill(ds3);
                if (List == null)
                    List = new ObservableCollection<Customer>();
                else
                    List.Clear();
                if (Output == null)
                    Output = new ObservableCollection<Output>();
                else
                    Output.Clear();
                if (User == null)
                    User = new ObservableCollection<User>();
                else
                    User.Clear();



                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    User user = new User
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString(),
                        BirthDay = DBNull.Value.Equals(dr[2]) ? Convert.ToDateTime(DateTime.Now.ToString()) : Convert.ToDateTime(dr[2].ToString()),
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
                    };
                    User.Add(user);
                }

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(dr[10].ToString()) == 0)
                    {
                        Customer customer = new Customer
                        {
                            Id = Convert.ToInt32(dr[0].ToString()),
                            DisplayName = dr[1].ToString(),
                            BirthDay = DBNull.Value.Equals(dr[2]) ? Convert.ToDateTime(DateTime.Now.ToString()) : Convert.ToDateTime(dr[2].ToString()),
                            Sex = dr[3].ToString(),
                            Address = dr[4].ToString(),
                            Phone = dr[5].ToString(),
                            Email = dr[6].ToString(),
                            MoreInfo = dr[7].ToString(),
                            ContractDate = DBNull.Value.Equals(dr[2]) ? Convert.ToDateTime(DateTime.Now.ToString()) : Convert.ToDateTime(dr[8].ToString()),
                            Status = dr[9].ToString(),
                            IsVisible = Convert.ToInt32(dr[10].ToString()),
                            LinkImage = path + dr[11].ToString(),
                            IsSelected = false
                        };
                        if (customer.LinkImage.Contains("null") || DBNull.Value.Equals(dr[11]) || string.IsNullOrEmpty(dr[11].ToString()))
                            customer.LinkImage = path + "IMG_Holder.png";

                        List.Add(customer);
                    }

                }

                foreach (DataRow dr in ds3.Tables[0].Rows)
                {
                    Customer customer = null;
                    if (DBNull.Value.Equals(dr[2]))
                    {
                        customer = new Customer
                        {
                            Id = null,
                            DisplayName = "Khách lẻ",
                            BirthDay = null,
                            Sex = null,
                            Address = null,
                            Phone = null,
                            Email = null,
                            MoreInfo = null,
                            ContractDate = null,
                            Status = null,
                            IsVisible = 0,
                            LinkImage = path + "IMG_Holder.png"
                        };
                    }
                    Output output = new Output
                    {
                        Id = dr[0].ToString(),
                        IdUser = Convert.ToInt32(dr[1].ToString()),
                        IdCustomer = DBNull.Value.Equals(dr[2]) ? null : (int?)Convert.ToInt32(dr[2].ToString()),
                        DateOutput = Convert.ToDateTime(dr[3].ToString()),
                        Discount = DBNull.Value.Equals(dr[4]) ? (double)0 : (float)Convert.ToDouble(dr[4].ToString()),
                        Payment = dr[5].ToString(),
                        TotalPrice = (float)Convert.ToDouble(dr[6].ToString()),
                        Note = dr[7].ToString(),
                        Status = dr[8].ToString(),
                        Customer = DBNull.Value.Equals(dr[2]) ? customer : List.FirstOrDefault(x => x.Id == (DBNull.Value.Equals(dr[2]) ? null : (int?)Convert.ToInt32(dr[2].ToString()))),
                        User = User.FirstOrDefault(x => x.Id == Convert.ToInt32(dr[1].ToString()))

                    };

                    Output.Add(output);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ds = null;
                ds2 = null;
                ds3 = null;
                adapter.Dispose();
                adapter2.Dispose();
                adapter3.Dispose();
                con.Close();
                con.Dispose();
            }
        }

    }
}

