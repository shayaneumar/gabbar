Feature: Change priority ranges
	In order to tune my alarm monitor to my needs
	As a security manager
	I want to be able to change the priority ranges used for color and sound

Scenario: Change alarm display options
	Given I am logged in as cardkey
	And I am on the alarm display options page
	And I have reset to default
	And I enter configure alarms like this:
	| UpperLimit | Color  | Sound |
	| 1          | Red    | Siren |
	| 3          | Orange | Siren |
	| 100        | Clear  | Siren |
	| 254        | Clear  | Siren |
	When I press the save button
	Then a success message should appear
