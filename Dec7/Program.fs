open System
open System.Collections.Generic
open System.IO
open System.Linq

type ElfIO =
    | File of Length: int * Name: string
    | Directory of Name: string * Contents: ElfIO list

    member self.Length =
        match self with
        | File(length: int, _) -> length
        | Directory(_, contents) when contents.Length = 0 -> 0
        | Directory(_, contents) -> contents.Sum(fun item -> item.Length)

type Content =
    | File of Length: int * Name: string
    | Directory of Name: string

type Command =
    | ChangeDirectory of Target: string
    | List of Contents: Content list


let filename: string = "input.txt"

let file: FileInfo = new FileInfo(filename)

let parseCommand (command: string) : Command =
    let lines: string[] = command.Split("\n", StringSplitOptions.RemoveEmptyEntries)
    let opParts: string[] = lines[ 0 ].Split(" ", StringSplitOptions.RemoveEmptyEntries)

    match opParts[0] with
    | "cd" -> ChangeDirectory(Target = opParts[1])
    | "ls" ->
        List(
            Contents =
                [ for line: string in lines[1..] ->
                      let parts: string[] = line.Split(" ", StringSplitOptions.RemoveEmptyEntries)

                      match parts[0] with
                      | "dir" -> Directory(Name = parts[1])
                      | (length: string) -> File(Length = int length, Name = parts[1]) ]
        )

let commands: Command[] =
    using (file.OpenText()) (fun (reader: StreamReader) ->
        reader.ReadToEnd().Split("$ ", StringSplitOptions.RemoveEmptyEntries)
        |> Array.map parseCommand)

let amountOfSlashes (path: string) : int =
    path.ToCharArray() |> Array.filter (fun c -> c = '/') |> Array.length

let appendPath (path: string) (appendage: string) : string =
    match (path, appendage) with
    | ("/", "/") -> "/"
    | ("/", _) -> $"/{appendage}"
    | (nonRootPath: string, _) -> $"{nonRootPath}/{appendage}"

let getParent (path: string) : string =
    match path with
    | "/" -> "/"
    | (nonRootPath: string) when amountOfSlashes nonRootPath = 1 -> "/"
    | (nonRootPath: string) -> nonRootPath[.. nonRootPath.LastIndexOf("/") - 1]

let mutable parentDirectory: string = "/"
let mutable currentDirectory: string = "/"

let mutable pathIOMap: Map<string, ElfIO> =
    Map.empty.Add("/", ElfIO.Directory("/", []))

for command: Command in commands do
    match command with
    | ChangeDirectory(target: string) ->
        match target with
        | "/" ->
            parentDirectory <- "/"
            currentDirectory <- "/"
        | ".." ->
            currentDirectory <- parentDirectory
            parentDirectory <- getParent parentDirectory
        | (directoryName: string) ->
            parentDirectory <- currentDirectory
            currentDirectory <- appendPath currentDirectory directoryName
    | List(contents: Content list) ->
        for content: Content in contents do
            match content with
            | File(length, name) ->
                let newPath = appendPath currentDirectory name
                pathIOMap <- pathIOMap.Add(newPath, (ElfIO.File(length, name)))
            | Directory(name) ->
                let newPath = appendPath currentDirectory name
                pathIOMap <- pathIOMap.Add(newPath, (ElfIO.Directory(name, [])))

let getDirectChildPaths (searchPath: string) =
    pathIOMap.Keys
    |> Seq.toList
    |> List.filter (fun (path: string) -> path.StartsWith searchPath && path <> searchPath) // Filter those paths not starting with the root path
    |> List.filter (fun (path: string) ->
        if searchPath = "/" then
            not (path[ searchPath.Length .. ].Contains("/"))
        else
            path[ searchPath.Length .. ].StartsWith("/")
                && not (path[ (searchPath.Length + 1) .. ].Contains("/")))

let rec makeTree (root: string) =
    match pathIOMap[root] with
    | ElfIO.File(length, _) -> (ElfIO.File(length, root))
    | ElfIO.Directory(_, contents) ->
        (ElfIO.Directory(
            (root),
            getDirectChildPaths root
            |> List.map (fun (childPath: string) -> (makeTree childPath))
        ))

let pathTree = makeTree "/"

let rec calculateDirLengths (root: ElfIO) : (string * int) list =
    match root with
    | ElfIO.Directory(name, contents) when contents.Length = 0 -> [ (name, 0) ]
    | ElfIO.Directory(name, contents) ->
        List.append
            [ (name, root.Length) ]
            [ for node in contents do
                  yield! calculateDirLengths node ]
    | ElfIO.File(_, name) -> [ (name, -1) ]

let dirLengths = 
    pathTree
    |> calculateDirLengths
    |> List.distinctBy (fun (name, _) -> name)
    |> List.filter (fun (_, length) -> length <> -1) 

let dirLengthsLessThan100000 =
    dirLengths
    |> List.filter (fun (_, length) -> length < 100000)

let sumOfDirLengthsLessThan100000 = List.sumBy (fun (_, length) -> length) dirLengthsLessThan100000

let totalSpaceOnDisk = 70000000
let neededSpaceOnDisk = 30000000
let totalUsedSpaceOnDisk = pathTree.Length
let unusedSpaceOnDisk = totalSpaceOnDisk - totalUsedSpaceOnDisk
let differenceToNeededSpace = neededSpaceOnDisk - unusedSpaceOnDisk

let dirLengthsBiggerThanDifference =
    dirLengths
    |> List.filter (fun (_, length) -> length >= differenceToNeededSpace)

let (_, smallestDirBiggerThanDifference) = dirLengthsBiggerThanDifference |> List.sortBy (fun (_, length)->length ) |> List.head

printfn "%d" sumOfDirLengthsLessThan100000
printfn "%d" smallestDirBiggerThanDifference