using CrossCutting.Common.Abstractions;

namespace DialogFramework.Domain.Tests;

public class DialogTests
{
    [Fact]
    public void Constructing_Dialog_With_Null_Id_Throws()
    {
        // Act & Assert
        Action a = () => _ = new Dialog(id: default!, string.Empty, new Version(), Enumerable.Empty<DialogPartResult>(), default);
        a.ShouldThrow<ValidationException>();
    }

    [Fact]
    public void Constructing_Dialog_With_Empty_Id_Throws()
    {
        // Arrange
        var builder = new DialogBuilder().WithId(string.Empty);

        // Act & Assert
        Action a = () => builder.Build();
        a.ShouldThrow<ValidationException>();
    }

    [Fact]
    public void Constructing_Dialog_With_Null_Optional_Parameters_Initializes_Correctly()
    {
        // Act
        var sut = new Dialog(TestDialogDefinitionFactory.CreateEmpty(), null, null, null);

        // Assert
        sut.Id.ShouldNotBeEmpty();
    }

    [Fact]
    public void Can_Validate_Builder_Before_Building_Entity()
    {
        // Arrange
        var builder = new DialogBuilder().WithId(string.Empty);
        var validationResults = new List<ValidationResult>();

        // Act
        var success = builder.TryValidate(validationResults);

        // Assert
        success.ShouldBeFalse();
        validationResults.Select(x => x.ErrorMessage).ToArray().ShouldBeEquivalentTo(new[] { "The Id field is required.", "The DefinitionId field is required." });
    }

    [Fact]
    public void Can_Keep_State_Of_Dialog()
    {
        // Act
        var sut = new DialogBuilder()
            .WithId(Guid.NewGuid().ToString())
            .WithDefinitionId("MyDialog")
            .AddResults(new SingleQuestionDialogPartResultBuilder<string>().WithPartId("MyPart").WithValue("Paul Deen"))
            .WithContext(new { PropertyName = "Some Value" })
            .Build();

        // Assert
        sut.ShouldNotBeNull();
        sut.Results.ShouldHaveSingleItem();
        sut.Context.ShouldNotBeNull();
        sut.Context!.GetType().GetProperty("PropertyName").ShouldNotBeNull();
    }

    [Fact]
    public void Can_Change_Nullable_Property_To_Null_Value()
    {
        // Arrange
        var sut = new TestDialog(new DialogDefinitionBuilder().WithId("Id").WithName("MyDialog").Build());

        // Act
        sut.Context = null;
        sut.Context = 1;
        sut.Context = null;

        // Assert
        sut.Context.ShouldBeNull();
    }

    public partial class TestDialog : System.ComponentModel.INotifyPropertyChanged, IBuildableEntity<DialogBuilder>
    {
        private string _id;

        private string _definitionId;

        private Version _definitionVersion;

        private System.Collections.ObjectModel.ObservableCollection<DialogPartResult> _results;

        private object? _context;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        [Required]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                bool hasChanged = !EqualityComparer<System.String>.Default.Equals(_id, value);
                _id = value ?? throw new ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Id));
            }
        }

        [Required]
        public string DefinitionId
        {
            get
            {
                return _definitionId;
            }
            set
            {
                bool hasChanged = !EqualityComparer<System.String>.Default.Equals(_definitionId, value);
                _definitionId = value ?? throw new ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(DefinitionId));
            }
        }

        [Required]
        public Version DefinitionVersion
        {
            get
            {
                return _definitionVersion;
            }
            set
            {
                bool hasChanged = !EqualityComparer<Version>.Default.Equals(_definitionVersion, value);
                _definitionVersion = value ?? throw new ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(DefinitionVersion));
            }
        }

        [Required]
        [CrossCutting.Common.DataAnnotations.ValidateObject]
        public System.Collections.ObjectModel.ObservableCollection<DialogPartResult> Results
        {
            get
            {
                return _results;
            }
            set
            {
                bool hasChanged = !EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogPartResult>>.Default.Equals(_results, value);
                _results = value ?? throw new ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Results));
            }
        }

        public object? Context
        {
            get
            {
                return _context;
            }
            set
            {
                bool hasChanged = !EqualityComparer<System.Object>.Default.Equals(_context, value);
                _context = value;
                if (hasChanged) HandlePropertyChanged(nameof(Context));
            }
        }

        public TestDialog(string id, string definitionId, Version definitionVersion, IEnumerable<DialogPartResult> results, object? context)
        {
            this._id = id;
            this._definitionId = definitionId;
            this._definitionVersion = definitionVersion;
            this._results = results is null ? null! : new System.Collections.ObjectModel.ObservableCollection<DialogPartResult>(results);
            this._context = context;
            Validator.ValidateObject(this, new ValidationContext(this, null, null), true);
        }

        public TestDialog(
            DialogDefinition definition,
            IEnumerable<DialogPartResult>? results = null,
            object? context = null,
            string? id = null)
            : this(
          id ?? Guid.NewGuid().ToString(),
          definition?.Id!,
          definition?.Version!,
          results ?? Enumerable.Empty<DialogPartResult>(),
          context)
        {
        }

        public DialogBuilder ToBuilder()
        {
            throw new NotImplementedException();
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
