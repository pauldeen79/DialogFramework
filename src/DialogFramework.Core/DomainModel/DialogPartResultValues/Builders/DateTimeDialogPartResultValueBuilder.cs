namespace DialogFramework.Core.DomainModel.DialogPartResultValues.Builders;

public class DateTimeDialogPartResultValueBuilder : DialogPartResultValueBuilder
{
    public DateTimeDialogPartResultValueBuilder() : base()
        => ResultValueType = ResultValueType.DateTime;

    public DateTimeDialogPartResultValueBuilder(IDialogPartResultValue source) : base(source)
        => ResultValueType = ResultValueType.DateTime;
}
