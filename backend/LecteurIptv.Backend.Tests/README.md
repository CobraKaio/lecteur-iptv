# LecteurIptv.Backend.Tests

This project contains automated tests for the LecteurIptv.Backend application.

## Overview

The test project follows the same structure as the main project, with tests organized by component type:

```
LecteurIptv.Backend.Tests/
├── Services/              # Tests for service classes
│   ├── ChannelsServiceTests.cs
│   ├── VodServiceTests.cs
│   ├── StreamingServiceTests.cs
│   └── M3UParserTests.cs
├── Helpers/               # Helper classes for testing
│   ├── TestDbContextFactory.cs
│   └── TestDataGenerator.cs
└── README.md              # This file
```

## Test Approach

### Unit Tests

The tests in this project are primarily unit tests that verify the behavior of individual components in isolation. Each test focuses on a specific functionality and uses mocks to simulate dependencies.

### Test Database

Tests that interact with the database use an in-memory database provider (Microsoft.EntityFrameworkCore.InMemory) to avoid the need for a real database. Each test creates its own isolated database instance with a unique name to prevent test interference.

### Mocking

We use the Moq library to create mock objects for dependencies like:
- ILogger
- IMemoryCache
- IStreamingService
- HttpClient (via HttpMessageHandler)

### Test Data

The `TestDataGenerator` class provides methods to seed the test database with predefined data for testing. This ensures consistent test data across all tests.

## Running Tests

To run the tests, use the following command from the solution directory:

```bash
dotnet test
```

Or from the test project directory:

```bash
dotnet test
```

## Adding New Tests

When adding new tests:

1. Follow the existing pattern of test organization
2. Use descriptive test names that indicate what is being tested
3. Structure tests using the Arrange-Act-Assert pattern
4. Mock external dependencies
5. Clean up resources in the Dispose method if needed

## Test Naming Convention

Tests are named using the following convention:

```
MethodName_Scenario_ExpectedBehavior
```

For example:
- `GetChannelByIdAsync_WithValidId_ReturnsChannel`
- `GetChannelByIdAsync_WithInvalidId_ReturnsNull`
- `SearchChannelsAsync_WithEmptyTerm_ReturnsActiveChannels`
