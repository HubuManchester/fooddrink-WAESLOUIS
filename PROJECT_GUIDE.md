# NutriBite Project Development Guide

## 1. Project Overview

This project is a .NET MAUI cross-platform mobile app named "NutriBite". The project theme aligns with the Food and Drink requirements in the specification. Core functionality includes recording food and drink items, viewing nutrition information, and enhancing the user experience with mobile device hardware.

Key capabilities:

- Browse food and drink lists
- Search by food, drink, category, or tags
- Add new food or drink records
- View nutrition details
- Capture food photos
- Get dining location including country, city, region, latitude, and longitude
- Text-to-speech for nutrition summaries or help content
- Stop speech to prevent playback after leaving a page
- Vibration and haptic feedback
- Large text mode, dark/light themes, and screen reader announcements

## 2. Requirements Checklist

| Criterion | Weight | Implementation Status |
|---|---:|---|
| UI/UX Design and Accessibility | 30% | Multi-page XAML UI; warm food-themed colour palette; bottom tab navigation; dark/light themes; large text mode; semantic hints; screen reader announcements. |
| Use of Mobile Hardware | 20% | Camera, Location/Geolocation, Geocoding, Text-to-speech, Vibration, Haptic feedback — exceeding the high-band threshold of 4 hardware capabilities. |
| Functionality | 20% | Food list, search, detail view, add record, hardware demo, settings page, speech stop, location display, and more. |
| Validation and Error Handling | 10% | Add-item page validates required fields and non-negative numbers; camera, location, TTS, and vibration all include exception handling and user prompts. |
| Code Quality | 10% | Models, Services, Pages/code-behind layering; shared services for speech, font scaling, and food data; clear naming. |
| Deployment | 5% | Supports Android and Windows; verified via command-line builds. |
| GitHub Usage | 5% | Project includes README and this development guide; meaningful commit history should be maintained on GitHub. |

Conclusion: The project meets the core requirements of the coursework. Note that final grading primarily depends on the screencast, so each feature must be demonstrated clearly during recording.

## 3. Project Structure

```text
FoodDrinkApp/
  App.xaml
  App.xaml.cs
  AppShell.xaml
  AppShell.xaml.cs
  MainPage.xaml
  MainPage.xaml.cs
  AddItemPage.xaml
  AddItemPage.xaml.cs
  FoodDetailPage.xaml
  FoodDetailPage.xaml.cs
  HardwarePage.xaml
  HardwarePage.xaml.cs
  SettingsPage.xaml
  SettingsPage.xaml.cs
  Models/
    FoodItem.cs
  Services/
    AccessibilityService.cs
    FoodCatalogService.cs
    SpeechService.cs
  Platforms/
    Android/
      AndroidManifest.xml
  Resources/
    Styles/
      Colors.xaml
      Styles.xaml
```

## 4. Key File Explanations

### App.xaml / App.xaml.cs

`App.xaml` loads global resource dictionaries including colours and control styles.

`App.xaml.cs` is one of the application entry points. It creates the main window and loads `AppShell`:

```csharp
return new Window(new AppShell());
```

### AppShell.xaml / AppShell.xaml.cs

`AppShell.xaml` defines the app navigation structure. It currently uses a bottom TabBar:

- Food
- Hardware
- Settings

`AppShell.xaml.cs` registers the detail page and add-item page routes:

```csharp
Routing.RegisterRoute(nameof(AddItemPage), typeof(AddItemPage));
Routing.RegisterRoute(nameof(FoodDetailPage), typeof(FoodDetailPage));
```

This allows the main page to navigate to the add-item and detail pages via Shell.

### MainPage.xaml / MainPage.xaml.cs

The main page displays the food and drink list.

Key controls:

- `SearchBar` — search food, drink, category, or tags
- `CollectionView` — display food cards
- `Button` — navigate to add-item or detail pages
- `RefreshView` — pull-to-refresh the list

Core logic:

```csharp
FoodCollection.ItemsSource = FoodCatalogService.Search(query);
```

The main page does not maintain data directly; it retrieves the list via `FoodCatalogService`, keeping the code cleaner.

### AddItemPage.xaml / AddItemPage.xaml.cs

The add-item page lets users create a new food or drink record.

Form fields include:

- Name
- Category
- Description
- Calories
- Protein
- Carbs
- Fat
- Allergy note

Validation logic is in `ValidateForm`:

- Name must not be empty
- Category must be selected
- Description must not be empty
- Calories, protein, carbs, and fat must be non-negative numbers

If validation fails, an error panel is shown and vibration feedback is triggered:

```csharp
ShowValidation(validationMessage);
Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
```

On successful save:

```csharp
FoodCatalogService.Add(new FoodItem { ... });
```

### FoodDetailPage.xaml / FoodDetailPage.xaml.cs

The detail page displays nutrition information for a single food or drink item.

Displayed content includes:

- Name
- Category
- Calories
- Protein, carbs, fat
- Description
- Allergy note

The detail page supports:

- Read nutrition summary aloud
- Stop reading
- Vibration alert

Speech uses `SpeechService`:

```csharp
await SpeechService.SpeakAsync(currentItem.AccessibleSummary);
```

Speech is automatically stopped when leaving the page:

```csharp
protected override void OnDisappearing()
{
    SpeechService.Stop();
    base.OnDisappearing();
}
```

This prevents speech from continuing after the user navigates away.

### HardwarePage.xaml / HardwarePage.xaml.cs

The hardware page demonstrates mobile device hardware capabilities.

Implemented hardware features:

| Feature | API |
|---|---|
| Photo capture | `MediaPicker.Default.CapturePhotoAsync()` |
| Location | `Geolocation.Default.GetLocationAsync()` |
| Geocoding | `Geocoding.Default.GetPlacemarksAsync()` |
| Text-to-speech | `TextToSpeech.Default.SpeakAsync()`, wrapped in `SpeechService` |
| Vibration | `Vibration.Default.Vibrate()` |
| Haptic feedback | `HapticFeedback.Default.Perform()` |

Location logic:

```csharp
var location = await Geolocation.Default.GetLocationAsync(request);
CoordinateLabel.Text = $"Latitude {location.Latitude:F5}, longitude {location.Longitude:F5}";
LocationLabel.Text = await BuildAddressTextAsync(location);
```

Geocoding prioritises system reverse geocoding. If the emulator does not return a country or city, fallback logic is used. For example, the default Google emulator coordinates `37.422, -122.084` will display:

```text
United States / California / Mountain View
```

Haptic feedback includes a counter for screencast verification:

```csharp
feedbackTestCount++;
FeedbackCountLabel.Text = $"Haptic feedback tests: {feedbackTestCount}";
```

### SettingsPage.xaml / SettingsPage.xaml.cs

The settings page handles accessibility-related features.

Currently supported:

- Follow system theme
- Light theme
- Dark theme
- Large text mode

Large text mode is implemented via `AccessibilityService`. When the switch is toggled on, text on the current page is immediately enlarged. When navigating to other pages, font scaling is applied in each page's `OnAppearing`.

```csharp
AccessibilityService.LargeTextEnabled = e.Value;
AccessibilityService.ApplyFontScale(this);
```

### Models/FoodItem.cs

`FoodItem` is the food/drink data model.

Key properties:

- `Name`
- `Category`
- `Description`
- `Calories`
- `Protein`
- `Carbs`
- `Fat`
- `AllergyNote`
- `Tags`

It also provides computed properties for UI display and speech:

```csharp
public string CaloriesLabel => $"{Calories} kcal";
public string MacroSummary => $"Protein {Protein}g, carbs {Carbs}g, fat {Fat}g";
public string AccessibleSummary => $"{Name}. {Category}. {Calories} kcal. {MacroSummary}. {AllergyNote}";
```

### Services/FoodCatalogService.cs

The food data service reads food information from mockapi.io or local fallback data:

- Prioritises mockapi.io REST API
- Uses local fallback data when the API is not configured or the network is unavailable
- Search food items
- Get details by ID
- Add new food records

Search matches against:

- Name
- Category
- Description
- Tags

The mockapi.io endpoint URL is configured in:

```text
FoodDrinkApp/Services/MockApiConfig.cs
```

Simply change `EndpointUrl` to the Resource address generated by mockapi.io:

```csharp
public const string EndpointUrl = "https://682xxxx.mockapi.io/api/v1/foods";
```

See `MOCKAPI_SETUP.md` in the project root for detailed configuration steps.

### Services/SpeechService.cs

The speech service handles Text-to-speech uniformly.

Features include:

- Stop previous speech before starting a new utterance
- Support for stopping speech on demand
- Select an English voice locale
- Set volume and pitch for slightly more natural output

Core code:

```csharp
var options = new SpeechOptions
{
    Volume = 0.9f,
    Pitch = 1.05f,
    Locale = await FindEnglishLocaleAsync()
};
```

Note: Speech quality depends on the TTS voice packages installed on the device. Physical devices are usually more natural than Android emulators.

### Services/AccessibilityService.cs

The accessibility font scaling service.

It traverses Label, Button, Entry, Editor, Picker, SearchBar, and other controls on the current page, and enlarges or restores fonts based on the large-text toggle.

### Platforms/Android/AndroidManifest.xml

Android permission configuration file.

Currently includes:

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.VIBRATE" />
```

These permissions correspond to camera, location, and vibration functionality.

### Resources/Styles/Styles.xaml

Global control styles.

Currently unified for:

- Button
- Entry
- Editor
- Picker
- SearchBar
- Label
- Shell TabBar

The visual style uses a warm cream background with tomato red, roasted orange, and basil green tones for a food-and-drink-appropriate theme.

## 5. Build and Run

### Environment Requirements

- .NET SDK 8.0 (the project locks the version to 8.0.421 via `global.json`)
- MAUI workloads installed: `dotnet workload install maui-android maui-windows`
- Android builds require the Android SDK (addable via Visual Studio Installer)

Install .NET 8.0 SDK and MAUI workloads:

```powershell
winget install Microsoft.DotNet.SDK.8
dotnet workload install maui-android maui-windows
```

> Note: If the nuget.org source is disabled, enable it first: `dotnet nuget enable source nuget.org`

### Build Output

Since the current project path may contain characters that cause issues with Android build tools, `Directory.Build.props` in the project root redirects build output to:

```text
C:\MauiBuild\FoodDrinkApp\
```

### Windows Build

```powershell
dotnet build .\FoodDrinkApp\FoodDrinkApp.csproj -f net8.0-windows10.0.19041.0
```

### Android Build

```powershell
dotnet build .\FoodDrinkApp\FoodDrinkApp.csproj -f net8.0-android
```

Running on Android:

```powershell
dotnet build .\FoodDrinkApp\FoodDrinkApp.csproj -f net8.0-android -t:Run
```

If Visual Studio says "Cannot start this project for Android", there is usually no online device. Start an Android emulator or connect a physical device first.

If you encounter `XA5300: Cannot find Android SDK directory`, install the Android SDK via Visual Studio Installer, or set the `ANDROID_HOME` environment variable pointing to the SDK path.

## 6. Screencast Demonstration Guide

Recommended recording order:

1. Introduce the app theme: Food and Drink, app name "NutriBite".
2. Show the main page UI, search bar, food cards, and bottom navigation.
3. Search for a food item, e.g. "Drink" or "Breakfast".
4. Open the detail page and show nutrition information.
5. Tap "Read Summary", then "Stop Reading", and explain that speech stops automatically when leaving the page.
6. Tap "Vibration Alert" and explain that Vibration and Haptic feedback are used.
7. Open the add-item page, deliberately leave fields blank or enter invalid numbers, and demonstrate validation and error prompts.
8. Correctly add a food record and return to the main page to show the new entry.
9. Open the hardware page and demonstrate photo capture.
10. Demonstrate location, explaining that country, city, region, latitude, and longitude are displayed.
11. Demonstrate read help and stop reading.
12. Demonstrate haptic feedback and explain that the count change is for screencast verification.
13. Open the settings page and demonstrate dark/light themes and large text mode.
14. Show code structure: Models, Services, page XAML, AndroidManifest permissions.
15. Show Android and Windows build or run results.
16. Show GitHub commit history and README.

## 7. Important Notes

- GitHub usage accounts for 5%; commit frequently rather than in a single batch at the end.
- The screencast is the primary grading source — each criterion must be clearly explained.
- If the emulator cannot display a real city for location, set a Location in the emulator Extended Controls, or test on a physical device.
- TTS voice quality depends on system voice packages. For more natural output, install or select a better English TTS engine on a physical device.
- The project locks .NET SDK to 8.0 via `global.json`. If multiple SDK versions are installed, ensure SDK 8.0 is available.
- Before the first build, confirm nuget.org source is enabled: `dotnet nuget enable source nuget.org`
- The Windows build has been verified in a .NET 8.0 + MAUI 8.0 environment.
