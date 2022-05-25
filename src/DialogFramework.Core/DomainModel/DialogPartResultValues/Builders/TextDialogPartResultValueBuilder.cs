namespace DialogFramework.Core.DomainModel.DialogPartResultValues.Builders;

public class TextDialogPartResultValueBuilder : DialogPartResultValueBuilder
{
    public TextDialogPartResultValueBuilder() : base()
        => ResultValueType = ResultValueType.Text;

    public TextDialogPartResultValueBuilder(IDialogPartResultValue source) : base(source)
        => ResultValueType = ResultValueType.Text;
}
