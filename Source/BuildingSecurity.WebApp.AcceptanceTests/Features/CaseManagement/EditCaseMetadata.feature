@track_cases
Feature: Edit Case Fields
In order to fix inaccuracies in a case’s metadata
As a user with access to case manager
I need to be able to edit a case’s metadata

Scenario: User updates case's title successfully
	Given I am logged in with edit case privileges
	And I am viewing an open case
	When I change the case's title
	Then the case's title changes to what I entered

Scenario: User tries to updates case's title but enters a title that is too long
	Given I am logged in with edit case privileges
	And I am viewing an open case
	When I change the case's title to one that is too long
	Then I see an error message
	And the title does not change

Scenario: User updates case's title successfully even though title had extra whitespace
	Given I am logged in with edit case privileges
	And I am viewing an open case
	When I change the case's title
	Then the case's title changes to a trimmed version of what I entered
