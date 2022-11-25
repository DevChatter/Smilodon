namespace Smilodon.Domain.Models
{
    public class AdminActionLog
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public string Action { get; set; } = null!;
        public string? TargetType { get; set; }
        public long? TargetId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? HumanIdentifier { get; set; }
        public string? RouteParam { get; set; }
        public string? Permalink { get; set; }

        public virtual Account? Account { get; set; }
    }
}
