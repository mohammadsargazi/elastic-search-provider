# ElasticSearch Provider

This repository provides a generic base class, `BaseElasticRepository`, for implementing a provider for ElasticSearch. It offers common CRUD (Create, Read, Update, Delete) operations and other utility methods for interacting with an ElasticSearch index.

## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
- [API Reference](#api-reference)
- [Contributing](#contributing)

## Installation

To use this ElasticSearch provider, follow these steps:

1. Install the required dependencies:
    - Ensure that you have the appropriate version of ElasticSearch installed.
    - Install the NuGet package `Nest` to access the ElasticSearch client.

2. Copy the `BaseElasticRepository` class into your project.

3. Create a concrete repository class that inherits from `BaseElasticRepository` and implement the abstract methods as per your requirements.

4. Initialize an instance of your concrete repository class by providing an `IElasticClient` instance and an `IStringLocalizer` instance.

5. Start using the provided methods to interact with your ElasticSearch index.

## Usage

Here's an example of how to use the ElasticSearch provider:

```csharp
// Create an instance of the ElasticSearch client
var elasticClient = new ElasticClient(settings);

// Create an instance of your concrete repository class, providing the ElasticSearch client and a string localizer
var repository = new YourConcreteRepository(elasticClient, yourStringLocalizer);

// Perform operations on the ElasticSearch index
var entity = new YourEntity { /* Set entity properties */ };

// Insert an entity
await repository.InsertAsync(entity, cancellationToken);

// Get a single entity
var retrievedEntity = await repository.SingleAsync(entity.Id, cancellationToken);

// Update an entity
retrievedEntity.Property = "Updated value";
await repository.UpdateAsync(retrievedEntity, cancellationToken);

// Delete an entity
await repository.DeleteAsync(retrievedEntity, cancellationToken);

// Perform other operations as needed

API Reference
--------------
The following methods are available in the `BaseElasticRepository` class:

- `GetIndexName()`: Returns the name of the ElasticSearch index associated with the repository.
- `CanDelete(T entity, CancellationToken cancellationToken)`: Determines whether an entity can be deleted.
- `DeleteAsync(T entity, CancellationToken cancellationToken)`: Deletes an entity from the ElasticSearch index.
- `InsertAsync(T entity, CancellationToken cancellationToken)`: Inserts a new entity into the ElasticSearch index.
- `InsertAllAsync(List<T> entities, CancellationToken cancellationToken)`: Inserts multiple entities into the ElasticSearch index.
- `UpdateAsync(T entity, CancellationToken cancellationToken)`: Updates an existing entity in the ElasticSearch index.
- `UpdateAllAsync(List<T> entities, CancellationToken cancellationToken)`: Updates multiple existing entities in the ElasticSearch index.
- `IncrementAsync(Guid id, string field, Guid version, double amount = 1)`: Increments a numeric field of an entity in the ElasticSearch index.
- `FirstOrDefaultAsync(CancellationToken cancellationToken)`: Retrieves the first entity from the ElasticSearch index.
- `SingleOrDefaultAsync(Guid id, CancellationToken cancellationToken)`: Retrieves a single entity by its ID from the ElasticSearch index.
- `SingleAsync(Guid id, CancellationToken cancellationToken)`: Retrieves a single entity by its ID from the ElasticSearch index.
- `ListAsync(CancellationToken cancellationToken)`: Retrieves all entities from the ElasticSearch index.
- `ListAsync(List<Guid?> ids, CancellationToken cancellationToken)`: Retrieves entities by their IDs from the ElasticSearch index.
- `ListAsync(List<Guid> ids, CancellationToken cancellationToken)`: Retrieves entities by their IDs from the ElasticSearch index.
- `ListPagedAsync(IPagedRequest request, CancellationToken cancellationToken)`: Retrieves a paged list of entities from the ElasticSearch index.
- `CountAsync()`: Returns the total count of entities in the ElasticSearch index.

Please refer to the source code and method documentation for more detailed information about each method's usage and parameters.


Contributing
------------
Contributions to this ElasticSearch provider are welcome! If you have any ideas, improvements, or bug fixes, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make the necessary code changes and additions.
4. Write unit tests to ensure the correctness of your changes.
5. Run the existing unit tests and ensure they all pass.
6. Commit your changes and push them to your forked repository.
7. Submit a pull request to the main repository, describing your changes in detail.
8. Your pull request will be reviewed, and any necessary feedback or changes will be provided.
9. Once your pull request is approved, it will be merged into the main repository.

Thank you for contributing to this project!

