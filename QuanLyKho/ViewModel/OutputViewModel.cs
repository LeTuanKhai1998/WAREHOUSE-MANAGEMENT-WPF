using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;
using Object = QuanLyKho.Model.Object;
namespace QuanLyKho.ViewModel
{

    public class OutputViewModel : BaseViewModel
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter, adapter2, adapter3;
        DataSet ds, ds2;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private ToastViewModel _toast = null;
        private ObservableCollection<Output> _List;
        public ObservableCollection<Output> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Customer> _Customer;
        public ObservableCollection<Customer> Customer { get => _Customer; set { _Customer = value; OnPropertyChanged(); } }

        private ObservableCollection<User> _User;
        public ObservableCollection<User> User { get => _User; set { _User = value; OnPropertyChanged(); } }
        private ObservableCollection<OutputInfo> _OutputInfo;
        public ObservableCollection<OutputInfo> OutputInfo { get => _OutputInfo; set { _OutputInfo = value; OnPropertyChanged(); } }
        private ObservableCollection<Object> _Object;
        public ObservableCollection<Object> Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }
        private ObservableCollection<Unit> _Unit;
        public ObservableCollection<Unit> Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.OutputInfoView> _ListOutputInfo;
        public ObservableCollection<Model.OutputInfoView> ListOutputInfo { get => _ListOutputInfo; set { _ListOutputInfo = value; OnPropertyChanged(); } }

        public ICollectionView ItemsView { get { return CollectionViewSource.GetDefaultView(List); } }


        //Lọc dữ liệu
        private bool Filter(object item)
        {
            Output obj = item as Output;

            bool[] kq = new bool[6];
            int i = 0;
            if (CheckBoxDone == false && CheckBoxCancel == false) return false;
            else
            if (CheckBoxDone == true && CheckBoxCancel == true && string.IsNullOrEmpty(OutputSearch) && string.IsNullOrEmpty(ObjectSearch) && string.IsNullOrEmpty(CustomerSearch) && string.IsNullOrEmpty(UserSearch)) return true;

            if (!string.IsNullOrEmpty(OutputSearch))
            {
                if (obj.Id.ToUpper().Contains(OutputSearch.ToUpper()))
                {
                    kq[0] = true;

                }
                i++;
            }
            if (!string.IsNullOrEmpty(ObjectSearch))
            {
                if (Object.FirstOrDefault(y => y.DisplayName.ToUpper().Contains(ObjectSearch.ToUpper()) || y.Id.Contains(ObjectSearch.ToUpper())) != null)
                    if (OutputInfo.FirstOrDefault(x => x.IdOutput == obj.Id && x.IdObject == Object.FirstOrDefault(y => y.DisplayName.ToUpper().Contains(ObjectSearch.ToUpper()) || y.Id.Contains(ObjectSearch.ToUpper())).Id) != null)
                    {
                        kq[1] = true;
                    }
                i++;

            }
            if (!string.IsNullOrEmpty(CustomerSearch))
            {
                if (obj.Customer.DisplayName.ToUpper().Contains(CustomerSearch.ToUpper()) || obj.Customer.Id.ToString().ToUpper().Contains(CustomerSearch.ToUpper()) || obj.Customer.Phone.Contains(CustomerSearch))
                {
                    kq[2] = true;

                }
                i++;
            }
            if (!string.IsNullOrEmpty(UserSearch))
            {
                if (obj.User.DisplayName.ToUpper().Contains(UserSearch.ToUpper()) || obj.User.Id.ToString().ToUpper().Contains(UserSearch.ToUpper()))
                {
                    kq[3] = true;

                }
                i++;
            }


            if (CheckBoxDone == true)
            {
                if (obj.Status.Contains("thành"))
                {
                    kq[4] = true;
                    i++;
                }
            }
            if (CheckBoxCancel == true)
            {
                if (obj.Status.Contains("hủy"))
                {
                    kq[5] = true;
                    i++;
                }
            }


            int dem = 0;
            foreach (bool b in kq)
                if (b == true) dem++;
            if (i == dem && i != 0) return true;
            return false;

        }




        private string _OutputSearch;
        public string OutputSearch
        {
            get { return _OutputSearch; }
            set
            {
                _OutputSearch = value;
                OnPropertyChanged();
                ItemsView.Refresh(); // required    
            }
        }

        private string _ObjectSearch;
        public string ObjectSearch
        {
            get { return _ObjectSearch; }
            set
            {
                _ObjectSearch = value;
                OnPropertyChanged();
                ItemsView.Refresh(); // required    
            }
        }

        private string _CustomerSearch;
        public string CustomerSearch
        {
            get { return _CustomerSearch; }
            set
            {
                _CustomerSearch = value;
                OnPropertyChanged();
                ItemsView.Refresh(); // required    
            }
        }

        private string _UserSearch;
        public string UserSearch
        {
            get { return _UserSearch; }
            set
            {
                _UserSearch = value;
                OnPropertyChanged();
                ItemsView.Refresh(); // required    
            }
        }

        private bool _CheckBoxDone;
        public bool CheckBoxDone
        {
            get { return this._CheckBoxDone; }
            set
            {
                this._CheckBoxDone = value;
                this.OnPropertyChanged("CheckBoxDone");
                ItemsView.Refresh();
            }
        }
        private bool _CheckBoxCancel;
        public bool CheckBoxCancel
        {
            get { return this._CheckBoxCancel; }
            set
            {
                this._CheckBoxCancel = value;
                this.OnPropertyChanged("CheckBoxCancel");
                ItemsView.Refresh();
            }
        }



        private Output _SelectedItem;

        public Output SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (value != null)
                {

                    AddListPut(value);
                    OutputInfoViewModel editViewModel = new OutputInfoViewModel(value);
                    View.OutputInfoView editView = new View.OutputInfoView();
                    editView.DataContext = editViewModel;
                    editViewModel.List = ListOutputInfo;
                    editViewModel.TotalFinal = value.TotalPrice - value.Discount;
                    editViewModel.TotalQuantity = ListOutputInfo.Sum(x => x.Quantity);
                    editView.ShowDialog();
                    _SelectedItem = null;
                    List.Clear();
                    Customer.Clear();
                    User.Clear();
                    OutputInfo.Clear();
                    Object.Clear();
                    Unit.Clear();
                    if (ListOutputInfo != null)
                        ListOutputInfo.Clear();
                    loadData();
                }

            }
        }

        void AddListPut(Output item)
        {
            if (ListOutputInfo == null)
                ListOutputInfo = new ObservableCollection<Model.OutputInfoView>();
            else
                ListOutputInfo.Clear();
            foreach (Object o in Object)
            {
                OutputInfo outputInfo = OutputInfo.FirstOrDefault(x => x.IdOutput == item.Id && x.IdObject == o.Id);
                int i = 1;
                if (outputInfo != null)
                {
                    Model.OutputInfoView output = new Model.OutputInfoView
                    {
                        LinkImage = o.LinkImage,
                        Id = o.Id,
                        DisplayName = o.DisplayName,
                        Quantity = outputInfo.Quantity,
                        Price = outputInfo.Price,
                        Discount = outputInfo.Discount == null ? (double)0 : outputInfo.Discount,
                        TotalPrice = (double)((double)outputInfo.Price * (double)outputInfo.Quantity - outputInfo.Discount),
                        STT = i,
                    };
                    ListOutputInfo.Add(output);
                    i++;
                }

            }
        }


        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ExportCommand { get; set; }
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
        public OutputViewModel()
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
            progressBarViewModel.Title = "Đang tải danh sách hóa đơn, vui lòng chờ...";
            progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffcc00"));
            progressBarView.ShowDialog();


            _toast = new ToastViewModel(Corner.BottomRight, 4, 10, 20);

            //loadData();
            CheckBoxDone = true;

            if (List != null)
            {
                this.ItemsView.Filter = Filter;
            }

            AddCommand = new RelayCommand<OutputInfo>((p) =>
            {

                return true;
            }, (p) =>
            {
                OutputEditViewModel editViewModel = new OutputEditViewModel();
                OutputEditView editView = new OutputEditView();
                editView.DataContext = editViewModel;
                editViewModel.List = new ObservableCollection<Model.OutputInfoView>();
                editViewModel.Output = new Output()
                {
                    Id = "",
                    IdUser = LoginViewModel.userCurrent.Id,
                    IdCustomer = 0,
                    DateOutput = null,
                    Discount = 0,
                    Payment = "",
                    TotalPrice = 0,
                    Note = "",
                    Status = "",
                    Customer = null,
                    User = User.FirstOrDefault(x => x.Id == LoginViewModel.userCurrent.Id),
                };
                editViewModel.OutputDiscount = 0;
                editViewModel.currentIndex = 0;
                editViewModel.TotalFinal = 0;
                editView.ShowDialog();
                List.Clear();
                Customer.Clear();
                User.Clear();
                OutputInfo.Clear();
                Object.Clear();
                Unit.Clear();
                if (ListOutputInfo != null)
                    ListOutputInfo.Clear();
                loadData();
              
            });







            RefreshCommand = new RelayCommand<OutputInfo>((p) =>
            {
                return true;
            }, (p) =>
            {
                OutputSearch = null;
                ObjectSearch = null;
                CustomerSearch = null;
                UserSearch = null;
                CheckBoxCancel = false;
                CheckBoxDone = true;



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

                    dataTable.Columns.Add("Mã hóa đơn");
                    dataTable.Columns.Add("Tên người lập");
                    dataTable.Columns.Add("Tên khách hàng");
                    dataTable.Columns.Add("Ngày lập");
                    dataTable.Columns.Add("Giảm giá");
                    dataTable.Columns.Add("Thanh toán");
                    dataTable.Columns.Add("Tổng tiền(vnđ)");
                    dataTable.Columns.Add("Ghi chú");
                    dataTable.Columns.Add("Trạng thái");

                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                    foreach (var inp in List)
                    {
                        var newRow = dataTable.NewRow();

                        // fill the properties into the cells
                        newRow["Mã hóa đơn"] = inp.Id;
                        newRow["Tên người lập"] = inp.User == null ? "" : inp.User.DisplayName;
                        newRow["Tên khách hàng"] = inp.Customer== null ? "Khách lẻ" : inp.Customer.DisplayName;
                        newRow["Ngày lập"] = inp.DateOutput.ToString();
                        newRow["Giảm giá"] = inp.Discount;
                        newRow["Thanh toán"] = inp.Payment;
                        newRow["Tổng tiền(vnđ)"] = double.Parse(inp.TotalPrice.ToString()).ToString("#,###", cul.NumberFormat);
                        newRow["Ghi chú"] = inp.Note;
                        newRow["Trạng thái"] = inp.Status;
                        dataTable.Rows.Add(newRow);
                    }
                    // Do excel export

                    ExportViewModel editViewModel = new ExportViewModel(dataTable, "DanhSachNguoiDung", "#ffcc00");
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
                cmd = new SqlCommand("exec usp_View_Customer", con);
                SqlCommand cmd2 = new SqlCommand("exec usp_View_Output", con);
                SqlCommand cmd3 = new SqlCommand("exec usp_View_User", con);
                SqlCommand cmd4 = new SqlCommand("exec usp_View_OutputInfo", con);
                SqlCommand cmd5 = new SqlCommand("exec usp_View_Object", con);
                SqlCommand cmd6 = new SqlCommand("exec usp_View_Unit", con);
                adapter = new SqlDataAdapter(cmd);
                adapter2 = new SqlDataAdapter(cmd2);
                adapter3 = new SqlDataAdapter(cmd3);
                SqlDataAdapter adapter4 = new SqlDataAdapter(cmd4);
                SqlDataAdapter adapter5 = new SqlDataAdapter(cmd5);
                SqlDataAdapter adapter6 = new SqlDataAdapter(cmd6);
                ds = new DataSet();
                ds2 = new DataSet();
                DataSet ds3 = new DataSet();
                DataSet ds4 = new DataSet();
                DataSet ds5 = new DataSet();
                DataSet ds6 = new DataSet();
                adapter.Fill(ds);
                adapter2.Fill(ds2);
                adapter3.Fill(ds3);
                adapter4.Fill(ds4);
                adapter5.Fill(ds5);
                adapter6.Fill(ds6);
                if (List == null)
                    List = new ObservableCollection<Output>();
                if (Customer == null)
                    Customer = new ObservableCollection<Customer>();
                if (User == null)
                    User = new ObservableCollection<User>();
                if (OutputInfo == null)
                    OutputInfo = new ObservableCollection<OutputInfo>();
                if (Object == null)
                    Object = new ObservableCollection<Object>();
                if (Unit == null)
                    Unit = new ObservableCollection<Unit>();
                foreach (DataRow dr in ds6.Tables[0].Rows)
                {
                    Unit unit = new Unit
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString()
                    };
                    Unit.Add(unit);

                }

                foreach (DataRow dr in ds.Tables[0].Rows)
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
                        LinkImage = CustomerViewModel.path + dr[11].ToString(),
                    };
                    if (customer.LinkImage.Contains("null") || DBNull.Value.Equals(dr[11]))
                        customer.LinkImage = CustomerViewModel.path + "IMG_Holder.png";
                    Customer.Add(customer);
                }

                foreach (DataRow dr in ds3.Tables[0].Rows)
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
                        IsVisible = Convert.ToInt32(dr[12].ToString())
                    };
                    User.Add(user);
                }

                foreach (DataRow dr in ds2.Tables[0].Rows)
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
                            LinkImage = CustomerViewModel.path + "IMG_Holder.png"
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
                        Customer = DBNull.Value.Equals(dr[2]) ? customer : Customer.FirstOrDefault(x => x.Id == (DBNull.Value.Equals(dr[2]) ? null : (int?)Convert.ToInt32(dr[2].ToString()))),
                        User = User.FirstOrDefault(x => x.Id == Convert.ToInt32(dr[1].ToString()))

                    };
                    List.Add(output);
                }

                foreach (DataRow dr in ds5.Tables[0].Rows)
                {
                    if (Convert.ToInt32(dr[6].ToString()) > 0)
                    {
                        Object objects = new Object
                        {
                            Id = dr[0].ToString(),
                            DisplayName = dr[1].ToString(),
                            IdUnit = DBNull.Value.Equals(dr[2]) ? 0 : Convert.ToInt32(dr[2].ToString()),
                            IdCategory = Convert.ToInt32(dr[3].ToString()),
                            InputPrice = Convert.ToDouble(dr[4].ToString()),
                            OutputPrice = Convert.ToDouble(dr[5].ToString()),
                            Count = Convert.ToInt32(dr[6].ToString()),
                            IdPosition = DBNull.Value.Equals(dr[7]) ? 0 : Convert.ToInt32(dr[7].ToString()),
                            LinkImage = ObjectViewModel.path + dr[8].ToString(),
                            Status = dr[9].ToString(),
                            IsVisible = Convert.ToInt32(dr[10].ToString()),
                            Unit = Unit.FirstOrDefault(x => x.Id == (DBNull.Value.Equals(dr[2]) ? 0 : Convert.ToInt32(dr[2].ToString()))),
                        };
                        if (objects.LinkImage.Contains("null") || DBNull.Value.Equals(dr[8]))
                            objects.LinkImage = ObjectViewModel.path + "IMG_Holder.png";
                        Object.Add(objects);
                    }
                }

                foreach (DataRow dr in ds4.Tables[0].Rows)
                {
                    OutputInfo outputInfo = new OutputInfo
                    {
                        IdOutput = dr[0].ToString(),
                        IdObject = dr[1].ToString(),
                        Price = Convert.ToDouble(dr[2].ToString()),
                        Quantity = Convert.ToInt32(dr[3].ToString()),
                        Discount = DBNull.Value.Equals(dr[4]) ? 0 : Convert.ToDouble(dr[4])
                    };
                    OutputInfo.Add(outputInfo);

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
                adapter.Dispose();
                adapter2.Dispose();
                con.Close();
                con.Dispose();
            }
        }

    }
}
