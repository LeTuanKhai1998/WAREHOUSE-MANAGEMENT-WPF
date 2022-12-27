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

    public class SupplierViewModel : BaseViewModel
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter, adapter2, adapter3;
        DataSet ds, ds2, ds3;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private ToastViewModel _toast = null;
        
        private ObservableCollection<Model.Input> _Input;
        public ObservableCollection<Model.Input> Input { get => _Input; set { _Input = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.Input> _ListInput;
        public ObservableCollection<Model.Input> ListInput { get => _ListInput; set { _ListInput = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.User> _User;
        public ObservableCollection<Model.User> User { get => _User; set { _User = value; OnPropertyChanged(); } }

        private ObservableCollection<Supplier> _List;
        public ObservableCollection<Supplier> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        public ICollectionView ItemsView { get { return CollectionViewSource.GetDefaultView(List); } }

        private int _TotalSupplier;
        public int TotalSupplier { get { return _TotalSupplier; } set { _TotalSupplier = value; OnPropertyChanged(); } }
        private int _BusinessSupplier;
        public int BusinessSupplier { get { return _BusinessSupplier; } set { _BusinessSupplier = value; OnPropertyChanged(); } }


        //Lọc dữ liệu
        private bool Filter(object item)
        {
            Supplier sup = item as Supplier;

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

        private void SelectAll(bool? select, ObservableCollection<Supplier> models)
        {
            if (select == false || select == true)
                foreach (var model in models)
                {
                    model.IsSelected = (bool)select;
                }

        }

        private Supplier _SelectedItem;
        public Supplier SelectedItem
        {
            get => _SelectedItem;
            set
            {

                _SelectedItem = value;
                OnPropertyChanged();
                if (value != null)
                {
                    AddListInPut(value);
                 

                }


            }
        }

        void AddListInPut(Supplier item)
        {
            if (item != null)
            {
                if (ListInput == null)
                    ListInput = new ObservableCollection<Input>();
                else
                    ListInput.Clear();
                foreach (Input i in Input)
                    if (i.IdSupplier == item.Id && i.Status.ToUpper().Contains("NHẬP HÀNG"))
                    {
                        Input input = new Input
                        {
                            Id = i.Id,
                            IdUser = i.IdUser,
                            IdSupplier = i.IdSupplier,
                            DateInput = i.DateInput,
                            Discount = i.Discount,
                            Payment = i.Payment,
                            TotalPrice = i.TotalPrice,
                            Note = i.Note,
                            Status = i.Status,
                            TotalObject = i.TotalObject,
                            TotalQuantity = i.TotalQuantity,
                            User = i.User
                        };
                        ListInput.Add(input);
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

        Supplier kiemTraChonXoa()
        {
            return List.FirstOrDefault(p => p.IsSelected == true);
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // do time-consuming work here, calling ReportProgress as and when you can
            loadData();
            loadCount();
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
        public SupplierViewModel()
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
            progressBarViewModel.Title = "Đang tải danh sách nhà cung cấp, vui lòng chờ...";
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
            AddCommand = new RelayCommand<Supplier>(
                         p => { return true; },
                         p =>
                         {
                             Supplier sup = new Supplier
                             {
                                 Id = 0,
                                 DisplayName = "",
                                 Address = "",
                                 Phone = "",
                                 Email = "",
                                 MoreInfo = "",
                                 ContractDate = DateTime.Now,
                                 Status = "Đang hoạt động",
                                 IsVisible = 0,
                             };
                             SupplierEditViewModel editViewModel = new SupplierEditViewModel("Thêm nhà cung cấp", sup);
                             SupplierEditView editView = new SupplierEditView();
                             editView.DataContext = editViewModel;
                             editView.ShowDialog();
                             List.Clear();
                             Input.Clear();
                             User.Clear();
                             loadData();
                             loadCount();

                         });


            EditCommand = new RelayCommand<Supplier>(p => { return true; }, p =>
            {
                if (SelectedItem != null)
                {
                    SupplierEditViewModel editViewModel = new SupplierEditViewModel("Cật nhật khách hàng", SelectedItem);
                    SupplierEditView editView = new SupplierEditView();
                    editView.DataContext = editViewModel;

                    editView.ShowDialog();
                    List.Clear();
                    loadData();
                    loadCount();
                }
            });










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
                Supplier objXoa = kiemTraChonXoa();

                string tex = "";
                int count = List.Count(x => x.IsSelected == true);
                if (count == 1)
                    tex = "Hệ thống sẽ xóa hoàn toàn nhà cung cấp " + objXoa.DisplayName + " nhưng vẫn giữ thông tin nhà cung cấp  trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";
                else
                    tex = "Hệ thống sẽ xóa hoàn toàn " + count + " nhà cung cấp bạn đã chọn nhưng vẫn giữ thông tin nhà cung cấp trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";

                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Xóa nhà cung cấp";
                editViewModel.Content = tex;
                editView.ShowDialog();
                if (editViewModel.Result == true)
                    try
                    {
                        SqlConnection con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        foreach (Supplier o in List)
                            if (o.IsSelected == true)
                            {
                                string texSql = "exec usp_Delete_Supplier " + o.Id;
                                cmd = new SqlCommand(texSql, con);
                                cmd.ExecuteNonQuery();
                            }

                        List.Clear();
                        Input.Clear();
                        User.Clear();
                        loadData();
                        loadCount();
                        _toast.ShowSuccess("Đã xóa thành công danh sách nhà cung cấp đã chọn!");
                    }
                    catch (TransactionAbortedException ex)
                    {
                        // Log error       
                        _toast.ShowError("Xóa thất bại! lỗi: " + ex.Message);
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


                string tex = "Hệ thống sẽ xóa hoàn toàn nhà cung cấp " + SelectedItem.DisplayName + " nhưng vẫn giữ thông tin nhà cung cấp  trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";


                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Xóa nhà cung cấp";
                editViewModel.Content = tex;
                editView.ShowDialog();
                if (editViewModel.Result == true)
                    try
                    {
                        SqlConnection con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();

                        string texSql = "exec usp_Delete_Supplier " + SelectedItem.Id;
                        cmd = new SqlCommand(texSql, con);
                        cmd.ExecuteNonQuery();
                        string name = SelectedItem.DisplayName;

                        List.Clear();
                        Input.Clear();
                        User.Clear();
                        loadData();
                        loadCount();
                        _toast.ShowSuccess(string.Format("Đã xóa nhà cung cấp {0} thành công!", name));
                    }
                    catch (TransactionAbortedException ex)
                    {
                        // Log error       
                        _toast.ShowError("Xóa thất bại! lỗi: " + ex.Message);
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
                        string sql = string.Format("exec usp_Update_Status_Supplier {0},N'Đang hoạt động'", SelectedItem.Id);
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
                        Input.Clear();
                        User.Clear();
                        loadData();
                        loadCount();
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
                        string sql = string.Format("exec usp_Update_Status_Supplier {0},N'Ngừng hoạt động'", SelectedItem.Id);
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        _toast.ShowSuccess(string.Format("Cật nhật trạng thái nhà cung cấp {0} thành công!", SelectedItem.DisplayName));
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
                        Input.Clear();
                        User.Clear();
                        loadData();
                        loadCount();
                    }
            });

            RefreshCommand = new RelayCommand<Supplier>((p) =>
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
                    
                    dataTable.Columns.Add("Mã nhà cung cấp");
                    dataTable.Columns.Add("Tên nhà cung cấp");
                    dataTable.Columns.Add("Điện thoại");
                    dataTable.Columns.Add("Địa chỉ");
                    dataTable.Columns.Add("Email");
                    dataTable.Columns.Add("Ngày hợp tác");
                    dataTable.Columns.Add("Thông tin khác");
                    dataTable.Columns.Add("Trạng thái");


                    foreach (var sup in List)
                    {
                        var newRow = dataTable.NewRow();

                        // fill the properties into the cells
                        newRow["Mã nhà cung cấp"] = sup.Id;
                        newRow["Tên nhà cung cấp"] = sup.DisplayName;
                        newRow["Điện thoại"] = sup.Phone;
                        newRow["Địa chỉ"] = sup.Address;
                        newRow["Email"] = sup.Email;
                        newRow["Ngày hợp tác"] = sup.ContractDate.ToString();
                        newRow["Thông tin khác"] = sup.MoreInfo;
                        newRow["Trạng thái"] = sup.Status;
                        dataTable.Rows.Add(newRow);
                    }

                    // Do excel export

                    ExportViewModel editViewModel = new ExportViewModel(dataTable,"DanhSachNhaCungCap", "#ff5349");
                    ExportView editView = new ExportView();
                    editView.DataContext = editViewModel;
                    editViewModel.Title = "Xuất file danh sách nhà cung cấp";
                    editView.ShowDialog();
                }
                catch (Exception e1)
                {
                    _toast.ShowError("Thao tác không thành công!, lỗi: " + e1.Message);
                }


            });
        }



        private void loadCount()
        {
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                string texSql = "set tran isolation level Serializable begin tran exec usp_Select_Count_Supplier commit tran";//Sửa lỗi BÓNG MA
                //string texSql = "set tran isolation level read uncommitted begin tran exec usp_Select_Count_Supplier commit tran";//LỖI 
                //string texSql = "set tran isolation level repeatable read begin tran exec usp_Select_Count_Supplier commit tran";//Sửa lỗi KHÔNG ĐỌC LẠI ĐƯỢC DỮ LIỆU
                cmd = new SqlCommand(texSql, con);
                adapter = new SqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    TotalSupplier = DBNull.Value.Equals(dr[0]) ? 0 : Convert.ToInt32(dr[0]);
                    BusinessSupplier = DBNull.Value.Equals(dr[1]) ? 0 : Convert.ToInt32(dr[1]);
                }
            }catch(Exception e)
            {
                _toast.ShowError("Lỗi: " + e.Message);
            }
        }
        private void loadData()
        {
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                //string texSql = "set tran isolation level Serializable begin tran exec usp_Select_Count_Supplier commit tran";//Sửa lỗi BÓNG MA
                //string texSql = "set tran isolation level read uncommitted begin tran exec usp_Select_Count_Supplier commit tran";//LỖI 
                ////string texSql = "set tran isolation level repeatable read begin tran exec usp_Select_Count_Supplier commit tran";//Sửa lỗi KHÔNG ĐỌC LẠI ĐƯỢC DỮ LIỆU
                //cmd = new SqlCommand(texSql, con);
                //adapter = new SqlDataAdapter(cmd);
                //ds = new DataSet();
                //adapter.Fill(ds);
                //foreach (DataRow dr in ds.Tables[0].Rows)
                //{
                //    TotalSupplier = DBNull.Value.Equals(dr[0]) ? 0 : Convert.ToInt32(dr[0]);
                //    BusinessSupplier = DBNull.Value.Equals(dr[1]) ? 0 : Convert.ToInt32(dr[1]);
                //}


                cmd = new SqlCommand("select * from uv_View_Supplier", con);
                SqlCommand cmd2 = new SqlCommand("select * from uv_View_User", con);
                SqlCommand cmd3 = new SqlCommand("select * from uv_View_Input", con);
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
                    List = new ObservableCollection<Supplier>();
                else
                    List.Clear();
                if (Input == null)
                    Input = new ObservableCollection<Input>();
                else
                    Input.Clear();
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
                        Status = dr[10].ToString()
                    };
                    User.Add(user);
                }

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(dr[8].ToString()) == 0)
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
                            Status = dr[7].ToString(),
                            IsVisible = Convert.ToInt32(dr[8].ToString()),
                        };
                        List.Add(supplier);
                    }
                }

                foreach (DataRow dr in ds3.Tables[0].Rows)
                {
                    if (!dr[8].ToString().Contains("Phiếu tạm"))
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
                            Supplier = List.FirstOrDefault(x => x.Id == (DBNull.Value.Equals(dr[2]) ? 0 : Convert.ToInt32(dr[2].ToString()))),
                            User = User.FirstOrDefault(x => x.Id == Convert.ToInt32(dr[1].ToString()))
                        };
                        Input.Add(input);
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

