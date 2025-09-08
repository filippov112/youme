# PCD (project context descriptor)

Project context parser for neural networks.

---

## 📦 Functionality

- 🔍 Recursive traversal of the project directory
- 🗂 Generating a JSON file with the project structure (`project_structure.json`)
- 🧠 Generating a text file with the contents of the files (`project_content.txt`)
- 🛠 The `struct` command — generating only the structure in `pcd_context.json`
- ✍️ Ability to manually edit `pcd_context.json` and use it as a filter
- 🧾 Support for filtering by extensions or automatic detection of "textuality"
- 🌐 Automatic detection of encodings using Ude
- ✅ Cross-platform (Windows/Linux)

---

## ⚙️ Installation and launch

### 🔧 Requirements
- [.NET SDK 8.0](https://dotnet.microsoft.com/download)

### 🚀 Build

#### Windows:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

#### Linux/macOS:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

---

## 🖥 Usage

### Full mode (gather structure and contents):

```bash
dotnet run -- ".cs,.json"
```

### Structure only (`struct`):

```bash
dotnet run -- struct
```

* This will create a file `pcd_context.json`, reflecting the current project structure
* It can be edited manually, leaving only the necessary directories and files

### Example of the `pcd_context.json` structure:

```json
{
".": ["Program.cs", "README.md"],
"Utils": {
".": ["Helpers.cs"]
}
}
```

If the `pcd_context.json` file exists, the program uses it as a filter.

---

## 📄 Output files

| File | Purpose |
| ------------------------ | ----------------------------------------- |
| `project_structure.json` | Project structure (taking into account filtering) |
| `project_content.txt` | Contents of files from the structure |
| `pcd_context.json` | (optional) manually defined structure |

---

## 🛠 Technologies

* Language: C# (.NET 6/7/8)
* Libraries:

* `Ude.NetStandard` — for encoding detection
* Architecture:

* `StructureBuilder.cs` — structure logic
* `ContentBuilder.cs` — content logic
* `EncodingHelper.cs` — encoding detection and filtering

---

## 🗂 Result example

### `project_structure.json`

```json
{
".": ["Program.cs"],
"Utils": {
".": ["Helpers.cs"]
}
}
```

### `project_content.txt`

````
File: Program.cs
```
using System; 

class Program { 
static void Main() { 
Console.WriteLine("Hello, world!"); 
} 
} 
```
````

---
[LICENSE](LICENSE)