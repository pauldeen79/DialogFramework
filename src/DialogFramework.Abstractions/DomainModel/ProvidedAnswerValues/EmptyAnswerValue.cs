namespace DialogFramework.Abstractions.DomainModel.ProvidedAnswerValues;

public class EmptyAnswerValue : IProvidedAnswerValue
{
    public object? Value => null;
}
