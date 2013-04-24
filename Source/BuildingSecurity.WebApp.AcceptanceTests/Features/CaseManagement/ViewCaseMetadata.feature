@track_cases
Feature: View case metadata
	In order to better identify a case
	As a user with access to case manager
	I want to see important metadata like case id, title, and creator information

Scenario: A user can view a case and see all the data associated with it
	Given I am logged in with view case privileges
	When I navigate to a case
	Then I see the case's title
	And I see the case's creator
	And I see the case's creation date and time
	And I see the case's owner

Scenario: A user that tries to view a case that does not exist (or was deleted) will get an error
	Given I am logged in with view case privileges
	When I navigate to a nonexistent case
	Then I see an error message

Scenario: A user without view case permissions that tries to view a case will get an error
	Given I am logged in without view case privileges
	When I navigate to a case
	Then I receive a permission denied error
