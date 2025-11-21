namespace CMCS.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Lecturer"; // Lecturer, Coordinator, Manager, HR
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Department { get; set; } = "CMCS";
        public string BankAccount { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
