using System; //for basic stuff like Exception
using System.Collections.Generic; //for data structures
using System.Configuration.Assemblies;
using System.IO;
using System.Runtime.CompilerServices; //for directory file tools

//get user input if not given
if (args.Length == 0)
{
    Console.WriteLine("Please provide a folder path.");
    return;
}

string rootPath = args[0];
List<FileMetadata> files = FileScanner.GetAllFiles(rootPath);
FileStatistics stats = FileAnalyzer.CalculateStatistics(files);

Console.WriteLine($"Total Files: {stats.TotalCount}");
Console.WriteLine($"Total Size: {stats.TotalSizeBytes}");
Console.WriteLine($"Average Size: {stats.AverageFileSize}");

FileAnalyzer.PrintExtensionBreakdown(files);
//print size of files
/*foreach (FileMetadata file in files)
{
    Console.WriteLine($"{file.Name}: {file.Size} bytes");
}*/

//print each file here:
/*foreach (FileMetadata file in files)
{
    Console.WriteLine(file.Path);
}*/

public class ExtensionStatistics
{
    public string Extension {get; set;} = "";
    public int Count {get; set;}
    public double Percentage {get; set;}
}

public class FileMetadata
{
    public string Name {get; set;} = "";
    public string Path {get; set;} = "";
    public long Size {get; set;}
    public string Extension {get; set;} = "";
}

public class FileStatistics
{
    public int TotalCount {get; set;}
    public long TotalSizeBytes {get; set;}
    public double AverageFileSize {get; set;}
}

public class FileAnalyzer
{
    public static FileStatistics CalculateStatistics(List<FileMetadata> files)
    {
        FileStatistics stats = new FileStatistics();
        stats.TotalCount = files.Count;
        long totalSize = 0;
        foreach (FileMetadata file in files)
        {
            totalSize += file.Size;
        }

        stats.TotalSizeBytes = totalSize;
        if (stats.TotalCount > 0)
        {
            stats.AverageFileSize = (double)totalSize / stats.TotalCount;
        }
        return stats;
    }

    public static void PrintExtensionBreakdown(List<FileMetadata> files)
    {
        int totalFiles = files.Count;
        foreach (var group in files.GroupBy(file => file.Extension))
        {
            //print each file in each extension
            /*Console.WriteLine($"\nExtension: {group.Key}");
            Console.WriteLine($"Count: {group.Count()}");

            foreach (var file in group)
            {
                Console.WriteLine($"{file.Name}");
            }*/
            double percentage = 
                (double)group.Count() / totalFiles * 100;


            Console.WriteLine(
                $"{group.Key}: {group.Count()} files ({percentage:F2}%)"
            );
        }
    }    
}

public class FileScanner
{
    private static HashSet<string> ignoredDirectories = new HashSet<string>
    {
        ".git",
        ".vscode",
        "node_modules",
        "bin",
        "obj",
        ".scriv",
        "venv",
        ".venv"
    };

    public static List<FileMetadata> GetAllFiles(string root_Path)
    {
        List<FileMetadata> files = new List<FileMetadata>();
        TraverseDirectory(root_Path, files);
        return files;
    }

    private static void TraverseDirectory(string path, List<FileMetadata> files)
    {
        try
        {
            // 1. Get files in current directory
            string[] currentFiles = Directory.GetFiles(path);

            foreach (string filePath in currentFiles)
            {
                FileInfo info = new FileInfo(filePath);
                FileMetadata file = new FileMetadata
                {
                    Name = info.Name,
                    Path = info.FullName,
                    Size = info.Length,
                    Extension = info.Extension
                };
                files.Add(file);
            }

            //2. Get subdirectories
            string[] subDirectories = Directory.GetDirectories(path);

            //3. Recurse into each subdirectory
            foreach (string dir in subDirectories)
            {
                //filter ignored folders
                string dirName = Path.GetFileName(dir);
                // ignore .scriv for this version of the project
                if (dirName.EndsWith(".scriv") || ignoredDirectories.Contains(dirName))
                {
                    continue;
                }

                TraverseDirectory(dir, files);
            }
        }

        catch (UnauthorizedAccessException)
        {
            //Skip folders we don't have permission to access
            return;
        }
        catch (PathTooLongException)
        {
            //Skip overly long paths
            return;
        }
        catch (IOException)
        {
            //Skip corrupted/inaccessible directories
            return;
        }
    }
}