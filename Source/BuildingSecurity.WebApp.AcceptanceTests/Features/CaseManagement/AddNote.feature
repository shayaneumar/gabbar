@track_cases
Feature: Add Note
In order to help others better understand information in a case
As a user with access to case manager
I need to add a note to the case

Scenario: User adds a note to an existing case
	Given I am logged in with view case privileges
	And I am viewing an open case
	When I add a note
	Then the note is added
	And I see an empty note input area

Scenario: User starts adding note, and cancels
	Given I am logged in with view case privileges
	And  I am viewing an open case
	And I have entered a note, but not added it
	When I cancel adding a note
	Then no note should be added
	And I see an empty note input area
	
Scenario: User enters multi-line text, and it is added as multi-line text
	Given I am logged in with view case privileges
	And I am viewing an open case
	When I add a note with multiple lines
	Then the note is added

Scenario: User adds a note with whitespace on either end
	Given I am logged in with view case privileges
	And I am viewing an open case
	When I add a note with extra white space
	Then a trimmed version of the note is added
