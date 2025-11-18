# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-11-17

### Added
- **TaskWrapper**: Simple task execution wrapper for sync and async operations
- **SimpleThread**: Easy thread management with cancellation support
- **AsyncOperation**: Manual async operation tracking with result management
- **ManagedThreadPool**: Managed thread pool with automatic queuing and load balancing
- **TaskScheduler**: Task scheduling with one-time and repeating execution
- **TaskRetry**: Automatic retry logic with exponential backoff
- **ConcurrentBatcher**: Concurrent batch processing with configurable concurrency
- **TaskExtensions**: LINQ-like extensions for cleaner async code
- Comprehensive XML documentation for all public APIs
- Full .NET 10.0 support with nullable reference types
- Zero external dependencies
- Complete example implementations
- Quick reference guide
- Comprehensive API documentation

### Features
- Run synchronous and asynchronous code on thread pool
- Create and manage individual threads with cancellation
- Track async operations manually
- Manage multiple concurrent threads with automatic queuing
- Schedule tasks with precise timing control
- Retry failed operations with exponential backoff
- Process large datasets in concurrent batches
- Chain async operations with LINQ-style extensions
- Handle task completion, cancellation, and errors with callbacks

### Documentation
- Comprehensive README with quick start examples
- API reference documentation
- Usage examples for all components
- Best practices guide
- Common patterns section
- Advanced usage scenarios

## [Unreleased]

### Planned Features
- Async stream processing utilities
- Task result caching utilities
- Advanced scheduling with cron expressions
- Task priority queues
- Performance metrics collection
- Integration with logging frameworks
