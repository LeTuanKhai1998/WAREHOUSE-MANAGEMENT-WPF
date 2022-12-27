using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;
using Object = QuanLyKho.Model.Object;

namespace QuanLyKho.ViewModel
{
    class ObjectViewModel : BaseViewModel
    {
        public static String path = Constants.objectImagesPath;
        SqlConnection con;
        SqlCommand cmd, cmd2, cmd3, cmd4;
        SqlDataAdapter adapter, adapter2, adapter3, adapter4;
        DataSet ds, ds2, ds3, ds4;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private ToastViewModel _toast = null;
        private ObservableCollection<Category> _Category;
        public ObservableCollection<Category> Category { get => _Category; set { _Category = value; OnPropertyChanged(); } }

        private ObservableCollection<Unit> _Unit;
        public ObservableCollection<Unit> Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }
        private ObservableCollection<Position> _Position;
        public ObservableCollection<Position> Position { get => _Position; set { _Position = value; OnPropertyChanged(); } }

        private ObservableCollection<Object> _List;
        public ObservableCollection<Object> List { get { return _List; } set { _List = value; OnPropertyChanged(); } }


        private ObservableCollection<StockCard> _ListStockCard;
        public ObservableCollection<StockCard> ListStockCard { get => _ListStockCard; set { _ListStockCard = value; OnPropertyChanged(); } }

        public ICollectionView ItemsView { get { return CollectionViewSource.GetDefaultView(List); } }

        //Lọc dữ liệu
        private bool Filter(object item)
        {
            Object obj = item as Object;

            bool[] kq = new bool[6];
            int i = 0;

            if (string.IsNullOrEmpty(Search) && SelectedCategory == null && RadioAll == true && RadioStatusAll) return true;

            if (!string.IsNullOrEmpty(Search))
            {
                if (obj.DisplayName.ToUpper().Contains(Search.ToUpper()) || obj.Id.ToUpper().Contains(Search.ToUpper()))
                {
                    kq[0] = true;
                }
                i++;
            }

            if (SelectedCategory != null)
            {
                if (obj.Category == SelectedCategory)
                {
                    kq[1] = true;
                }
                i++;
            }

            if (RadioAll == false)
            {
                if (RadioStock == true)
                {
                    if (obj.Count > 0) kq[2] = true;

                }
                else if (RadioOut == true)
                    if (obj.Count == 0) kq[3] = true;
                i++;
            }

            if (RadioStatusAll == false)
            {
                if (RadioBusiness == true)
                {
                    if (obj.Status.Contains("Cho phép")) kq[4] = true;
                }
                else if (RadioStop == true)
                    if (obj.Status.Contains("Ngừng")) kq[5] = true;
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

        private void SelectAll(bool? select, ObservableCollection<Object> models)
        {
            if (select == false || select == true)
                foreach (var model in models)
                {
                    model.IsSelected = (bool)select;
                }

        }



        private int? _CountSelected;
        public int? CountSelected { get { return _CountSelected; } set { _CountSelected = value; OnPropertyChanged(); } }

        private Status _Status = Status.All;
        public Status Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value)
                    return;
                _Status = value;
                OnPropertyChanged();
            }
        }

        private Object _SelectedItem;
        public Object SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (value != null)
                {
                    loadStockCard(value.Id);
                    //ObjectInfoViewModel editViewModel = new ObjectInfoViewModel(value);
                    //ObjectViewInfoWindow editView = new ObjectViewInfoWindow();
                    //editView.DataContext = editViewModel;
                    //editView.ShowDialog();
                    //_SelectedItem = null;
                    //Position.Clear();
                    //Category.Clear();
                    //Unit.Clear();
                    //List.Clear();
                    //loadData();
                }



            }
        }

        private Category _SelectedCategory;
        public Category SelectedCategory
        {
            get => _SelectedCategory;
            set
            {
                _SelectedCategory = value;
                OnPropertyChanged();
                ItemsView.Refresh(); // required    


            }
        }

        private Unit _SelectedUnit;
        public Unit SelectedUnit
        {
            get => _SelectedUnit;
            set
            {
                _SelectedUnit = value;
                OnPropertyChanged();

            }
        }

        private Position _SelectedPosition;
        public Position SelectedPosition
        {
            get => _SelectedPosition;
            set
            {
                _SelectedPosition = value;
                OnPropertyChanged();
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

        private bool _RadioAll;
        public bool RadioAll
        {
            get { return this._RadioAll; }
            set
            {
                this._RadioAll = value;
                this.OnPropertyChanged("RadioAll");
                ItemsView.Refresh();
            }
        }
        private bool _RadioStock;
        public bool RadioStock
        {
            get { return this._RadioStock; }
            set
            {
                this._RadioStock = value;
                this.OnPropertyChanged("RadioStock");
                ItemsView.Refresh();
            }
        }
        private bool _RadioOut;
        public bool RadioOut
        {
            get { return this._RadioOut; }
            set
            {
                this._RadioOut = value;
                this.OnPropertyChanged("RadioOut");
                ItemsView.Refresh();
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

        Object kiemTraChonXoa()
        {
            return List.FirstOrDefault(p => p.IsSelected == true);
        }

        public ObjectViewModel()
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
            progressBarViewModel.Title = "Đang tải danh sách hàng hóa, vui lòng chờ...";
            progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00b3b3"));
            progressBarView.ShowDialog();
            _toast = new ToastViewModel(Corner.BottomRight, 4, 10, 20);

            RadioAll = true;
            RadioStatusAll = true;
            SelectedItem = null;
            if (List != null)
            {
                this.ItemsView.Filter = Filter;
            }




            AddCommand = new RelayCommand<Object>((p) =>
        {
            return true;
        }, (p) =>
        {
            Object obj = new Object()
            {
                Id = "",
                DisplayName = null,
                IdUnit = null,
                IdCategory = 0,
                InputPrice = 0,
                OutputPrice = 0,
                Count = 0,
                IdPosition = null,
                LinkImage = path + "IMG_Holder.png",
                Status = "Cho phép kinh doanh",
                IsVisible = 0,
                Unit = null,
                Category = null,
                Position = null
            };
            ObjectEditViewModel editViewModel = new ObjectEditViewModel(obj);
            ObjectEditView editView = new ObjectEditView();
            editView.DataContext = editViewModel;
            editViewModel.Title = "Thêm hàng hóa";
            editViewModel.InputPriceDisplay = Visibility.Visible;
            editView.ShowDialog();
            Position.Clear();
            Category.Clear();
            Unit.Clear();
            List.Clear();
            loadData();
        });


            EditCommand = new RelayCommand<Object>(
                           p => { return true; },
                           p =>
                           {
                               if (SelectedItem != null)
                               {
                                   ObjectEditViewModel editViewModel = new ObjectEditViewModel(SelectedItem);
                                   ObjectEditView editView = new ObjectEditView();
                                   editView.DataContext = editViewModel;
                                   editViewModel.Title = "Cật nhật hàng hóa";
                                   editViewModel.SelectedCategory = SelectedItem.Category;
                                   editViewModel.SelectedUnit = SelectedItem.Unit;
                                   editViewModel.SelectedPosition = SelectedItem.Position;
                                   editViewModel.InputPriceDisplay = Visibility.Collapsed;
                                   editView.ShowDialog();
                                   List.Clear();
                                   Unit.Clear();
                                   Position.Clear();
                                   Category.Clear();
                                   loadData();
                               }
                           }
            );



            DeleteCommand = new RelayCommand<Object>((p) =>
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
                Object objXoa = kiemTraChonXoa();


                string tex = "";
                int count = List.Count(x => x.IsSelected == true);
                if (count == 1)
                    tex = "Hệ thống sẽ xóa hoàn toàn hàng hóa " + objXoa.Id + " nhưng vẫn giữ thông tin hàng hóa trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";
                else
                    tex = "Hệ thống sẽ xóa hoàn toàn " + count + " hàng hóa bạn đã chọn nhưng vẫn giữ thông tin hàng hóa trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";

                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Xóa hàng hóa";
                editViewModel.Content = tex;
                editView.ShowDialog();
                if (editViewModel.Result == true)
                    try
                    {
                        SqlConnection con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        foreach (Object o in List)
                            if (o.IsSelected == true)
                            {
                                string texSql = "exec usp_Delete_Object " + o.Id;
                                cmd = new SqlCommand(texSql, con);
                                cmd.ExecuteNonQuery();
                            }

                        Position.Clear();
                        Category.Clear();
                        Unit.Clear();
                        List.Clear();
                        loadData();
                        _toast.ShowSuccess("Đã xóa thành công danh sách hàng hóa đã chọn!");
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
            DeleteOneCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                string tex = "Hệ thống sẽ xóa hoàn toàn hàng hóa " + SelectedItem.Id + " nhưng vẫn giữ thông tin hàng hóa trong các giao dịch lịch sử nếu có.Bạn có chắc chắn muốn xóa?";

                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Xóa hàng hóa";
                editViewModel.Content = tex;
                editView.ShowDialog();
                if (editViewModel.Result == true)
                    try
                    {
                        SqlConnection con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();

                        string texSql = "exec usp_Delete_Object " + SelectedItem.Id;
                        cmd = new SqlCommand(texSql, con);
                        cmd.ExecuteNonQuery();
                        string name = SelectedItem.DisplayName;
                        Position.Clear();
                        Category.Clear();
                        Unit.Clear();
                        List.Clear();
                        loadData();
                        _toast.ShowSuccess(string.Format("Đã xóa hàng hóa {0} thành công!", name));


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

            BusinessCommand = new RelayCommand<Object>(p =>
            {
                if (SelectedItem != null)
                    if (SelectedItem.Status.ToUpper().Contains("Cho phép".ToUpper()))
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
                        string s = string.Format("exec usp_Update_Status_Object '{0}',N'Cho phép kinh doanh'", SelectedItem.Id);
                        SqlCommand cmd = new SqlCommand(s, con);
                        cmd.ExecuteNonQuery();
                        SelectedItem.Status = "Cho phép kinh doanh";
                        _toast.ShowSuccess(string.Format("Cật nhật trạng thái hàng hóa {0} thành công!", SelectedItem.DisplayName));
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
                        Unit.Clear();
                        Position.Clear();
                        Category.Clear();
                        loadData();
                    }
            });
            StopCommand = new RelayCommand<Object>(p =>
            {
                if (SelectedItem != null)
                    if (SelectedItem.Status.ToUpper().Contains("Cho phép".ToUpper()))
                        return true;
                return false;
            }, p =>
            {
                if (SelectedItem != null)
                    try
                    {
                        con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        string s = string.Format("exec usp_Update_Status_Object '{0}',N'Ngừng kinh doanh'", SelectedItem.Id);
                        SqlCommand cmd = new SqlCommand(s, con);
                        cmd.ExecuteNonQuery();
                        SelectedItem.Status = "Ngừng kinh doanh";
                        _toast.ShowSuccess(string.Format("Cật nhật trạng thái hàng hóa {0} thành công!", SelectedItem.DisplayName));
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
                        Unit.Clear();
                        Position.Clear();
                        Category.Clear();
                        loadData();
                    }
            });

            RefreshCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                RadioAll = true;
                RadioStatusAll = true;
                Search = null;
                SelectedCategory = null;

            });
            ExportCommand = new RelayCommand<Object>((p) =>
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
                    dataTable.Columns.Add("Loại hàng");
                    dataTable.Columns.Add("Mã hàng");
                    dataTable.Columns.Add("Tên hàng");
                    dataTable.Columns.Add("Giá bán(vnđ)");
                    dataTable.Columns.Add("Giá vốn(vnđ)");
                    dataTable.Columns.Add("Tồn kho");
                    dataTable.Columns.Add("ĐVT");
                    dataTable.Columns.Add("Vị trí");

                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                    foreach (var obj in List)
                    {
                        var newRow = dataTable.NewRow();

                        // fill the properties into the cells
                        newRow["Loại hàng"] = obj.Category.DisplayName;
                        newRow["Mã hàng"] = obj.Id;
                        newRow["Tên hàng"] = obj.DisplayName;
                        newRow["Giá bán(vnđ)"] = double.Parse(obj.OutputPrice.ToString()).ToString("#,###", cul.NumberFormat); ;
                        newRow["Giá vốn(vnđ)"] = double.Parse(obj.InputPrice.ToString()).ToString("#,###", cul.NumberFormat); ; ;
                        newRow["Tồn kho"] = obj.Count;
                        newRow["ĐVT"] = obj.Unit == null ? "" : obj.Unit.DisplayName;
                        newRow["Vị trí"] = obj.Position == null ? "" : obj.Position.DisplayName;

                        dataTable.Rows.Add(newRow);
                    }

                    // Do excel export

                    ExportViewModel editViewModel = new ExportViewModel(dataTable,"DanhSachHangHoa", "#00b3b3");
                    ExportView editView = new ExportView();
                    editView.DataContext = editViewModel;
                    editViewModel.Title = "Xuất file danh sách hàng hóa";
                    editView.ShowDialog();
                }
                catch (Exception e1)
                {
                    _toast.ShowError("Thao tác không thành công!, lỗi: " + e1.Message);
                }


            });

        }


        private void loadStockCard(string id)
        {
            con = new SqlConnection(ConnectionString.connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(string.Format("select * from uf_Select_Deal('{0}') order by date", id), con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            if (ListStockCard == null)
                ListStockCard = new ObservableCollection<StockCard>();
            else ListStockCard.Clear();


            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StockCard stockCard = new StockCard
                {
                    Id = dr[0].ToString(),
                    Method = dr[1].ToString(),
                    Date = DBNull.Value.Equals(dr[2]) ? null : (Nullable<DateTime>)Convert.ToDateTime(dr[2].ToString()),
                    InputPrice = DBNull.Value.Equals(dr[3]) ? null : (Nullable<Double>)Convert.ToDouble(dr[3].ToString()),
                    Quantity = DBNull.Value.Equals(dr[4]) ? null : (Nullable<int>)Convert.ToInt32(dr[4].ToString()),
                    Stock = DBNull.Value.Equals(dr[5]) ? null : (Nullable<int>)Convert.ToInt32(dr[5].ToString())
                };
                ListStockCard.Add(stockCard);
            }
        }

        private void loadData()
        {
            try
            {

                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                cmd = new SqlCommand("exec usp_View_Object", con);
                cmd2 = new SqlCommand("exec usp_View_Unit", con);
                cmd3 = new SqlCommand("exec usp_View_Position", con);
                cmd4 = new SqlCommand("exec usp_View_Category", con);
                adapter = new SqlDataAdapter(cmd);
                adapter2 = new SqlDataAdapter(cmd2);
                adapter3 = new SqlDataAdapter(cmd3);
                adapter4 = new SqlDataAdapter(cmd4);
                ds = new DataSet();
                ds2 = new DataSet();
                ds3 = new DataSet();
                ds4 = new DataSet();
                adapter.Fill(ds);
                adapter2.Fill(ds2);
                adapter3.Fill(ds3);
                adapter4.Fill(ds4);

                if (List == null)
                    List = new ObservableCollection<Object>();
                else List.Clear();
                if (Unit == null)
                    Unit = new ObservableCollection<Unit>();
                else Unit.Clear();
                if (Position == null)
                    Position = new ObservableCollection<Position>();
                else Position.Clear();
                if (Category == null)
                    Category = new ObservableCollection<Category>();
                else Category.Clear();

                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    Unit unit = new Unit
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString()
                    };
                    Unit.Add(unit);
                }

                foreach (DataRow dr in ds4.Tables[0].Rows)
                {
                    Category category = new Category
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString()
                    };
                    Category.Add(category);
                }

                foreach (DataRow dr in ds3.Tables[0].Rows)
                {
                    Position position = new Position
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString(),

                    };
                    Position.Add(position);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(dr[10].ToString()) == 0)
                    {
                        Object objects = new Object
                        {
                            Id = dr[0].ToString(),
                            DisplayName = dr[1].ToString(),
                            IdUnit = DBNull.Value.Equals(dr[2]) ? null : (Nullable<int>)Convert.ToInt32(dr[2].ToString()),
                            IdCategory = Convert.ToInt32(dr[3].ToString()),
                            InputPrice = Convert.ToDouble(dr[4].ToString()),
                            OutputPrice = Convert.ToDouble(dr[5].ToString()),
                            Count = Convert.ToInt32(dr[6].ToString()),
                            IdPosition = DBNull.Value.Equals(dr[7]) ? null : (Nullable<int>)Convert.ToInt32(dr[7].ToString()),
                            LinkImage = path + dr[8].ToString(),
                            Status = dr[9].ToString(),
                            IsVisible = Convert.ToInt32(dr[10].ToString()),
                            Unit = Unit.FirstOrDefault(x => x.Id == (DBNull.Value.Equals(dr[2]) ? null : (Nullable<int>)Convert.ToInt32(dr[2].ToString()))),
                            Category = Category.FirstOrDefault(x => x.Id == Convert.ToInt32(dr[3].ToString())),
                            Position = Position.FirstOrDefault(x => x.Id == (DBNull.Value.Equals(dr[7]) ? null : (Nullable<int>)Convert.ToInt32(dr[7].ToString())))
                        };
                        if (objects.LinkImage.Contains("null") || DBNull.Value.Equals(dr[8]))
                            objects.LinkImage = path + "IMG_Holder.png";
                        List.Add(objects);
                    }
                }

            }
            catch (Exception ex)
            {
                //throw ex;
                _toast.ShowError("Lỗi: " + ex.Message);
            }
            finally
            {
                ds = null;
                ds2 = null;
                ds3 = null;
                ds4 = null;
                adapter.Dispose();
                adapter2.Dispose();
                adapter3.Dispose();
                adapter4.Dispose();
                con.Close();
                con.Dispose();
            }
        }
    }
}
