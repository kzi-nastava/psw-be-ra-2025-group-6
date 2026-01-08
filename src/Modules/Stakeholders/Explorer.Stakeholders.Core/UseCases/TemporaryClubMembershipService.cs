using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TemporaryClubMembershipService : IClubMembershipService
    {
        public bool IsMember(long userId, long clubId)
        {
            // za sad je svako clan svakog kluba, ovo cu promeniti kad koleginica uradi svoj deo 
            return true;
        }
    }
}
