namespace DialogFramework.Domain.DomainModel.DialogPartResultValues.Builders;

public class DateDialogPartResultValueBuilder : DialogPartResultValueBuilder
{
    public DateDialogPartResultValueBuilder() : base()
        => ResultValueType = ResultValueType.Date;

    public DateDialogPartResultValueBuilder(IDialogPartResultValue source) : base(source)
        => ResultValueType = ResultValueType.Date;
}
