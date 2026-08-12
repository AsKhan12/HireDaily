namespace Hiredaily.Modules.Jobs.API.InputRequestModels;

public class UpdateJobHourlyRateInput
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}