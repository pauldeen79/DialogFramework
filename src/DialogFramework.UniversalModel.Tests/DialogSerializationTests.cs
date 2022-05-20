using DialogFramework.UniversalModel.DomainModel;
using DialogFramework.UniversalModel.DomainModel.Builders;
using DialogFramework.UniversalModel.DomainModel.DialogParts.Builders;
using FluentAssertions;
using Newtonsoft.Json;

namespace DialogFramework.UniversalModel.Tests
{
    public class DialogSerializationTests
    {
        [Fact(Skip = "Need to think of something else, as Newtonsoft doesn't like abstract or interface types in c'tor")]
        public void Can_Serialize_And_Deserialize_Dialog()
        {
            // Arrange
            var completedGroup = new DialogPartGroupBuilder().WithId("CompletedGroup").WithNumber(2).WithTitle("Completed");
            var dialogToSerialize = new DialogBuilder()
                .WithMetadata(new DialogMetadataBuilder().WithFriendlyName("Test").WithId("Test").WithVersion("1.0.0"))
                .WithAbortedPart(new AbortedDialogPartBuilder().WithMessage("Aborted"))
                .WithCompletedPart(new CompletedDialogPartBuilder().WithMessage("Thank you!").WithGroup(completedGroup))
                .WithErrorPart(new ErrorDialogPartBuilder().WithErrorMessage("Something went wrong"))
                .AddPartGroups(completedGroup)
                .Build();

            // Serialize
            var json = JsonConvert.SerializeObject(dialogToSerialize);

            // Deserialize
            var deserializedDialog = JsonConvert.DeserializeObject<Dialog>(json);

            // Assert
            deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
        }
    }
}
