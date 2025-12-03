namespace TaxChecker.Domain
{
    public class TaxRule
    {
        public int Id { get; private set; }
        public int CityId { get; private set; }
        public TaxRuleType Type { get; private set; }
        public decimal Rate { get; private set; }
        public DateTime ValidFrom { get; private set; }
        public DateTime ValidTo { get; private set; }

        public TaxRule(int cityId, TaxRuleType type, decimal rate, DateTime validFrom, DateTime validTo)
        {
            if (rate <= 0)
                throw new ArgumentException("Tax rate must be positive.");

            if (validFrom > validTo)
                throw new ArgumentException("Start date must be <= end date.");

            CityId = cityId;
            Type = type;
            Rate = rate;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
