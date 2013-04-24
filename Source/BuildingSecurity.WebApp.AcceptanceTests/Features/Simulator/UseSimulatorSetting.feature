Feature: Use Simulator Setting
	In order to quickly test the web ui independant of the p2000
	As a tester
	I want to be able to turn on the simulator after I've deployed the webui

Scenario: Activating simulator via webconfig
	Given I have access to the webserver
	When I set the application setting UseSimulation to true
	Then The webui should be using the simulator

Scenario: Deactivating simulator via webconfig
	Given I have access to the webserver
	When I set the application setting UseSimulation to false
	Then The webui should be using the p2000