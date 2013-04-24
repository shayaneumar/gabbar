@track_cases
Feature: Create Case
    In order to track information related to a real world incident
    As a user with access to case manager
    I want to be able to create/open a case

Scenario: Cardkey creates a case about a doughnut theft at 507
    Given I am logged in as cardkey
    And I am on the create case page
    And I have entered a valid case title
    When I press the create button
    Then a case is created with the title I entered

Scenario: Cardkey clears the case title field and is prevented from creting a case
    Given I am logged in as cardkey
    And I am on the create case page
    When I clear the title field
    Then the create button is disabled

Scenario: Cardkey tries to create a case, but is prevented when the title is empty
    Given I am logged in as cardkey
    And I am on the create case page
	And the title field is empty
    Then the create button is disabled

Scenario: Cardkey tries to create a case, but enter a long title
    Given I am logged in as cardkey
    And I am on the create case page
    And I have entered This is a really really really really really really really really really really really really really really really reallyreally really really reallyreally really really reallyreally really really reallyreally really really really really really really really long case title in the title field
    When I press the create button
    Then an error message should appear

Scenario: Cardkey tries to create a case, with trailing spaces
    Given I am logged in as cardkey
    And I am on the create case page
    And I have entered a case title with extra whitespace
    When I press the create button
    Then a case is created with the title trimmed
