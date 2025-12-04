#Tax Checker API
Application which allows users to see taxes applied in their city, on any given day.

##Design Decisions:
### Minimal Hosting Model
Though more familiar with the legacy pattern Program.cs + Startup.cs, using newer version for less boilerplate

### Domain: Value Objects Not Used
Value Objects (e.g., `DateRange`, `TaxRate`, `CityName`) were not used given the small scope

### Database Init in Startup
Database bootstrap logic is inside Program.cs to make app init flow easier to follow. 
Could later be moved into the Infrastructure layer to simplify Program.cs.

### Removed https
Don't need TLS, avoids cert prompts.