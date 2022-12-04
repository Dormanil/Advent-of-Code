open System
open System.IO

let filename: string = "input.txt"

let file: FileInfo = new FileInfo(filename)

let sectionAssignmentPairs: (string * string)[] =
    using (file.OpenText()) (fun (reader: StreamReader) ->
        reader.ReadToEnd().Split("\n", StringSplitOptions.RemoveEmptyEntries))
    |> Array.map (fun s -> s.Split(","))
    |> Array.map (fun (pairs: string[]) -> (pairs[0], pairs[1]))

/// <summary>Compare section assignments</summary>
/// <param name="firstSection">Assignment for the first Elf</param>
/// <param name="secondSection">Assignment for the second Elf</param>
/// <returns>True, if an assignment is fully contained in the other Elf's assignment, False if not.</returns>
let compareSections (firstSection: string) (secondSection: string) =
    let valuesOfFirst = firstSection.Split("-") |> Array.map int
    let valuesOfSecond = secondSection.Split("-") |> Array.map int

    (valuesOfFirst[0] <= valuesOfSecond[0] && valuesOfFirst[1] >= valuesOfSecond[1])
    || (valuesOfSecond[0] <= valuesOfFirst[0] && valuesOfSecond[1] >= valuesOfFirst[1])

/// <summary>Compare section assignments</summary>
/// <param name="firstSection">Assignment for the first Elf</param>
/// <param name="secondSection">Assignment for the second Elf</param>
/// <returns>True, if an assignment is partially or fully contained in the other Elf's assignment, False if not.</returns>
let compareSections2 (firstSection: string) (secondSection: string) =
    let valuesOfFirst = firstSection.Split("-") |> Array.map int
    let valuesOfSecond = secondSection.Split("-") |> Array.map int

    (valuesOfFirst[0] <= valuesOfSecond[0] && valuesOfFirst[1] >= valuesOfSecond[1])
    || (valuesOfSecond[0] <= valuesOfFirst[0] && valuesOfSecond[1] >= valuesOfFirst[1])
    || (valuesOfFirst[0] <= valuesOfSecond[0] && valuesOfFirst[1] >= valuesOfSecond[0])
    || (valuesOfSecond[0] <= valuesOfFirst[0] && valuesOfSecond[1] >= valuesOfFirst[0])
    || (valuesOfFirst[0] <= valuesOfSecond[1] && valuesOfFirst[1] >= valuesOfSecond[1])
    || (valuesOfSecond[0] <= valuesOfFirst[1] && valuesOfSecond[1] >= valuesOfFirst[1])

let fullOverlaps = sectionAssignmentPairs |> Array.map (fun (leftSection: string, rightSection: string) ->
    compareSections leftSection rightSection)

let partialOverlaps = sectionAssignmentPairs |> Array.map (fun (leftSection: string, rightSection: string) ->
    compareSections2 leftSection rightSection)

printfn "Fully overlapping pairs: %d" (Array.filter id <| fullOverlaps).Length
printfn "At least partially overlapping pairs: %d" (Array.filter id <| partialOverlaps).Length
