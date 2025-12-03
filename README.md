#Tax Checker API
Application which allows users tosee taxes applied in their city, on any given day.

##Design Decisions:
### Minimal Hosting Model
Though more familiar with the legacy pattern Program.cs + Startup.cs, using newer version for less boilerplate

### Domain: Value Objects Not Used
Value Objects (e.g., `DateRange`, `TaxRate`, `CityName`) were not used given the small scope
