namespace IzinSistemi_Back.Models
{
    public class Leave
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string? LeaveType { get; set; }
        public string Status { get; set; } = string.Empty;


        public Employee? Employee { get; set; }
    
    }
}
