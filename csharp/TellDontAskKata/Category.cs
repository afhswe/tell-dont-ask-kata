using System;

namespace TellDontAskKata.Domain
{
    public class Category
    {
        private String _name;
        private decimal _taxPercentage;

        public String getName()
        {
            return _name;
        }

        public void setName(String name)
        {
            this._name = name;
        }

        public decimal getTaxPercentage()
        {
            return _taxPercentage;
        }

        public void setTaxPercentage(decimal taxPercentage)
        {
            this._taxPercentage = taxPercentage;
        }
    }
}
