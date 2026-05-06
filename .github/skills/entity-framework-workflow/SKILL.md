---
name: entity-framework-workflow
description: 'Use for Entity Framework Core model changes, DbContext updates, repository migrations, and moving mock repository data into an EF repository.'
argument-hint: 'Describe the EF model or repository change'
---

# Entity Framework Workflow

## When to Use
Use this skill when:
- Entity Framework entity classes change
- `DbContext`, `DbSet`, or Fluent API configuration changes
- mock repository data needs to be migrated into a real EF Core repository
- dependency injection must switch from a mock repository to an EF-backed repository
- database seed data or relationships must stay aligned with the domain model

## Goal
Keep the EF model, repository layer, and seeded data consistent so the app can move from mock data to a database-backed implementation without breaking existing behavior.

## Procedure
1. Identify the affected entity classes and repository methods.
2. Compare the model changes against current navigation properties, keys, and relationship cardinality.
3. Update the EF model layer first:
   - entity properties
   - required/optional fields
   - navigation properties
   - foreign keys
   - enum mappings or value conversions if needed
4. Update `DbContext` and Fluent configuration:
   - add or adjust `DbSet` properties
   - configure relationships, constraints, delete behavior, and indexes
   - verify table and column names if the schema is already established
5. Migrate mock data into EF-friendly seed data:
   - preserve realistic task states, categories, users, comments, attachments, and histories
   - keep IDs and relationships deterministic when possible
   - seed enough variation to cover normal, empty, and edge-case scenarios
6. Replace repository logic carefully:
   - keep the existing repository interface stable when possible
   - move read/write logic from the mock repository to the EF repository
   - use asynchronous EF queries and eager loading where the UI needs related data
7. Update dependency injection so the app uses the EF repository implementation.
8. Run the narrowest validation available:
   - compile the touched project
   - check for missing navigation properties or type mismatches
   - verify migrations or seed data compile cleanly
9. Confirm completion only when the domain model, repository, and seeded data all agree.

## Migration Commands
Use these commands when the schema needs to change:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
dotnet ef migrations remove
dotnet ef migrations list
```

Follow this order:
1. Update entities and `DbContext`.
2. Generate the migration.
3. Review the generated migration for relationship and column changes.
4. Apply the migration only after the model matches the intended schema.
5. If the migration is wrong, remove it before adding the corrected version.

## Seed-Data Generation
When mock repository data is being moved into EF seed data:
1. Start from the mock repository dataset as the source of truth for coverage.
2. Map each mock record to an entity instance with stable IDs where practical.
3. Seed principal entities first, then dependent entities.
4. Keep foreign keys aligned with the seeded principals.
5. Preserve the same scenario mix the mock data provided:
   - populated lists
   - empty states
   - overdue or due-soon records
   - completed and pending records
   - long text and optional-field edge cases
6. Prefer deterministic seed values so the UI and tests remain repeatable.
7. If the model changes, regenerate the seed data so it matches the new schema before running the app.

## Decision Rules
- If the change is only a property rename, update the entity, the repository, and every query or projection that depends on that property.
- If a relationship changes, update both sides of the navigation and the FK configuration before touching seed data.
- If mock data is being migrated, treat the mock repository as the source of domain coverage, not as production logic.
- If the EF repository exposes data used by views, verify includes or projections so null navigation properties do not break rendering.
- If the schema is already in use, prefer non-breaking changes and preserve existing IDs where practical.

## Completion Checks
- All entity classes compile
- `DbContext` reflects the new model
- Repository methods return the same shape the controllers and views expect
- Seeded data covers the same scenarios as the mock repository
- DI points to the EF repository
- No stale references remain to removed properties or relationships

## Useful Checks
- Build the project after every model or repository change
- Inspect model binding and view data for renamed properties
- If migrations are part of the change, verify the generated migration matches the intended schema
- If mock data was replaced, confirm the EF seed data still supports the existing UI states
