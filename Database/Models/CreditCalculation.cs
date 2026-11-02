namespace AutoSalon.Database.Models
{
    // Fixed monthly installments; the final installment absorbs cent rounding.
    public sealed record CreditCalculation(decimal Principal, decimal Monthly, decimal Last, decimal Total, decimal Interest)
    {
        public static CreditCalculation Calculate(decimal price, decimal down, decimal annualRate, int months)
        {
            if (price <= 0 || down < 0 || down >= price || annualRate < 0 || annualRate > 100 || months < 1 || months > 120)
                throw new ArgumentException("Verificați prețul, avansul, dobânda și perioada creditului.");
            decimal principal = price - down;
            double rate = (double)annualRate / 1200;
            decimal exact = rate == 0 ? principal / months :
                principal * (decimal)(rate / (1 - Math.Pow(1 + rate, -months)));
            decimal monthly = decimal.Round(exact, 2, MidpointRounding.AwayFromZero);
            decimal total = decimal.Round(exact * months, 2, MidpointRounding.AwayFromZero);
            decimal last = total - monthly * (months - 1);
            if (monthly <= 0 || last <= 0) throw new ArgumentException("Suma finanțată este prea mică pentru perioada aleasă.");
            return new(principal, monthly, last, down + total, total - principal);
        }
    }
}
