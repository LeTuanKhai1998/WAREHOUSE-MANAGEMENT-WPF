using QuanLyKho.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Model
{
    public class Sale:BaseViewModel
    {
        private double _Sales;
        public double Sales { get => _Sales; set { _Sales = value; OnPropertyChanged(); } }

        private System.DateTime _Day;
        public System.DateTime Day { get => _Day; set { _Day = value; OnPropertyChanged(); } }
    }
}
