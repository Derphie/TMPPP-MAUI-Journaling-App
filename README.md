# Reflecta — AI Journaling App

Dark, calm journaling companion built with **.NET 8 MAUI** (Android).  
Demonstrates **12 Gang-of-Four design patterns** across all four categories.  
University project for the TMPPP course (Tehnici și Mecanisme de Proiectare a Produsului Program).

## Architecture

```
Reflecta/
├── Models/             Domain entities (JournalEntry hierarchy)
├── Patterns/
│   ├── Creational/     Factory Method · Builder · Abstract Factory · Prototype
│   ├── Structural/     Adapter · Facade · Decorator · Composite
│   └── Behavioral/     Observer · Strategy · State · Command
├── Services/           IAiService (mock + HTTP-ready), notifications, export
├── Repositories/       SQLite-backed journal and chat stores
├── ViewModels/         MVVM with CommunityToolkit.Mvvm
├── Views/              XAML pages + reusable controls (CardView, TagChip, PrimaryButton, MessageBubble)
└── MauiProgram.cs      DI wiring
```

See **[PATTERNS.md](./PATTERNS.md)** for the full pattern → file mapping.

## Build & Run (Android)

### Prerequisites

```bash
# Install .NET 8 + MAUI workload
dotnet workload install maui-android

# Accept Android SDK licences
$ANDROID_SDK_ROOT/cmdline-tools/latest/bin/sdkmanager --licenses
```

### Run on a connected device / emulator

```bash
cd Reflecta
dotnet build -t:Run -f net8.0-android
```

### Build a debug APK

```bash
cd Reflecta
dotnet build -f net8.0-android -c Debug
# APK: bin/Debug/net8.0-android/com.reflecta.journaling-Signed.apk
```

## Key Technologies

| Concern | Library |
|---------|---------|
| UI framework | .NET MAUI 8 |
| MVVM | CommunityToolkit.Mvvm 8.2 |
| Local DB | sqlite-net-pcl 1.9 |
| Charts | Microcharts.Maui 0.9.5 |
| MAUI extras | CommunityToolkit.Maui 7.0 |

## AI Service

`MockAiService` does keyword matching fully offline — good for the demo.  
To wire a real LLM, implement `IAiService` and swap the DI registration in `MauiProgram.cs`:

```csharp
// services.AddSingleton<IAiService, MockAiService>();
services.AddSingleton<IAiService, OpenAiService>(); // your implementation
```

## Design System

- **Background:** `#0A0A0F`
- **Surface:** `#1A1825` with `#29FFFFFF` border
- **Accent gradient:** `#A855F7` → `#7C3AED`
- **Primary text:** `#F5F5F7` · **Muted:** `#8B8B9A`
- **Cards:** 16 px corner radius · **Buttons:** 26 px corner radius

## Gang-of-Four Patterns (12 total)

| Category | Pattern |
|----------|---------|
| Creational | Factory Method, Builder, Abstract Factory, Prototype |
| Structural | Adapter, Facade, Decorator, Composite |
| Behavioral | Observer, Strategy, State, Command |
