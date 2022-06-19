Feature: Dialog
Question and response dialogue between two actors

Scenario: Complete simple form flow from beginning to end
	Given I start the 'SimpleFormFlowDialog' dialog
	When I answer the following questions for the 'ContactInfo' dialog part
        | Result          | ResultValueType | Value             |
        | EmailAddress    | Text            | email@address.com |
        | TelephoneNumber | Text            | 911               |
	When I answer the following questions for the 'Newsletter' dialog part
        | Result              | ResultValueType | Value |
        | SignUpForNewsletter | YesNo           | false |
	Then the current state should be Completed