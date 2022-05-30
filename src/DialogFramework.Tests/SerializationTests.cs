using DialogFramework.Domain.Builders;

namespace DialogFramework.Tests;

public class SerializationTests
{
    [Fact]
    public void Can_Serialize_And_Deserialize_SimpleFormFlowDialog()
    {
        // Arrange
        var dialogToSerialize = SimpleFormFlowDialog.Create();

        // Serialize
        var json = JsonSerializerFixture.Serialize(new DialogBuilder(dialogToSerialize));

        // Deserialize
        var deserializedDialog = JsonSerializerFixture.Deserialize<DialogBuilder>(json)!.Build();

        // Assert
        deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
    }

    [Fact]
    public void Can_Serialize_And_Deserialize_TestFlowDialog()
    {
        // Arrange
        var dialogToSerialize = TestFlowDialog.Create();

        // Serialize
        var json = JsonSerializerFixture.Serialize(new DialogBuilder(dialogToSerialize));

        // Deserialize
        var deserializedDialog = JsonSerializerFixture.Deserialize<DialogBuilder>(json)!.Build();

        // Assert
        deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
    }

    [Fact]
    public void Can_Serialize_And_Deserialize_DialogContext()
    {
        // Arrange
        var dialogContextToSerialize = new DialogContextFactory().Create(SimpleFormFlowDialog.Create());

        // Serialize
        var json = JsonSerializerFixture.Serialize(new DialogContextBuilder(dialogContextToSerialize));

        // Deserialize
        var deserializedDialog = JsonSerializerFixture.Deserialize<DialogContextBuilder>(json)!.Build();

        // Assert
        deserializedDialog.Should().BeEquivalentTo(dialogContextToSerialize);
    }
}
