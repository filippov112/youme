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

For Windows: Place the compiled executable file anywhere on the computer. Add the path to the directory with the file to the "PATH" environment variable. After that, we can call commands directly from the terminal (for "pcd.exe"):

Example of analysis with a filter by file type:
```bash
pcd ".cs,.json"
```

Generating the filter file `pcd_context.json` (also supports selecting file types):
```bash
pcd struct
```

If the scanned directory contains `pcd_context.json`, it is used as a primary filter. It can be edited manually, leaving only the necessary directories and files.

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
