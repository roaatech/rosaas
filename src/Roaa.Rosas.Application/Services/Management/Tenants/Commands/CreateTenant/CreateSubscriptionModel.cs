namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;


public record CreateSubscriptionModel
{
    public Guid ProductId { get; set; }
    public Guid? PlanId { get; set; }
    public Guid? PlanPriceId { get; set; }
    public int? CustomPeriodInDays { get; set; } = null;
    public bool UserEnabledTheTrial { get; set; } = true;
    public List<CreateSpecificationValueModel> Specifications { get; set; } = new();

    private int? _sequenceNum;

    public void SetSequenceNum(int sequenceNum)
    {
        _sequenceNum = sequenceNum;
    }

    public int? GetSequenceNum()
    {
        return _sequenceNum;
    }
}

public record CreateSpecificationValueModel
{
    public Guid SpecificationId { get; set; }
    public string Value { get; set; } = string.Empty;
}