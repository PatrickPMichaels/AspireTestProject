Creating a migration is a bit different in this context

We create a migration using a start up project that's hooked up with the Aspire configuration for the Database, e.g the API
Then we target the EFModels project as where we want the migrations to be added to and checked from

This leads to this command being ran from within the API project
dotnet ef migrations --project ..\Database\EFModels.csproj add Intial

Or from solution
dotnet ef migrations --startup .\API --project .\Database add Intial