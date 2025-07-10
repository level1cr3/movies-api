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


| From → To                        | Where?                       | Tools                         |
|----------------------------------| ---------------------------- | ----------------------------- |
| `Contract Request` → `DtoModel`  | Controller or a Mapper class | Manual / Mapster / AutoMapper |
| `DtoModel` → `Entity`            | Service or Domain            | Manual or Factory method      |
| `Entity` → `DtoModel`            | Service layer                | Manual or projection          |
| `DtoModel` → `Contract Response` | Controller or Mapper         | Manual / Mapster              |

> we will only go with manual mapping only

> in application I have made many things internal which i don't think should be accessed directly from api.
> like applicationDbContext, repository, entity and configurations for entity and so on..  





| Failure Type                     | Common Real-World Status Code                                          |
| -------------------------------- | ---------------------------------------------------------------------- |
| **Validation errors**            | `400 Bad Request`                                                      |
| **Business rule violations**     | ✅ Some teams use `422`, but <br>❌ Most default to `400` for simplicity |
| **Duplicate data (e.g., email)** | Usually `400`, some use `409 Conflict`                                 |
| **Authorization failures**       | `403 Forbidden`                                                        |
| **Unexpected errors**            | `500 Internal Server Error`                                            |



✅ Short Answer (Pragmatic):
Use Task<Result<MovieDto>> — you don't need MovieDto? inside Result<...> unless you want to distinguish between:

Operation succeeded, but value is null vs.

Operation failed.

In most clean implementations of Result<T>, the absence of value is already implied by Result.Failure(...), so MovieDto (non-nullable) is sufficient.