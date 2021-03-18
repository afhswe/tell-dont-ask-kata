using System.Collections.Generic;
using System.Linq;
using TellDontAskKata.Domain;
using TellDontAskKata.Repository;

namespace TellDontAskKata.Test.Doubles
{
    public class InMemoryProductCatalog : IProductCatalog
    {
        private readonly List<Product> products;

        public InMemoryProductCatalog(List<Product> products)
        {
            this.products = products;
        }

        public Product GetByName(string name)
        {
            return products.Where(p => p.GetName().Equals(name)).FirstOrDefault();
        }
    }
}
