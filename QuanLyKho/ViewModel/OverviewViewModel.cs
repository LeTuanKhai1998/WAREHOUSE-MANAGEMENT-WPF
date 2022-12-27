using QuanLyKho.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using LiveCharts;
using LiveCharts.Wpf;

namespace QuanLyKho.ViewModel
{
    class OverviewViewModel : BaseViewModel
    {

        SqlConnection con;

        public Boolean IsLoaded = false;

        private string _CountInput = "0 Phiếu nhập";
        public string CountInput { get => _CountInput; set { _CountInput = value; OnPropertyChanged(); } }
        private string _CountOutput = "0 Hóa đơn";
        public string CountOutput { get => _CountOutput; set { _CountOutput = value; OnPropertyChanged(); } }

        private double _RevenueInput = 0.0;
        public double RevenueInput { get => _RevenueInput; set { _RevenueInput = value; OnPropertyChanged(); } }
        private double _RevenueOutput = 0.0;
        public double RevenueOutput { get => _RevenueOutput; set { _RevenueOutput = value; OnPropertyChanged(); } }


        private string _SelectedDay;
        public string SelectedDay
        {
            get => _SelectedDay;
            set
            {
                _SelectedDay = value;
                OnPropertyChanged();
                if (value != null)
                {
                    TitleSale = "DOANH THU ";
                    switch (value)
                    {
                        case "Hôm nay":
                            loadDataRevenue(DateTime.Now.Date, DateTime.Now.Date);
                            TotalSale = SaleList.Where(x => x.Day.Date == DateTime.Now.Date).Sum(x => x.Sales);
                            TitleSale += "HÔM NAY";
                            break;
                        case "Hôm qua":
                            loadDataRevenue(DateTime.Now.AddDays(-1).Date, DateTime.Now.AddDays(-1).Date);
                            TotalSale = SaleList.Where(x => x.Day.Date.Day == DateTime.Now.AddDays(-1).Date.Day).Sum(x => x.Sales);
                            TitleSale += "HÔM QUA";
                            break;
                        case "7 ngày qua":
                            loadDataRevenue(DateTime.Now.AddDays(-7).Date, DateTime.Now.Date);
                            TotalSale = SaleList.Where(x => x.Day.Date >= DateTime.Now.AddDays(-7).Date && x.Day.Date <= DateTime.Now.Date).Sum(x => x.Sales);
                            TitleSale += "7 NGÀY QUA";
                            break;
                        //case "Tháng này":
                        //    loadDataRevenue(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1));
                        //    TotalSale = SaleList.Where(x => x.Day.Date >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && x.Day.Date <= new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1)).Sum(x => x.Sales);
                        //    TitleSale += "THÁNG NÀY";
                        //    break;
                        //case "Tháng trước":
                        //    loadDataRevenue(new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1));
                        //    TotalSale = SaleList.Where(x => x.Day.Date >= new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1) && x.Day.Date <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1)).Sum(x => x.Sales);
                        //    TitleSale += "THÁNG TRƯỚC";
                        //    break;
                    }
                }
            }
        }

        private double _TotalSale;
        public double TotalSale { get => _TotalSale; set { _TotalSale = value; OnPropertyChanged(); } }
        private string _TitleSale;
        public string TitleSale { get => _TitleSale; set { _TitleSale = value; OnPropertyChanged(); } }

        private ObservableCollection<string> _Day;
        public ObservableCollection<string> Day { get => _Day; set { _Day = value; OnPropertyChanged(); } }

        private ObservableCollection<Sale> _SaleList;
        public ObservableCollection<Sale> SaleList { get => _SaleList; set { _SaleList = value; OnPropertyChanged(); } }


        private SeriesCollection _SeriesCollection;
        public SeriesCollection SeriesCollection { get => _SeriesCollection; set { _SeriesCollection = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _Labels;
        public ObservableCollection<string> Labels { get => _Labels; set { _Labels = value; OnPropertyChanged(); } }
        public Func<double, string> Formatter { get; set; }


        //mọi xử lý ở đây
        public OverviewViewModel()
        {





            if (Day == null)
            {
                Day = new ObservableCollection<string>();
                Day.Add("Hôm nay");
                Day.Add("Hôm qua");
                Day.Add("7 ngày qua");
                //Day.Add("Tháng này");
                //Day.Add("Tháng trước");
            }
            loadData();


        }


        public void loadRevenueToDay(DateTime dateIn, DateTime dateOut)
        {
            SqlConnection con = new SqlConnection(ConnectionString.connectionString);
            string sql = "select * from uf_Select_Revenue_Input('" + dateIn.Add(new TimeSpan(0, 0, 0)) + "','" + dateOut.Add(new TimeSpan(23, 59, 59)) + "')";
            String sql2 = "select * from uf_Select_Revenue_Output('" + dateIn.Add(new TimeSpan(0, 0, 0)) + "','" + dateOut.Add(new TimeSpan(23, 59, 59)) + "')";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlCommand cmd2 = new SqlCommand(sql2, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
            DataSet ds = new DataSet();
            DataSet ds2 = new DataSet();
            adapter.Fill(ds);
            adapter2.Fill(ds2);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CountInput = (DBNull.Value.Equals(dr[0]) ? 0 : Convert.ToInt32(dr[0].ToString())) + " Phiếu nhập";
                RevenueInput = DBNull.Value.Equals(dr[1]) ? (double)0 : (float)Convert.ToDouble(dr[1].ToString());
            }
            foreach (DataRow dr in ds2.Tables[0].Rows)
            {
                CountOutput = (DBNull.Value.Equals(dr[0]) ? 0 : Convert.ToInt32(dr[0].ToString())) + " Hóa đơn";
                RevenueOutput = DBNull.Value.Equals(dr[1]) ? (double)0 : (float)Convert.ToDouble(dr[1].ToString());
            }
        }
        public void loadDataRevenue(DateTime dateIn, DateTime dateOut)
        {

            SqlConnection con = new SqlConnection(ConnectionString.connectionString);
            string sql = "select * from uf_Select_Revenue('" + dateIn + "','" + dateOut + "')";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            if (SaleList == null)
                SaleList = new ObservableCollection<Sale>();
            else
                SaleList.Clear();

            SeriesCollection = new SeriesCollection { new ColumnSeries { Title = "Doanh thu", Values = new ChartValues<double> { } } };

            if (Labels == null)
                Labels = new ObservableCollection<string>();
            else
                Labels.Clear();


            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Sale sale = new Sale
                {
                    Day = Convert.ToDateTime(dr[0].ToString()),
                    Sales = Convert.ToDouble(dr[1].ToString())
                };
                SaleList.Add(sale);
                SeriesCollection[0].Values.Add(Convert.ToDouble(dr[1].ToString()));
                Labels.Add(Convert.ToDateTime(dr[0].ToString()).Day.ToString());
            }
            Formatter = value => value.ToString("N0");
        }
        public void loadData()
        {
            SelectedDay = "7 ngày qua";

            loadRevenueToDay(DateTime.Now.Date, DateTime.Now.Date);


        }

        private int LoadCountTotal(String sqlString)
        {
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sqlString;
                return DBNull.Value.Equals(cmd.ExecuteScalar()) ? 0 : (int)cmd.ExecuteScalar();
                //int x = cmd.ExecuteScalar() != null ? (int)cmd.ExecuteScalar() : 0;

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
