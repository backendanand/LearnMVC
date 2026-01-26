namespace LearnMVC.Models
{
    public class BaseEntity
    {
        public long id { get; set; }
        public DateTime created_at { get; set; }
        public long created_by { get; set; }
        public string created_ip { get; set; }
        public DateTime? updated_at { get; set; }
        public long? updated_by { get; set; }
        public string? updated_ip { get; set; }
        public DateTime? deleted_at { get; set; }
        public long? deleted_by { get; set; }
        public string? deleted_ip { get; set; }
    }
}
