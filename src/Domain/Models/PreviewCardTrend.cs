namespace Smilodon.Domain.Models
{
    public class PreviewCardTrend
    {
        public long Id { get; set; }
        public long PreviewCardId { get; set; }
        public double Score { get; set; }
        public int Rank { get; set; }
        public bool Allowed { get; set; }
        public string? Language { get; set; }

        public virtual PreviewCard PreviewCard { get; set; } = null!;
    }
}
