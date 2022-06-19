Feature: Dialog
Question and response dialogue between two actors

Scenario: Complete simple form flow from beginning to end
	Given I start the 'SimpleFormFlowDialog' dialog
	When I answer the following questions for the current dialog part
        | Result          | ResultValueType | Value             |
        | EmailAddress    | Text            | email@address.com |
        | TelephoneNumber | Text            | 911               |
    #When I answer 'email@address.com' for result 'EmailAddress'
    #And I answer '911' for result 'TelephoneNumber'
    And I answer No for result 'SignUpForNewsletter'
	Then the current state should be Completed