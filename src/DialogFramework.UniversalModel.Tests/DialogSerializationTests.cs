using DialogFramework.UniversalModel.DomainModel;
using DialogFramework.UniversalModel.Tests.Fixtures;
using FluentAssertions;
using Newtonsoft.Json;

namespace DialogFramework.UniversalModel.Tests
{
    public class DialogSerializationTests
    {
        [Fact]
        public void Can_Serialize_And_Deserialize_SimpleFormFlowDialog()
        {
            // Arrange
            var dialogToSerialize = SimpleFormFlowDialog.Create();
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            // Serialize
            var json = JsonConvert.SerializeObject(dialogToSerialize, settings);

            // Deserialize
            var deserializedDialog = JsonConvert.DeserializeObject<Dialog>(json, settings);

            // Assert
            deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
        }
    }
}
