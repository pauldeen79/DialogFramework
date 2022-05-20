using Newtonsoft.Json;

namespace DialogFramework.Core.Tests;

public class DialogSerializationTests
{
    [Fact(Skip = "Need to think of something else, as Newtonsoft doesn't like abstract or interface types in c'tor")]
    public void Can_Serialize_And_Deserialize_Dialog()
    {
        // Arrange
        var dialogToSerialize = new TestFlowDialog() as Dialog;

        // Serialize
        var json = JsonConvert.SerializeObject(dialogToSerialize);

        // Deserialize
        var deserializedDialog = JsonConvert.DeserializeObject<Dialog>(json);

        // Assert
        deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
    }
}
