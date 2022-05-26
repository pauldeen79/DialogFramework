namespace DialogFramework.Domain.DomainModel.DialogPartResultValues.Builders;

public class NumberDialogPartResultValueBuilder : DialogPartResultValueBuilder
{
    public NumberDialogPartResultValueBuilder() : base()
        => ResultValueType = ResultValueType.Number;

    public NumberDialogPartResultValueBuilder(IDialogPartResultValue source) : base(source)
        => ResultValueType = ResultValueType.Number;
}
