namespace DialogFramework.Domain.DialogPartResultValues.Builders;

public class YesNoDialogPartResultValueBuilder : DialogPartResultValueBuilder
{
    public YesNoDialogPartResultValueBuilder() : base()
        => ResultValueType = ResultValueType.YesNo;

    public YesNoDialogPartResultValueBuilder(IDialogPartResultValue source) : base(source)
        => ResultValueType = ResultValueType.YesNo;
}
