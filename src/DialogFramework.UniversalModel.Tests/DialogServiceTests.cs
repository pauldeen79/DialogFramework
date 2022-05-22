using DialogFramework.Abstractions.DomainModel.DialogParts;
using DialogFramework.Abstractions.DomainModel.Domains;
using DialogFramework.Core.DomainModel.DialogPartResultValues;
using DialogFramework.Core.Extensions;
using DialogFramework.UniversalModel.DomainModel;
using DialogFramework.UniversalModel.Tests.Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using DialogService = DialogFramework.Core.DialogService;

namespace DialogFramework.UniversalModel.Tests
{
    public class DialogServiceTests
    {
        [Fact]
        public void Can_Complete_SimpleFormFlow_Dialog_In_One_Step()
        {
            // Arrange
            var dialog = SimpleFormFlowDialog.Create();
            var factory = new DialogContextFactory();
            var repository = new TestDialogRepository();
            var sut = new DialogService(factory, repository);

            // Act
            var context = sut.Start(dialog.Metadata);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "EmailAddress",
                    new TextDialogPartResultValue("email@address.com")
                ),
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "TelephoneNumber",
                    new TextDialogPartResultValue("911")
                )
            ); // ContactInfo -> Newsletter
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "SignUpForNewsletter",
                    new YesNoDialogPartResultValue(false)
                )
            ); // Newsletter -> Completed

            // Assert
            context.CurrentState.Should().Be(DialogState.Completed);
            context.CurrentPart.Id.Should().Be("Completed");
            context.GetDialogPartResultsByPart(dialog.Parts.Single(x => x.Id == "ContactInfo")).Should().BeEquivalentTo(new[]
            {
                new DialogPartResult("ContactInfo", "EmailAddress", new TextDialogPartResultValue("email@address.com")),
                new DialogPartResult("ContactInfo", "TelephoneNumber", new TextDialogPartResultValue("911"))
            });
            context.GetDialogPartResultsByPart(dialog.Parts.Single(x => x.Id == "Newsletter")).Should().BeEquivalentTo(new[]
            {
                new DialogPartResult("Newsletter", "SignUpForNewsletter", new YesNoDialogPartResultValue(false))
            });
        }

        [Fact]
        public void Can_Complete_SimpleFormFlow_Dialog_With_NavigateBack()
        {
            // Arrange
            var dialog = SimpleFormFlowDialog.Create();
            var factory = new DialogContextFactory();
            var repository = new TestDialogRepository();
            var sut = new DialogService(factory, repository);

            // Act
            var context = sut.Start(dialog.Metadata);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "EmailAddress",
                    new TextDialogPartResultValue("wrong@address.com")
                ),
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "TelephoneNumber",
                    new TextDialogPartResultValue("911")
                )
            ); // ContactInfo -> Newsletter
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "SignUpForNewsletter",
                    new YesNoDialogPartResultValue(true)
                )
            ); // Newsletter -> Completed
            context = sut.NavigateTo(context, dialog.Parts.Single(x => x.Id == "ContactInfo")); // navigate back: Completed -> ContactInfo
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "EmailAddress",
                    new TextDialogPartResultValue("email@address.com")
                ),
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "TelephoneNumber",
                    new TextDialogPartResultValue("911")
                )
            ); // ContactInfo -> Newsletter
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "SignUpForNewsletter",
                    new YesNoDialogPartResultValue(false)
                )
            ); // Newsletter -> Completed

            // Assert
            context.CurrentState.Should().Be(DialogState.Completed);
            context.CurrentPart.Id.Should().Be("Completed");
            context.GetDialogPartResultsByPart(dialog.Parts.Single(x => x.Id == "ContactInfo")).Should().BeEquivalentTo(new[]
            {
                new DialogPartResult("ContactInfo", "EmailAddress", new TextDialogPartResultValue("email@address.com")),
                new DialogPartResult("ContactInfo", "TelephoneNumber", new TextDialogPartResultValue("911"))
            });
            context.GetDialogPartResultsByPart(dialog.Parts.Single(x => x.Id == "Newsletter")).Should().BeEquivalentTo(new[]
            {
                new DialogPartResult("Newsletter", "SignUpForNewsletter", new YesNoDialogPartResultValue(false))
            });
        }

        [Fact]
        public void Can_Complete_SimpleFormFlow_In_Different_Session()
        {
            // Arrange
            var dialog = SimpleFormFlowDialog.Create();
            var factory = new DialogContextFactory();
            var repository = new TestDialogRepository();
            var sut = new DialogService(factory, repository);

            // Act step 1: Start a session, submit first question
            var context = sut.Start(dialog.Metadata);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "EmailAddress",
                    new TextDialogPartResultValue("email@address.com")
                ),
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "TelephoneNumber",
                    new TextDialogPartResultValue("911")
                )
            ); // ContactInfo -> Newsletter

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            // Serialize
            var json = JsonConvert.SerializeObject(context, settings);

            // Act step 2: Re-create the context in a new session (simulating that the context is saved to a store, and reconstructed again)
            var context2 = JsonConvert.DeserializeObject<DialogContext>(json, settings);
            var result = sut.Continue
            (
                context2,
                new DialogPartResult
                (
                    context2.CurrentPart.Id,
                    "SignUpForNewsletter",
                    new YesNoDialogPartResultValue(false)
                )
            ); // Newsletter -> Completed

            // Assert
            result.CurrentState.Should().Be(DialogState.Completed);
            result.CurrentPart.Id.Should().Be("Completed");
            result.GetDialogPartResultsByPart(dialog.Parts.Single(x => x.Id == "ContactInfo")).Should().BeEquivalentTo(new[]
            {
                new DialogPartResult("ContactInfo", "EmailAddress", new TextDialogPartResultValue("email@address.com")),
                new DialogPartResult("ContactInfo", "TelephoneNumber", new TextDialogPartResultValue("911"))
            });
            result.GetDialogPartResultsByPart(dialog.Parts.Single(x => x.Id == "Newsletter")).Should().BeEquivalentTo(new[]
            {
                new DialogPartResult("Newsletter", "SignUpForNewsletter", new YesNoDialogPartResultValue(false))
            });
        }

        [Fact]
        public void Providing_Wrong_ValueTypes_Leads_To_ValidationErrors()
        {
            // Arrange
            var dialog = SimpleFormFlowDialog.Create();
            var factory = new DialogContextFactory();
            var repository = new TestDialogRepository();
            var sut = new DialogService(factory, repository);

            // Act
            var context = sut.Start(dialog.Metadata);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "EmailAddress",
                    new NumberDialogPartResultValue(911)
                ),
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "TelephoneNumber",
                    new YesNoDialogPartResultValue(true)
                )
            ); // Current part remains ContactInfo because of validation errors

            // Assert
            context.CurrentState.Should().Be(DialogState.InProgress);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
            var questionDialogPart = (IQuestionDialogPart)context.CurrentPart;
            questionDialogPart.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
            {
                "Result for [ContactInfo.EmailAddress] should be of type [Text], but type [Number] was answered",
                "Result value of [ContactInfo.EmailAddress] is not of type [System.String]",
                "Result for [ContactInfo.TelephoneNumber] should be of type [Text], but type [YesNo] was answered",
                "Result value of [ContactInfo.TelephoneNumber] is not of type [System.String]"
            });
        }

        [Fact]
        public void Providing_Results_With_Empty_Values_On_Required_Values_Leads_To_ValidationErrors()
        {
            // Arrange
            var dialog = SimpleFormFlowDialog.Create();
            var factory = new DialogContextFactory();
            var repository = new TestDialogRepository();
            var sut = new DialogService(factory, repository);

            // Act
            var context = sut.Start(dialog.Metadata);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "EmailAddress",
                    new TextDialogPartResultValue(string.Empty)
                ),
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "TelephoneNumber",
                    new TextDialogPartResultValue(null)
                )
            ); // Current part remains ContactInfo because of validation errors

            // Assert
            context.CurrentState.Should().Be(DialogState.InProgress);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
            var questionDialogPart = (IQuestionDialogPart)context.CurrentPart;
            questionDialogPart.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
            {
                "Result value of [ContactInfo.EmailAddress] is required",
                "Result value of [ContactInfo.TelephoneNumber] is required"
            });
        }

        [Fact]
        public void Providing_Results_With_No_Values_On_Required_Values_Leads_To_ValidationErrors()
        {
            // Arrange
            var dialog = SimpleFormFlowDialog.Create();
            var factory = new DialogContextFactory();
            var repository = new TestDialogRepository();
            var sut = new DialogService(factory, repository);

            // Act
            var context = sut.Start(dialog.Metadata);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context = sut.Continue(context); // Current part remains ContactInfo because of validation errors

            // Assert
            context.CurrentState.Should().Be(DialogState.InProgress);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
            var questionDialogPart = (IQuestionDialogPart)context.CurrentPart;
            questionDialogPart.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
            {
                "Result value of [ContactInfo.EmailAddress] is required",
                "Result value of [ContactInfo.TelephoneNumber] is required"
            });
        }

        [Fact]
        public void Providing_Results_With_Wrong_ValueType_Leads_To_ValidationErrors()
        {
            // Arrange
            var dialog = SimpleFormFlowDialog.Create();
            var factory = new DialogContextFactory();
            var repository = new TestDialogRepository();
            var sut = new DialogService(factory, repository);

            // Act
            var context = sut.Start(dialog.Metadata);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context = sut.Continue
            (
                context,
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "EmailAddress",
                    new NumberDialogPartResultValue(1)
                ),
                new DialogPartResult
                (
                    context.CurrentPart.Id,
                    "TelephoneNumber",
                    new TextDialogPartResultValue("911")
                )
            ); // Current part remains ContactInfo because of validation errors

            // Assert
            context.CurrentState.Should().Be(DialogState.InProgress);
            context.CurrentPart.Id.Should().Be("ContactInfo");
            context.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
            var questionDialogPart = (IQuestionDialogPart)context.CurrentPart;
            questionDialogPart.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
            {
                "Result for [ContactInfo.EmailAddress] should be of type [Text], but type [Number] was answered",
                "Result value of [ContactInfo.EmailAddress] is not of type [System.String]"
            });
        }
    }
}
