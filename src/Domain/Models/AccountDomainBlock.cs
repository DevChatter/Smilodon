namespace Smilodon.Domain.Models
{
    public class AccountDomainBlock
    {
        public long Id { get; set; }
        public string? Domain { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? AccountId { get; set; }

        public virtual Account? Account { get; set; }
    }
}
