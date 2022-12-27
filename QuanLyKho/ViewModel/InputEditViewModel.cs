using QuanLyKho.Model;
using QuanLyKho.View;
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
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    class InputEditViewModel : BaseViewModel
    {
        SqlConnection con;
        private ToastViewModel _toast = null;
        public bool isEdit { set; get; }
        public bool isBack { set; get; }
        public int currentIndex { set; get; }
        private Input _Input;
        public Input Input { get => _Input; set { _Input = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.InputInfoView> _List;
        public ObservableCollection<Model.InputInfoView> List { get => _List; set { _List = value; OnPropertyChanged(); } }


        private ObservableCollection<Supplier> _Supplier;
        public ObservableCollection<Supplier> Supplier { get => _Supplier; set { _Supplier = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.Object> _ListObject;
        public ObservableCollection<Model.Object> ListObject { get => _ListObject; set { _ListObject = value; OnPropertyChanged(); } }
        public ICollectionView ItemsView { get { return CollectionViewSource.GetDefaultView(ListObject); } }

        private ObservableCollection<Unit> _Unit;
        public ObservableCollection<Unit> Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }
        private List<String> _Payment;
        public List<String> Payment { get { return new List<string>() { "Tiền mặt", "Thẻ", "Chuyển khoản" }; } set { _Payment = value; } }
        private bool Filter(object item)
        {
            Model.Object obj = item as Model.Object;
            if (string.IsNullOrEmpty(Search)) return false;
            else if (Search.Contains("*")) return true;
            else if (!string.IsNullOrEmpty(Search))
                if (obj.Id.ToUpper().Contains(Search.ToUpper()) || obj.DisplayName.ToUpper().Contains(Search.ToUpper()))
                    return true;
            return false;

        }


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
                    Model.InputInfoView i = List.FirstOrDefault(x => x.Id == value.Id);
                    if (i != null)
                    {
                        i.Quantity++;
                        i.TotalPrice += i.Price;
                        Input.TotalQuantity++;
                        Input.TotalPrice += i.Price;
                        TotalFinal = Input.TotalPrice - Input.Discount;
                    }
                    else
                    {
                        Model.InputInfoView input = new Model.InputInfoView
                        {
                            LinkImage = value.LinkImage,
                            Id = value.Id,
                            DisplayName = value.DisplayName,
                            Quantity = 1,
                            Price = value.InputPrice,
                            Discount = 0,
                            TotalPrice = (double)((double)value.InputPrice),
                            Unit = value.Unit,
                            STT = currentIndex + 1,
                        };
                        if (input != null)
                        {
                            List.Add(input);
                            currentIndex++;
                            Input.TotalObject++;
                            Input.TotalQuantity++;
                            Input.TotalPrice += input.Price;
                            TotalFinal = Input.TotalPrice - Input.Discount;
                        }
                    }
                }

            }
        }
        private Supplier _SelectedSupplier;
        public Supplier SelectedSupplier
        {
            get => _SelectedSupplier;
            set
            {
                _SelectedSupplier = value;
                OnPropertyChanged();
                if (value != null)
                {
                    Input.Supplier = value;
                    Input.IdSupplier = (int)value.Id;
                }

            }
        }
        private Model.InputInfoView _SelectedItem;
        public Model.InputInfoView SelectedItem
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
                    SelectedUnit = value.Unit;
                }
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
        private String _SelectedPayment;
        public String SelectedPayment
        {
            get => _SelectedPayment;
            set
            {
                _SelectedPayment = value;
                OnPropertyChanged();
                if (value != null)
                    Input.Payment = value;

            }
        }


        private double? _TotalFinal;
        public double? TotalFinal { get => _TotalFinal; set { _TotalFinal = value; OnPropertyChanged(); } }
        private double? _InputDiscount;
        public double? InputDiscount
        {
            get => _InputDiscount; set
            {
                _InputDiscount = value;
                OnPropertyChanged();
                if (value > 0)
                {
                    Input.Discount = value;
                    TotalFinal = Input.TotalPrice - value;
                }
            }
        }
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
                if (value > 0)
                    ObjectTotal = value * ObjectPrice - ObjectDiscount;
                else if (value == 0)
                    ObjectTotal = 0;
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


        public ICommand AddObjectCommand { get; set; }
        public ICommand AddSupplierCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand TempCommand { get; set; }
        public ICommand FinishCommand { get; set; }
        public ICommand DeleteCommand { set; get; }
        public ICommand EditCommand { set; get; }



        double TongTien()
        {
            double tong = 0;
            foreach (Model.InputInfoView i in List)
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

        bool XuLy(string status)
        {
            using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    using (con = new SqlConnection(ConnectionString.connectionString))
                    {
                        con.Open();
                        if (isEdit == true)
                        {
                            Input.IdUser = LoginViewModel.userCurrent.Id;
                            Input.User = LoginViewModel.userCurrent;
                        }
                        Input.DateInput = DateTime.Now;
                        Input.Status = status;
                        SqlCommand cmd;
                        string s = "", s2 = "";
                        if (Input.IdSupplier != null)
                            s = "exec usp_Insert_Update_Input '" + Input.Id + "'," + Input.IdUser + "," + Input.IdSupplier + ",'" + Input.DateInput + "'," + Input.Discount + ",N'" + Input.Payment + "'," + Input.TotalPrice + ",N'" + Input.Note + "', N'" + Input.Status + "'," + Input.TotalObject + "," + Input.TotalQuantity;
                        else
                            s = "exec usp_Insert_Update_Input '" + Input.Id + "'," + Input.IdUser + ",null,'" + Input.DateInput + "'," + Input.Discount + ",N'" + Input.Payment + "'," + Input.TotalPrice + ",N'" + Input.Note + "', N'" + Input.Status + "'," + Input.TotalObject + "," + Input.TotalQuantity;
                        cmd = new SqlCommand(s, con);
                        //cmd.ExecuteNonQuery();
                        if (Input.Id == "")
                            Input.Id = cmd.ExecuteScalar().ToString();
                        else
                            cmd.ExecuteNonQuery();
                        foreach (Model.InputInfoView i in List)
                        {
                            SqlCommand cmd2 = null;
                            if (i.Unit != null)
                                s2 = "exec usp_Insert_Update_InputInfo " + Input.Id + "," + i.Id + "," + i.Price + "," + i.Quantity + "," + i.Discount + "," + i.Unit.Id;
                            else
                                s2 = "exec usp_Insert_Update_InputInfo " + Input.Id + "," + i.Id + "," + i.Price + "," + i.Quantity + "," + i.Discount + ",null";
                            cmd2 = new SqlCommand(s2, con);
                            cmd2.ExecuteNonQuery();
                        }
                        //The Transaction will be completed    
                        txscope.Complete();
                        if (isEdit == true)
                            _toast.ShowSuccess("Cật nhật thành công!");
                        else
                            _toast.ShowSuccess("Thêm thành công!");
                        con.Close();
                        return true;
                    }
                }
                catch (TransactionAbortedException ex)
                {
                    // Log error    
                    txscope.Dispose();
                    _toast.ShowError(string.Format("Thao tác không thành công! lỗi: {0}", ex.Message));
                    con.Close();
                    return false;
                }

            }
        }
        public InputEditViewModel()
        {
            _toast = new ToastViewModel(Corner.BottomRight, 1, 10, 100);

            isBack = false;
            if (this.ListObject == null)
                loadData();

            this.ItemsView.Filter = Filter;


            EditCommand = new RelayCommand<Model.InputInfoView>((p) =>
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
                        SelectedItem.Unit = SelectedUnit;
                        Input.TotalQuantity += quan;
                        Input.TotalPrice = this.TongTien();
                        TotalFinal = Input.TotalPrice - Input.Discount;
                        ObjectId = null;
                        ObjectDisplayName = null;
                        ObjectCount = 0;
                        ObjectPrice = 0;
                        ObjectDiscount = 0;
                        ObjectTotal = 0;
                        SelectedItem = null;
                        SelectedUnit = null;
                    }
                }
            });
            DeleteCommand = new RelayCommand<Model.InputInfoView>((p) =>
        {
            return true;
        }, (p) =>
        {
            int x = this.List.IndexOf((Model.InputInfoView)p) - 1;
            this.List.Remove((Model.InputInfoView)p);
            this.IndexRefresh(x);
            currentIndex--;

            Input.TotalObject--;
            Input.TotalQuantity -= (int)p.Quantity;
            Input.TotalPrice -= (int)p.Quantity * p.Price;
            TotalFinal = Input.TotalPrice - Input.Discount;
        });
            BackCommand = new RelayCommand<Window>((p) => true, (p) => { isBack = true; p.Close(); });
            TempCommand = new RelayCommand<Window>((p) => true, (p) =>
            {
                if (List.ToArray().Length == 0) _toast.ShowError("Không thể lưu, bạn chưa nhập mặt hàng nào!");
                else
                    if (XuLy("Phiếu tạm") == true) p.Close();

            });
            FinishCommand = new RelayCommand<Window>((p) => true, (p) =>
             {
                 if (List.ToArray().Length == 0) _toast.ShowError("Không thể lưu, bạn chưa nhập mặt hàng nào!");
                 else if (SelectedSupplier == null || SelectedPayment == null) _toast.ShowError("Bạn chưa chọn nhà cung cấp hoặc phương thức thanh toán!");
                 else
                     if (XuLy("Đã nhập hàng") == true) p.Close();
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
                Supplier = null;
                Unit = null;
                loadData();
            });
            AddSupplierCommand = new RelayCommand<Window>((p) => true, (p) =>
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
                ListObject.Clear();
                Supplier = null;
                Unit = null;
                loadData();
            });
        }


        private void loadData()
        {
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("exec usp_View_Supplier", con);
                SqlCommand cmd2 = new SqlCommand("exec usp_View_Object", con);
                SqlCommand cmd3 = new SqlCommand("exec usp_View_Unit", con);
                //cmd6.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                SqlDataAdapter adapter3 = new SqlDataAdapter(cmd3);
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                DataSet ds3 = new DataSet();
                adapter.Fill(ds);
                adapter2.Fill(ds2);
                adapter3.Fill(ds3);

                if (ListObject == null)
                    ListObject = new ObservableCollection<Model.Object>();
                if (Supplier == null)
                    Supplier = new ObservableCollection<Supplier>();
                if (Unit == null)
                    Unit = new ObservableCollection<Unit>();

                foreach (DataRow dr in ds3.Tables[0].Rows)
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
                    if (dr[7].ToString().ToUpper().Contains("Đang hoạt động".ToUpper()) && Convert.ToInt32(dr[8].ToString())==0)
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
                        Supplier.Add(supplier);
                    }
                }

                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    if (dr[9].ToString().ToUpper().Contains("Cho phép".ToUpper()))
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
                            IsVisible = Convert.ToInt32(dr[10].ToString()),
                            Unit = Unit.FirstOrDefault(x => x.Id == (DBNull.Value.Equals(dr[2]) ? 0 : Convert.ToInt32(dr[2].ToString()))),
                        };
                        ListObject.Add(objects);
                    }
                }


            }
            catch (Exception ex)
            {
                _toast.ShowError("lỗi: " + ex.Message);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }
}
