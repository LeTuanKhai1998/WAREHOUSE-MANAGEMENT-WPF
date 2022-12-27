using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ToastNotifications.Position;
using Object = QuanLyKho.Model.Object;
using System.Windows.Media;

namespace QuanLyKho.ViewModel
{
    public class InputViewModel : BaseViewModel
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter, adapter2, adapter3;
        DataSet ds, ds2;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private ToastViewModel _toast = null;
        private ObservableCollection<Input> _List;
        public ObservableCollection<Input> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Supplier> _Supplier;
        public ObservableCollection<Supplier> Supplier { get => _Supplier; set { _Supplier = value; OnPropertyChanged(); } }

        private ObservableCollection<User> _User;
        public ObservableCollection<User> User { get => _User; set { _User = value; OnPropertyChanged(); } }
        private ObservableCollection<InputInfo> _InputInfo;
        public ObservableCollection<InputInfo> InputInfo { get => _InputInfo; set { _InputInfo = value; OnPropertyChanged(); } }
        private ObservableCollection<Object> _Object;
        public ObservableCollection<Object> Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }
        private ObservableCollection<Unit> _Unit;
        public ObservableCollection<Unit> Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.InputInfoView> _ListInputInfo;
        public ObservableCollection<Model.InputInfoView> ListInputInfo { get => _ListInputInfo; set { _ListInputInfo = value; OnPropertyChanged(); } }

        public ICollectionView ItemsView { get { return CollectionViewSource.GetDefaultView(List); } }


        //Lọc dữ liệu
        private bool Filter(object item)
        {
            Input obj = item as Input;

            bool[] kq = new bool[7];
            int i = 0;
            if (CheckBoxTemp == false && CheckBoxDone == false && CheckBoxCancel == false) return false;
            if (CheckBoxTemp == true && CheckBoxDone == true && CheckBoxCancel == true && string.IsNullOrEmpty(InputSearch) && string.IsNullOrEmpty(ObjectSearch) && string.IsNullOrEmpty(SupplierSearch) && string.IsNullOrEmpty(UserSearch)) return true;

            if (!string.IsNullOrEmpty(InputSearch))
            {
                if (obj.Id.ToUpper().Contains(InputSearch.ToUpper()))
                {
                    kq[0] = true;

                }
                i++;
            }
            if (!string.IsNullOrEmpty(ObjectSearch))
            {
                if (Object.FirstOrDefault(y => y.DisplayName.ToUpper().Contains(ObjectSearch.ToUpper()) || y.Id.Contains(ObjectSearch.ToUpper())) != null)
                    if (InputInfo.FirstOrDefault(x => x.IdInput == obj.Id && x.IdObject == Object.FirstOrDefault(y => y.DisplayName.ToUpper().Contains(ObjectSearch.ToUpper()) || y.Id.Contains(ObjectSearch.ToUpper())).Id) != null)
                    {
                        kq[1] = true;
                    }
                i++;

            }
            if (!string.IsNullOrEmpty(SupplierSearch))
            {
                if (obj.Supplier.DisplayName.ToUpper().Contains(SupplierSearch.ToUpper()) || obj.Supplier.Id.ToString().ToUpper().Contains(SupplierSearch.ToUpper()))
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

            if (CheckBoxTemp == true)
            {
                if (obj.Status.Contains("tạm"))
                {
                    kq[4] = true;
                    i++;
                }
            }
            if (CheckBoxDone == true)
            {
                if (obj.Status.Contains("nhập"))
                {
                    kq[5] = true;
                    i++;
                }
            }
            if (CheckBoxCancel == true)
            {
                if (obj.Status.Contains("hủy"))
                {
                    kq[6] = true;
                    i++;
                }
            }

            //if (RadioAll == false)
            //{
            //    if (RadioStock == true)
            //    {
            //        if (obj.Count > 0) kq[2] = true;

            //    }
            //    else if (RadioOut == true)
            //        if (obj.Count == 0) kq[3] = true;
            //    i++;
            //}
            int dem = 0;
            foreach (bool b in kq)
                if (b == true) dem++;
            if (i == dem && i != 0) return true;
            return false;

        }




        private string _InputSearch;
        public string InputSearch
        {
            get { return _InputSearch; }
            set
            {
                _InputSearch = value;
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

        private string _SupplierSearch;
        public string SupplierSearch
        {
            get { return _SupplierSearch; }
            set
            {
                _SupplierSearch = value;
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


        private bool _CheckBoxTemp;
        public bool CheckBoxTemp
        {
            get { return this._CheckBoxTemp; }
            set
            {
                this._CheckBoxTemp = value;
                this.OnPropertyChanged("CheckBoxTemp");
                ItemsView.Refresh();
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



        private Input _SelectedItem;

        public Input SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (value != null)
                {

                    AddListPut(value);
                    InputInfoViewModel editViewModel = new InputInfoViewModel(value);
                    View.InputInfoView editView = new View.InputInfoView();
                    editView.DataContext = editViewModel;
                    editViewModel.List = ListInputInfo;

                    editViewModel.TotalFinal = value.TotalPrice - value.Discount;
                    editView.ShowDialog();
                    _SelectedItem = null;
                    List.Clear();
                    Supplier.Clear();
                    User.Clear();
                    InputInfo.Clear();
                    Object.Clear();
                    Unit.Clear();
                    if (ListInputInfo != null)
                        ListInputInfo.Clear();
                    loadData();
                }

            }
        }

        void AddListPut(Input item)
        {
            if (ListInputInfo == null)
                ListInputInfo = new ObservableCollection<Model.InputInfoView>();
            else
                ListInputInfo.Clear();
            foreach (Object o in Object)
            {
                InputInfo inputInfo = InputInfo.FirstOrDefault(x => x.IdInput == item.Id && x.IdObject == o.Id);
                int i = 1;
                if (inputInfo != null)
                {
                    Model.InputInfoView input = new Model.InputInfoView
                    {
                        LinkImage = o.LinkImage,
                        Id = o.Id,
                        DisplayName = o.DisplayName,
                        Quantity = inputInfo.Quantity,
                        Price = inputInfo.Price,
                        Discount = inputInfo.Discount == null ? (double)0 : inputInfo.Discount,
                        TotalPrice = (double)((double)inputInfo.Price * (double)inputInfo.Quantity - inputInfo.Discount),
                        Unit = o.Unit,
                        STT = i,
                    };
                    ListInputInfo.Add(input);
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
        public InputViewModel()
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
            progressBarViewModel.Title = "Đang tải danh sách phiếu nhập, vui lòng chờ...";
            progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffcc00"));
            progressBarView.ShowDialog();


            _toast = new ToastViewModel(Corner.BottomRight, 4, 10, 20);
            //loadData();
            CheckBoxTemp = true;
            CheckBoxDone = true;

            if (List != null)
                this.ItemsView.Filter = Filter;

            AddCommand = new RelayCommand<Window>((p) =>
            {

                return true;
            }, (p) =>
            {
                InputEditViewModel editViewModel = new InputEditViewModel();
                InputEditView editView = new InputEditView();
                editView.DataContext = editViewModel;
                editViewModel.List = new ObservableCollection<Model.InputInfoView>();

                editViewModel.Input = new Input()
                {
                    Id = "",
                    IdUser = LoginViewModel.userCurrent.Id,
                    IdSupplier = null,
                    DateInput = null,
                    Discount = 0,
                    Payment = "",
                    TotalPrice = 0,
                    Note = "",
                    Status = "Phiếu tạm",
                    TotalObject = 0,
                    TotalQuantity = 0,
                    Supplier = null,
                    User = User.FirstOrDefault(x => x.Id == LoginViewModel.userCurrent.Id),
                };
                editViewModel.InputDiscount = 0;
                editViewModel.currentIndex = 0;
                editViewModel.TotalFinal = 0;
                editViewModel.isEdit = false;
                editView.ShowDialog();

                List.Clear();
                Supplier.Clear();
                User.Clear();
                InputInfo.Clear();
                Object.Clear();
                Unit.Clear();
                if (ListInputInfo != null)
                    ListInputInfo.Clear();
                loadData();

            });



            RefreshCommand = new RelayCommand<InputInfo>((p) =>
            {
                return true;
            }, (p) =>
            {
                InputSearch = null;
                ObjectSearch = null;
                SupplierSearch = null;
                UserSearch = null;
                CheckBoxCancel = false;
                CheckBoxTemp = true;
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

                    dataTable.Columns.Add("Mã phiếu nhập");
                    dataTable.Columns.Add("Tên người lập");
                    dataTable.Columns.Add("Tên nhà cung cấp");
                    dataTable.Columns.Add("Ngày nhập");
                    dataTable.Columns.Add("Giảm giá");
                    dataTable.Columns.Add("Thanh toán");
                    dataTable.Columns.Add("Tổng tiền(vnđ)");
                    dataTable.Columns.Add("Tổng số mặt hàng");
                    dataTable.Columns.Add("Tổng số lượng");
                    dataTable.Columns.Add("Ghi chú");
                    dataTable.Columns.Add("Trạng thái");

                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                    foreach (var inp in List)
                    {
                        var newRow = dataTable.NewRow();

                        // fill the properties into the cells
                        newRow["Mã phiếu nhập"] = inp.Id;
                        newRow["Tên người lập"] = inp.User == null ? "" : inp.User.DisplayName;
                        newRow["Tên nhà cung cấp"] = inp.Supplier == null ? "" : inp.Supplier.DisplayName;
                        newRow["Ngày nhập"] = inp.DateInput.ToString();
                        newRow["Giảm giá"] = inp.Discount;
                        newRow["Thanh toán"] = inp.Payment;
                        newRow["Tổng tiền(vnđ)"] = double.Parse(inp.TotalPrice.ToString()).ToString("#,###", cul.NumberFormat); 
                        newRow["Tổng số mặt hàng"] = inp.TotalObject;
                        newRow["Tổng số lượng"] = inp.TotalQuantity;
                        newRow["Ghi chú"] = inp.Note;
                        newRow["Trạng thái"] = inp.Status;
                        dataTable.Rows.Add(newRow);
                    }
                    // Do excel export

                    ExportViewModel editViewModel = new ExportViewModel(dataTable, "DanhSachPhieuNhap", "#ffcc00");
                    ExportView editView = new ExportView();
                    editView.DataContext = editViewModel;
                    editViewModel.Title = "Xuất file danh sách phiếu nhập";
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
                cmd = new SqlCommand("exec usp_View_Supplier", con);
                SqlCommand cmd2 = new SqlCommand("exec usp_View_Input", con);
                SqlCommand cmd3 = new SqlCommand("exec usp_View_User", con);
                SqlCommand cmd4 = new SqlCommand("exec usp_View_InputInfo", con);
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
                    List = new ObservableCollection<Input>();
                if (Supplier == null)
                    Supplier = new ObservableCollection<Supplier>();
                if (User == null)
                    User = new ObservableCollection<User>();
                if (InputInfo == null)
                    InputInfo = new ObservableCollection<InputInfo>();
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
                    Supplier supplier = new Supplier
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString(),
                        Phone = dr[2].ToString(),
                        Address = dr[3].ToString(),
                        Email = dr[4].ToString(),
                        MoreInfo = dr[5].ToString(),
                        ContractDate = Convert.ToDateTime(dr[6].ToString()),
                        Status = dr[7].ToString()
                    };
                    Supplier.Add(supplier);
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


                    Input input = new Input
                    {
                        Id = dr[0].ToString(),
                        IdUser = Convert.ToInt32(dr[1].ToString()),
                        IdSupplier = DBNull.Value.Equals(dr[2]) ? null : (int?)Convert.ToInt32(dr[2].ToString()),
                        DateInput = Convert.ToDateTime(dr[3].ToString()),
                        Discount = DBNull.Value.Equals(dr[4]) ? (double)0 : (float)Convert.ToDouble(dr[4].ToString()),
                        Payment = dr[5].ToString(),
                        TotalPrice = (float)Convert.ToDouble(dr[6].ToString()),
                        Note = dr[7].ToString(),
                        Status = dr[8].ToString(),
                        TotalObject = Convert.ToInt32(dr[9].ToString()),
                        TotalQuantity = Convert.ToInt32(dr[10].ToString()),
                        Supplier = Supplier.FirstOrDefault(x => x.Id == (DBNull.Value.Equals(dr[2]) ? 0 : Convert.ToInt32(dr[2].ToString()))),
                        User = User.FirstOrDefault(x => x.Id == Convert.ToInt32(dr[1].ToString()))
                    };


                    List.Add(input);
                }

                foreach (DataRow dr in ds5.Tables[0].Rows)
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

                foreach (DataRow dr in ds4.Tables[0].Rows)
                {
                    InputInfo inputInfo = new InputInfo
                    {
                        IdInput = dr[0].ToString(),
                        IdObject = dr[1].ToString(),
                        Price = Convert.ToDouble(dr[2].ToString()),
                        Quantity = Convert.ToInt32(dr[3].ToString()),
                        Discount = DBNull.Value.Equals(dr[4]) ? 0 : Convert.ToDouble(dr[4])
                    };
                    InputInfo.Add(inputInfo);

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
