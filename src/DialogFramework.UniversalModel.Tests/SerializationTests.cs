using DialogFramework.UniversalModel.DomainModel;
using DialogFramework.UniversalModel.Tests.Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DialogFramework.UniversalModel.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void Can_Serialize_And_Deserialize_SimpleFormFlowDialog()
        {
            // Arrange
            var dialogToSerialize = SimpleFormFlowDialog.Create();
            var settings = CreateJsonSerializerSetetings();

            // Serialize
            var json = JsonConvert.SerializeObject(dialogToSerialize, settings);

            // Deserialize
            var deserializedDialog = JsonConvert.DeserializeObject<Dialog>(json, settings);

            // Assert
            deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
        }

        [Fact]
        public void Can_Serialize_And_Deserialize_TestFlowDialog()
        {
            // Arrange
            var dialogToSerialize = TestFlowDialog.Create();
            var settings = CreateJsonSerializerSetetings();

            // Serialize
            var json = JsonConvert.SerializeObject(dialogToSerialize, settings);

            // Deserialize
            var deserializedDialog = JsonConvert.DeserializeObject<Dialog>(json, settings);

            // Assert
            deserializedDialog.Should().BeEquivalentTo(dialogToSerialize);
        }

        [Fact]
        public void Can_Serialize_And_Deserialize_DialogContext()
        {
            // Arrange
            var dialogContextToSerialize = new DialogContextFactory().Create(SimpleFormFlowDialog.Create());
            var settings = CreateJsonSerializerSetetings();

            // Serialize
            var json = JsonConvert.SerializeObject(dialogContextToSerialize, settings);

            // Deserialize
            var deserializedDialog = JsonConvert.DeserializeObject<DialogContext>(json, settings);

            // Assert
            deserializedDialog.Should().BeEquivalentTo(dialogContextToSerialize);
        }

        private static JsonSerializerSettings CreateJsonSerializerSetetings()
            => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new[] { new StringEnumConverter() }
            };
    }
}
