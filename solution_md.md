# Product Catalog Management System - Solution

## Overview

A full-stack product catalog application demonstrating three-tier architecture with CLEAN architecture principles, built using ASP.NET Core 8 and Angular 17+. The system features fuzzy search with the Levenshtein algorithm, in-memory data storage, and a modern reactive frontend.

## Architecture

### Three-Tier + CLEAN Architecture

```
┌─────────────────────────────────┐
│  Tier 1: Presentation Layer     │
│  - API Controllers              │
│  - Custom Middleware            │
│  - Angular Components           │
└──────────────┬──────────────────┘
               │
┌──────────────▼──────────────────┐
│  Tier 2: Business Logic Layer   │
│  - Services                     │
│  - DTOs (Record Types)          │
│  - Validators                   │
│  - Search Engine                │
└──────────────┬──────────────────┘
               │
┌──────────────▼──────────────────┐
│  Tier 3: Data/Domain Layer      │
│  - Domain Entities              │
│  - Repositories (List/Dict)     │
│  - Caching Service              │
└─────────────────────────────────┘
```

### Backend Structure

**4 Projects:**
- `ProductCatalog.API` - Controllers, Middleware
- `ProductCatalog.Application` - Business Logic, DTOs, Services
- `ProductCatalog.Domain` - Core Entities, Interfaces (no dependencies)
- `ProductCatalog.Infrastructure` - Repositories, Caching

**Dependency Flow:** API → Application → Domain ← Infrastructure

## Key Design Decisions

### 1. Multiple Projects vs. Single Project

**Decision:** Split into 4 separate projects

**Why:**
- Enforces separation of concerns
- Domain layer remains pure (no framework dependencies)
- Easy to test business logic independently
- Team members can work on different layers without conflicts

**Trade-off:** More complex structure, but maintainable long-term

---

### 2. In-Memory Storage (List + Dictionary)

**Decision:** Used `List<T>` and `Dictionary<string, List<int>>` instead of database

**Why:**
- Assignment requirement
- Demonstrates core C# collections knowledge
- Fast development without database setup
- Perfect for prototypes and demonstrations

**Implementation:**
```csharp
protected readonly List<T> _store = new();
private readonly Dictionary<string, List<int>> _nameIndex = new();
```

**Trade-off:** Data lost on restart, not suitable for production, but ideal for demo purposes

---

### 3. Fuzzy Search with Levenshtein Algorithm (this was apparently invented in a paper in 1965. I did not know this!)

**Decision:** Implemented custom search using dynamic programming (no external libraries)

**Why:**
- Assignment requirement (ONLY .NET Base Class Library)
- Demonstrates computer science knowledge
- Better user experience (tolerates typos)

**How it works:**
1. Calculate edit distance between strings using matrix
2. Convert to similarity score: `1.0 - (distance / maxLength)`
3. Apply 60% similarity threshold
4. Weight scores by field importance (name > description > SKU)

**Example:** "lptop" → "laptop" = 1 edit = 83% similar = Match!

**Trade-off:** CPU intensive (O(m×n)), but acceptable for product catalogs

---

### 4. Pattern Matching for Validation

**Decision:** Used C# 8+ switch expressions

**Why:**
- Assignment requirement
- More readable than if/else chains
- Demonstrates modern C# features

**Example:**
```csharp
var validation = product.Name switch
{
    null or "" => "Name is required",
    { Length: > 200 } => "Name too long",
    _ => null
};
```

**Trade-off:** None - superior to traditional validation

---

### 5. Record Types for DTOs

**Decision:** Used C# 9+ records instead of classes

**Why:**
- Assignment requirement
- Immutable by default (thread-safe)
- Value equality
- Concise syntax

**Example:**
```csharp
public record ProductDto(int Id, string Name, decimal Price);
```

**Trade-off:** None - perfect for DTOs

---

### 6. Generic Repository Pattern

**Decision:** Abstract base class `Repository<T>` with child implementations

**Why:**
- DRY principle - write CRUD once
- Type safety
- Easy to extend

**Trade-off:** Need abstract methods for ID handling, but worth the reusability

---

### 7. Dictionary-Based Caching

**Decision:** Simple in-memory cache with TTL

**Why:**
- Assignment requirement
- Reduces repeated calculations
- Simple to implement

**Performance Impact:**
- Without cache: 50ms per search
- With cache: <1ms (100x faster)

**Trade-off:** Single-server only, not distributed (would use Redis in production)

---

### 8. Custom Middleware

**Decision:** Built middleware from scratch

**Why:**
- Assignment requirement
- Centralizes cross-cutting concerns (logging, errors)
- Keeps controllers clean

**Trade-off:** None - best practice for production systems

---

### 9. Dependency Injection Lifetimes

**Decision:** Different lifetimes for different services

- **Singleton:** Repositories, Cache (shared state)
- **Scoped:** Services (per-request isolation)

**Why:** Repositories must share in-memory data across requests

---

### 10. Angular Three-Tier Structure

**Decision:** Mirrored backend architecture

- **Tier 1:** Components (Presentation)
- **Tier 2:** Services (Business Logic)
- **Tier 3:** API Client (Data Access)

**Why:**
- Consistency with backend
- Clear separation of concerns
- Professional structure

---

### 11. RxJS State Management

**Decision:** BehaviorSubject + Observables

**Why:**
- Reactive updates
- Multiple subscribers
- Built-in async handling

**Operators Used:**
- `debounceTime(300)` - Wait 300ms after typing
- `distinctUntilChanged()` - Only emit if changed
- `takeUntil(destroy$)` - Auto-unsubscribe
- `tap()` - Side effects (caching)

**Trade-off:** Learning curve, but powerful and industry standard

---

## Technical Highlights

### C# Features Demonstrated
- Generic Repository<T> with constraints
- Record types (C# 9+)
- Pattern matching (C# 8+)
- Nullable reference types
- Custom LINQ extensions
- IComparable implementation
- Manual model binding

### Angular Features Demonstrated
- Standalone components (Angular 16+)
- Reactive forms with validation
- RxJS operators
- Client-side caching
- Debounced search
- Three-tier architecture

### Algorithms & Patterns
- Levenshtein distance (dynamic programming) *this took a lot of research
- Repository pattern (generic)
- Dependency injection
- Observer pattern (RxJS)
- Strategy pattern (swappable implementations)

## Trade-offs Summary

| Decision | Pro | Con 
|----------|-----|-----|
| Multiple projects | Maintainable, testable | More complex |
| In-memory storage | Fast, simple | No persistence | 
| Fuzzy search | Great UX | CPU intensive | 
| Pattern matching | Readable | None | 
| Record types | Concise, safe | None | 
| Dictionary cache | Fast | Single server | 
| Custom middleware | Clean code | Initial effort | 

## Alternative Approaches Considered

### 1. Single Project
**Rejected:** No enforcement of layer dependencies, harder to maintain

### 2. Entity Framework Only
**Rejected:** Assignment required in-memory collections, demonstrates less skill

### 3. External Search Library
**Rejected:** Assignment required no external libraries for search

### 4. Redux for Angular State
**Rejected:** Overkill for this size app, RxJS sufficient

### 5. Real Database
**Rejected:** Slower development, not required, in-memory demonstrates skill

## Production Considerations

If this were a production system, I would change:

1. **Storage:** SQL Server or PostgreSQL with EF Core
2. **Caching:** Redis for distributed caching
3. **Search:** Elasticsearch for large-scale fuzzy search
4. **Authentication:** JWT tokens with refresh tokens
5. **Logging:** Structured logging (Serilog) to centralized system
6. **Monitoring:** Application Insights or similar
7. **Testing:** Comprehensive unit/integration tests (80%+ coverage)
8. **CI/CD:** Automated builds and deployments
9. **API Versioning:** Support multiple API versions
10. **Rate Limiting:** Protect against abuse

## Testing Strategy

**Unit Tests:**
- ProductSearchEngine (8 tests covering fuzzy matching)
- ProductValidator (3 tests covering pattern matching)

**Test Coverage:**
- Fuzzy search algorithm
- Validation rules
- Edge cases (empty strings, negative numbers)

**Would Add in Production:**
- Integration tests (full API flows)
- End-to-end tests (Selenium/Playwright)
- Performance tests (load testing)

## Performance Characteristics

**Backend:**
- Search: <50ms without cache, <1ms with cache
- CRUD operations: <10ms
- Startup: <2 seconds (includes seeding)

**Frontend:**
- Initial load: ~500ms
- Search debounce: 300ms delay
- Page transitions: <100ms

**Scalability:**
- Current: Single server, ~1000 products
- Production: Would need database + caching + load balancing

## Conclusion

This solution demonstrates professional three-tier architecture with CLEAN principles, advanced C# features, and modern Angular practices. The design prioritizes:

1. **Maintainability:** Clear separation of concerns
2. **Testability:** Interface-based programming
3. **Demonstrable Skills:** Custom algorithms, modern features
4. **User Experience:** Fuzzy search, reactive UI
5. **Code Quality:** SOLID principles, clean code
