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
    public class UpdateVoucherVM
    {

        public string? ModifiedBy { get; set; }
        [Required(ErrorMessage = "Mã voucher không được để trống")]
        [StringLength(10, ErrorMessage = "Mã voucher không được vượt quá 10 ký tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Mã voucher không được chứa khoảng trắng")]
        public string Code { get; set; } = null!;
        [Required(ErrorMessage = "Tên voucher không được để trống")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.DateTime)] // Sử dụng DateTime để bao gồm giờ và phút
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        //[CustomDateGreaterThanToday(ErrorMessage = "Ngày bắt đầu phải lớn hơn hoặc bằng ngày hiện tại")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [DataType(DataType.DateTime)] // Sử dụng DateTime để bao gồm giờ và phút
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [CustomDateGreaterThanToday(ErrorMessage = "Ngày kết thúc phải lớn hơn hoặc bằng ngày hiện tại")]
        [CustomDateGreaterThan(nameof(StartDate), ErrorMessage = "Ngày kết thúc phải lớn hơn ngày bắt đầu")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, 9999, ErrorMessage = "Số lượng phải lớn hơn 0 và nhỏ hơn 10000")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Phương thức giảm giá không được để trống")]
        public Types Type { get; set; }
        [Required(ErrorMessage = "Số tiền chi tối thiểu không được để trống")]
        [Range(0.01, 10000000, ErrorMessage = "Số tiền giảm tối thiểu phải lớn hơn 0 10000000")]
        public decimal MinimumAmount { get; set; }
        [Required(ErrorMessage = "Số tiền giảm tối đa không được để trống")]
        [Range(0.01, 10000000, ErrorMessage = "Số tiền giảm tối đa phải lớn hơn 0 10000000")]
        //[CustomDecimalGreaterThan(nameof(MinimumAmount), ErrorMessage = "Số tiền giảm tối đa phải lớn hơn số tiền giảm tối thiểu")]
        public decimal MaximumAmount { get; set; }
        [Required(ErrorMessage = "Số tiền giảm không được để trống")]
        [Range(0.01, 10000000, ErrorMessage = "Số tiền giảm phải lớn hơn 0 10000000")]
        [ValidateReducedValueUpdate]
        public decimal ReducedValue { get; set; }
        public StatusVoucher IsActive { get; set; }
        public int Status { get; set; }
        
    }
    public class CustomerVM
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
