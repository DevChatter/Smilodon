namespace Smilodon.Domain.Models
{
    public class Mention
    {
        public long Id { get; set; }
        public long? StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? AccountId { get; set; }
        public bool Silent { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Status? Status { get; set; }
    }
}
