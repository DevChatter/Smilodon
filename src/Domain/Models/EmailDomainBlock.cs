namespace Smilodon.Domain.Models
{
    public class EmailDomainBlock
    {
        public long Id { get; set; }
        public string Domain { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? ParentId { get; set; }

        public virtual EmailDomainBlock? Parent { get; set; }
        public virtual ICollection<EmailDomainBlock> InverseParent { get; set; } = new HashSet<EmailDomainBlock>();
    }
}
