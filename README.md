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


> Make Contracts, Query and Command Model of Record type.

> Make Entity, Aggregate Models of Class Type.

> No. Repositories should not receive query models (or command models) — they should receive primitive values like Guid, string, etc., or Domain/Entity objects, depending on the operation.


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



You're right on track — and your instincts are mostly solid. Let's break this down carefully and answer your **three key questions**:

---

## ✅ Q1: **What should you validate — Command/Query model or Entity?**

> ✅ **Validate the Command or Query models** — not the Entity.

### 💡 Why?

* **Command/Query models** represent the **incoming intent from the client**
* They are the **first boundary** where user input is available
* Entities may have **internal rules**, but FluentValidation is better suited for validating external input, which happens at the command/query level

### 🚫 Don't validate entities using FluentValidation

* Entity validation should be done via **domain rules**, maybe using methods like `EnsureValid()`, value objects, or throwing domain exceptions
* Entities might be created by systems or composed from other entities — not always just user input

---

### ✅ Example — `CreateMovieCommand` validator:

```csharp
public class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ReleaseDate).LessThanOrEqualTo(DateTime.UtcNow);
    }
}
```

Then in the controller or pipeline (e.g., MediatR with validation behavior), this is validated **before hitting the domain**.

---

## ✅ Q2: **Where should mappings live?**

You nailed the idea. Here's a breakdown:

### 🔹 1. **Contracts → Command/Query model**

* ✅ Lives in the **API/controller layer**
* This is **presentation-level mapping**
* Should not bleed into the service/application layer

```csharp
// Controller
var command = new CreateMovieCommand(req.Title, req.ReleaseDate);
await mediator.Send(command);
```

---

### 🔹 2. **Command model → Entity**

* ✅ Lives in the **application layer** (usually inside service or handler)
* This is **domain mapping**, and should be explicit
* Avoid pushing contracts or commands into the domain

```csharp
// Application layer
var movie = new Movie
{
    Title = command.Title,
    Slug = _slugGenerator.Generate(command.Title)
};
```

> If it’s repetitive, extract a mapping method in a helper class inside Application layer.

---

## ✅ Q3: Summary Table

| Step                     | What to Validate           | Where to Map                             |
| ------------------------ | -------------------------- | ---------------------------------------- |
| API → Controller         | ✅ `Request` model          | ➡️  Controller maps to `Command/Query`   |
| Controller → Application | ✅ `Command` or `Query`     | ➡️  Service maps to `Entity`             |
| Application → Domain     | ❌ Don't validate           | ✅ Map to `Entity` inside service/handler |
| Domain/Entity layer      | 🔁 Optional business rules | Not FluentValidation — use invariants    |

---

## ✳️ Optional: Where to put mapping logic?

| Mapping           | Where to Put                                | Tip                                         |
| ----------------- | ------------------------------------------- | ------------------------------------------- |
| Request → Command | Controller or API Mappers                   | Keep clean and flat                         |
| Command → Entity  | Application layer (service or mapper class) | Avoid passing command to repo               |
| Entity → DTO      | Application or API layer                    | Create dedicated `Mapper` class if reusable |

---

## 🔚 TL;DR

* ✅ Validate only **command/query models** (FluentValidation)
* ❌ Don’t validate **entities** via FluentValidation
* ✅ Map:

   * Contract → Command in **Controller**
   * Command → Entity in **Application Layer**

You're designing with solid DDD + Clean Architecture principles. Let me know if you want a quick base class setup or mapping extensions to streamline this.

