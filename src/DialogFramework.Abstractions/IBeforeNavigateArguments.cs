namespace DialogFramework.Abstractions;

public interface IBeforeNavigateArguments : INavigateArguments
{
    void CancelStateUpdate();
}
