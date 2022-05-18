using System;
using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.Domains;

namespace DialogFramework.Core.DomainModel.DialogPartResultValues
{
    public record DateTimeDialogPartResultValue : IDialogPartResultValue
    {
        public DateTimeDialogPartResultValue(DateTime value) => Value = value;
        public object? Value { get; }
        public ResultValueType ResultValueType => ResultValueType.DateTime;
    }
}
