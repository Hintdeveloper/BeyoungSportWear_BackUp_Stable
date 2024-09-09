using System.ComponentModel;

namespace DataAccessLayer.Entity.Base
{
    public class EnumBase
    {
        public enum PaymentMethod
        {
            ChuyenKhoanNganHang, 
            TienMatKhiGiaoHang, 
        }
        public enum AddressCreationStatus
        {
            InvalidRequest = 0,
            Success = 1,
            PhoneNumberExists = 2,
            MaxAddressesReached = 3
        }

        public enum ShippingMethod
        {
            NhanTaiCuaHang,      // Nhận tại cửa hàng
            GiaoHangTieuChuan,   // Giao hàng tiêu chuẩn
        }
        public enum PaymentStatus
        {
            Unknown,   // Chưa thanh toán
            Success,     // Đã thanh toán
            Pending,        // Đang xử lý
            Failure,    // Lỗi thanh toán
        }
        public enum OrderStatus
        {
            [Description("Chưa giải quyết")]
            Pending,        //Chưa giải quyết
            [Description("Đã xác nhận")]
            Processed,     //Đã Xử lý
            [Description("Đang vận chuyển")]
            Shipping,        //Đang vận chuyển
            [Description("Hoàn thành")]
            Delivered,      //Đã giao hàng <=> success
            [Description("Đã hủy")]
            Cancelled,      //Đã hủy
        }
        public enum SearchCriteria
        {
            Product,
            Material,
            Brand,
            Category,
            Manufacturer
        }
        public enum StatusVoucher
        {
            HasntStartedYet,
            IsBeginning,
            Finished
        }
        public enum OrderType
        {
            Online,
            InStore
        }
    }
}
