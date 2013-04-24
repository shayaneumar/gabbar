@track_cases
Feature: List all cases
	In order to discover all cases in the system
	As a user with access to case manager
	I want a list of all cases on the case management homepage

Scenario: Cardkey navigates to case management page and sees a list of cases
	Given I am logged in as cardkey
	And a case exists with the title "Bob stole my car"
	And I am on the case management page
	When I view all cases
    Then I see "Bob stole my car" in the all cases list

Scenario: Cardkey navigates to case management page and there are no cases
	Given I am logged in as cardkey
	And 0 cases exist
	And I am on the case management page
	When I view all cases
	Then I see zero cases in the all cases list

Scenario: Cardkey navigates to case management page and sees a lot of cases
	Given I am logged in as cardkey 
	And 100 cases exist
	And I am on the case management page
	When I view all cases
	Then I see 100 cases in the all cases list
