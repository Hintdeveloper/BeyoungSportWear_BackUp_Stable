namespace DataAccessLayer.Entity.Base
{
    public class NoIDEntityBase
    {
        public string CreateBy { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? DeleteBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int Status { get; set; }
    }
}
