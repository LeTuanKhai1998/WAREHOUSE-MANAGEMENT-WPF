using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    class ConfirmViewModel : BaseViewModel
    {
        private SqlConnection con;

        private bool _Result = false;
        public bool Result { get => _Result; set { _Result = value; OnPropertyChanged(); } }

        private String _Title;
        public String Title { get => _Title; set { _Title = value; OnPropertyChanged(); } }
        private String _Content;

        public String Content { get => _Content; set { _Content = value; OnPropertyChanged(); } }

        public ICommand OkCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }
        public ConfirmViewModel()
        {
            OkCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Result = true; p.Close(); });
            CloseCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Result = false; p.Close(); });
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
    }
}
