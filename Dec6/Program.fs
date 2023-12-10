open AoC.Common
open System

[<Measure>] type ms
[<Measure>] type mm

let convert (input: string): int64<'unit> = 
    input
    |> Int64.Parse
    |> LanguagePrimitives.Int64WithMeasure

let velocity (timeButtonPressed: int64<ms>): int64<mm/ms> = 
    timeButtonPressed * 1L<mm/ms^2>

let distanceTravelledUsingLinearSpeed (raceLength: int64<ms>) (timeButtonPressed: int64<ms>) : int64<mm> =
    (timeButtonPressed |> velocity) * (raceLength - timeButtonPressed)

let calculatePossibleWins (raceLength: int64<ms>, recordDistance: int64<mm>): int = 
    [for timeButtonPressed: int64<ms> in 0L<ms> .. 1L<ms> .. raceLength do yield ((raceLength |> distanceTravelledUsingLinearSpeed) timeButtonPressed) > recordDistance]
        |> Seq.where id 
        |> Seq.length

let input: string array = Parsing.ReadInputAsync() |> Async.AwaitTask |> Async.RunSynchronously
let raceLengthsAndDistanceRecords: (int64<ms>* int64<mm>) list = 
    let racelengths = input[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
    let distanceRecords = input[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
    (racelengths[1..], distanceRecords[1..])
    ||> Seq.map2 (fun time distance -> (convert time, convert distance))
    |> Seq.toList

let marginOfError: int =
    raceLengthsAndDistanceRecords
    |> Seq.map calculatePossibleWins
    |> Seq.reduce (fun i j -> i * j)

let raceLengthAndDistanceRecord = 
    let raceLength: int64<ms> = 
        input[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1..] 
        |> Seq.reduce (fun a b -> a + b)
        |> convert
    let distanceRecord: int64<mm> = 
        input[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1..] 
        |> Seq.reduce (fun a b -> a + b)
        |> convert
    (raceLength, distanceRecord)

let marginOfErrorOfBigRace: int =
    raceLengthAndDistanceRecord
    |> calculatePossibleWins
    

printfn $"{marginOfError}"
printfn $"{marginOfErrorOfBigRace}"