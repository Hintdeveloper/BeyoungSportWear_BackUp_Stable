using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;
using static DataAccessLayer.Entity.Voucher;

namespace BusinessLogicLayer.Viewmodels.VoucherM
{
    public class CreateVoucherVM
    {

        public string CreateBy { get; set; }
        [Required(ErrorMessage = "Mã voucher không được để trống")]
        [StringLength(10, ErrorMessage = "Mã voucher không được vượt quá 10 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Mã voucher không được chứa ký tự đặc biệt hoặc khoảng trắng")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Tên voucher không được để trống")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Tên voucher không được chứa ký tự đặc biệt")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CustomDateGreaterThanToday(ErrorMessage = "Ngày bắt đầu phải lớn hơn hoặc bằng ngày hiện tại")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CustomDateGreaterThanToday(ErrorMessage = "Ngày kết thúc phải lớn hơn hoặc bằng ngày hiện tại")]
        [CustomDateGreaterThan(nameof(StartDate), ErrorMessage = "Ngày kết thúc phải lớn hơn ngày bắt đầu")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, 9999, ErrorMessage = "Số lượng phải lớn hơn 0 và nhỏ hơn 10000")]
        [Numeric(ErrorMessage = "Số lượng phải là số nguyên")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Phương thức giảm giá không được để trống")]
        public Types Type { get; set; }

        [Required(ErrorMessage = "Số tiền giảm tối thiểu không được để trống")]
        [Range(0.01, 10000000, ErrorMessage = "Số tiền chi tối thiểu phải lớn hơn 0 và nhỏ hơn hoặc bằng 10,000,000")]
        [CustomDecimalLessThan(nameof(MaximumAmount), ErrorMessage = "Số tiền chi tối thiểu phải lớn hơn số tiền giảm tối đa")]
        public decimal MinimumAmount { get; set; }  

        [Required(ErrorMessage = "Số tiền giảm tối đa không được để trống")]
        [Range(0.01, 10000000, ErrorMessage = "Số tiền giảm tối đa phải lớn hơn 0 và nhỏ hơn hoặc bằng 10,000,000")]
        //[CustomDecimalGreaterThan(nameof(MinimumAmount), ErrorMessage = "Số tiền giảm tối đa phải lớn hơn số tiền giảm tối thiểu")]
        public decimal MaximumAmount { get; set; }

        [Required(ErrorMessage = "Số tiền giảm không được để trống")]
        [Range(0.01, 10000000, ErrorMessage = "Số tiền giảm phải lớn hơn 0 và nhỏ hơn hoặc bằng 10,000,000")]
        [ValidateReducedValue]
        public decimal ReducedValue { get; set; }

        public StatusVoucher? IsActive { get; set; }
        
        public List<string> SelectedUser { get; set; } = new List<string>();

        public bool ApplyToAllUsers { get; set; }
        [CustomUserSelection]
        public List<string> SelectedUserForValidation => SelectedUser;
    }

}
