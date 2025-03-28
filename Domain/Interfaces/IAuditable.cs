namespace Domain.Interfaces
{
    public interface IAuditable
    {
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
