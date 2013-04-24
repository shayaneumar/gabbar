@track_cases
Feature: Case Management Dashboard
	In order to quickly see cases I am interested in
	As a user with access to case manager
	I want to have dashboard for cases

Scenario: Cardkey navigates to the case management from the site navigation menu
	Given I am logged in as cardkey
	And I am on the alarm manager page
	When I click the Cases link in the site navigation menu
	Then I should arrive on the case management page

Scenario: User without permissions to the case management can not access it via the site navigation menu
	Given I am logged in as nocasemanview
	Then I should not see cases in the site navigation menu

Scenario: User without permissions to case management can not navigate directly to the case management dashboard
	Given I am logged in as nocasemanview
	When I navigate to the case management page
	Then I receive a permission denied error