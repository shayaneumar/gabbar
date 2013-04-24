@track_cases
Feature: Live updating of Case
	In order to save time and increase my confidence in the system
	As a user with access to case manager
	I my screen to update automatically

Scenario: I change a case's title and my screen updates
	Given I am logged in with edit case privileges
	And I am viewing a case
	When I change the case title
	Then I see the case title changes to what I entered

Scenario: I add a note to a case and my screen updates
Given I am logged in with edit case privileges
	And I am viewing a case
	When I add a case note
	Then I see the case note I added

Scenario: The case I am viewing is updated by someone else and my screen updates
	Given I am logged in with view case privileges
	And I am viewing a case
	When another user changes the case title
	Then I see the case title changes to what they entered

Scenario: Someone adds a note to the case I am viewing and my screen updates
	Given I am logged in with view case privileges
	And I am viewing a case
	When another user adds a case note to the case
	Then I see the case note they added

Scenario: A closed case I am viewing is reopened
	Given I am logged in with edit case privileges
	And a closed case exists
	And I am viewing a closed case
	When the case is reopened
	Then I see the open case display
	And the case can be edited
