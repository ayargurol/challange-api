using Challange.Core.Repositories;
using Challenge.Domain;
using Challenge.Domain.Interfaces;
using Challenge.Infrastructure.Context;

namespace Challenge.Infrastructure.Repositories;

public class ProductRepository : Repository<Product, ChallengeContext>, IProductRepository
{
    public ProductRepository(ChallengeContext context) : base(context)
    {
    }
}