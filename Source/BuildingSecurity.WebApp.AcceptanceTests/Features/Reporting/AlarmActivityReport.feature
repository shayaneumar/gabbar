Feature: Alarm Activity Report
	In order to find out something
	As a security manager
	I want to be able to run an alarm activity report

@asCardkey
Scenario: Default parameters
	Given reports server is configured
	And I am on the alarm activity report page
	When I press the run button
	Then a pdf should be downloaded
