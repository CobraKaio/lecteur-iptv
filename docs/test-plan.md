# Test Plan for LecteurIptv Application

This document outlines the testing strategy for the LecteurIptv application, including both backend and frontend components.

## 1. Backend Testing

### 1.1 Unit Tests

#### Completed
- ✅ ChannelsService
- ✅ VodService
- ✅ StreamingService
- ✅ M3UParser
- ✅ UserService
- ✅ FavoritesService
- ✅ HistoryService
- ✅ JwtService

#### To Be Implemented
- ⬜ EpgService

### 1.2 Integration Tests

Integration tests verify that different components work together correctly.

#### To Be Implemented
- ⬜ ChannelsController
- ⬜ VodController
- ⬜ StreamingController
- ⬜ UsersController
- ⬜ FavoritesController
- ⬜ HistoryController
- ⬜ M3UController
- ⬜ Database Integration Tests

### 1.3 API Tests

API tests verify that the API endpoints work correctly from an external perspective.

#### To Be Implemented
- ⬜ Authentication Endpoints
- ⬜ Channels Endpoints
- ⬜ VOD Endpoints
- ⬜ Streaming Endpoints
- ⬜ Favorites Endpoints
- ⬜ History Endpoints
- ⬜ M3U Import Endpoints

## 2. Frontend Testing

### 2.1 Unit Tests

#### To Be Implemented
- ⬜ Composables (useChannels, useVod, useAuth, useFavorites, useHistory, useEpg)
- ⬜ Services (channelsService, vodService, authService, favoritesService, historyService, epgService)
- ⬜ Utility Functions

### 2.2 Component Tests

#### To Be Implemented
- ⬜ ChannelCard
- ⬜ VodCard
- ⬜ VideoPlayer
- ⬜ FavoriteButton
- ⬜ PaginationControls
- ⬜ Navigation Components
- ⬜ Form Components

### 2.3 End-to-End Tests

#### To Be Implemented
- ⬜ Authentication Flow
- ⬜ Channel Browsing and Playback
- ⬜ VOD Browsing and Playback
- ⬜ Favorites Management
- ⬜ History Tracking
- ⬜ Search and Filtering

## 3. Test Environment Setup

### 3.1 Backend Test Environment
- ✅ xUnit Test Project
- ✅ In-Memory Database
- ✅ Mocking Framework (Moq)
- ✅ Test Data Generation

### 3.2 Frontend Test Environment
- ⬜ Jest/Vitest Setup
- ⬜ Vue Test Utils
- ⬜ Mock API Responses
- ⬜ Test Data Generation

## 4. Continuous Integration

### 4.1 GitHub Actions Workflow
- ⬜ Backend Tests
- ⬜ Frontend Tests
- ⬜ Code Coverage Reporting
- ⬜ Pull Request Validation

## 5. Test Documentation

### 5.1 Test Reports
- ⬜ Test Coverage Reports
- ⬜ Test Results Dashboard

## 6. Next Steps

1. Complete the remaining backend unit test for EpgService
2. Implement integration tests for controllers
3. Set up the frontend testing environment
4. Implement frontend unit tests for composables and services
5. Implement component tests for key UI components
6. Set up continuous integration for automated testing
7. Implement end-to-end tests for critical user flows

## 7. Testing Tools and Libraries

### Backend
- xUnit: Testing framework
- Moq: Mocking framework
- Entity Framework Core InMemory: In-memory database provider
- FluentAssertions (to be added): More expressive assertions

### Frontend
- Jest/Vitest: Testing framework
- Vue Test Utils: Vue component testing
- Testing Library: DOM testing utilities
- Cypress (for E2E): End-to-end testing framework
