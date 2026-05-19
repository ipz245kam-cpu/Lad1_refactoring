# Programming Principles

## 1. Single Responsibility Principle (SRP)

Each class has one clear responsibility:
- `SudokuSolver` — only solves the board
- `DB` — only handles database connections and queries
- `GlobalUser` — only stores the current user's session data
- `RecordForm` — only displays the leaderboard

Example: `SudokuSolver.cs` contains only solving logic (`Solve`, `IsSafe`, `CountSolutions`), with no UI or database code mixed in.

---

## 2. DRY (Don't Repeat Yourself)

The `FillListView` method in `RecordForm` is defined once and reused for all four difficulty tabs instead of duplicating the same loop four times.

See `RecordForm.cs` — `FillListView` method.

---

## 3. Encapsulation

The `DB` class hides all database connection details behind a clean interface. External code never touches `OdbcConnection` directly — it only calls `Open()`, `ExecuteReader()`, `ExecuteScalar()`, `ExecuteNonQuery()`.

See `DB.cs`.

---

## 4. Dispose Pattern (IDisposable)

The `DB` class correctly implements the full `IDisposable` pattern with a finalizer `~DB()`, a `disposed` flag, and `GC.SuppressFinalize` to prevent double-disposal and ensure database connections are always released.

See `DB.cs` — `Dispose()` and `~DB()` methods.

---

## 5. Separation of Concerns

UI logic is separated from business logic:
- `SudokuSolver` handles solving algorithms with no UI code
- `Form1` handles rendering and user interaction with no solver logic embedded
- `GlobalUser` is a stateless data holder separate from both UI and DB layers

---

## 6. Meaningful Names

Most classes and methods have clear, descriptive names:
- `CountSolutions` — counts how many solutions a board has
- `IsAuthenticated` — property that clearly returns auth status
- `BackgroundWorker_DoWork` / `BackgroundWorker_RunWorkerCompleted` — event names describe their purpose

See `GlobalUser.cs` — `IsAuthenticated` property.
