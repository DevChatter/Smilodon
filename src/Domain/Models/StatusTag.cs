namespace Smilodon.Domain.Models
{
    public class StatusTag
    {
        public long StatusId { get; set; }
        public long TagId { get; set; }

        public virtual Status Status { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
