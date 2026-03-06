namespace PicurBackend.Domain.Entities
{
    public class Reading
    {
        public int Id { get; set; }
        public double Temperature { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Door { get; set; }
    }
}
