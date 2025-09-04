namespace Movie_management_system.DTOs
{
    public class TicketPDFDTO
    {
        public string TicketNumber { get; set; }
        public string Cinema { get; set; }
        public string Hall { get; set; }
        public string Movie { get; set; }
        public string Seats { get; set; }
        public DateTime ShowDateTime { get; set; }
        public decimal Price { get; set; }
    }

}
