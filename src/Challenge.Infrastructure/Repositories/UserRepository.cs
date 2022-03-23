using Challange.Core.Repositories;
using Challenge.Domain;
using Challenge.Domain.Interfaces;
using Challenge.Infrastructure.Context;

namespace Challenge.Infrastructure.Repositories;

public class UserRepository : Repository<User, ChallengeContext>, IUserRepository
{
    public UserRepository(ChallengeContext context) : base(context)
    {
    }
}