using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    class PositionViewModel : BaseViewModel
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataSet ds;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private ToastViewModel _toast = null;
        private ObservableCollection<Position> _List;
        public ObservableCollection<Position> List { get => _List; set { _List = value; OnPropertyChanged(); } }
        private String _IdHeader = null;
        public String IdHeader { get => _IdHeader; set { _IdHeader = value; OnPropertyChanged(); } }
        private String _DisplayNameHeader;
        public String DisplayNameHeader { get => _DisplayNameHeader; set { _DisplayNameHeader = value; OnPropertyChanged(); } }
        private String _Error = null;
        public String Error { get => _Error; set { _Error = value; OnPropertyChanged(); } }
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

        private void SelectAll(bool? select, ObservableCollection<Position> models)
        {
            if (select == false || select == true)
                foreach (var model in models)
                {
                    model.IsSelected = (bool)select;
                }

        }
        private int? _CountSelected;
        public int? CountSelected { get { return _CountSelected; } set { _CountSelected = value; OnPropertyChanged(); } }
        private Category _SelectedItem;
        public Category SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                    DisplayName = SelectedItem.DisplayName;
            }
        }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }


        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        Position kiemTraChonXoa()
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
        public PositionViewModel() { }
        public PositionViewModel(string _idHeader, string _displayNameHeader)
        {
            this.IdHeader = _idHeader;
            this.DisplayName = _displayNameHeader;
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
            worker.RunWorkerAsync();
            progressBarViewModel = new ProgressBarViewModel();
            progressBarView = new ProgressBarView();
            progressBarView.DataContext = progressBarViewModel;
            progressBarViewModel.Title = "Đang tải danh sách vị trí, vui lòng chờ...";
            progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#8a2be2"));
            progressBarView.ShowDialog();


            _toast = new ToastViewModel(Corner.BottomRight, 10, 10, 20);

            AddCommand = new RelayCommand<Category>((p) => { return true; }, (p) =>
            {

                AddViewModel addViewModel = new AddViewModel();
                AddView addView = new AddView();
                addView.DataContext = addViewModel;
                addViewModel.NameTitle = "Tên vị trí:";
                addViewModel.Title = "Thêm vị trí";
                addViewModel.NameTable = "Position";
                addView.ShowDialog();
                List.Clear();
                loadData();

            });


            EditCommand = new RelayCommand<Category>((p) =>
            {
                if (List.Where(x => x.IsSelected == true).ToArray().Length == 1)
                    return true;
                return false;
            }, (p) =>
            {
                Position ca = List.Where(i => i.IsSelected == true).ToArray()[0];
                AddViewModel addViewModel = new AddViewModel();
                AddView addView = new AddView();
                addViewModel.Id = ca.Id;
                addViewModel.Name = ca.DisplayName;
                addView.DataContext = addViewModel;
                addViewModel.NameTitle = "Tên vị trí:";
                addViewModel.Title = "Sửa vị trí";
                addViewModel.NameTable = "Position";
                addView.ShowDialog();
                List.Clear();
                loadData();

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
                Position objXoa = kiemTraChonXoa();

                string tex = "";
                int count = List.Count(x => x.IsSelected == true);
                if (count == 1)
                    tex = "Hệ thống sẽ xóa hoàn toàn vị trí " + objXoa.DisplayName + ". Bạn có chắc chắn muốn xóa?";
                else
                    tex = "Hệ thống sẽ xóa hoàn toàn " + count + " vị trí bạn đã chọn. Bạn có chắc chắn muốn xóa?";

                ConfirmViewModel editViewModel = new ConfirmViewModel();
                ConfirmView editView = new ConfirmView();
                editView.DataContext = editViewModel;
                editViewModel.Title = "Xóa loại hàng";
                editViewModel.Content = tex;
                editView.ShowDialog();
                if (editViewModel.Result == true)
                {


                    SqlConnection con = new SqlConnection(ConnectionString.connectionString);
                    con.Open();

                    foreach (Position ca in List)
                        if (ca.IsSelected == true)
                            try
                            {
                                string texSql = "exec usp_Delete_Position " + ca.Id;
                                cmd = new SqlCommand(texSql, con);
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {

                                if (ex.Message.Contains("đã được sử dụng"))
                                {
                                    string[] mess = ex.Message.ToString().Split('.');
                                    //_toast.ShowError(mess[mess.Length - 1]);


                                    Error = mess[mess.Length - 1];
                                }
                                else
                                    Error = "Thao tác không thàng công! lỗi: " + ex.Message;
                                con.Close();

                            }
                            finally
                            {
                                con.Close();
                            }


                    if (!string.IsNullOrEmpty(Error))
                    {
                        _toast.ShowError(Error);
                        Error = "";
                    }
                    else
                    {

                        List.Clear();
                        loadData();
                        _toast = null;
                        _toast = new ToastViewModel(Corner.BottomRight, 2, 10, 20);
                        _toast.ShowSuccess("Đã xóa thành công danh sách vị trí đã chọn!");

                    }
                }

            });
        }

        private void loadData()
        {

            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                cmd = new SqlCommand("select * from uv_View_Position", con);
                adapter = new SqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds);
                if (List == null)
                    List = new ObservableCollection<Position>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Position Position = new Position
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString()
                    };
                    List.Add(Position);
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

