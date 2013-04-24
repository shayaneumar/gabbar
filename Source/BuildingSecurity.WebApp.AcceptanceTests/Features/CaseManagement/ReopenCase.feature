@track_cases
Feature: Reopen Case
	In order to add originally unknown or missed data
	As a user with access to case manager
	I want to reopen a previously closed case

Scenario: Reopening a case should change its status to open
	Given I am logged in with view case privileges
	And a closed case exists
	And I navigate to a closed case
	When a closed case is reopened by the user
	Then the case status will be open
