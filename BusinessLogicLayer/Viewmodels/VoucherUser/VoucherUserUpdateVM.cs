namespace BusinessLogicLayer.Viewmodels.VoucherUser
{
    public class VoucherUserUpdateVM
    {
        public string? ModifiedBy { get; set; }
        public Guid IDVoucher { get; set; }
        public string IDUser { get; set; } = null!;
        public int Status { get; set; }
    }
}
