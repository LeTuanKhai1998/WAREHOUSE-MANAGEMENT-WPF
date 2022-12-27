using System;
using System.Collections.Generic;
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
using QuanLyKho.Model;
using QuanLyKho.View;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    class OutputEditViewModel : BaseViewModel
    {
        SqlConnection con;
        //public bool isBack { set; get; }
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private ToastViewModel _toast = null;
        public int currentIndex { set; get; }
        private Output _Output;
        public Output Output { get => _Output; set { _Output = value; OnPropertyChanged(); } }

        private ObservableCollection<Customer> _Customer;
        public ObservableCollection<Customer> Customer { get => _Customer; set { _Customer = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.Object> _ListObject;
        public ObservableCollection<Model.Object> ListObject { get => _ListObject; set { _ListObject = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.OutputInfoView> _List;
        public ObservableCollection<Model.OutputInfoView> List { get => _List; set { _List = value; OnPropertyChanged(); } }


        public ICollectionView ItemsView { get { return CollectionViewSource.GetDefaultView(ListObject); } }


        private List<String> _Payment;
        public List<String> Payment { get { return new List<string>() { "Tiền mặt", "Thẻ", "Chuyển khoản" }; } set { _Payment = value; } }
        private bool Filter(object item)
        {
            Model.Object obj = item as Model.Object;
            if (obj.Count == 0) return false;
            else if (string.IsNullOrEmpty(Search)) return false;
            else if (Search.Contains("*")) return true;
            else if (obj.Id.ToUpper().Contains(Search.ToUpper()) || obj.DisplayName.ToUpper().Contains(Search.ToUpper()))
                return true;
            return false;

        }
        private String _Error;
        public String Error { get => _Error; set { _Error = value; OnPropertyChanged(); } }

        private string _Search;
        public string Search
        {
            get { return _Search; }
            set
            {
                _Search = value;
                OnPropertyChanged();
                ItemsView.Refresh();


            }
        }

        private Model.Object _SelectedObject;
        public Model.Object SelectedObject
        {
            get => _SelectedObject;
            set
            {
                _SelectedObject = value;
                OnPropertyChanged();


                if (value != null)
                {
                    Model.OutputInfoView i = List.FirstOrDefault(x => x.Id == value.Id);
                    if (i != null)
                    {
                        i.Quantity++;
                        i.TotalPrice += i.Price;
                        Output.TotalPrice += i.Price;
                        TotalFinal = Output.TotalPrice - Output.Discount;
                        Quantity++;
                    }
                    else
                    {
                        Model.OutputInfoView output = new Model.OutputInfoView
                        {
                            LinkImage = value.LinkImage,
                            Id = value.Id,
                            DisplayName = value.DisplayName,
                            Quantity = 1,
                            Price = value.OutputPrice,
                            Discount = 0,
                            TotalPrice = (double)((double)value.InputPrice),
                            STT = currentIndex + 1,
                        };
                        if (output != null)
                        {
                            List.Add(output);
                            currentIndex++;
                            Output.TotalPrice += output.Price;
                            TotalFinal = Output.TotalPrice - Output.Discount;
                            Quantity++;
                        }
                    }

                }

            }
        }
        private Customer _SelectedCustomer;
        public Customer SelectedCustomer
        {
            get => _SelectedCustomer;
            set
            {
                _SelectedCustomer = value;
                OnPropertyChanged();
                if (value != null)
                {
                    Output.Customer = value;
                    Output.IdCustomer = (int)value.Id;
                }

            }
        }
        private Model.OutputInfoView _SelectedItem;

        public Model.OutputInfoView SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (value != null)
                {
                    ObjectId = value.Id;
                    ObjectDisplayName = value.DisplayName;
                    ObjectCount = (int)value.Quantity;
                    ObjectPrice = (double)value.Price;
                    ObjectDiscount = (double)value.Discount;
                    ObjectTotal = (double)value.TotalPrice;
                }
            }
        }

        private String _SelectedPayment;
        public String SelectedPayment
        {
            get => _SelectedPayment;
            set
            {
                _SelectedPayment = value;
                OnPropertyChanged();
                if (value != null)
                    Output.Payment = value;

            }
        }


        private double? _TotalFinal;
        public double? TotalFinal { get => _TotalFinal; set { _TotalFinal = value; OutputReceived = value; OnPropertyChanged(); } }
        private double? _OutputDiscount;
        public double? OutputDiscount
        {
            get => _OutputDiscount; set
            {
                _OutputDiscount = value;
                OnPropertyChanged();
                if (value >= 0)
                {
                    Output.Discount = value;
                    TotalFinal = Output.TotalPrice - value;
                }
            }
        }
        private double? _OutputReceived;
        public double? OutputReceived
        {
            get => _OutputReceived; set
            {
                _OutputReceived = value;
                OnPropertyChanged();
                if (value > 0)
                {
                    if (value < TotalFinal)
                        OutputExcess = 0;
                    else
                        OutputExcess = value - TotalFinal;
                }
            }
        }

        private double? _OutputExcess;
        public double? OutputExcess { get => _OutputExcess; set { _OutputExcess = value; OnPropertyChanged(); } }






        private string _ObjectId;
        public string ObjectId { get => _ObjectId; set { _ObjectId = value; OnPropertyChanged(); } }
        private string _ObjectDisplayName;
        public string ObjectDisplayName { get => _ObjectDisplayName; set { _ObjectDisplayName = value; OnPropertyChanged(); } }
        private int _ObjectCount;
        public int ObjectCount
        {
            get => _ObjectCount; set
            {
                _ObjectCount = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    int quan = (int)ListObject.FirstOrDefault(x => x.Id == SelectedItem.Id).Count;
                    if (value > 0 && value <= quan)
                        ObjectTotal = value * ObjectPrice - ObjectDiscount;
                    else if (value == 0)
                        ObjectTotal = 0;
                    else if (value > quan)
                    {
                        _toast.ShowError("Số lượng mua không vượt quá số lượng tồn " + quan);
                        _ObjectCount = (int)quan;
                        ObjectTotal = value * ObjectPrice - ObjectDiscount;
                    }
                }

            }
        }
        private double _ObjectPrice;
        public double ObjectPrice
        {
            get => _ObjectPrice; set
            {
                _ObjectPrice = value;
                OnPropertyChanged();
                if (value > 0)
                    ObjectTotal = value * ObjectCount - ObjectDiscount;
                else if (value == 0)
                    ObjectTotal = 0;
            }
        }
        private double _ObjectDiscount;
        public double ObjectDiscount
        {
            get => _ObjectDiscount; set
            {
                _ObjectDiscount = value;
                OnPropertyChanged();
                if (value >= 0)
                    ObjectTotal = ObjectCount * ObjectPrice - value;


            }
        }
        private double _ObjectTotal;
        public double ObjectTotal { get => _ObjectTotal; set { _ObjectTotal = value; OnPropertyChanged(); } }



        private int _Quantity;
        public int Quantity { get => _Quantity; set { _Quantity = value; OnPropertyChanged(); } }



        public ICommand AddObjectCommand { get; set; }
        public ICommand AddCustomerCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand TempCommand { get; set; }
        public ICommand FinishCommand { get; set; }
        public ICommand DeleteCommand { set; get; }
        public ICommand EditCommand { set; get; }




        double TongTien()
        {
            double tong = 0;
            foreach (Model.OutputInfoView i in List)
                tong += (double)i.TotalPrice;
            return tong;
        }
        void IndexRefresh(int x)
        {
            if (x == List.ToArray().Length) return;
            if (x == -1)
            {
                for (int i = 0; i < List.ToArray().Length; i++)
                    List[i].STT--;
                return;
            }
            for (int i = x; i < List.ToArray().Length - 1; i++)
                if (List[i].STT != (List[i + 1].STT - 1))
                    List[i + 1].STT = List[i].STT + 1;
        }



        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // do time-consuming work here, calling ReportProgress as and when you can
            using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    using (con = new SqlConnection(ConnectionString.connectionString))
                    {
                        con.Open();
                        Output.DateOutput = DateTime.Now;
                        Output.Status = "Hoàn thành";
                        string idCustomer = "";
                        if (Output.Customer == null)
                            idCustomer = "null";
                        else
                            idCustomer = Output.IdCustomer.ToString();
                        string sql = string.Format("exec usp_Insert_Output {0},{1},'{2}',{3},N'{4}',{5},N'{6}',N'{7}'", Output.IdUser, idCustomer, Output.DateOutput, Output.Discount, Output.Payment, Output.TotalPrice, Output.Note, Output.Status);
                        SqlCommand cmd = new SqlCommand(sql, con);
                        Output.Id = (string)cmd.ExecuteScalar();
                        foreach (Model.OutputInfoView i in List)
                        {

                            string sql2 = string.Format("exec usp_Insert_OutputInfo N'{0}',N'{1}',{2},{3},{4}", Output.Id, i.Id, i.Price, i.Quantity, i.Discount);
                            SqlCommand cmd2 = new SqlCommand(sql2, con);
                            cmd2.ExecuteNonQuery();
                            txscope.Complete();
                            
                        }

                    }
                }
                catch (TransactionAbortedException ex)
                {

                    // Log error    
                    txscope.Dispose();
                    Error = "Thao tác không thàng công!lỗi: " + ex.Message;

                }
                finally
                {
                    con.Close();
                    con.Dispose();
                    
                }

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
        public OutputEditViewModel()
        {

            _toast = new ToastViewModel(Corner.BottomRight, 1, 10, 100);
            Quantity = 0;
            //isBack = false;
            if (this.ListObject == null)
                loadData();
            this.ItemsView.Filter = Filter;


            EditCommand = new RelayCommand<Model.OutputInfoView>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (ObjectCount <= 0 || ObjectPrice <= 0 || ObjectDiscount < 0)
                    _toast.ShowError("Dữ liệu không được <= 0");
                else
                {
                    if (SelectedItem != null)
                    {
                        int quan = ObjectCount - (int)SelectedItem.Quantity;
                        SelectedItem.Quantity = ObjectCount;
                        SelectedItem.Price = ObjectPrice;
                        SelectedItem.Discount = ObjectDiscount;
                        SelectedItem.TotalPrice = ObjectTotal;
                        Output.TotalPrice = this.TongTien();
                        TotalFinal = Output.TotalPrice - Output.Discount;
                        Quantity += quan;

                        ObjectId = null;
                        ObjectDisplayName = null;
                        ObjectCount = 0;
                        ObjectPrice = 0;
                        ObjectDiscount = 0;
                        ObjectTotal = 0;
                        SelectedItem = null;

                    }
                }
            });
            DeleteCommand = new RelayCommand<Model.OutputInfoView>((p) =>
            {
                return true;
            }, (p) =>
            {
                int x = this.List.IndexOf((Model.OutputInfoView)p) - 1;
                this.List.Remove((Model.OutputInfoView)p);
                this.IndexRefresh(x);
                currentIndex--;
                if (this.List.ToArray().Length == 0)
                    OutputDiscount = 0;

                Output.TotalPrice -= (int)p.Quantity * p.Price;
                TotalFinal = Output.TotalPrice - Output.Discount;
                Quantity -= (int)p.Quantity;
                ObjectId = null;
                ObjectDisplayName = null;
                ObjectCount = 0;
                ObjectPrice = 0;
                ObjectDiscount = 0;
                ObjectTotal = 0;
                SelectedItem = null;
            });
            BackCommand = new RelayCommand<Window>((p) => true, (p) => { /*isBack = true; */p.Close(); });
            FinishCommand = new RelayCommand<Window>((p) => true, (p) =>
            {

                if (List.ToArray().Length == 0) _toast.ShowError("bạn chưa chọn mặt hàng nào!");
                else if (_SelectedPayment == null) _toast.ShowError("Bạn chưa chọn  phương thức thanh toán!");
                else if (OutputReceived < TotalFinal) _toast.ShowError("Khách chưa đưa đủ tiền!");
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
                    progressBarViewModel.Title = "Đang tải danh sách hóa đơn, vui lòng chờ...";
                    progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffcc00"));
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
                        _toast.ShowSuccess("Thông tin hóa đơn được cật nhật thành công!");

                    }



                }


            });
            AddObjectCommand = new RelayCommand<Window>((p) => true, (p) =>
            {
                Model.Object obj = new Model.Object()
                {
                    Id = "",
                    DisplayName = null,
                    IdUnit = null,
                    IdCategory = 0,
                    InputPrice = 0,
                    OutputPrice = 0,
                    Count = 0,
                    IdPosition = null,
                    LinkImage = ObjectViewModel.path + "IMG_Holder.png",
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
                ListObject.Clear();
                Customer = null;
                loadData();
            });
            AddCustomerCommand = new RelayCommand<Window>((p) => true, (p) =>
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
                    LinkImage = CustomerViewModel.path + "IMG_Holder.png"
                };
                CustomerEditViewModel editViewModel = new CustomerEditViewModel(sup);
                CustomerEditView editView = new CustomerEditView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Thêm khách hàng";
                editViewModel.RadioMale = true;
                editView.ShowDialog();
                ListObject.Clear();
                Customer = null;
                loadData();
            });

        }

        private void loadData()
        {
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("exec usp_View_Customer", con);
                SqlCommand cmd2 = new SqlCommand("exec usp_View_Object", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                adapter.Fill(ds);
                adapter2.Fill(ds2);
                if (Customer == null)
                    Customer = new ObservableCollection<Customer>();
                if (ListObject == null)
                    ListObject = new ObservableCollection<Model.Object>();


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr[9].ToString().ToUpper().Contains("Đang hoạt động".ToUpper()) && Convert.ToInt32(dr[10].ToString()) == 0)
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
                }


                foreach (DataRow dr in ds2.Tables[0].Rows)
                {


                    if (Convert.ToInt32(dr[6].ToString()) > 0 && dr[9].ToString().ToUpper().Contains("Cho phép".ToUpper()))
                    {
                        Model.Object objects = new Model.Object
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
                            IsVisible = Convert.ToInt32(dr[10].ToString())
                        };
                        if (objects.LinkImage.Contains("null") || DBNull.Value.Equals(dr[8]))
                            objects.LinkImage = ObjectViewModel.path + "IMG_Holder.png";
                        ListObject.Add(objects);
                    }
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