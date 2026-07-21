namespace IzinSistemi_Back.Models
{
    public class Personel
    {
        public int Id { get; set; }

        public string NameSurname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }

        public int? TotalLeaveDays { get; set; }
        public int? RemainingLeaveDays { get; set; }
        public DateTime LeaveReset { get; set; }

        public List<Leave> Leaves { get; set; } = new();
    }
}
