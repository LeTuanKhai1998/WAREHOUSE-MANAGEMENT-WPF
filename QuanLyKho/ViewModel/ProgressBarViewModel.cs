using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QuanLyKho.ViewModel
{
    class ProgressBarViewModel:BaseViewModel
    {
        private string _Title;
        public string Title { get { return _Title; } set { _Title = value; OnPropertyChanged(); } }
        private SolidColorBrush _Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"));
        public SolidColorBrush Color { get { return _Color; } set { _Color = value; OnPropertyChanged(); } }
        public ICommand MouseMoveWindowCommand { get; set; }
        public ProgressBarViewModel()
        {
            MouseMoveWindowCommand = new RelayCommand<Window>((p) => { return p == null ? false : true; }, (p) => {
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
