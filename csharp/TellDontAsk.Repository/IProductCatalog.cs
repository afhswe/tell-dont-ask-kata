using System;
using System.Collections.Generic;
using System.Text;
using TellDontAskKata.Domain;

namespace TellDontAskKata.Repository
{
    public interface IProductCatalog
    {
        Product getByName(String name);
    }

}
