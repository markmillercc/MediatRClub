using MediatR;

namespace MediatRClub
{
    public class MembershipExpired : INotification
    { 
        public Member Member { get; set; }
    }
}
