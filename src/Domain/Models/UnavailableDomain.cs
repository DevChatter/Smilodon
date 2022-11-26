namespace Smilodon.Domain.Models
{
    public class UnavailableDomain
    {
        public long Id { get; set; }
        public string Domain { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
