# LionFire Virtual Object System (VOS) Documentation

The Virtual Object System (VOS) is a powerful abstraction layer that provides a unified hierarchical interface for accessing various data sources through a virtual filesystem-like API.

## Documentation Structure

1. [VOS Overview](./vos-overview.md) - Introduction and high-level concepts
2. [Architecture](./vos-architecture.md) - System design and components
3. [Core Concepts](./vos-core-concepts.md) - Fundamental concepts and terminology
4. [API Reference](./vos-api-reference.md) - Detailed API documentation
5. [Mounting System](./vos-mounting-system.md) - How to mount and configure data sources
6. [Persistence](./vos-persistence.md) - Data persistence and storage
7. [Examples and Usage](./vos-examples.md) - Practical examples and common patterns

## Quick Start

The VOS provides a virtual filesystem-like interface where:
- **Vobs** (Virtual Objects) are the nodes in the virtual tree
- **References** provide paths to locate objects
- **Mounts** connect different data sources to the tree
- **Handles** provide read/write access to data
- **Persistence** manages storage and retrieval

See the [Overview](./vos-overview.md) for a complete introduction.