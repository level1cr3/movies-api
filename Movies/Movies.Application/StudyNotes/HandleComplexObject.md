I'm build web api in asp.net core controller version. I have a post endpoint that will get the request object. This endpoint is for learning purpose. it is for creating movie record in db.
This is the object structure based on db and how i'm currently getting request. below is structure.
```
{
    "Name" : "Iron man 2" (Type string)
    "Poster" : image  (Type IFormFile)
    "Description" : "This movie is sequel to iron man"
    "Actors": [
                {
                    "Name" : "Robert downey jr",
                    "HeroName" : "Iron man"  
                },
                {
                    "Name" : "Scarlet johanson",
                    "HeroName" : "Black widow"  
                }
             ]
}
```

Tell me best to handle this kind of object in controller endpoint. What is pragmatic and widely used and battle tested for this kind of object structure.


Excellent question — and it’s a very *real-world* one. Handling **mixed JSON + file uploads** (like your `IFormFile` image and a JSON body for movie details) is common in enterprise APIs (e.g., uploading profile pictures, product images, etc.), and doing it *pragmatically and cleanly* matters.

Let’s go through the **battle-tested best practice** way used in ASP.NET Core Web APIs (controller-based).

---

## ✅ The Right Way (Enterprise-grade & pragmatic)

Use **multipart/form-data** with a **DTO (Data Transfer Object)** that separates the file from the rest of the JSON.

ASP.NET Core can’t automatically bind a complex JSON + file object in one go from a raw JSON body — but it can if we use `[FromForm]` and send JSON-serialized nested parts.

---

### **1. The DTOs**

```csharp
public class CreateMovieRequest
{
    public string Name { get; set; }

    // The file itself (handled as multipart/form-data)
    public IFormFile Poster { get; set; }

    public string Description { get; set; }

    // The actors JSON will be sent as a serialized string and manually deserialized
    public string ActorsJson { get; set; }

    [NotMapped] // not a DB property; for controller use
    public List<ActorRequest> Actors => 
        string.IsNullOrEmpty(ActorsJson)
            ? new List<ActorRequest>()
            : JsonSerializer.Deserialize<List<ActorRequest>>(ActorsJson);
}

public class ActorRequest
{
    public string Name { get; set; }
    public string HeroName { get; set; }
}
```

---

### **2. The Controller Endpoint**

```csharp
[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateMovie([FromForm] CreateMovieRequest request)
    {
        // 1️⃣ Handle the file (Poster)
        if (request.Poster is null || request.Poster.Length == 0)
            return BadRequest("Poster is required");

        var filePath = Path.Combine("wwwroot", "posters", request.Poster.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.Poster.CopyToAsync(stream);
        }

        // 2️⃣ Deserialize actors
        var actors = request.Actors;

        // 3️⃣ Simulate saving to DB (your ORM e.g., EF Core)
        var movie = new Movie
        {
            Name = request.Name,
            Description = request.Description,
            PosterPath = filePath,
            Actors = actors.Select(a => new Actor
            {
                Name = a.Name,
                HeroName = a.HeroName
            }).ToList()
        };

        // await _dbContext.Movies.AddAsync(movie);
        // await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Movie created successfully" });
    }
}
```

---

### **3. Sending Request from Frontend or Postman**

Since this is **multipart/form-data**, you can’t just send JSON directly.

In Postman or frontend:

* Use `Content-Type: multipart/form-data`
* Fields:

    * `Name` → `"Iron man 2"`
    * `Poster` → *(select file)*
    * `Description` → `"This movie is sequel to iron man"`
    * `ActorsJson` →

      ```json
      [
        { "Name": "Robert downey jr", "HeroName": "Iron man" },
        { "Name": "Scarlet johanson", "HeroName": "Black widow" }
      ]
      ```

---

### ✅ Why This Is the Best Practice

| Concern                     | Approach                            | Reason                                                                      |
| --------------------------- | ----------------------------------- | --------------------------------------------------------------------------- |
| **File upload**             | `IFormFile` via `[FromForm]`        | Handles large files efficiently (streamed, not buffered in memory fully).   |
| **Complex nested data**     | JSON string (`ActorsJson`)          | Simplifies model binding. Avoids custom formatters or input stream parsing. |
| **Framework compatibility** | `[Consumes("multipart/form-data")]` | Explicitly documents your endpoint for OpenAPI / Swagger tooling.           |
| **Scalability**             | Save to disk / cloud storage first  | Avoids bloating DB. Common in enterprise setups.                            |
| **Clean separation**        | DTO + Entity mapping                | Follows CQRS-style or clean architecture principles.                        |

---

### ⚙️ Optional: Use FluentValidation

You can add robust validation:

```csharp
public class CreateMovieValidator : AbstractValidator<CreateMovieRequest>
{
    public CreateMovieValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Poster).NotNull();
        RuleFor(x => x.ActorsJson).NotEmpty();
    }
}
```

---

### 🧠 Alternative (less common)

If your API is *only JSON-based* (no file uploads), you could accept `byte[] Poster` or Base64 image string, but **this is not recommended in production** — it bloats payloads, hurts performance, and makes uploads slow.

---

### 🚀 TL;DR (Summary)

✅ **Recommended pattern:**

```csharp
[HttpPost]
[Consumes("multipart/form-data")]
public async Task<IActionResult> CreateMovie([FromForm] CreateMovieRequest request)
```

📦 Inside `CreateMovieRequest`:

* `string Name`
* `IFormFile Poster`
* `string Description`
* `string ActorsJson` → then `JsonSerializer.Deserialize<List<ActorRequest>>`

This is **the most common, battle-tested** way seen in:

* ASP.NET Core e-commerce, CMS, and media APIs
* Microsoft sample apps
* Real enterprise backends (especially with React/Next.js or mobile clients)

---

✅ **Exactly right** — in the frontend (React, Next.js, or any JS client), you’ll use the **`FormData`** API to send this kind of mixed (JSON + file) request.

That’s the *standard, battle-tested* way to send a multipart/form-data request to ASP.NET Core.

Let’s go step by step with a pragmatic example 👇

---

## 🧱 1. Example Frontend Object

Here’s the object you have in your app:

```ts
const movie = {
  name: "Iron Man 2",
  description: "This movie is sequel to iron man",
  poster: selectedFile, // this comes from <input type="file" />
  actors: [
    { name: "Robert Downey Jr", heroName: "Iron Man" },
    { name: "Scarlett Johansson", heroName: "Black Widow" }
  ]
};
```

---

## ⚙️ 2. Convert to `FormData`

Use JavaScript’s built-in `FormData` API:

```ts
const formData = new FormData();
formData.append("Name", movie.name);
formData.append("Description", movie.description);
formData.append("Poster", movie.poster);
formData.append("ActorsJson", JSON.stringify(movie.actors));
```

✅ Key thing:

* The keys (`"Name"`, `"Description"`, `"Poster"`, `"ActorsJson"`) **must exactly match** the property names in your backend DTO (`CreateMovieRequest`).
* For nested objects or arrays, you **stringify** them (`ActorsJson` → JSON string).

---

## 🌐 3. Send Using Axios (recommended)

Here’s how to send it cleanly with Axios:

```ts
import axios from "axios";

const createMovie = async (movie) => {
  const formData = new FormData();
  formData.append("Name", movie.name);
  formData.append("Description", movie.description);
  formData.append("Poster", movie.poster);
  formData.append("ActorsJson", JSON.stringify(movie.actors));

  const response = await axios.post("/api/movies", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  return response.data;
};
```

---

## 🧩 4. Example React Form Integration

```tsx
const MovieForm = () => {
  const [poster, setPoster] = useState<File | null>(null);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [actors, setActors] = useState([
    { name: "", heroName: "" },
  ]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append("Name", name);
    formData.append("Description", description);
    if (poster) formData.append("Poster", poster);
    formData.append("ActorsJson", JSON.stringify(actors));

    await axios.post("/api/movies", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  };

  return (
    <form onSubmit={handleSubmit}>
      <input type="text" placeholder="Movie name" value={name} onChange={(e) => setName(e.target.value)} />
      <textarea placeholder="Description" value={description} onChange={(e) => setDescription(e.target.value)} />
      <input type="file" onChange={(e) => setPoster(e.target.files[0])} />
      <button type="submit">Create Movie</button>
    </form>
  );
};
```

---

## 🔥 Summary — Frontend → Backend Flow

| Layer                      | Action                                       | Format                                             |
| -------------------------- | -------------------------------------------- | -------------------------------------------------- |
| **Frontend (React/JS)**    | Build `FormData` and append fields           | multipart/form-data                                |
| **Axios / Fetch**          | POST request to `/api/movies`                | Content-Type: multipart/form-data                  |
| **Backend (ASP.NET Core)** | `[FromForm] CreateMovieRequest` binds values | DTO receives file + fields                         |
| **Model Binding**          | `ActorsJson` deserialized manually           | `JsonSerializer.Deserialize<List<ActorRequest>>()` |

---

## ✅ Why FormData is the standard choice

* Handles files and text fields in one request.
* Works natively with browser APIs and Axios.
* Efficient for large file uploads.
* Automatically sets correct boundaries for `multipart/form-data`.


# now what if we also have genras array. that will be inserted in mapping table of genras and movie ?

Perfect — that’s exactly the kind of real-world complexity you encounter in actual production APIs. 🎯

When you add a **genres array** (IDs referring to a many-to-many relationship via a mapping table), the pattern stays **the same** as before — you just extend your DTO and handle the relationship in your persistence layer (EF Core, Dapper, etc.).

Let’s go over the **battle-tested approach**.

---

## 🧩 1. Updated DTO

We'll keep `[FromForm]` since you’re still uploading a file (`Poster`).

You’ll now have:

* Simple text fields
* A file
* JSON arrays for complex/nested data (`Actors`, `Genres`)

```csharp
public class CreateMovieRequest
{
    public string Name { get; set; }
    public IFormFile Poster { get; set; }
    public string Description { get; set; }

    // JSON strings (sent via FormData)
    public string ActorsJson { get; set; }
    public string GenresJson { get; set; }

    // Computed convenience properties (not bound directly)
    [NotMapped]
    public List<ActorRequest> Actors =>
        string.IsNullOrEmpty(ActorsJson)
            ? new()
            : JsonSerializer.Deserialize<List<ActorRequest>>(ActorsJson)!;

    [NotMapped]
    public List<int> Genres =>
        string.IsNullOrEmpty(GenresJson)
            ? new()
            : JsonSerializer.Deserialize<List<int>>(GenresJson)!;
}

public class ActorRequest
{
    public string Name { get; set; }
    public string HeroName { get; set; }
}
```

---

## ⚙️ 2. Controller Endpoint (EF Core example)

```csharp
[HttpPost]
[Consumes("multipart/form-data")]
public async Task<IActionResult> CreateMovie([FromForm] CreateMovieRequest request)
{
    if (request.Poster == null || request.Poster.Length == 0)
        return BadRequest("Poster is required");

    // Save poster
    var filePath = Path.Combine("wwwroot", "posters", request.Poster.FileName);
    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
    await using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await request.Poster.CopyToAsync(stream);
    }

    // Create Movie entity
    var movie = new Movie
    {
        Name = request.Name,
        Description = request.Description,
        PosterPath = filePath
    };

    // Map Actors
    foreach (var actorReq in request.Actors)
    {
        movie.Actors.Add(new Actor
        {
            Name = actorReq.Name,
            HeroName = actorReq.HeroName
        });
    }

    // Map Genres (many-to-many)
    foreach (var genreId in request.Genres)
    {
        movie.MovieGenres.Add(new MovieGenre
        {
            GenreId = genreId
        });
    }

    _dbContext.Movies.Add(movie);
    await _dbContext.SaveChangesAsync();

    return Ok(new { message = "Movie created successfully" });
}
```

---

## 🗂️ 3. Example EF Core Entities

### Movie.cs

```csharp
public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PosterPath { get; set; }

    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}
```

### Actor.cs

```csharp
public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string HeroName { get; set; }

    public int MovieId { get; set; }
    public Movie Movie { get; set; }
}
```

### Genre.cs

```csharp
public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}
```

### MovieGenre.cs (Mapping Table)

```csharp
public class MovieGenre
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; }

    public int GenreId { get; set; }
    public Genre Genre { get; set; }
}
```

---

## 🌐 4. Frontend (FormData)

Here’s what you’d send from React or JS:

```ts
const formData = new FormData();
formData.append("Name", movie.name);
formData.append("Description", movie.description);
formData.append("Poster", movie.poster);
formData.append("ActorsJson", JSON.stringify(movie.actors));
formData.append("GenresJson", JSON.stringify(movie.genres)); // [1,2,3]
```

Then:

```ts
await axios.post("/api/movies", formData, {
  headers: { "Content-Type": "multipart/form-data" },
});
```

---

## ✅ Why This Design Works Well in Real Projects

| Concern                             | Practice                            | Why                                              |
| ----------------------------------- | ----------------------------------- | ------------------------------------------------ |
| **File Uploads**                    | `IFormFile`                         | Efficient streaming, supported by ASP.NET Core   |
| **Nested JSON (Actors)**            | JSON string (`ActorsJson`)          | Avoids custom model binders, straightforward     |
| **IDs for Mapping Table (Genres)**  | JSON string of IDs                  | Easy to deserialize, aligns with EF many-to-many |
| **Clean Architecture**              | DTO → Entity mapping                | Keeps API layer separate from data layer         |
| **OpenAPI / Swagger compatibility** | `[Consumes("multipart/form-data")]` | Documented correctly in API spec                 |

---

## 🧠 Notes

* Don’t try to mix `[FromBody]` and `[FromForm]` — ASP.NET Core doesn’t support that in one action.
* You could technically create a **custom model binder** to automatically parse JSON fields, but that’s overkill for 99% of APIs.
* If you need to validate `Genres` or `Actors`, use **FluentValidation** and validate the JSON after deserialization.

---



# Now one last thing how do i tell this in swagger document that to only provide  Json values not the below property. They would be programatically handled. I can't even remove them because we want to show the structure of json object they need to send. also in a way we have to make below property as optional so postman or js don't get error saying that we need these when sending the request

    // Computed convenience properties (not bound directly)
    [NotMapped]
    public List<ActorRequest> Actors =>
        string.IsNullOrEmpty(ActorsJson)
            ? new()
            : JsonSerializer.Deserialize<List<ActorRequest>>(ActorsJson)!;

    [NotMapped]
    public List<int> Genres =>
        string.IsNullOrEmpty(GenresJson)
            ? new()
            : JsonSerializer.Deserialize<List<int>>(GenresJson)!;


Excellent — this is a very thoughtful and **real** question 👏
You’ve hit one of the subtle, enterprise-level problems that appear in real APIs:

> ❓ How to make Swagger show JSON field *structure* (so users know what to send)
> but *not* require or expose the internal computed properties (`Actors`, `Genres`)
> which are only derived from JSON strings sent via `FormData`.

---

## ✅ The Pragmatic, Battle-Tested Solution

You solve this cleanly using **Swagger annotations** (from `Swashbuckle.AspNetCore.Annotations`) — specifically `[SwaggerSchema]`, `[SwaggerIgnore]`, and `[SwaggerExample]`.

Here’s how you’d do it step-by-step.

---

### **1️⃣ Mark the computed properties as hidden in Swagger**

Use `[SwaggerSchema(ReadOnly = true, WriteOnly = false, Description = "...")]`
OR simply `[SwaggerIgnore]` to completely hide them.

```csharp
using Swashbuckle.AspNetCore.Annotations;

public class CreateMovieRequest
{
    public string Name { get; set; }

    public IFormFile Poster { get; set; }

    public string Description { get; set; }

    [SwaggerSchema(
        Description = "JSON string of actors. Example: " +
                      "[{\"name\": \"Robert Downey Jr\", \"heroName\": \"Iron Man\"}]")]
    public string ActorsJson { get; set; }

    [SwaggerSchema(
        Description = "JSON array of genre IDs. Example: [1, 2, 3]")]
    public string GenresJson { get; set; }

    [NotMapped]
    [SwaggerIgnore] // 👈 hides from Swagger schema completely
    public List<ActorRequest> Actors =>
        string.IsNullOrEmpty(ActorsJson)
            ? new()
            : JsonSerializer.Deserialize<List<ActorRequest>>(ActorsJson)!;

    [NotMapped]
    [SwaggerIgnore] // 👈 hides from Swagger schema completely
    public List<int> Genres =>
        string.IsNullOrEmpty(GenresJson)
            ? new()
            : JsonSerializer.Deserialize<List<int>>(GenresJson)!;
}
```

🧠 **Result:**

* Swagger will **not** show `Actors` or `Genres` (the computed properties).
* It **will** show `ActorsJson` and `GenresJson` with example values and clear text explaining the format.

---

### **2️⃣ Make sure to register Swagger Annotations**

In `Program.cs`:

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // 👈 enables [SwaggerSchema], [SwaggerIgnore], etc.
});
```

---

### **3️⃣ Optionally show example payloads**

You can add more developer-friendly guidance in Swagger:

```csharp
[SwaggerSchema(Description = "Movie creation request")]
public class CreateMovieRequest
{
    public string Name { get; set; }

    public IFormFile Poster { get; set; }

    public string Description { get; set; }

    [SwaggerSchema(
        Description = "JSON string representing list of actors",
        Example = "[{\"name\":\"Robert Downey Jr\",\"heroName\":\"Iron Man\"}]")]
    public string ActorsJson { get; set; }

    [SwaggerSchema(
        Description = "JSON array of genre IDs",
        Example = "[1,2,3]")]
    public string GenresJson { get; set; }

    [SwaggerIgnore]
    [NotMapped]
    public List<ActorRequest> Actors { get; }

    [SwaggerIgnore]
    [NotMapped]
    public List<int> Genres { get; }
}
```

Now Swagger UI will clearly show:

| Field       | Type   | Description                          |
| ----------- | ------ | ------------------------------------ |
| Name        | string | Movie name                           |
| Poster      | file   | Image file                           |
| Description | string | Movie description                    |
| ActorsJson  | string | JSON list of actors (see example)    |
| GenresJson  | string | JSON list of genre IDs (see example) |

---

### **4️⃣ Mark them as optional**

To prevent Postman / Swagger-generated clients from thinking these are *required*, mark them as nullable or optional:

```csharp
public string? ActorsJson { get; set; }
public string? GenresJson { get; set; }
```

Swagger will then generate these fields as *optional* in OpenAPI spec.

---

### ✅ Final Swagger Behavior Summary

| Property      | Appears in Swagger | Required   | Description                       |
| ------------- | ------------------ | ---------- | --------------------------------- |
| `Name`        | ✅ Yes              | ✅ Yes      | Movie name                        |
| `Poster`      | ✅ Yes              | ✅ Yes      | File upload                       |
| `Description` | ✅ Yes              | ✅ Yes      | Description text                  |
| `ActorsJson`  | ✅ Yes              | ❌ Optional | JSON list of actors               |
| `GenresJson`  | ✅ Yes              | ❌ Optional | JSON array of genre IDs           |
| `Actors`      | 🚫 No              | —          | Hidden internal computed property |
| `Genres`      | 🚫 No              | —          | Hidden internal computed property |

---

### 🧠 Bonus Tip — if you want Swagger to *show example JSON structure nicely*

You can add a **custom schema filter** that prettifies JSON examples, but in practice adding `[SwaggerSchema(Example = "...")]` is simpler and used in production across large .NET teams.

---

### 🧾 TL;DR Summary

✅ What you want to do (enterprise-ready):

* Use `[SwaggerIgnore]` on computed, internal-only properties.
* Use `[SwaggerSchema(Description = "...", Example = "...")]` on the `...Json` properties to guide the consumer.
* Make the `...Json` properties nullable to make them *optional* in Swagger and Postman clients.
* Enable annotations via `EnableAnnotations()` in `AddSwaggerGen()`.

---


# no what i was saying is how to make actors and Genres (hidden or not recive data from UI side or postman) not their json version. will [NotMapped] take care of it ? or is that for something else like not not mapped to ef entitty for table ?

Excellent clarification 👏 — and you’re absolutely right to question the purpose of `[NotMapped]`.

Let’s break it down precisely:

---

## 🧠 `[NotMapped]` — What It Actually Does

`[NotMapped]` belongs to **Entity Framework (EF)**, not the ASP.NET Core model binding system.
It simply tells EF:

> “Don’t try to persist this property to the database table.”

So:

* ✅ EF **ignores** the property when generating the database schema or performing inserts/updates.
* ❌ ASP.NET Core model binding **still sees** the property — meaning clients (Swagger, Postman, React) can *still* send data to it unless you explicitly hide or ignore it in the controller binding or Swagger config.

---

## ⚙️ What You Actually Want

You want:

1. `Actors` and `Genres` to be **computed only on the server side**.
2. The **client (UI / Postman)** should **not send** these values.
3. Swagger should not show them.
4. EF should not map them to a database column.

So you need to combine attributes from **both ASP.NET Core** and **Entity Framework** layers.

---

## ✅ The Correct Combination

```csharp
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CreateMovieRequest
{
    public string Name { get; set; }

    public IFormFile Poster { get; set; }

    public string Description { get; set; }

    public string? ActorsJson { get; set; }

    public string? GenresJson { get; set; }

    // 🧩 Computed, not part of incoming payload, not sent by client
    [NotMapped] // <-- EF should not map to DB
    [JsonIgnore] // <-- ASP.NET model binder ignores it for incoming requests
    [SwaggerIgnore] // <-- Hide from Swagger UI completely
    public List<ActorRequest> Actors =>
        string.IsNullOrEmpty(ActorsJson)
            ? new()
            : JsonSerializer.Deserialize<List<ActorRequest>>(ActorsJson)!;

    [NotMapped]
    [JsonIgnore]
    [SwaggerIgnore]
    public List<int> Genres =>
        string.IsNullOrEmpty(GenresJson)
            ? new()
            : JsonSerializer.Deserialize<List<int>>(GenresJson)!;
}
```

---

## 🔍 Breakdown of Each Attribute

| Attribute                                | Purpose                                                                                          |
| ---------------------------------------- | ------------------------------------------------------------------------------------------------ |
| `[NotMapped]`                            | EF won’t map it to a column. (Database layer)                                                    |
| `[JsonIgnore]`                           | ASP.NET Core won’t bind it from JSON or form data (input). Also won’t serialize it in responses. |
| `[SwaggerIgnore]`                        | It won’t show up in Swagger UI.                                                                  |
| `public` property with only a **getter** | Prevents model binding from setting it — the property is computed only on server side.           |

---

## ✅ Result

* When a request comes in:

    * The client can send `ActorsJson` and `GenresJson`.
    * ASP.NET will **ignore** any attempt to send `Actors` or `Genres`.
* In Swagger/Postman:

    * `Actors` and `Genres` will **not appear** in the model schema.
* EF Core:

    * Will not create columns for `Actors` or `Genres`.

---

### ✅ TL;DR Summary

| Concern                        | Solution                 |
| ------------------------------ | ------------------------ |
| Don’t store in DB              | `[NotMapped]`            |
| Don’t accept from client       | `[JsonIgnore]`           |
| Don’t show in Swagger          | `[SwaggerIgnore]`        |
| Keep computed logic in backend | Use getter-only property |

---

Would you like me to show a **one-file complete version** of the DTO and controller with these attributes together, ready for production use (including Swagger annotations and validation)?
