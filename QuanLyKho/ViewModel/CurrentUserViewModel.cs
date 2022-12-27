using QuanLyKho.Model;
using QuanLyKho.View;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    public class CurrentUserViewModel : BaseViewModel
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataSet ds;
        private ToastViewModel _toast = null;
        private User _User;
        public User User { get => _User; set { _User = value; OnPropertyChanged(); } }

        public ICommand EditCommand { get; set; }

        public CurrentUserViewModel(int x)
        {

        }

        public CurrentUserViewModel()
        {

            User = LoginViewModel.userCurrent;

            _toast = new ToastViewModel(Corner.BottomRight, 4, 10, 20);


            EditCommand = new RelayCommand<User>(p => { return true; }, p =>
            {

                if (LoginViewModel.userCurrent != null)
                {
                    UserEditViewModel editViewModel = new UserEditViewModel();
                    UserEditView editView = new UserEditView();
                    editView.DataContext = editViewModel;
                    editViewModel.Title = "Cật nhật người dùng";
                    editViewModel.SelectedUserRole = LoginViewModel.userCurrent.UserRole;
                    editViewModel.User = LoginViewModel.userCurrent;
                    editViewModel.RoleVisible = Visibility.Collapsed;
                    if (LoginViewModel.userCurrent.Sex.Contains("Nam"))
                        editViewModel.RadioMale = true;
                    else
                        editViewModel.RadioFeMale = true;
                    editView.ShowDialog();
                    loadData();

                }
            });

        }



        private void loadData()
        {


            try
            {
                con = new SqlConnection(ConnectionString.connectionString);
                con.Open();
                cmd = new SqlCommand("exec usp_View_CurrentUser " + LoginViewModel.userCurrent.Id, con);
                SqlCommand cmd2 = new SqlCommand("exec usp_View_CurrentUserRole " + LoginViewModel.userCurrent.IdRole, con);
                adapter = new SqlDataAdapter(cmd);
                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                ds = new DataSet();
                DataSet ds2 = new DataSet();
                adapter.Fill(ds);
                adapter2.Fill(ds2);
                UserRole role = new UserRole();
                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    role = new UserRole
                    {
                        Id = Convert.ToInt32(dr[0].ToString()),
                        DisplayName = dr[1].ToString(),
                        RolePermision = dr[2].ToString()
                    };

                }

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(dr[12].ToString()) == 0)
                    {
                         LoginViewModel.userCurrent = new User
                        {
                            Id = Convert.ToInt32(dr[0].ToString()),
                            DisplayName = dr[1].ToString(),
                            BirthDay = DBNull.Value.Equals(dr[2]) ? null : (DateTime?)Convert.ToDateTime(dr[2].ToString()),
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
                            IsSelected = false,
                            UserRole = role
                        };
                        User = LoginViewModel.userCurrent;
                        
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
                adapter.Dispose();
                con.Close();
                con.Dispose();
            }
        }
    }
}