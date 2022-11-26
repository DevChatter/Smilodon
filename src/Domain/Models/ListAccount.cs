namespace Smilodon.Domain.Models
{
    public class ListAccount
    {
        public long Id { get; set; }
        public long ListId { get; set; }
        public long AccountId { get; set; }
        public long? FollowId { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Follow? Follow { get; set; }
        public virtual List List { get; set; } = null!;
    }
}
