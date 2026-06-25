# Waldorf

A command-line directory analysis tool written in C# and .NET.

Waldorf recursively scans directory structures, collects file metadata, generates storage analytics, and identifies duplicate files using SHA-256 content hashing.

---

## Features

### File System Scanning

* Recursive directory traversal
* File metadata collection

  * File name
  * Full path
  * File size
  * File extension
* Configurable ignored directories
* Exception handling for:

  * Unauthorized directories
  * Long file paths
  * I/O errors

### Analytics

* Total file count
* Total storage usage
* Average file size
* Standard deviation of file sizes
* File type distribution analysis
* Percentage breakdown by extension

### Duplicate Detection

* SHA-256 content hashing
* Content-based duplicate identification
* Duplicate grouping independent of file names

---

## Example Output

```text
Total Files: 406
Total Size: 6650.05 MB
Average Size: 16.38 MB
Standard Deviation: 172.37 MB

.pdf: 158 files (38.92%)
.png: 118 files (29.06%)
.jpeg: 21 files (5.17%)
.docx: 4 files (0.99%)

Duplicate Groups Found: 5

Duplicate Set (2 files):
  /path/to/file1.pdf
  /path/to/file2.pdf
```

---

## Installation

Clone the repository:

```bash
git clone https://github.com/retinxl/waldorf.git
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

* Name
* Path
* Size
* Extension

### DuplicateGroup

Stores files that share the same SHA-256 hash.

### FileScanner

Responsible for:

* Recursive directory traversal
* File discovery
* Metadata collection
* Directory filtering
* Filesystem exception handling

### FileAnalyzer

Responsible for:

* Aggregate statistics
* Standard deviation calculation
* Extension grouping
* Percentage calculations
* SHA-256 hash generation
* Duplicate file detection

---

## Technologies

* C#
* .NET
* LINQ
* System.IO
* SHA-256 Cryptography
* Collections (List, Dictionary, HashSet)

---

## Key Concepts Demonstrated

* Recursive algorithms
* Object-oriented design
* File I/O
* Exception handling
* Statistical analysis
* Hash-based duplicate detection
* Data aggregation and reporting

---

## Author

Bianca Bacchus

B.S. Computer Science, Rensselaer Polytechnic Institute
