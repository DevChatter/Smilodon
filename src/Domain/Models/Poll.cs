namespace Smilodon.Domain.Models
{
    public class Poll
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public long? StatusId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string[] Options { get; set; } = null!;
        public long[] CachedTallies { get; set; } = null!;
        public bool Multiple { get; set; }
        public bool HideTotals { get; set; }
        public long VotesCount { get; set; }
        public DateTime? LastFetchedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LockVersion { get; set; }
        public long? VotersCount { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Status? Status { get; set; }
        public virtual ICollection<PollVote> PollVotes { get; set; } = new HashSet<PollVote>();
    }
}
