namespace Domain.Common
{
    public class AuditableEntity
    {
        public DateTime Created { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? LastModified { get; set; }

        public string? LastModifiedBy { get; set; }
    }
}