Feature: Dialog
Question and response dialogue between two actors

Scenario: Complete simple form flow from beginning to end
	Given I start the 'SimpleFormFlowDialog' dialog
	When I answer the following results for the current dialog part
        | Result          | Value             |
        | EmailAddress    | email@address.com |
        | TelephoneNumber | 911               |
    #When I answer 'email@address.com' for result 'EmailAddress'
    #And I answer '911' for result 'TelephoneNumber'
    #And I answer No for the current result
    And I answer No for result 'SignUpForNewsletter'
	Then the current state should be Completed