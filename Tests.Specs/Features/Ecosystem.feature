Feature: Ecosystem Management
  Scenarios for creating and managing ecosystems via API

  Scenario: TS1 - Successfully create a new ecosystem
    When I create ecosystem name as 'API demo'
    Then Save response 'EcosystemId' s value as 'Ecosystem_ID'
  
  Scenario: TS2 - Successfully create a new company
    When I create a company with domain 'github.com' using the ecosystem ID saved as 'Ecosystem_ID'
    Then Save response 'CompanyId' s value as 'Company_ID'

  Scenario: TS3 - Successfully get the details for the company
    Then Get the details for the company saved as 'Company_ID'
    Then the response body key 'ScanStatus' should have one of the following values:
      | Value                         |
      | Extended Rescan Running       |
      | Extended Rescan Queued        |
      | Extended Rescan Results Ready |

  Scenario: TS4 - Successfully get the details for the ecosystem
    Then Get the details for the ecosystem saved as 'Ecosystem_ID'
    Then the response body key 'EcosystemName' should have value 'API demo'

  Scenario: TS5 - Successfully get notifications for the company
    Then Get notifications for the company saved as 'Company_ID'
    Then the first element in the response array should have 'CompanyId' equal to the value saved as 'Company_ID'
    Then the first element in the response array should have 'Company' equal to 'GitHub'

  Scenario: TS6 - Successfully get email security findings for the company and a specific finding
    Then Get email security findings for the company saved as 'Company_ID'
    Then save random 'FindingId' from root array as 'Finding_ID'
    Then Get the specific email security finding saved as 'Finding_ID' for the company saved as 'Company_ID'

  Scenario: TS7 - Successfully update the status of the finding and verify logs
    When Update the status to 'Suppressed' for the finding saved as 'Finding_ID' for company saved as 'Company_ID'
    Then Get logs for the company saved as 'Company_ID' for date range 'LastWeek'
    Then the first element in the response array should have 'LogType' equal to 'Finding Status Changed'
    Then the first element in the response array should have 'InsertUser' equal to 'API'

  Scenario: TS8 - Successfully delete the company and verify deletion
    When Delete the company saved as 'Company_ID'
    Then Get the details for the company saved as 'Company_ID'
    Then the response body key 'Message' should have value 'Company not found.'

  Scenario: TS9 -  Successfully delete the ecosystem and verify deletion
    When Delete the ecosystem saved as 'Ecosystem_ID'
    Then Get the details for the ecosystem saved as 'Ecosystem_ID'
    Then the response body key 'Message' should have value 'Ecosystem not found.'