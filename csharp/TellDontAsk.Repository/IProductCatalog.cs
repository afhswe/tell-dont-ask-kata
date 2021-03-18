using System;
using TellDontAskKata.Domain;

namespace TellDontAskKata.Repository
{
    public interface IProductCatalog
    {
        Product getByName(String name);
    }

}
