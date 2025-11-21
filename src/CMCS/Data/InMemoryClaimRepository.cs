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
            // Seed with a sample claim
            _claims.Add(new Claim { Id = _nextId++, HoursWorked = 10, HourlyRate = 50, Notes = "Sample claim" });
            _users.Add(new User { Id = 1, Username = "alice", Role = "Lecturer" });
            _users.Add(new User { Id = 2, Username = "bob", Role = "Coordinator" });
            _users.Add(new User { Id = 3, Username = "carol", Role = "Manager" });
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

        public User? GetUserByName(string username) => _users.FirstOrDefault(u => u.Username == username);

        public Claim? Get(int id) => _claims.FirstOrDefault(x => x.Id == id);

        public IEnumerable<Claim> GetAll() => _claims.OrderByDescending(c => c.SubmittedAt);

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