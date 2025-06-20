# movies-api
my movies api for learning rest api development in asp.net core using web api


# general migration setups.
1. create entity model.
2. Do configuration inside Configuration directory using IEntityTypeConfiguration<Movie>
3. In configuration, we could also seed the database here.
4. Then run migrations add AddMovieTable 
5. Then database update

# Guidelines
> Never call db.saveChanges() inside repository call it using IUnitOfWork in services only.

> A controller should ideally call only one service per endpoint.
    
1. The controller’s job is to:

   * Handle the HTTP request and response.

   * Validate inputs (or delegate to model binders/validators).

   * Delegate the actual processing to one service.

2. That service then:

    * Coordinates the necessary work.

    * Calls multiple repositories or other services as needed.

    * Returns the result to the controller.


| From → To                              | Where?                       | Tools                         |
| -------------------------------------- | ---------------------------- | ----------------------------- |
| `Contract Request` → `CommandModel`    | Controller or a Mapper class | Manual / Mapster / AutoMapper |
| `CommandModel` → `Entity`              | Service or Domain            | Manual or Factory method      |
| `Entity` → `AggregateModel`            | Service layer                | Manual or projection          |
| `AggregateModel` → `Contract Response` | Controller or Mapper         | Manual / Mapster              |




# Yes, it feels like we’re just wrapping DbContext — because we are, in many cases.

# Why You Still Usually Want a Repository Layer

1. Abstraction and Decoupling
   
   * You're decoupling:

   * Your application/domain logic from the data access technology (EF Core, Dapper, Cosmos, etc.)

   * Makes it easier to mock or unit test your services

2. Centralized Logic
   
   * Even if now it's just db.Add(), tomorrow it might:

   * Set metadata (created/updated timestamps)

   * Check business rules before persisting

   * Include related entities or tracking logic

   * Implement caching or batching logic

   > Today: thin. Tomorrow: powerful.

3. Control over queries

   * Repositories allow:
   
   * Projection to specific models (e.g., MovieSummary)
   
   * Include navigations in one place
   
   * Define query filters (soft deletes, tenant filters, etc.)

4. Consistency
   * When every service calls IWhateverRepository, you enforce a single, consistent data access entry point, even if the implementation is thin.


