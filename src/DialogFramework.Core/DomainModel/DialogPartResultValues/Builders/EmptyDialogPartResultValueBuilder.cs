namespace DialogFramework.Core.DomainModel.DialogPartResultValues.Builders;

public class EmptyDialogPartResultValueBuilder : DialogPartResultValueBuilder
{
    public EmptyDialogPartResultValueBuilder() : base()
        => ResultValueType = ResultValueType.None;

    public EmptyDialogPartResultValueBuilder(IDialogPartResultValue source) : base(source)
        => ResultValueType = ResultValueType.None;
}
