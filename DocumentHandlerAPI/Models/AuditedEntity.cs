namespace DocumentHandlerAPI.Models
{
    public class AuditedEntity
    {
        public Ulid Id { get; set; }
        public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? LastModificationTime { get; set; }
        public DateTimeOffset? DeletionTime { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
