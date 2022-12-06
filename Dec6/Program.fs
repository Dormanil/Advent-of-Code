open System.IO

let filename: string = "input.txt"

let file: FileInfo = new FileInfo(filename)

let charStream = using (file.OpenText()) (fun (reader: StreamReader) ->
    reader.ReadToEnd()
)

let sopmLength = 4
let sommLength = 14
let mutable sopMarkers = []
let mutable somMarkers = []

for i in 0 .. charStream.Length - sopmLength do
    let slice = charStream[i .. i + sopmLength - 1].ToCharArray()
    if Array.distinct slice |> Array.length |> (=) sopmLength then
        sopMarkers <- [i + sopmLength] |> List.append sopMarkers

for i in 0 .. charStream.Length - sommLength do
    let slice = charStream[i .. i + sommLength - 1].ToCharArray()
    if Array.distinct slice |> Array.length |> (=) sommLength then
        somMarkers <- [i + sommLength] |> List.append somMarkers

printfn "%A" sopMarkers
printfn "%A" somMarkers