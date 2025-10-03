# Contributing to TravelRequests API

Thank you for your interest in contributing to TravelRequests API! This document provides guidelines and information for contributors.

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK or higher
- SQL Server (local or Docker)
- Git
- Docker (optional)

### Development Setup
1. Fork the repository
2. Clone your fork: `git clone https://github.com/yourusername/net-consware.git`
3. Create a feature branch: `git checkout -b feature/your-feature-name`
4. Follow the [README instructions](README.md) to set up the project locally

## 📋 Development Guidelines

### Code Style
- Follow C# naming conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and small
- Use async/await for I/O operations

### Testing
- Write unit tests for new features
- Ensure all tests pass: `dotnet test`
- Maintain or improve test coverage
- Follow the existing test patterns

### Commits
Use [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` for new features
- `fix:` for bug fixes
- `docs:` for documentation changes
- `test:` for test additions/changes
- `chore:` for maintenance tasks

### Pull Requests
1. Ensure all tests pass
2. Update documentation if needed
3. Follow the PR template
4. Request review from maintainers

## 🏗️ Architecture

This project follows Clean Architecture principles:

- **Domain**: Business entities and rules
- **Application**: Use cases and interfaces
- **Infrastructure**: Data access and external services
- **API**: Controllers and DTOs

## 🧪 Testing

### Running Tests
```bash
# All tests
dotnet test

# Specific test class
dotnet test --filter "AuthServiceTests"

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure
- Unit tests in `tests/TravelRequests.Tests/Services/`
- Integration tests in `tests/TravelRequests.Tests/Services/IntegrationTests.cs`
- Use Moq for mocking dependencies
- Follow AAA pattern (Arrange, Act, Assert)

## 🔧 Development Commands

### Database
```bash
# Create migration
dotnet ef migrations add MigrationName --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api

# Update database
dotnet ef database update --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api
```

### Code Quality
```bash
# Format code
dotnet format

# Check formatting
dotnet format --verify-no-changes

# Check vulnerabilities
dotnet list package --vulnerable --include-transitive

# Check outdated packages
dotnet outdated
```

### Docker
```bash
# Build and run with Docker Compose
docker-compose up --build

# Run specific service
docker-compose up api

# View logs
docker-compose logs -f
```

## 📚 Documentation

### API Documentation
- Swagger UI available at `/swagger` when running
- Add XML documentation for new endpoints
- Update README.md for significant changes

### Code Documentation
- Use XML comments for public APIs
- Document complex business logic
- Keep comments up to date

## 🐛 Bug Reports

When reporting bugs, please include:
- Clear description of the issue
- Steps to reproduce
- Expected vs actual behavior
- Environment details (.NET version, OS, etc.)
- Screenshots if applicable

## ✨ Feature Requests

When requesting features, please include:
- Clear description of the feature
- Use case and motivation
- Potential implementation approach
- Any breaking changes

## 🔒 Security

- Report security issues privately to maintainers
- Don't commit secrets or sensitive data
- Follow security best practices
- Use secure coding practices

## 📝 License

By contributing, you agree that your contributions will be licensed under the MIT License.

## 🤝 Code of Conduct

- Be respectful and inclusive
- Focus on constructive feedback
- Help others learn and grow
- Follow professional communication standards

## 📞 Getting Help

- Check existing issues and discussions
- Review documentation and README
- Ask questions in discussions
- Contact maintainers if needed

Thank you for contributing to TravelRequests API! 🚀
