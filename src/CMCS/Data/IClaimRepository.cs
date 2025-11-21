using CMCS.Models;

namespace CMCS.Data
{
    public interface IClaimRepository
    {
        IEnumerable<Claim> GetAll();
        IEnumerable<Claim> GetByStatus(ClaimStatus status);
        Claim? Get(int id);
        Claim Add(Claim c);
        void Update(Claim c);
        void Delete(int id);
    void AddApproval(Approval approval);
        IEnumerable<Approval> GetApprovalsForClaim(int claimId);
        IEnumerable<Approval> GetAllApprovals();
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> GetUsersByRole(string role);
        User? GetUser(int id);
        User? GetUserByName(string username);
        void UpdateUser(User user);
        void VerifyClaim(int claimId, string verifier);
    }
}