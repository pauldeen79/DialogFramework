namespace DialogFramework.Tests;

public class SerializationTests
{
    [Fact]
    public void Can_Serialize_And_Deserialize_SimpleFormFlowDialog()
    {
        // Arrange
        var dialogToSerialize = SimpleFormFlowDialog.Create();

        // Serialize
        var json = JsonSerializerFixture.Serialize(dialogToSerialize);

        // Deserialize
        var deserializedDialog = JsonSerializerFixture.Deserialize<Dialog>(json);

        // Assert
        deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
    }

    [Fact]
    public void Can_Serialize_And_Deserialize_TestFlowDialog()
    {
        // Arrange
        var dialogToSerialize = TestFlowDialog.Create();

        // Serialize
        var json = JsonSerializerFixture.Serialize(dialogToSerialize);

        // Deserialize
        var deserializedDialog = JsonSerializerFixture.Deserialize<Dialog>(json);

        // Assert
        deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
    }

    [Fact]
    public void Can_Serialize_And_Deserialize_DialogContext()
    {
        // Arrange
        var dialogContextToSerialize = new DialogContextFactory().Create(SimpleFormFlowDialog.Create());

        // Serialize
        var json = JsonSerializerFixture.Serialize(dialogContextToSerialize);

        // Deserialize
        var deserializedDialog = JsonSerializerFixture.Deserialize<DialogContext>(json);

        // Assert
        deserializedDialog.Should().BeEquivalentTo(dialogContextToSerialize);
    }
}
