# ELaunch

**Launch Microsoft Edge profiles in one click.**
ELaunch is an open-source profile launcher for Microsoft Edge on Windows.

**Why ELaunch?**
- Switching Edge profiles manually takes too many steps: opening the browser, clicking your avatar, and selecting a profile from the dropdown menu.
- ELaunch cuts out the middleman—launch directly into any Edge profile straight from your desktop in a single click.
- ELaunch shows all your profiles as cards with their icons and lets you launch any profile instantly — or browse as a guest.
- Built for Multi-Profile Users: The fastest way to organize and switch between Work, Personal, Family, and test profiles.

---

## # Index
- [Features](#-features)
- [Installation and Download](#-how-to-install)
- [Contribute](#-how-to-contribute)
- [Requirements](#-requirements)
- [Troubleshooting](#-troubleshooting)
- [License & Legal](#-license--legal)
- [Privacy Policy](#-privacy-policy)
- [Links & Contact](#-links)

---

## # Features
- **Profile Launcher:** See every local Microsoft Edge profile as a card with its avatar, and launch it with a single click.
- **Guest Mode:** Browse as a guest without touching any of your saved profiles.
- **Edge Detection:** Automatically detects Microsoft Edge and shows a "Download Edge For Windows" fallback screen when it isn't installed.
- **Themes:** Light, dark, and system-follow themes that match your Windows setting.
- **100% Local & Private:** No data leaves your device — nothing is tracked or uploaded.
- **In-App Settings:** Theme switching, privacy policy, open-source licenses, and about screen, all built in.
- **In-App Feedback:** A built-in feedback page lets you reach the developer without leaving the app.

---

## # How to install

> **Download:** [Get the latest release here](https://github.com/SubhamSathua/elaunch/releases)

1.  **Download & Extract:** Unzip the latest release into a permanent folder on your PC.
2.  **Run:** Double-click `ELaunch.exe` to open the launcher.
3.  **Pick a profile:** Select the Microsoft Edge profile you want to open — or choose "Browse as Guest".
4.  **Enjoy:** Your chosen profile opens in Microsoft Edge right away.

> **CAUTION:** ELaunch requires Microsoft Edge to be installed on your PC. If it's missing, the app shows a download button that links to the official Microsoft Edge download page.

---

## # How to contribute
We welcome contributions! Follow these steps to set up your local development environment:

1.  **Clone the Repo:** `https://github.com/SubhamSathua/elaunch.git`
2.  **Build:** Open a terminal in the project folder and run:
    ```bash
    dotnet build ELaunch\ELaunch.csproj
    ```
3.  **Run:**
    ```bash
    dotnet run --project ELaunch\ELaunch.csproj
    ```
    Or build then launch the exe directly:
    ```bash
    dotnet build ELaunch\ELaunch.csproj; & "ELaunch\bin\Debug\net8.0-windows\ELaunch.exe"
    ```
4.  **Publish** (optional) — portable or self-contained builds:
    ```bash
    dotnet publish ELaunch\ELaunch.csproj -c Release -r win-x64 --self-contained false -o publish\portable\win-x64
    dotnet publish ELaunch\ELaunch.csproj -c Release -r win-x64 --self-contained true -o publish\installer\win-x64
    ```

---

## # Requirements
- **OS:** Windows 10 or 11 (64-bit).
- **Browser:** Microsoft Edge installed on the same PC (required to launch profiles).
- **Runtime:** No .NET runtime needed for self-contained builds; portable builds require the .NET 8 Desktop Runtime.
- **Build:** .NET 8 SDK (only if building from source).

---

## # Troubleshooting
- **"Download Edge For Windows" screen:** Microsoft Edge is not installed — click the button to install it from the official Microsoft page, then restart ELaunch.
- **Profile not listed:** ELaunch reads profiles from your local Microsoft Edge installation; make sure Edge is installed and you have created at least one profile.
- **Profile won't launch:** Make sure Edge is set as a default and your profile data isn't corrupted — try opening the same profile directly in Edge first.

---

## # License & Legal
This project is licensed under the **Apache License 2.0**.

**Liability Protection:** The author provides this software "as is" without warranties. By using this software, you agree that the author is not liable for any damages, data loss, or system issues resulting from its use.

**Modifications:** If you modify and distribute this software, you must:
1.  Retain all original copyright notices.
2.  Include a copy of the Apache License.
3.  Protect the original author from any liability claims arising from your modified version.

**Third-Party Licenses:** The app uses [Fluent System Icons](https://github.com/microsoft/fluentui-system-icons) (MIT License) and the WebView2 runtime. See the in-app "Open Source Licenses" section for details.

---

## # Privacy Policy
- **100% Local:** All profile reading and launching happens strictly on your device.
- **No Internet Required:** The app does not send any data to external servers or the cloud.
- **No Tracking:** Zero telemetry, zero analytics, and zero background tracking services.
- **Read-Only:** Local Microsoft Edge profile names and avatars are read locally and never transmitted, stored, or cached by ELaunch.

---

## # Links
- [Report an Issue](https://github.com/SubhamSathua/elaunch/issues) - If you find a bug, please report it here.
- [Security Policy](SECURITY.md) - For reporting security vulnerabilities.
- [Apache 2.0 License](LICENSE)

---

## # Contact
- **Author:** Subham Kumar Sathua
- **GitHub:** [@SubhamSathua](https://github.com/SubhamSathua)
- **Email:** [hyper.devstudio@protonmail.com](mailto:hyper.devstudio@protonmail.com)

---

Copyright © 2026 Subham Kumar Sathua. Licensed under the Apache License 2.0.
