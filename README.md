# Tax Checker API  
An API that lets people check what tax rate applies in their city on any given day.  
Cities can define tax rules of four types:
- **Yearly**
- **Monthly**
- **Weekly**
- **Daily**

When multiple rules overlap, the priority is:
**Daily → Weekly → Monthly → Yearly**  (highest → lowest)

The API supports adding, updating, deleting, and querying tax rules, plus calculating averages over a date range.

---

##How to Run the API locally

### **Prerequisites**
- Docker Desktop (running)
- .NET SDK 10 installed
- IDE of your choice (VS, Rider, VS Code)

### **Launch**
Using command line:

```sh
dotnet run --project TaxChecker.API
```

Or start the API from **Visual Studio** / **Rider**.

On launch, the app will automatically:
* Create the PostgreSQL database if it's missing
* Apply EF Core migrations
* Insert initial seed data
* Open **Swagger UI** in your browser

### **Try It in Swagger**

Click *Authorize* → enter:
```
User
```
or
```
Admin
```
depending on which endpoints you want to hit.

---

## TODO / Known Issues

Features and fixes that were not completed due to time limitations.

### Error Handling
Planned but not finished:

* Global exception-handling middleware
* Mapping domain exceptions → HTTP status codes
* More detailed validation error responses - structured **ProblemDetails** JSON

### Bugs
Some tests are failing

### Nicer Swagger Documentation
Would add:
* Example providers
* More detailed summaries with return/error examples

### Code Quality Check
Most code in repeating elements such as tests/controllers was written quickly or AI-generated.
Need to look through in more detail:
* Unit test suite (not full coverage/not passing/double-check logic)
* Controllers (naming and endpoints not looked through yet)

---

## What IS Working
* PostgreSQL Docker setup
* EF Core migrations + automatic seeding
* Clean architecture structure
* Header-based role authentication
* Swagger UI with header auth

---

## Design Decision
### Minimal Hosting Model
Though more familiar with the legacy pattern Program.cs + Startup.cs, I'm using newer version for less boilerplate

### Domain: Value Objects Not Used
Value Objects (e.g., `DateRange`, `TaxRate`, `CityName`) were not used given the small scope

### Database Init in Startup
Database bootstrap logic is inside Program.cs now for simplicity/init flow easier to follow. 
Should later be moved into the Infrastructure layer to simplify Program.cs.

### Removed https
Don't need TLS, avoids cert prompts.

### No Result Models
Would use in larger scope to handle known/expected exceptions.

---

## Requirement Assumptions
### No auth header
Assuming no header (or anything other than 'User' or 'Admin') is not authorized in the application.
