# BigFitness 🏋️

A mobile calorie and weight tracking app built with **.NET 9 MAUI Blazor Hybrid**.

## Features

- Daily calorie and macronutrient tracking
- Donut chart showing protein / fat / carb breakdown
- Product database with search
- Food entry journal
- Weight history graph
- Profile with daily calorie goal calculation (BMR / TDEE)

## Download

Get the latest Android APK from the [Releases](../../releases/latest) page.

> Enable "Install from unknown sources" in your Android settings before installing.

## Build from Source

**Requirements:**
- .NET 9 SDK (9.0.311+)
- MAUI workload: `dotnet workload install maui`
- Android workload: `dotnet workload install android`

**Windows:**
```bash
git clone https://github.com/YOUR_USERNAME/BigFitnes.git
cd BigFitnes
dotnet build BigFitnes/BigFitnes.csproj -f net9.0-windows10.0.19041.0
```

**Android APK:**
```bash
dotnet publish BigFitnes/BigFitnes.csproj -f net9.0-android -c Release
```

## Tech Stack

| Component | Technology |
|---|---|
| UI | .NET MAUI + Blazor Hybrid |
| Database | SQLite + Entity Framework Core 9 |
| Charts | Blazor-ApexCharts |
| Platforms | Android 7.0+, Windows 10 |
