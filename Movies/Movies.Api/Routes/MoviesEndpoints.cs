namespace Movies.Api.Routes;

public static class MoviesEndpoints
{
    private const string Base = $"{ApiEndpointConstants.ApiBase}/Movies";

    public const string Create = Base;
    public const string Get = $"{Base}/{{id:guid}}";
    
}



/*
 # some notes on endpoints.
 
 You're absolutely right in your understanding of how RESTful APIs typically work:
   
   ```
   /posts       : GET (get all), POST (create)
   /posts/1     : GET (get one), PUT (update), DELETE (delete)
   ```
   
   But if your company **only allows `GET` and `POST` methods**, and **disallows `PUT` and `DELETE`**, you need to work around that limitation.
   
   ---
   
   ## ✅ Goal: RESTful-like behavior using only `GET` and `POST`
   
   Since you can't use `PUT` or `DELETE`, the alternative is to **use POSTs to perform actions that would normally be handled by other methods**.
   
   ---
   
   ## ✅ Recommended Design with Only `GET` and `POST`
   
   ```
   GET    /posts         -> get all posts
   POST   /posts         -> create a new post
   
   GET    /posts/1       -> get a specific post
   
   POST   /posts/1/update   -> update post 1
   POST   /posts/1/delete   -> delete post 1
   ```
   
   ---
   
   ### 🔁 Why do it this way?
   
   * You're sticking to `GET` and `POST` only.
   * You're not violating the HTTP spec (e.g., using GET to do deletes, which would be bad).
   * It's easy to understand: you're essentially using POST as an "action".
   
   
   ## ✅ Optional: Action-Based Endpoint Structure
   
   If your API gets more complex, you can even group actions:
   
   ```
   POST /posts/1?action=update
   POST /posts/1?action=delete
   ```
   
   or
   
   ```
   POST /posts/1/update
   POST /posts/1/delete
   
 
 Great question — and the honest answer is:
   
   > **No, not fully.**
   > If an API only uses `GET` and `POST`, and doesn't support `PUT`, `DELETE`, etc., then it's **not truly RESTful** — it's **REST-like** or **REST-inspired**.
   
   ---
   
   ## 🔍 Why It’s Not Fully RESTful
   
   REST (Representational State Transfer), as defined by Roy Fielding’s dissertation, includes several key constraints:
   
   1. **Statelessness**
   2. **Client-Server Separation**
   3. **Cacheability**
   4. **Uniform Interface** (this is the key one here)
   5. **Layered System**
   6. **Code on Demand** (optional)
   
   The **"uniform interface"** constraint means using HTTP methods **according to their intended purpose**:
   
   | HTTP Method | Typical Use     |
   | ----------- | --------------- |
   | `GET`       | Retrieve data   |
   | `POST`      | Create new data |
   | `PUT`       | Update existing |
   | `DELETE`    | Delete data     |
   | `PATCH`     | Partial updates |
   
   By **not using `PUT`, `DELETE`, etc.**, your API breaks the uniform interface constraint — so it can't be fully RESTful.
   
   ---
   
   ## ✅ What *Is* It Then?
   
   You can call it:
   
   * **REST-like**
   * **RESTful over POST**
   * **RPC-ish with REST endpoints**
   * **HTTP-based API** (the most neutral)
   
   It’s not wrong or broken — lots of legacy systems and enterprise environments do this due to firewalls, proxies, or organizational policy. 
   But it’s **technically not REST** in the pure sense.
   
   ---
   
   ## 🚧 Why Companies Restrict HTTP Methods
   
   Common reasons:
   
   * Some firewalls or proxies block non-GET/POST methods (especially `DELETE`, `PUT`)
   * Legacy browsers or systems don’t handle other methods well
   * Simplicity or compliance constraints
   * Lack of tooling or understanding
   
   ---
   
   ## ✅ What You Can Do
   
   If you're stuck with only `GET` and `POST`, just be **intentional and consistent** with your API design:
   
   * Use `GET` only for reading (no side effects)
   * Use `POST` for anything else: create, update, delete — but make the endpoint path or parameters clear about what action is being done
   * Document everything clearly for clients
   
   ---
   
   ## 📝 TL;DR
   
   * ❌ Not a fully RESTful API if you're only using `GET` and `POST`
   * ✅ It's a **REST-like** or **HTTP-based API**
   * ✅ Still totally valid if designed well and consistently
   * ⚠️ Just avoid calling it “RESTful” in strict technical documentation
   
   Let me know if you want help wording this for a tech spec or internal documentation!
   
  
 */