//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyKho.Model
{
    using QuanLyKho.ViewModel;
    using System;
    using System.Collections.Generic;

    public partial class OutputInfo : BaseViewModel
    {
        private string _IdOutput;
        private string _IdObject;
        private Nullable<double> _Price;
        private Nullable<int> _Quantity;
        private Nullable<double> _Discount;
        private Output _Output;
        private Object _Object;



        public string IdOutput { get => _IdOutput; set { _IdOutput = value; OnPropertyChanged(); } }
        public string IdObject { get => _IdObject; set { _IdObject = value; OnPropertyChanged(); } }
        public Nullable<double> Price { get => _Price; set { _Price = value; OnPropertyChanged(); } }
        public Nullable<int> Quantity { get => _Quantity; set { _Quantity = value; OnPropertyChanged(); } }
        public Nullable<double> Discount { get => _Discount; set { _Discount = value; OnPropertyChanged(); } }
        public virtual Output Output { get => _Output; set { _Output = value; OnPropertyChanged(); } }
        public virtual Object Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }



    }
}
