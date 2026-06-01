[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/uM_GSLJS)
# Food-Drink — NutriBite (食光营养助手)

Final assignment — a .NET MAUI cross-platform mobile app for tracking food and drink nutrition.

## Tech Stack

- .NET 8.0 + MAUI
- XAML UI with warm food-themed styling
- REST API data source via mockapi.io (with local fallback)

## Features

- Browse, search, and view nutrition details for food and drink items
- Add new records with input validation
- Camera photo capture
- Geolocation with reverse geocoding
- Text-to-speech nutrition summaries
- Vibration and haptic feedback
- Light/dark theme and large text accessibility

## Build

```powershell
# Windows
dotnet build .\FoodDrinkApp\FoodDrinkApp.csproj -f net8.0-windows10.0.19041.0

# Android
dotnet build .\FoodDrinkApp\FoodDrinkApp.csproj -f net8.0-android
```

See `项目开发指南.md` (Chinese) or `FoodDrinkApp/README.md` for detailed documentation.
