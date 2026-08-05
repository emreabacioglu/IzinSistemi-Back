namespace IzinSistemi_Back.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public bool IsAdmin { get; set; } = false;

        public int? TotalLeaveDays { get; set; }
        public int? RemainingLeaveDays { get; set; }
        public DateTime? LeaveReset { get; set; }
        public DateTime? BirthDay { get; set; }

        public List<Leave> Leaves { get; set; } = new();
    }
}
