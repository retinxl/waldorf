using System; //for basic stuff like Exception
using System.Collections.Generic; //for data structures
using System.Configuration.Assemblies;
using System.IO;
using System.Runtime.CompilerServices; //for directory file tools
using System.Security.Cryptography;
using System.Threading.Tasks;

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
//convert to bytes
double mb = 1024 * 1024;
Console.WriteLine($"Total Size: {stats.TotalSizeBytes/mb:F2} MB");
Console.WriteLine($"Average Size: {stats.AverageFileSize/mb:F2} MB");
Console.WriteLine($"Standard Deviation: {stats.StandardDeviation/mb:F2} MB");
FileAnalyzer.PrintExtensionBreakdown(files);
List<DuplicateGroup> duplicates =
    FileAnalyzer.FindDuplicates(files);

Console.WriteLine($"\nDuplicate Groups Found: {duplicates.Count}");

foreach (DuplicateGroup group in duplicates)
{
    Console.WriteLine(
        $"\nDuplicate Set ({group.Files.Count} files):");

    foreach (FileMetadata file in group.Files)
    {
        Console.WriteLine($"  {file.Path}");
    }
}


public class DuplicateGroup
{
    public string Hash { get; set; } = "";
    public List<FileMetadata> Files { get; set;} = new();
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
    public double StandardDeviation {get; set;}
}
public class ExtensionStatistics
{
    public string Extension {get; set;} = "";
    public int Count {get; set;}
    public double Percentage {get; set;}
    public double StandardDeviation {get; set;}
}

public class FileScanner
{
    private static HashSet<string> ignoredDirectories = new HashSet<string>
    {
        ".git",
        ".vscode",
        "node_modules",
        ".DS_Store",
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
                // FileMetadata is a wrapper on top of FileInfo to allow for later extendability if desired
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
                // ignore .scriv for this version of the project bc its not a file its a weird
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
        stats.AverageFileSize = CalculateAvg(files);
        stats.StandardDeviation = CalculateStdDev(files, stats.AverageFileSize);
        return stats;
    }

    private static double CalculateAvg(List<FileMetadata> files)
    {
        long totalSize = 0;
        long totalCount = 0;
        double avg = 0;
        foreach (FileMetadata file in files)
        {
            totalSize += file.Size;
            totalCount++;
        }

        if (totalSize > 0)
        {
            avg = (double)totalSize / totalCount;
        }

        return avg;
    }

    private static double CalculateStdDev(List<FileMetadata> files, double average)
    {
        if (files.Count == 0)
        {
            return 0;
        }

        double squareTotal = 0;
        foreach (FileMetadata file in files)
        {
            double difference = file.Size - average;
            squareTotal += difference * difference;
        }
        double populationVariance = squareTotal/files.Count;
        double stdDev = Math.Sqrt(populationVariance);
        return stdDev;
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

    private static string GetFileHash(string filePath)
    {
        using var fileStream =
            new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        byte[] hashBytes = SHA256.HashData(fileStream);

        return Convert.ToHexString(hashBytes);
    }

    public static List<DuplicateGroup> FindDuplicates(List<FileMetadata> files)
    {
        Dictionary<string, List<FileMetadata>> hashGroups =
            new Dictionary<string, List<FileMetadata>>();

        foreach (FileMetadata file in files)
        {
            string hash = GetFileHash(file.Path);

            if (hashGroups.ContainsKey(hash))
            {
                hashGroups[hash].Add(file);
            }
            else
            {
                hashGroups[hash] = new List<FileMetadata>();
                hashGroups[hash].Add(file);
            }
        }

        List<DuplicateGroup> duplicateGroups =
            new List<DuplicateGroup>();

        foreach (var entry in hashGroups)
        {
            if (entry.Value.Count > 1)
            {
                duplicateGroups.Add(
                    new DuplicateGroup
                    {
                        Hash = entry.Key,
                        Files = entry.Value
                    }
                );
            }
        }

        return duplicateGroups;
    }



}


