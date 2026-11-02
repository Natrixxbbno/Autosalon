using AutoSalon.Database.Models;

void Equal(decimal expected, decimal actual)
{
    if (actual != expected) throw new Exception($"Expected {expected}, got {actual}.");
}
void Reject(Action action)
{
    try { action(); } catch (ArgumentException) { return; }
    throw new Exception("Invalid credit terms were accepted.");
}

var zero = CreditCalculation.Calculate(20000, 5000, 0, 12);
Equal(15000, zero.Principal); Equal(1250, zero.Monthly); Equal(1250, zero.Last);
Equal(20000, zero.Total); Equal(0, zero.Interest);

var interest = CreditCalculation.Calculate(10000, 0, 12, 12);
Equal(888.49m, interest.Monthly); Equal(888.46m, interest.Last);
Equal(10661.85m, interest.Total); Equal(661.85m, interest.Interest);
Equal(interest.Total, interest.Monthly * 11 + interest.Last);

var rounding = CreditCalculation.Calculate(100, 0, 0, 3);
Equal(33.33m, rounding.Monthly); Equal(33.34m, rounding.Last);
Equal(100, rounding.Monthly * 2 + rounding.Last);

var single = CreditCalculation.Calculate(10000, 1000, 12, 1);
Equal(9090, single.Monthly); Equal(10090, single.Total);
Reject(() => CreditCalculation.Calculate(100, 100, 5, 12));
Reject(() => CreditCalculation.Calculate(100, -1, 5, 12));
Reject(() => CreditCalculation.Calculate(100, 0, -1, 12));
Reject(() => CreditCalculation.Calculate(100, 0, 101, 12));
Reject(() => CreditCalculation.Calculate(100, 0, 5, 0));
Reject(() => CreditCalculation.Calculate(100, 0, 5, 121));
Reject(() => CreditCalculation.Calculate(0.01m, 0, 0, 120));
Console.WriteLine("Credit calculations passed: zero interest, annuity, final installment rounding, one-month term and invalid inputs.");
