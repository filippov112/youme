# Schiza

A tool for working on projects in tandem with neural network assistants.

#### Functionality

The following features are currently implemented:
- Global and local prompt creation settings.
- Conveniently connect required context parts.
- Drag-and-drop links to project files and directories in the request.
- Context-sensitive token counter in the resulting request.
- Preview of the final request after it's built and saved to the clipboard.


---
## Stack

#### Architecture

- MVVM

#### Technology

- *Platform*: .NET 8.0
- *Framework*: WPF
- *Code Editor*: AvalonEdit 6.3.1.120
- *Token Counter*: SharpToken 2.0.4
- *Encoding support*: Ude.NetStandart 1.2.0

---
## Quick start

Dependency recovery:
```bash
dotnet restore
```

Building a solution:
```bash
dotnet build --configuration Release --project Schiza/Schiza.csproj
```

`Schiza\bin\Release` - the compiled application will be here.

---
## Screenshots

<center><image src="assets/1.png"/></center>
<br>
<center><image src="assets/2.png"/></center>


