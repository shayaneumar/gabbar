Feature: Signin
	In order to use the web ui
	As a person with a browser
	I want to be able to sigin in iff I am an authorized user

@asNewUser
Scenario: Cardkey can sign in
	Given I am on the sign in page
	And I have entered cardkey in the username field
	And I have entered master in the password field
	When I press the Sign in button
	Then I should be signed in as cardkey

@asNewUser
Scenario: Lockout can not sign in
	Given I am on the sign in page
	And I have entered lockout in the username field
	And I have entered lockout in the password field
	When I press the Sign in button
	Then I see "account disabled"
