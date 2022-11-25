namespace Smilodon.Domain.Models
{
    public class PollVote
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public long? PollId { get; set; }
        public int Choice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Uri { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Poll? Poll { get; set; }
    }
}
