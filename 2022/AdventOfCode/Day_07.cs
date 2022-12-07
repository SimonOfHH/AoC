using MoreLinq.Extensions;

namespace AdventOfCode;

public class Day_07 : BaseDay
{
    private readonly bool Sample = false;
    private readonly Filesystem filesystem;
    public Day_07()
    {
        filesystem = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new(GetDirectoriesBySize(filesystem.Directories, 100000, false).Sum(d => d.Size).ToString());

    public override ValueTask<string> Solve_2() => new(GetDirectoriesBySize(filesystem.Directories, filesystem.SpaceToMakeAvailableForUpdate, true).OrderBy(d => d.Size).First().Size.ToString());

    private Filesystem ParseInput()
    {
        return new Filesystem(this.GetInput(Sample));
    }
    public List<FSDirectory> GetDirectoriesBySize(List<FSDirectory> dirs, int sizeToLookFor, bool minimum)
    {
        var childDirs = new List<FSDirectory>();
        dirs.ForEach(dir => childDirs.AddRange(GetDirectoriesBySize(dir.Directories, sizeToLookFor, minimum)));
        var result = minimum ? dirs.Where(d => d.Size >= sizeToLookFor).ToList() : dirs.Where(d => d.Size <= sizeToLookFor).ToList();
        result.AddRange(childDirs);
        return result;
    }
}

public class Filesystem
{
    public List<FSDirectory> Directories { get; set; }
    public FSDirectory Root => Directories.First();
    public static int TotalDiskspace => 70000000;
    public static int SpaceNeededForUpdate => 30000000;
    public int AvailableDiskspace => TotalDiskspace - Root.Size;
    public int UsedSpace => Root.Size;
    public int SpaceToMakeAvailableForUpdate => SpaceNeededForUpdate - AvailableDiskspace;
    public Filesystem(string[] input)
    {
        Directories = new List<FSDirectory>();
        InitializeFilesystem(input);
    }
    private void InitializeFilesystem(string[] input)
    {
        var currDirectory = new FSDirectory("/", null);
        Directories.Add(currDirectory);
        for (int i = 1; i < input.Length; i++)
        {
            if (IsCommand(input[i]))
            {
                string cmd = input[i].Replace("$", "").Trim();
                if (cmd == "ls")
                {
                    var directoryContent = input.Skip(i + 1).TakeWhile(dc => !dc.StartsWith("$"));
                    foreach (var content in directoryContent)
                    {
                        currDirectory.AddToDirectory(content, currDirectory);
                    }
                    i += directoryContent.Count();
                }
                if (cmd.StartsWith("cd"))
                    currDirectory = ChangeDirectory(cmd, currDirectory);
            }
        }
    }
    private FSDirectory ChangeDirectory(string cmd, FSDirectory currDirectory)
    {
        if (cmd.EndsWith(".."))
        {
            return FindParent(Root, currDirectory);
        }
        else
        {
            var dirName = cmd.Split(" ")[1].Trim();
            if (dirName == "/")
                currDirectory = Root;
            else
                currDirectory = currDirectory.Directories.First(d => d.Name == dirName);
            return currDirectory;
        }
    }
    public static FSDirectory FindParent(FSDirectory searchDirectory, FSDirectory currDirectory)
    {
        if (searchDirectory == null)
            return null;

        if ((searchDirectory.Name == currDirectory.ParentDirectory) && searchDirectory.Directories.Contains(currDirectory))
            return searchDirectory;

        foreach (var child in searchDirectory.Directories)
        {
            var found = FindParent(child, currDirectory);
            if (found != null)
                return found;
        }
        return null;
    }
    private static bool IsCommand(string s)
    {
        return s.StartsWith("$");
    }
}
public class FSDirectory
{
    public string Name { get; set; }
    public string ParentDirectory { get; set; }
    public List<FSDirectory> Directories { get; set; }
    public List<FSFile> Files { get; set; }
    public int Size => Files.Sum(f => f.Size) + Directories.Sum(d => d.Size);
    public FSDirectory(string s, FSDirectory parentDirectory)
    {
        Directories = new List<FSDirectory>();
        Files = new List<FSFile>();
        Name = s;
        ParentDirectory = parentDirectory?.Name;
    }
    public void AddToDirectory(string s, FSDirectory parentDirectory)
    {
        if (s.StartsWith("dir "))
            Directories.Add(new FSDirectory(s.Replace("dir", "").Trim(), parentDirectory));
        else
            Files.Add(new FSFile(s));
    }
    public override string ToString()
    {
        return String.Format("Name: {0}, Size {1}", Name, Size);
    }
}
public class FSFile
{
    public string Name { get; set; }
    public int Size { get; set; }
    public FSFile(string s)
    {
        Size = int.Parse(s.Split(" ")[0].Trim());
        Name = s.Split(" ")[1].Trim();
    }
    public override string ToString()
    {
        return String.Format("Name: {0}, Size {1}", Name, Size);
    }
}