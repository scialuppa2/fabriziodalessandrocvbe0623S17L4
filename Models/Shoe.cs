namespace esercizioS17L3.Models
{
    public class Shoe
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? CoverImage { get; set; }
        public string? AdditionalImage1 { get; set; }
        public string? AdditionalImage2 { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
    }
}
