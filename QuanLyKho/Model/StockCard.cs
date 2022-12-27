using QuanLyKho.ViewModel;
using System;

namespace QuanLyKho.Model
{

    public partial class StockCard : BaseViewModel
    {


        private string _Id;
        public string Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        private string _Method;
        public string Method { get => _Method; set { _Method = value; OnPropertyChanged(); } }
        private Nullable<System.DateTime> _Date;
        public Nullable<System.DateTime> Date { get => _Date; set { _Date = value; OnPropertyChanged(); } }
        private Nullable<double> _InputPrice;
        public Nullable<double> InputPrice { get => _InputPrice; set { _InputPrice = value; OnPropertyChanged(); } }
        private int? _Quantity;
        public int? Quantity { get => _Quantity; set { _Quantity = value; OnPropertyChanged(); } }

        private int? _Stock;
        public int? Stock { get => _Stock; set { _Stock = value; OnPropertyChanged(); } }



    }
}
