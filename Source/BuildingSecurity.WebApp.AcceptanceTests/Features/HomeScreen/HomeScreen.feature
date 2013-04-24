Feature: Home Screen
	In order to make my life easy
	As a user
	I want to be taken to the best page for me after signing in

@asNewUser
Scenario: Cardkey is sent to alarm manager on sign in
	Given I am on the sign in page
	And I have entered cardkey in the username field
	And I have entered master in the password field
	When I press the Sign in button
	Then I should arrive on the alarm manager page

@asNewUser
Scenario: Report is sent to reports page on sign in
	Given I am on the sign in page
	And I have entered report in the username field
	And I have entered report in the password field
	When I press the Sign in button
	Then I should arrive on the reports page

@asNewUser
Scenario: Setup is sent to unauthorized page on sign in
	Given I am on the sign in page
	And I have entered setup in the username field
	And I have entered setup in the password field
	When I press the Sign in button
	Then I should arrive on the unauthorized page
