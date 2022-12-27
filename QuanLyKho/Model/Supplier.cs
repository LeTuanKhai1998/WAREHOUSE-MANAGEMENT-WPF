using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Model
{
    using QuanLyKho.ViewModel;
    using System;
    using System.Collections.Generic;

    public partial class Supplier : BaseViewModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Supplier()
        {
            this.Inputs = new HashSet<Input>();
        }

        private Nullable<int> _Id;
        private string _DisplayName;
        private string _Phone;
        private string _Address;
        private string _Email;
        private string _MoreInfo;
        private Nullable<System.DateTime> _ContractDate;
        private string _Status;

        private ICollection<Input> _Inputs;



        public Nullable<int> Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }
        public string Phone { get => _Phone; set { _Phone = value; OnPropertyChanged(); } }
        public string Address { get => _Address; set { _Address = value; OnPropertyChanged(); } }
        public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }
        public string MoreInfo { get => _MoreInfo; set { _MoreInfo = value; OnPropertyChanged(); } }
        public Nullable<System.DateTime> ContractDate { get => _ContractDate; set { _ContractDate = value; OnPropertyChanged(); } }
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }


        private bool _IsSelected;
        public bool IsSelected { get => _IsSelected; set { _IsSelected = value; OnPropertyChanged(); } }
        private int _IsVisible;
        public int IsVisible { get => _IsVisible; set { _IsVisible = value; OnPropertyChanged(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Input> Inputs { get => _Inputs; set { _Inputs = value; OnPropertyChanged(); } }
    }
}
