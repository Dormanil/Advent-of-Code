// For more information see https://aka.ms/fsharp-console-apps
open System
open System.IO

let filename: string = "input.txt"

let file: FileInfo = new FileInfo(filename)

let contents: string[] =
    using (file.OpenText()) (fun (reader: StreamReader) -> reader.ReadToEnd().Split "\n\n")

let caloriesOfElves: int[] =
    contents
    |> Array.map (fun (elfCalories: string) ->
        let calories: int =
            elfCalories.Split("\n", StringSplitOptions.RemoveEmptyEntries)
            |> Array.map int
            |> Array.sum

        calories)

let biggestElf: int = caloriesOfElves |> Array.max

let topThreeElves: int =
    caloriesOfElves |> Seq.sortDescending |> Seq.take 3 |> Seq.sum

printfn "Biggest: %d, TopThree: %d" biggestElf topThreeElves
