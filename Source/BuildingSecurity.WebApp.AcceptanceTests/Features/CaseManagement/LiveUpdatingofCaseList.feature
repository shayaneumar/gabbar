@track_cases
Feature: Automatic updating of case list
    In order to save time and increase my confidence in the system
    As a user with access to case manager
    I see my case list update automatically

Scenario: When a new case is created it shows in the list automatically
    Given I am logged in with view case privileges
    And I am on the case management page
	And I am viewing all cases
	When a case is created
	Then I see the new case in the case list

Scenario: When a case title changes it updates in the list automatically
    Given I am logged in with view case privileges
	And I am on the case management page
	And I am viewing all cases
	When a case title is changed
    Then I see the changed case title in the case list