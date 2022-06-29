namespace DialogFramework.Tests;

public class SerializationTests
{
    [Fact]
    public void Can_Serialize_And_Deserialize_SimpleFormFlowDialog()
    {
        // Arrange
        var dialogDefinitionToSerialize = SimpleFormFlowDialog.Create();

        // Serialize
        var json = JsonSerializerFixture.Serialize(new DialogDefinitionBuilder(dialogDefinitionToSerialize));

        // Deserialize
        var deserializedDialogDefinition = JsonSerializerFixture.Deserialize<DialogDefinitionBuilder>(json)!.Build();

        // Assert
        deserializedDialogDefinition.Should().BeEquivalentTo(dialogDefinitionToSerialize);
    }

    [Fact]
    public void Can_Serialize_And_Deserialize_TestFlowDialog()
    {
        // Arrange
        var dialogDefinitionToSerialize = TestFlowDialog.Create();

        // Serialize
        var json = JsonSerializerFixture.Serialize(new DialogDefinitionBuilder(dialogDefinitionToSerialize));

        // Deserialize
        var deserializedDialogDefinition = JsonSerializerFixture.Deserialize<DialogDefinitionBuilder>(json)!.Build();

        // Assert
        deserializedDialogDefinition.Should().BeEquivalentTo(dialogDefinitionToSerialize);
    }

    [Fact]
    public void Can_Serialize_And_Deserialize_Dialog()
    {
        // Arrange
        var definition = SimpleFormFlowDialog.Create();
        var dialogToSerialize = new DialogFactory().Create(definition, Enumerable.Empty<IDialogPartResult>());

        // Serialize
        var json = JsonSerializerFixture.Serialize(new DialogBuilder(dialogToSerialize, definition!));

        // Deserialize
        var deserializedDialog = JsonSerializerFixture.Deserialize<DialogBuilder>(json)!.Build();

        // Assert
        deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
    }
}
