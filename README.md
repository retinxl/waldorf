# Waldorf

A command-line directory analyzer and archiver written in C# and .NET.

Waldorf recursively scans folders, collects file metadata, and generates storage analytics to help users understand the contents of large directory structures.

---

## Features

### Completed

- Recursive directory traversal
- File metadata collection
  - File name
  - Full path
  - File size
  - File extension
- Aggregate statistics
  - Total file count
  - Total storage usage
  - Average file size
- Extension breakdown analysis
- Exception handling for:
  - Unauthorized directories
  - Long file paths
  - I/O errors
- Configurable ignored directories
- Standard deviation analysis of file sizes

### In Progress

- Duplicate file detection using SHA-256 hashing
- ZIP archive generation
- Archive filtering by size and age
- Dry-run mode
- Spectre.Console terminal UI
- Progress bars and formatted reports

---

## Example Output

```text
Total Files: 11
Total Size: 1844.11 MB
Average Size: 167.65 MB
Standard Deviation: 526.74 MB

.pdf: 7 files (63.64%)
.mp4: 1 files (9.09%)
.DS_Store: 1 files (9.09%)
.docx: 2 files (18.18%)
```

---

## Installation

Clone the repository:

```bash
git clone https://github.com/yourusername/waldorf.git
cd waldorf
```

Run the application:

```bash
dotnet run "/path/to/folder"
```

Example:

```bash
dotnet run "/Users/bianca/Documents"
```

---

## Architecture

### FileMetadata

Stores information about each discovered file:

- Name
- Path
- Size
- Extension

### FileScanner

Responsible for:

- Recursive directory traversal
- File discovery
- Metadata collection
- Directory filtering

### FileAnalyzer

Responsible for:

- Aggregate statistics
- Extension grouping
- Percentage calculations
- Future analytics features

---

## Project Roadmap

### Phase 1 — File System Exploration ✅

- [x] Recursive directory traversal
- [x] Metadata collection
- [x] Directory filtering
- [x] Exception handling

### Phase 2 — Analytics Engine 🚧

- [x] Total file count
- [x] Total size
- [x] Average file size
- [x] Extension breakdown
- [ ] Standard deviation
- [ ] Duplicate detection

### Phase 3 — Archiving Pipeline

- [ ] ZIP archive generation
- [ ] Conditional filtering
- [ ] Dry-run mode

### Phase 4 — CLI UI

- [ ] Spectre.Console integration
- [ ] Analytics tables
- [ ] Progress bars

---

## Technologies

- C#
- .NET 8
- LINQ
- System.IO
- SHA-256 Cryptography
- Spectre.Console (planned)

---

## Author

**Bianca Bacchus**

B.S. Computer Science, Rensselaer Polytechnic Institute