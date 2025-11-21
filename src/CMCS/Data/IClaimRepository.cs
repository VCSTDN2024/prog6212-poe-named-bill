using CMCS.Models;

namespace CMCS.Data
{
    public interface IClaimRepository
    {
        IEnumerable<Claim> GetAll();
        Claim? Get(int id);
        Claim Add(Claim c);
        void Update(Claim c);
        void Delete(int id);
    }
}