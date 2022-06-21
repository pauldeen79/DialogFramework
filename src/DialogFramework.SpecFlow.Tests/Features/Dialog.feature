Feature: Dialog
Question and response dialogue between two actors

Scenario: Complete simple form flow from beginning to end
	Given I start the 'SimpleFormFlowDialog' dialog
    # Note that currently, you have to answer all results of the same question in one operation.
    # As a result, you have to use a table to specify 2 results on the same question
	When I answer the following results for the current dialog part
        | Result          | Value             |
        | EmailAddress    | email@address.com |
        | TelephoneNumber | 911               |
    # When you only have one answer, then you can use a one-liner.
    # In the step definitions, we have overloads for boolean/numeric/text value types.
    # This could probably be improved by using StepArgumentTransformation...
    And I answer No for result 'SignUpForNewsletter'
	Then the current state should be Completed