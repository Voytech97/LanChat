# LanChat — Secure Cross-Platform LAN Messenger

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Avalonia UI](https://img.shields.io/badge/Avalonia_UI-11.0-8A2BE2)](https://avaloniaui.net/)
[![Architecture](https://img.shields.io/badge/Architecture-Client--Server%20%2F%20P2P-blue)](#)

[English](#english) | [Polski](#polski)

---

## English

### Project Overview
**LanChat** is a secure, cross-platform instant messaging system designed for internal local area networks (LAN). Built using **C#**, **.NET**, and the **Avalonia UI** framework, the solution delivers native desktop and mobile support (Windows, Linux, Android) from a unified codebase.

The primary objective of this engineering project is to provide organizations (enterprises, institutions, local offices) with complete digital autonomy and sovereign data governance. By eliminating dependencies on external cloud services, LanChat mitigates third-party data processor risks (simplifying GDPR/RODO compliance), prevents external network leaks, and operates reliably in air-gapped or restricted intranet environments.

### Key Architectural Concepts
* **Zero-Knowledge Relay Server (In-Memory Relay):** The dedicated central server handles user authentication, presence tracking, and dynamic routing. To preserve privacy and security, messages are forwarded strictly in-memory (RAM) and immediately discarded once delivered, ensuring zero persistence on the central node.
* **Decentralized Local Storage:** Full chat histories are stored solely on end-user devices using embedded local databases (e.g., SQLite).
* **Robust Built-in Authentication:** Dedicated, native identity verification service integrated directly into the application server, leveraging industry-standard salted password hashing (Argon2id/BCrypt) and cryptographically signed JWT tokens for secure session management.
* **Multi-Device Session Dispatch:** Outgoing messages targeting an employee are securely multiplexed and delivered across all active client instances associated with their identity.

### Technical Stack
* **Framework & UI:** C#, .NET, Avalonia UI (MVVM Pattern)
* **Platforms:** Windows, Linux, Android
* **Authentication:** JWT (JSON Web Tokens), secure salted password hashing (Argon2id/BCrypt)
* **Local Persistence:** SQLite (local client-side data store)
* **Networking:** TCP Sockets / WebSockets

### Roadmap & Planned Modules
- [x] Cross-platform GUI skeleton (Authentication, Contact List, Direct Messaging)
- [x] Central Server with token-based access control
- [x] In-memory message dispatch engine
- [ ] End-to-End Encryption (E2EE) with client-side key management
- [ ] Direct Peer-to-Peer (P2P) file transfer over LAN
- [ ] Server containerization via Docker / Docker Compose

---

## Polski

### Opis Projektu
**LanChat** to bezpieczny, międzyplatformowy komunikator sieciowy zoptymalizowany pod kątem pracy w lokalnych sieciach korporacyjnych i instytucjonalnych (LAN). Rozwiązanie zostało zaimplementowane w języku **C#** i środowisku **.NET** z wykorzystaniem frameworka **Avalonia UI**, co zapewnia pełną kompatybilność na systemach Windows, Linux oraz Android w oparciu o wspólny kod źródłowy.

Głównym założeniem projektu inżynierskiego jest uniezależnienie komunikacji wewnątrzorganizacyjnej od zewnętrznych serwerów i dostawców chmurowych. Podejście to gwarantuje pełną suwerenność danych, ułatwia spełnienie wymogów RODO poprzez eliminację powierzenia przetwarzania danych podmiotom trzecim, uniemożliwia wycieki do publicznej sieci oraz pozwala na pracę w odizolowanych środowiskach bez dostępu do Internetu.

### Kluczowe Założenia Architektoniczne
* **Serwer pośredniczący typu *Zero-Knowledge* (In-Memory Relay):** Serwer centralny odpowiada wyłącznie za rejestrację, uwierzytelnianie oraz przekazywanie pakietów. Wiadomości są przetwarzane wyłącznie w pamięci operacyjnej (RAM) i natychmiast z niej usuwane po dostarczeniu do adresata.
* **Zdecentralizowana historia rozmów:** Cała historia konwersacji jest utrwalana wyłącznie lokalnie na stacjach roboczych i urządzeniach użytkowników (np. przy użyciu bazy SQLite).
* **Natywny moduł uwierzytelniania:** Dedykowany, wewnętrzny komponent serwera odpowiedzialny za bezpieczną weryfikację tożsamości, oparty na nowoczesnych standardach solonego haszowania haseł (Argon2id/BCrypt) oraz kryptograficznie podpisywanych tokenach sesyjnych JWT.
* **Wielodostęp i współbieżność sesji:** Obsługa wielu urządzeń jednocześnie – pakiety wiadomości są klonowane przez serwer i dostarczane do wszystkich zalogowanych instancji danego pracownika.

### Stos Technologiczny
* **Technologie bazowe:** C#, .NET, Avalonia UI (wzorzec MVVM)
* **Wspierane platformy:** Windows, Linux, Android
* **Bezpieczeństwo i uwierzytelnianie:** JWT (JSON Web Tokens), bezpieczne solone haszowanie haseł (Argon2id/BCrypt)
* **Baza danych:** SQLite (lokalna baza danych klienta)
* **Protokół komunikacji:** TCP / WebSockets

### Harmonogram Rozwoju (Roadmapa)
- [x] Podstawowy międzyplatformowy interfejs klienta (Logowanie, Lista kontaktów, Okno czatu)
- [x] Dedykowana aplikacja serwerowa z autoryzacją opartą o JWT
- [x] Bezstanowe przekazywanie wiadomości w pamięci RAM
- [ ] Pełne szyfrowanie End-to-End (E2EE) po stronie aplikacji klienckich
- [ ] Bezpośredni transfer plików w modelu Peer-to-Peer (P2P) w obrębie sieci LAN
- [ ] Konteneryzacja serwera z bazą danych (Docker / Docker Compose)
