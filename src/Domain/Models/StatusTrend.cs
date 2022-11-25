namespace Smilodon.Domain.Models
{
    public class StatusTrend
    {
        public long Id { get; set; }
        public long StatusId { get; set; }
        public long AccountId { get; set; }
        public double Score { get; set; }
        public int Rank { get; set; }
        public bool Allowed { get; set; }
        public string? Language { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
    }
}
