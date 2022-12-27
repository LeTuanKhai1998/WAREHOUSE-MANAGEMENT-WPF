using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    class InputInfoViewModel : BaseViewModel
    {
        SqlConnection con;

        private ToastViewModel _toast = null;
        private Input _Input;
        public Input Input { get => _Input; set { _Input = value; OnPropertyChanged(); } }


        private ObservableCollection<Model.InputInfoView> _List;
        public ObservableCollection<Model.InputInfoView> List { get => _List; set { _List = value; OnPropertyChanged(); } }



        private double? _TotalFinal;
        public double? TotalFinal { get => _TotalFinal; set { _TotalFinal = value; OnPropertyChanged(); } }
        private Visibility _OpenTemp = Visibility.Visible;
        public Visibility OpenTemp { get => _OpenTemp; set { _OpenTemp = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public InputInfoViewModel() { }

        public InputInfoViewModel(Input _in)
        {
            _toast = new ToastViewModel(Corner.BottomRight, 1, 10, 100);

            this.Input = _in;
            if (this.Input.Status.ToUpper().Contains("tạm".ToUpper()))
                OpenTemp = Visibility.Visible;
            else
                OpenTemp = Visibility.Collapsed;


            SaveCommand = new RelayCommand<Window>(p => true, p =>
            {
                try
                {
                    con = new SqlConnection(ConnectionString.connectionString);
                    con.Open();
                    string s = "exec usp_Update_Note_Input '" + (object)Input.Id + "',N'" + Input.Note + "'";
                    SqlCommand cmd = new SqlCommand(s, con);
                    cmd.ExecuteNonQuery();
                    //_toast.ShowSuccess("Lưu thành công!");
                }
                catch (Exception e) { _toast.ShowError("Thao tác không thành công!"); }
                finally
                {
                    con.Close();
                    con.Dispose();
                    p.Close();
                    _toast = null;
                    _toast = new ToastViewModel(Corner.BottomRight, 2, 10, 20);
                    _toast.ShowSuccess("Thông tin phiếu nhập kho được cật nhật thành công!");
                }
            });

            EditCommand = new RelayCommand<Input>(p =>
            {
                if (!Input.Status.Contains("tạm")) OpenTemp = Visibility.Collapsed;

                return true;
            }, p =>
            {
                Input temp = new Input()
                {
                    Id = Input.Id,
                    IdUser = Input.IdUser,
                    IdSupplier = Input.IdSupplier,
                    DateInput = Input.DateInput,
                    Discount = Input.Discount,
                    Payment = Input.Payment,
                    TotalPrice = Input.TotalPrice,
                    Note = Input.Note,
                    Status = Input.Status,
                    TotalObject = Input.TotalObject,
                    TotalQuantity = Input.TotalQuantity,
                    Supplier = Input.Supplier,
                    User = Input.User,
                };
                ObservableCollection<Model.InputInfoView> ltemp = new ObservableCollection<Model.InputInfoView>();
                foreach (Model.InputInfoView i in List)
                    ltemp.Add(i);
                InputEditViewModel editViewModel = new InputEditViewModel();
                InputEditView editView = new InputEditView();
                editView.DataContext = editViewModel;
                editViewModel.List = List;
                editViewModel.Input = Input;
                editViewModel.SelectedSupplier = Input.Supplier;
                editViewModel.SelectedPayment = Input.Payment;
                editViewModel.currentIndex = List.ToArray().Length;
                editViewModel.InputDiscount = Input.Discount;
                editViewModel.TotalFinal = TotalFinal;
                editViewModel.isEdit = true;
                editView.ShowDialog();
                if (editViewModel.isBack == true)
                {
                    Input = temp;
                    List = ltemp;
                }

            });


            CancelCommand = new RelayCommand<Window>(p =>
            {
                if (Input.Status.Contains("hủy")) return false;
                return true;
            }, p =>
            {

                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Hủy phiếu nhật hàng";
                editViewModel.Content = "Bạn có chắc chắn muốn hủy phiếu nhập hàng " + Input.Id + " không?";
                editView.ShowDialog();
                if (editViewModel.Result == true)
                    try
                    {
                        con = new SqlConnection(ConnectionString.connectionString);
                        con.Open();
                        string s = "exec usp_Update_Status_Input '" + (object)Input.Id + "',N'Đã hủy'";
                        SqlCommand cmd = new SqlCommand(s, con);
                        cmd.ExecuteNonQuery();
                        Input.Status = "Đã hủy";
                        _toast.ShowSuccess("Hủy phiếu hàng thành công!");
                    }
                    catch (Exception e) { _toast.ShowError("Thao tác không thành công!"); }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                        p.Close();
                    }
            });
        }
    }
}
