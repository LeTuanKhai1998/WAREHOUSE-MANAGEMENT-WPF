using QuanLyKho.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Model
{
    public partial class OutputInfoView : BaseViewModel
    {

        private string _LinkImage;
        public string LinkImage { get => _LinkImage; set { _LinkImage = value; OnPropertyChanged(); } }
        private string _Id;
        public string Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }
        private int? _Quantity;
        public int? Quantity { get => _Quantity; set { _Quantity = value; OnPropertyChanged(); } }


        private Nullable<double> _Price;
        public Nullable<double> Price { get => _Price; set { _Price = value; OnPropertyChanged(); } }
        private Nullable<double> _Discount;
        public Nullable<double> Discount { get => _Discount; set { _Discount = value; OnPropertyChanged(); } }
        private Nullable<double> _TotalPrice;
        public Nullable<double> TotalPrice { get => _TotalPrice; set { _TotalPrice = value; OnPropertyChanged(); } }

        private int? _STT;
        public int? STT { get => _STT; set { _STT = value; OnPropertyChanged(); } }

        public static explicit operator OutputInfoView(int? v)
        {
            throw new NotImplementedException();
        }
    }
}
