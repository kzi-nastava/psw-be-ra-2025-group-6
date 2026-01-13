using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain;

public class Wallet : Entity
{
    public long TouristId { get; private set; }
    public double BalanceAc { get; private set; }

    private Wallet() { }

    public Wallet(long touristId)
    {
        TouristId = touristId;
        BalanceAc = 0;
        Validate();
    }

    public void IncreaseBalance(double amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");

        BalanceAc += amount;
    }

    private void Validate()
    {
        if (TouristId == 0) throw new ArgumentException("Invalid TouristId");
        if (BalanceAc < 0) throw new ArgumentException("Invalid balance");
    }
}
