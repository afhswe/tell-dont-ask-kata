using System;

namespace TellDontAskKata.Domain
{
    public class Category
    {
        private String _name;
        private decimal _taxPercentage;

        public String GetName()
        {
            return _name;
        }

        public void SetName(String name)
        {
            this._name = name;
        }

        public decimal GetTaxPercentage()
        {
            return _taxPercentage;
        }

        public void SetTaxPercentage(decimal taxPercentage)
        {
            this._taxPercentage = taxPercentage;
        }
    }
}
