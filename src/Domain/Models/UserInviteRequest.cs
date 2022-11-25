namespace Smilodon.Domain.Models
{
    public class UserInviteRequest
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual User? User { get; set; }
    }
}
