using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    public class ObjectEditViewModel : BaseViewModel
    {

        private SqlConnection con;
        private ToastViewModel _toast = null;
        ProgressBarViewModel progressBarViewModel = null;
        ProgressBarView progressBarView = null;
        private String _Title;
        public String Title { get => _Title; set { _Title = value; OnPropertyChanged(); } }

        private String _Error;
        public String Error { get => _Error; set { _Error = value; OnPropertyChanged(); } }
        private String _Id;
        public String Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        private ObservableCollection<Category> _Category;
        public ObservableCollection<Category> Category { get => _Category; set { _Category = value; OnPropertyChanged(); } }

        private ObservableCollection<Unit> _Unit;
        public ObservableCollection<Unit> Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }
        private ObservableCollection<Position> _Position;
        public ObservableCollection<Position> Position { get => _Position; set { _Position = value; OnPropertyChanged(); } }

        private Model.Object _Object;
        public Model.Object Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }


        private Visibility _InputPriceDisplay;
        public Visibility InputPriceDisplay { get => _InputPriceDisplay; set { _InputPriceDisplay = value; OnPropertyChanged(); } }

        private Category _SelectedCategory;
        public Category SelectedCategory
        {

            get => _SelectedCategory;
            set
            {
                _SelectedCategory = value;
                OnPropertyChanged();
                if (value != null && Object != null)
                {
                    Object.Category = value;
                    Object.IdCategory = value.Id;
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
                if (value != null && Object != null)
                {
                    Object.Unit = value;
                    Object.IdUnit = value.Id;
                }
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
                if (value != null && Object != null)
                {
                    Object.Position = value;
                    Object.IdPosition = value.Id;
                }
            }
        }


        public ICommand SaveCommand { get; set; }
        public ICommand AddImageCommand { get; set; }
        public ICommand AddPositionCommand { get; set; }
        public ICommand AddUnitCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand AddCategoryCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }
        
        
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // do time-consuming work here, calling ReportProgress as and when you can

            try
            {
                string idPos = "", idUnit = "", linkImage = "";
                if (Object.IdPosition == null)
                    idPos = "null";
                else
                    idPos = "" + Object.IdPosition;
                if (Object.IdUnit == null)
                    idUnit = "null";
                else
                    idUnit = "" + Object.IdUnit;

                if (Object.LinkImage.Contains("IMG_Holder.png"))
                    linkImage = "null";
                else
                {
                    string[] link = Object.LinkImage.Split('\\');
                    linkImage = "N'" + link[link.Length - 1] + "'";
                }
                string sql = string.Format("exec usp_Insert_Update_Object N'{0}',N'{1}',{2},{3},{4},{5},{6},{7},{8},N'{9}',{10}", Object.Id, Object.DisplayName, idUnit, Object.IdCategory, Object.InputPrice, Object.OutputPrice, Object.Count, idPos, linkImage, Object.Status, Object.IsVisible);
                SqlCommand cmd1 = new SqlCommand(sql, con);
                cmd1.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("đã tồn tại"))
                {
                    string[] mess = ex.Message.ToString().Split('.');
                    Error = mess[mess.Length - 1];
                }
                else
                    Error = "Thao tác không thàng công!lỗi: " + ex.Message;
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

        public ObjectEditViewModel() { }
        public ObjectEditViewModel(Model.Object _object)
        {
            this.Object = _object;
            if (Object.Id == "")
                Id = "Tự động sinh mã";
            else
                Id = Object.Id.ToString();
            loadData();
            _toast = new ToastViewModel(Corner.BottomLeft, 1, 245, 5);

            AddImageCommand = new RelayCommand<Model.Object>((p) => true,
                (p) =>
                {
                    OpenFileDialog open = new OpenFileDialog();
                    open.Filter = "(*.jpg;*jpeg,*bmp;*png) | *.jpg;*.jpeg;*bmp;*png";
                    if (open.ShowDialog() == DialogResult.OK)
                    {
                        Object.LinkImage = open.FileName;
                    }

                });

            SaveCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (string.IsNullOrEmpty((string)Object.DisplayName))
                    _toast.ShowError((string)"Bạn chưa nhập tên hàng hóa!");
                else if (SelectedCategory == null)
                    _toast.ShowError((string)"Bạn chưa chọn loại hàng hóa!");
                else
                {
                    bool kt = false;
                    int exec = -1;
                    try
                    {

                        con = new SqlConnection((string)ConnectionString.connectionString);
                        con.Open();
                        String tex = string.Format("usp_Check_DisplayName_Exist_Object N'{0}',N'{1}'", Object.DisplayName, Object.Id);
                        SqlCommand cmd = new SqlCommand(tex, con);
                        exec = (int)cmd.ExecuteScalar();
                        if (exec == 1)
                        {
                            ConfirmViewModel editViewModel = new ConfirmViewModel();
                            ConfirmView editView = new ConfirmView();
                            editView.DataContext = editViewModel;
                            editViewModel.Title = "Thông báo";
                            editViewModel.Content = "Tên hàng hóa đã tồn tại, bạn có chắc chắn muốn lưu không?";
                            editView.ShowDialog();
                            kt = editViewModel.Result;
                        }
                        if (kt == true || (int)cmd.ExecuteScalar() == 0)
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
                            progressBarViewModel.Title = "Đang xử lý, vui lòng chờ...";
                            progressBarViewModel.Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00b3b3"));
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
                                _toast.ShowSuccess("Thông tin khách hàng được cật nhật thành công!");

                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {

                    }



                }
            });

            AddCategoryCommand = new RelayCommand<Category>((p) => true, (p) =>
            {
                AddViewModel addViewModel = new AddViewModel();
                AddView addView = new AddView();
                addView.DataContext = addViewModel;
                addViewModel.NameTitle = "Tên loại hàng:";
                addViewModel.Title = "Thêm loại hàng";
                addViewModel.NameTable = "Category";
                addView.ShowDialog();
                Category.Clear();
                Unit.Clear();
                Position.Clear();
                loadData();
            });

            AddPositionCommand = new RelayCommand<Position>((p) => true, (p) =>
            {
                AddViewModel addViewModel = new AddViewModel();
                AddView addView = new AddView();
                addView.DataContext = addViewModel;
                addViewModel.NameTitle = "Tên vị trí:";
                addViewModel.Title = "Thêm vị trí";
                addViewModel.NameTable = "Position";
                addView.ShowDialog();
                Category.Clear();
                Unit.Clear();
                Position.Clear();
                loadData();
            });

            AddUnitCommand = new RelayCommand<Unit>((p) => true, (p) =>
            {
                AddViewModel addViewModel = new AddViewModel();
                AddView addView = new AddView();
                addView.DataContext = addViewModel;
                addViewModel.NameTitle = "Tên đơn vị đo:";
                addViewModel.Title = "Thêm đơn vị đo";
                addViewModel.NameTable = "Unit";
                addView.ShowDialog();
                Category.Clear();
                Unit.Clear();
                Position.Clear();
                loadData();
            });


            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });

            MouseMoveWindowCommand = new RelayCommand<Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement window = p;
                var w = window as Window;
                if (w != null)
                {
                    w.DragMove();

                }
            });
          
        }

        private void loadData()
        {
            try
            {

                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("exec usp_View_Unit", con);
                SqlCommand cmd2 = new SqlCommand("exec usp_View_Position", con);
                SqlCommand cmd3 = new SqlCommand("exec usp_View_Category", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                SqlDataAdapter adapter3 = new SqlDataAdapter(cmd3);
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                DataSet ds3 = new DataSet();
                adapter.Fill(ds);
                adapter2.Fill(ds2);
                adapter3.Fill(ds3);

                if (Unit == null)
                    Unit = new ObservableCollection<Unit>();
                else Unit.Clear();
                if (Position == null)
                    Position = new ObservableCollection<Position>();
                if (Category == null)
                    Category = new ObservableCollection<Category>();
                else Category.Clear();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Unit unit = new Unit
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString()
                    };
                    Unit.Add(unit);
                }

                foreach (DataRow dr in ds3.Tables[0].Rows)
                {
                    Category category = new Category
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString()
                    };
                    Category.Add(category);
                }

                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    Position position = new Position
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString(),

                    };
                    Position.Add(position);
                }


            }
            catch (Exception ex)
            {
                //throw ex;
                _toast.ShowError("Thao tác không thàng công!lỗi: " + ex.Message);
            }
            finally
            {

                con.Close();
                con.Dispose();
            }
        }



    }
}
