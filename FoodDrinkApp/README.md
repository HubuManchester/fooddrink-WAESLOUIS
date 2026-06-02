# NutriBite

NutriBite is a .NET MAUI "Food and Drink" coursework app. It allows recording food and drink items, displaying nutrition summaries, validating user input, and demonstrating mobile device hardware capabilities.

## Key Features

- Food and drink list with search and detail pages.
- Add-record form with required-field and nutrition-value validation.
- Take food photos with the camera and preview them.
- Record dining or purchase location using geolocation.
- Read nutrition summaries and help content aloud with text-to-speech.
- Provide operation alerts with vibration and haptic feedback.
- Support theme switching and large-text mode.
- Include semantic labels, screen reader announcements, and clear validation prompts.

## Grading Criteria Coverage

- UI/UX and Accessibility: XAML pages, bottom navigation, consistent visual style, dark mode, semantic descriptions, and screen reader announcements.
- Mobile Hardware: Camera, location, text-to-speech, vibration, and haptic feedback.
- Functional Completeness: List, search, add, detail, settings, and hardware demo flows.
- Validation and Error Handling: Required-field checks, numeric checks, permission errors, and hardware-unavailable prompts.
- Code Quality: Model and service separation, clear naming, reusable catalog service, and well-scoped page code.
- Deployment: .NET MAUI cross-platform app targeting Android and Windows.
- GitHub Usage: Commit regularly, e.g. `add food list`, `implement hardware page`, `add input validation`.

## How to Run

Open `FoodDrinkApp.csproj` or `FoodDrinkApp.sln` in Visual Studio 2022 with .NET MAUI workloads installed.

Recommended demo targets:

- Android Emulator
- Windows Machine

Windows build command:

```powershell
dotnet build .\FoodDrinkApp.csproj -f net8.0-windows10.0.19041.0
```

Android build command:

```powershell
dotnet build .\FoodDrinkApp.csproj -f net8.0-android
```

The project uses `Directory.Build.props` to redirect build output to `C:\MauiBuild\FoodDrinkApp\`, which avoids Android packaging tool issues with certain path characters.

The project locks the .NET SDK version to 8.0.421 via `global.json` to ensure a consistent build environment.

## Screencast Demo Checklist

- Explain the "Food and Drink" theme and the "NutriBite" app concept.
- Demonstrate search, detail page, and adding a new record.
- Demonstrate validation prompts when required fields are empty or invalid numbers are entered.
- Demonstrate camera, location, text-to-speech, vibration, and haptic feedback.
- Show dark mode and large-text mode.
- Show key code files: models, services, pages, and Android permission configuration.
- Show Android and Windows deployment results.
- Show GitHub commit history and README.
