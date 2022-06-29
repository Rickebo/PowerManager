namespace PowerManager;

public interface IActivePowerPlanProvider
{
    Guid ActivePlanGuid { get; }
}