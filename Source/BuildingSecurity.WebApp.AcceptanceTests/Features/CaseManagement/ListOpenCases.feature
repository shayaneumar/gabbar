@track_cases
Feature: ListOpenCases
	In order to find open cases faster
	As a user with access to case manager
	I want see all open cases

Scenario: At least one open case exists and it appears in the open cases list
	Given I am logged in with view case privileges
	And a closed case exists
	And an open case exists
	And I am on the case management page
	When I view open cases
	Then I see all open cases in the cases list
	And I see no closed cases in the cases list