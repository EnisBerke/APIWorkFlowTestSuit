The API Work Flow Test Suite is a test automation project built to validate API endpoints and workflows. It uses C#, RestSharp, and SpecFlow with Cucumber-style syntax to define behavior-driven test scenarios that ensure APIs behave correctly across different usage patterns.

Technologies & Frameworks Used

	•	C# – Main programming language for implementing test logic
	•	RestSharp – Used to send HTTP requests and handle responses
	•	SpecFlow – BDD framework for .NET that enables writing tests in Gherkin syntax
	•	Cucumber – Style of writing test cases in natural language (via Gherkin) for better readability and collaboration

Project Structure

	•	Tests.Specs/
Contains Gherkin (.feature) files where each file defines a specific feature and its test scenarios.

	•	Steps Definitions
Contains step definition classes in C# that map to Gherkin steps and use RestSharp for API interactions.

Make sure the following packages are installed:

	•	RestSharp
	•	SpecFlow
	•	SpecFlow.Tools.MsBuild.Generation
	•	NUnit or any preferred test runner
