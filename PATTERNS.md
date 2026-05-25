# Reflecta — Gang-of-Four Design Patterns

All 12 patterns are functionally integrated.  Each key class carries an XML-doc
comment that names the pattern and explains why it was chosen.

---

## Creational

| # | Pattern | File(s) | One-line justification |
|---|---------|---------|------------------------|
| 1 | **Factory Method** | `Patterns/Creational/EntryFactory.cs` | Abstract `EntryFactory.CreateEntry()` lets `TextEntryFactory`, `MoodEntryFactory`, etc. produce different `JournalEntry` subtypes without the caller knowing the concrete class. |
| 2 | **Builder** | `Patterns/Creational/JournalEntryBuilder.cs` | Fluent step-by-step assembly of a `JournalEntry` (title → body → tags → mood → metadata) prevents telescoping constructor anti-patterns and is reused by both the UI flow and Prototype templates. |
| 3 | **Abstract Factory** | `Patterns/Creational/IServiceAbstractFactory.cs` | `IServiceAbstractFactory` groups platform-specific services (`INotificationService`, `IShareService`) into a family; `AndroidServiceFactory` vs `IosServiceFactory` are swapped at DI registration time based on `DeviceInfo.Platform`. |
| 4 | **Prototype** | `Patterns/Creational/EntryPrototype.cs` | `IEntryPrototype.Clone()` deep-copies pre-configured templates (`DailyReflectionTemplate`, `GratitudeTemplate`, etc.) so the user gets a fully pre-filled entry without triggering the full factory chain. |

---

## Structural

| # | Pattern | File(s) | One-line justification |
|---|---------|---------|------------------------|
| 5 | **Adapter** | `Patterns/Structural/MoodAnalyticsAdapter.cs` | `MoodAnalyticsAdapter` wraps `ExternalMoodApiClient` (which returns `ExternalMoodApiResponse` DTOs) and exposes the internal `IMoodAnalyzer` interface, shielding the rest of the app from the external format. |
| 6 | **Facade** | `Patterns/Structural/ReflectaFacade.cs` | `ReflectaFacade` provides four coarse-grained operations (`SaveEntryAsync`, `GetWeeklySummaryAsync`, `ExportJournalAsync`, `ScheduleReminderAsync`) so ViewModels are decoupled from repositories, AI, notifications, and analytics. |
| 7 | **Decorator** | `Patterns/Structural/EntryDecorator.cs` | `EntryDecorator` base wraps `IEntry`; `PinnedEntry`, `FavoriteEntry`, and `EncryptedEntry` stack transparently to add a prefix to display titles or mask body text — without modifying `JournalEntry` itself. |
| 8 | **Composite** | `Patterns/Structural/JournalComposite.cs` | `JournalFolder` and `JournalNote` both implement `IJournalComponent`; folders recurse into children for `GetCount()` and `GetTreeDisplay()`, enabling arbitrarily nested journal structures. |

---

## Behavioral

| # | Pattern | File(s) | One-line justification |
|---|---------|---------|------------------------|
| 9 | **Observer** | `Patterns/Behavioral/MoodObserver.cs` + `ViewModels/BaseViewModel.cs` | Explicit `MoodSubject` / `IMoodObserver` pair drives the Summary chart update (`SummaryChartObserver`) and low-mood alert (`MoodAlertObserver`) on every save; `ObservableObject` / `INotifyPropertyChanged` handles MVVM binding. |
| 10 | **Strategy** | `Patterns/Behavioral/MoodAnalysisStrategy.cs` | `IMoodAnalysisStrategy` is satisfied by `SimpleAnalysisStrategy` (keyword rules, offline) or `AIWeightedStrategy` (uses `IAiService`); the Profile page lets the user switch at runtime; the Facade consumes the active strategy. |
| 11 | **State** | `Patterns/Behavioral/EntryState.cs` | `IEntryState` defines `BeginEdit / Save / Archive / Restore`; `DraftState`, `EditingState`, `SavedState`, and `ArchivedState` enforce legal transitions (e.g. archiving a draft throws) and update `JournalEntry.StateName`. |
| 12 | **Command** | `Patterns/Behavioral/EntryCommands.cs` | `SaveEntryCommand`, `DeleteEntryCommand` (with undo — re-inserts the deleted entry), and `ExportCommand` implement `IEntryCommand`; `CommandInvoker` maintains an undo stack and is used by `JournalViewModel`. |

---

---

## v0.3 extensions (Journal actions · Material You · ngrok AI)

| Pattern | Where extended | What changed |
|---------|---------------|--------------|
| **Command** (12) | `EntryCommands.cs` · `JournalViewModel` | `DeleteEntryCommand.UndoAsync` surfaced via CommunityToolkit Snackbar ("Entry deleted — Undo"); `ShowEntryMenuAsync` wraps the existing invoker in a Material bottom-sheet action flow. |
| **Decorator** (7) | `EntryDecorator.cs` · `ReflectaFacade` | `TogglePinAsync` on the facade toggles `JournalEntry.IsPinned` (the flag read by `PinnedEntry` / `EntryDecoratorFactory.Decorate`) without re-running mood analysis; pinned entries float to the top in `GetEntriesAsync`. |
| **Strategy** (10) | `MauiProgram.cs` | When `AppConfig.UseRemoteAi = true`, `AIWeightedStrategy` is registered (instead of `SimpleAnalysisStrategy`) and automatically routes through `HttpAiService` — no changes to the strategy classes themselves. |
| **Facade** (6) | `ReflectaFacade.cs` | Added `TogglePinAsync`, updated `GetEntriesAsync` (pin-first sort), added `GetAiChatResponseAsync` with history — ViewModels still call only the facade. |

> Generated for **Reflecta v0.3** — .NET 9 MAUI, Android primary target.
