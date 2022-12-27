using System;
using System.Windows;
using System.Windows.Input;
using ToastNotifications.Position;

namespace QuanLyKho.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public Boolean IsLoaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand Logout { get; set; }


        private Visibility _ObjectVisible = Visibility.Collapsed;
        public Visibility ObjectVisible { get => _ObjectVisible; set { _ObjectVisible = value; OnPropertyChanged(); } }
        private Visibility _OutputVisible = Visibility.Collapsed;
        public Visibility OutputVisible { get => _OutputVisible; set { _OutputVisible = value; OnPropertyChanged(); } }
        private Visibility _InputVisible = Visibility.Collapsed;
        public Visibility InputVisible { get => _InputVisible; set { _InputVisible = value; OnPropertyChanged(); } }
        private Visibility _CustomerVisible = Visibility.Collapsed;
        public Visibility CustomerVisible { get => _CustomerVisible; set { _CustomerVisible = value; OnPropertyChanged(); } }
        private Visibility _SupplierVisible = Visibility.Collapsed;
        public Visibility SupplierVisible { get => _SupplierVisible; set { _SupplierVisible = value; OnPropertyChanged(); } }
        private Visibility _UserVisible = Visibility.Collapsed;
        public Visibility UserVisible { get => _UserVisible; set { _UserVisible = value; OnPropertyChanged(); } }
        private Visibility _CPUVisible = Visibility.Collapsed;
        public Visibility CPUVisible { get => _CPUVisible; set { _CPUVisible = value; OnPropertyChanged(); } }
        private Visibility _UserRoleVisible = Visibility.Collapsed;
        public Visibility UserRoleVisible { get => _UserRoleVisible; set { _UserRoleVisible = value; OnPropertyChanged(); } }

        private void ResetVisibleRole()
        {
            ObjectVisible = Visibility.Collapsed;
            InputVisible = Visibility.Collapsed;
            OutputVisible = Visibility.Collapsed;
            SupplierVisible = Visibility.Collapsed;
            CustomerVisible = Visibility.Collapsed;
            UserVisible = Visibility.Collapsed;
            CPUVisible = Visibility.Collapsed;
            UserRoleVisible = Visibility.Collapsed;
        }
        private void loadRole()
        {
            if (LoginViewModel.userCurrent != null)
                for (int i = 0; i < 8; i++)
                    if (LoginViewModel.userCurrent.UserRole.RolePermision.Contains((i + 1).ToString()))
                        switch (i + 1)
                        {
                            case 1: ObjectVisible = Visibility.Visible; break;
                            case 2: InputVisible = Visibility.Visible; break;
                            case 3: OutputVisible = Visibility.Visible; break;
                            case 4: SupplierVisible = Visibility.Visible; break;
                            case 5: CustomerVisible = Visibility.Visible; break;
                            case 6: UserVisible = Visibility.Visible; break;
                            case 7: CPUVisible = Visibility.Visible; break;
                            case 8: UserRoleVisible = Visibility.Visible; break;

                        }
        }

        public MainViewModel()
        {
            CurrentViewModel = new OverviewViewModel();
            NavCommand = new RelayCommand<string>(OnNav);

            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
           {
               IsLoaded = true;
               if (p == null)
                   return;
               p.Hide();
               LoginWindow loginWindow = new LoginWindow();
               loginWindow.ShowDialog();

               if (loginWindow.DataContext == null)
                   return;
               var loginVM = loginWindow.DataContext as LoginViewModel;
               if (loginVM.IsLogin)
               {

                   p.Show();
                   ToastViewModel _toast = new ToastViewModel(Corner.TopRight, 3, 30, 130);
                   _toast.ShowInformation(string.Format("Xin chào {0}, chúc bạn một ngày tốt lành!", LoginViewModel.userCurrent.DisplayName));
                   loadRole();
               }
               else
               {
                   p.Close();
               }
           });
            Logout = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                IsLoaded = false;
                LoginViewModel.userCurrent = null;
                ResetVisibleRole();
                if (p == null)
                    return;
                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();

                if (loginWindow.DataContext == null)
                    return;
                var loginVM = loginWindow.DataContext as LoginViewModel;
                if (loginVM.IsLogin)
                {

                    p.Show();
                    ToastViewModel _toast = new ToastViewModel(Corner.TopRight, 3, 30, 130);
                    _toast.ShowInformation(string.Format("Xin chào {0}, chúc bạn một ngày tốt lành!", LoginViewModel.userCurrent.DisplayName));
                    loadRole();
                }
                else
                {
                    p.Close();
                }

            });
        }



        private BaseViewModel _CurrentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get { return _CurrentViewModel; }
            set { SetProperty(ref _CurrentViewModel, value); }
        }

        public RelayCommand<string> NavCommand { get; private set; }
        private void OnNav(string destination)
        {

            switch (destination)
            {
                case "overView":
                    CurrentViewModel = new OverviewViewModel();
                    break;
                case "objectView":
                    CurrentViewModel = new ObjectViewModel();
                    break;
                case "outputView":
                    CurrentViewModel = new OutputViewModel();
                    break;
                case "inputView":
                    CurrentViewModel = new InputViewModel();
                    break;
                case "customerView":
                    CurrentViewModel = new CustomerViewModel();
                    break;
                case "supplierView":
                    CurrentViewModel = new SupplierViewModel();
                    break;
                case "userView":
                    CurrentViewModel = new UserViewModel();
                    break;
                case "categoryView":
                    CurrentViewModel = new CategoryViewModel("Mã loại hàng", "Tên loại hàng");
                    break;
                case "positionView":
                    CurrentViewModel = new PositionViewModel("Mã vị trí", "Tên vị trí");
                    break;
                case "unitView":
                    CurrentViewModel = new UnitViewModel("Mã đơn vị tính", "Tên đơn vị tính");
                    break;
                case "userRoleView":
                    CurrentViewModel = new UserRoleViewModel();
                    break;
                case "currentUserView":
                    CurrentViewModel = new CurrentUserViewModel();
                    break;
                default:

                    break;
            }
        }



    }
}
