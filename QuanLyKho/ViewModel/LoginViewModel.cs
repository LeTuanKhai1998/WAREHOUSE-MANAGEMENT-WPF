using QuanLyKho.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private ToastViewModel _toast = null;
        public static User userCurrent;
        private ObservableCollection<UserRole> _List;
        public ObservableCollection<UserRole> List { get => _List; set { _List = value; OnPropertyChanged(); } }
        public bool IsLogin { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }

        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        private string _Password;
        private SqlConnection con;

        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }

        //mọi xử lý ở đây
        public LoginViewModel()
        {
            IsLogin = false;
            Password = "";
            UserName = "";
            LoginCommand = new RelayCommand<Window>((p) =>
            {
                if (String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(Password))
                    return false;
                else
                    return true;
            }, (p) => { Login(p); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });


        }

        void Login(Window p)
        {
            loadData();
            _toast = new ToastViewModel(Corner.BottomCenter, 1, 0, 100);
            if (p == null)
                return;
            Password = MD5Hash(Base64Encode(Password));
            User user = null;
            string query = "Select * from uf_Check_Login('" + UserName + "','" + Password + "')";
            using (SqlConnection connection = new SqlConnection(ConnectionString.connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                //command.Parameters.AddWithValue("@Password", Password);
                //command.Parameters.AddWithValue("@UserName", UserName);

                //id = command.ExecuteScalar() == null ? 0 : (int)command.ExecuteScalar();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    user = new User
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
                        IsVisible = Convert.ToInt32(dr[12].ToString()),
                        UserRole = List.FirstOrDefault(x => x.Id == Convert.ToInt32(dr[10].ToString()))
                    };


                }

                connection.Close();
            }
            if (user != null)
            {
                IsLogin = true;
                userCurrent = user;
                p.Close();
            }
            else
            {
                IsLogin = false;
                _toast.ShowError("Sai tài khoản hoặc mật khẩu!");
            }


            //using (var ctx = new QuanLyKhoDemoEntities())
            //{

            //    var accCount = ctx.Users.SqlQuery("Select * from Users where Password = @Password and UserName = @UserName"
            //        , new SqlParameter("@Password", Password), new SqlParameter("@UserName", UserName)).Count();

            //    if (accCount > 0)
            //    {
            //        IsLogin = true;
            //        p.Close();
            //    }
            //    else
            //    {
            //        IsLogin = false;
            //        _toast.Show("Sai tài khoản hoặc mật khẩu!");
            //    }
            //}
        }
        private void loadData()
        {
            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from uv_View_UserRole", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (List == null)
                    List = new ObservableCollection<UserRole>();
                else
                    List.Clear();


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    UserRole user = new UserRole
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString(),
                        RolePermision = dr[2].ToString()
                    };
                    List.Add(user);
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
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string MD5Hash(string input)
        {

            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }




    }
}
