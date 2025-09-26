# BASTA! Mainz 2025

Source code und Slides for BASTA! Mainz 2025

- C# 13 and C# 14 - Was gibt es Neues?
- Developing und Debugging Source Generators
- Umbau eines .NET Backends mit .NET Aspire (Workshop)

## Christian Nagel

...ist Buchautor, Microsoft MVP für .NET, Gründer von CN innovation, Softwarearchitekt und -entwickler mit Microsoft-.NET-Technologien. Er arbeitet seit mehr als 25 Jahren als Softwareentwickler auf unterschiedlichsten Plattformen und mit unterschiedlichen Technologien und Sprachen, startete mit PDP 11 und VAX/VMS-Systemen. Seit dem Jahr 2000 arbeitet er an verteilten .NET-Lösungen, wozu er sein Wissen auch in zahlreichen Büchern vermittelt, u. a. im neuesten Buch Professional C# and .NET - 2021 Edition. Seine Schwerpunkte sind heute Microsoft Azure, ASP.NET Core, Web-API, EF Core, WinUI und .NET MAUI.

## Links

- [C# Blog](https://csharp.christiannagel.com)
- [CN innovation](https://www.cninnovation.com)
- [Pragmatic Microservices with C# and Azure](https://github.com/PacktPublishing/Pragmatic-Microservices-with-CSharp-and-Azure/)
- [Expert C#](https://github.com/PacktPublishing/Expert-CSharp-Programming)
- [Codebreaker App - .NET Aspire](https://github.com/codebreakerapp)

## C# 13 und C# 14 - Was gibt es Neues?

Dienstag, 23. September 2025, 10:45 - 11:45 Uhr, Gutenberg 1

C# 13 ist released, und C# 14 steht vor der Tür. In dieser Session lernen Sie neue Features in C# kennen, und erfahren, wie diese mit .NET verwendet werden. Wir behandeln z. B. Erweiterungen von partial members, und wie diese in .NET mit Source Generators genutzt werden, welche Vorteile Runtime Async in Applikationen bringen kann, was hinter dem Span Typ steckt und wie er First-Class wird, und wie Extensions nicht mehr nur mit Methoden möglich sind.

### Slides

[C# 13 und C# 14 - Was gibt es Neues? (PDF)](slides/CSharp14.pdf)

### [C# Samples](csharp)

- Simple
  - Escacpe character game
  - Beep with .NET 8 and 9
  - Implicit index access
  - nameof with unbound generics
- Types
  - Field-backed Properties (WPF)
  - Field-backed with partial properties (WPF with Source Generators)
  - Weak Events
  - Extension blocks
- Span and Memory
  - Ref struct
  - Allows ref struct
  - Params
  - Span conversion
  - Lambda parameter modifiers
- Async
  - lock keyword with Lock type
  - Interceptors
  - Native AOT
  - Runtime Async
  
## Developing und Debugging Source Generators

Mittwoch, 24. September 2025, 17:00 - 18:00 Uhr, Zagrebsaal

Was ist bei der Entwicklung von C# Source Generators zu beachten? Wie funktionieren Source Generators, was ist möglich, was sollte bei der Entwicklung beachtet werden? Anhand spannender Erweiterungen, die die Produktivität in der Entwicklung steigern können, werden wichtige Teile von Source Generators gezeigt, und wie diese debuggt werden können.

### Slides

[Source generators (PDF)](slides/SourceGenerators.pdf)

### [Source generators Samples](sourcegenerators)

- Roslyn (Syntax and semantic model)
- Hello world generator
- Practical source generator
  - Stage 1: DataSource attribute, create a factory of items
  - Stage 2: Add external data source for configuration of factory
  - Stage 3: Cache external data source
  - Stage 4: Cache enhancements
  - Stage 5: Cache optimizations
  - Stage 6: (based on Stage 3) more efficient attribute (marker) search
- C# version: generate different code based on C# version
- Partial events: use weak events with partial members
- Access private members of types (Context type with initalization from JSON data)
- Call-site rewriting (ActivitySource)

## Umbau eines .NET Backends mit .NET Aspire (Workshop)

Freitag, 26. September 2025, 09:00 - 16:30 Uhr, Gutenberg 3

In diesem praxisorientierten Workshop refaktorieren und migrieren wir eine bestehende .NET‑Backend‑Solution auf .NET Aspire. Teilnehmende lernen, wie man Integrationen für Datenbanken, Caching, Authentication und Messaging implementiert, das .NET Aspire Dashboard zur Live‑Beobachtung nutzt und die Lösung nach Azure deployed. Ziel ist ein produktionsfähiger, skalierbarer Microservice‑Stack mit Monitoring, Sicherheit und optimierter Performance.

### Slides

[Umbau eines .NET Backends mit .NET Aspire (PDF)](slides/Aspire2025.pdf)
