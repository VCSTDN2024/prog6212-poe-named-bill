using CMCS.Models;

namespace CMCS.Data
{
    public class InMemoryClaimRepository : IClaimRepository
    {
        private readonly List<Claim> _claims = new();
        private readonly List<Approval> _approvals = new();
        private readonly List<User> _users = new();
        private int _nextId = 1;

        public InMemoryClaimRepository()
        {
            // Seed with sample claims and users that exercise each workflow state
            var now = DateTime.UtcNow;

            _claims.Add(new Claim
            {
                Id = _nextId++,
                HoursWorked = 24,
                HourlyRate = 380,
                Notes = "Tutorial support block release",
                SubmittedBy = "alice",
                SubmittedAt = now.AddDays(-3),
                AttachedFiles = new List<string> { "timesheet-mar.pdf" }
            });

            _claims.Add(new Claim
            {
                Id = _nextId++,
                HoursWorked = 92,
                HourlyRate = 450,
                Notes = "Semester wrap-up workshops",
                SubmittedBy = "alice",
                SubmittedAt = now.AddDays(-2),
                Status = ClaimStatus.Verified,
                AttachedFiles = new List<string> { "workshop-roster.xlsx", "invoice.pdf" }
            });

            _claims.Add(new Claim
            {
                Id = _nextId++,
                HoursWorked = 130,
                HourlyRate = 620,
                Notes = "Exam support and moderation",
                SubmittedBy = "alice",
                SubmittedAt = now.AddDays(-1),
                Status = ClaimStatus.Approved,
                AttachedFiles = new List<string> { "marking-proof.zip" }
            });

            _claims.Add(new Claim
            {
                Id = _nextId++,
                HoursWorked = 210,
                HourlyRate = 1500,
                Notes = "Emergency weekend marking",
                SubmittedBy = "alice",
                SubmittedAt = now.AddHours(-8),
                Status = ClaimStatus.Pending,
                AttachedFiles = new List<string>()
            });

            _users.Add(new User { Id = 1, Username = "alice", FullName = "Alice Mokoena", Role = "Lecturer", Email = "alice@example.com", PhoneNumber = "+27 82 555 0101", Department = "Information Systems", BankAccount = "FNB ••••1234", Address = "12 King St, Johannesburg" });
            _users.Add(new User { Id = 2, Username = "bob", FullName = "Bob Naidoo", Role = "Coordinator", Email = "bob@example.com", PhoneNumber = "+27 83 555 2223", Department = "Programme Office" });
            _users.Add(new User { Id = 3, Username = "carol", FullName = "Carol Smith", Role = "Manager", Email = "carol@example.com", PhoneNumber = "+27 84 555 7890", Department = "Academic Management" });
            _users.Add(new User { Id = 4, Username = "helen", FullName = "Helen Dlamini", Role = "HR", Email = "helen@example.com", PhoneNumber = "+27 82 555 0042", Department = "Human Resources" });
        }

        public Claim Add(Claim c)
        {
            c.Id = _nextId++;
            c.SubmittedAt = DateTime.UtcNow;
            _claims.Add(c);
            return c;
        }

        public void AttachFilesToClaim(int claimId, IEnumerable<string> fileNames)
        {
            var c = Get(claimId);
            if (c == null) return;
            if (c.AttachedFiles == null) c.AttachedFiles = new List<string>();
            c.AttachedFiles.AddRange(fileNames);
        }

        public void Delete(int id)
        {
            var existing = Get(id);
            if (existing != null) _claims.Remove(existing);
        }

        public void AddApproval(Approval a)
        {
            a.Id = _approvals.Count + 1;
            _approvals.Add(a);
        }

        public void VerifyClaim(int claimId, string verifier)
        {
            var c = Get(claimId);
            if (c == null) return;
            c.Status = ClaimStatus.Verified;
            AddApproval(new Approval { ClaimId = claimId, ApprovedBy = verifier, ApprovedAt = DateTime.UtcNow, IsApproved = true });
        }

        public IEnumerable<Approval> GetApprovalsForClaim(int claimId) => _approvals.Where(x => x.ClaimId == claimId);

        public IEnumerable<Approval> GetAllApprovals() => _approvals;

        public User? GetUserByName(string username) => _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<User> GetAllUsers() => _users;

        public IEnumerable<User> GetUsersByRole(string role) => _users.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase));

        public User? GetUser(int id) => _users.FirstOrDefault(u => u.Id == id);

        public void UpdateUser(User user)
        {
            var existing = GetUser(user.Id);
            if (existing == null) return;
            existing.FullName = user.FullName;
            existing.Email = user.Email;
            existing.PhoneNumber = user.PhoneNumber;
            existing.Department = user.Department;
            existing.BankAccount = user.BankAccount;
            existing.Address = user.Address;
            existing.Role = user.Role;
        }

        public Claim? Get(int id) => _claims.FirstOrDefault(x => x.Id == id);

        public IEnumerable<Claim> GetAll() => _claims.OrderByDescending(c => c.SubmittedAt);

        public IEnumerable<Claim> GetByStatus(ClaimStatus status) => _claims.Where(c => c.Status == status).OrderBy(c => c.SubmittedAt);

        public void Update(Claim c)
        {
            var existing = Get(c.Id);
            if (existing == null) return;
            existing.HoursWorked = c.HoursWorked;
            existing.HourlyRate = c.HourlyRate;
            existing.Notes = c.Notes;
            existing.AttachedFiles = c.AttachedFiles;
            existing.Status = c.Status;
        }
    }
}