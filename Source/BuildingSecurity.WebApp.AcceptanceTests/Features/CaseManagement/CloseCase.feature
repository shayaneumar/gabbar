@track_cases
Feature: Close case
	In order to prevent the case from being further modified
	As a user of case management
	I want to be able to close cases

Scenario: Closing a case causes the case status to go to closed.
	Given I am logged in with view case privileges
	And I am viewing an open case
	When I close the case
	Then the case should be closed

Scenario: When viewing a closed case I should not be able to edit the case
	Given I am logged in with view case privileges
	And I am viewing a closed case
	When I close the case
	Then I should see that the case is closed
	And I should not see a way to add a note
	And I should not be able to edit the title
