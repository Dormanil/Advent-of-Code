open System
open System.IO
open System.Collections.Generic

let filename: string = "input.txt"

let file: FileInfo = new FileInfo(filename)

let contents: (string * string array) =
    using (file.OpenText()) (fun (reader: StreamReader) ->
        let input = reader.ReadToEnd().Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
        (input[0], input[ 1 ].Split("\n", StringSplitOptions.RemoveEmptyEntries)))

/// <summary>Converts the description of the initial state into a list of stacks</summary>
/// <param name="initialDescription">The initial state to convert.</param>
/// <returns>A list of stacks reflecting the initial state.</returns>
let convertToStacks (initialDescription: string) : Stack<char> list =
    let numStacks = (/) ((+) (initialDescription.IndexOf '\n') 1) 4 // Calculate the number of stacks based on the length of the first line and the width of a stack

    let stackground =
        initialDescription.Split("\n", StringSplitOptions.RemoveEmptyEntries)

    let stackWidth = stackground[0].Length / numStacks // Save the stackwidth for posterity

    let stackChunks = // Use previously calculated offsets to grab the relevant characters and put them into nice and easily worked with chunks
        stackground
        |> Array.map (fun line ->
            seq { for i in 0 .. stackWidth + 1 .. line.Length -> (line[i .. i + stackWidth - 1])[1] }
            |> Seq.toList)
        |> Array.toList
        |> List.rev
        |> List.skip 1


    seq {
        for i in 0 .. numStacks - 1 do
            let stack = new Stack<char>()

            for s in stackChunks do
                if s[i] <> ' ' then
                    stack.Push s[i]

            stack
    }
    |> Seq.toList

let (initDesc, instructions) = contents

let mutable stackList = convertToStacks initDesc

/// <summary>Interprets the instruction string into amount of crates, original stack, and destination stack</summary>
/// <param name="instruction">The instruction to interpret</param>
/// <returns>Wellformed instructive numbers</returns>
let parseInstruction (instruction: string) : int * int * int =
    let instructionParts = instruction.Split(' ', StringSplitOptions.RemoveEmptyEntries)
    (int instructionParts[1], int instructionParts[3] - 1, int instructionParts[5] - 1)

for instruction in instructions do
    let (amount, originStack, destinationStack) = parseInstruction instruction

    for i in 0 .. amount - 1 do
        stackList[ destinationStack ].Push(stackList[ originStack ].Pop())

let generateStackTopString (stackList:Stack<char> list) : string = 
    ("", seq {
        for stack in stackList ->
            match stack.TryPeek() with
            | true, value -> Some value
            | false, _ -> None
    }) ||> Seq.fold (fun concatenation topStack ->
        match topStack.IsSome with
            | true -> concatenation + $"{topStack.Value}"
            | false -> concatenation
    )

let topOfTheStacksA = generateStackTopString stackList    

stackList <- convertToStacks initDesc

for instruction in instructions do
    let (amount, originStack, destinationStack) = parseInstruction instruction
    let tempStack = new Stack<char>()

    for i in 0 .. amount - 1 do
        tempStack.Push (stackList[ originStack ].Pop())
    
    for i in 0 .. amount - 1 do
        stackList[ destinationStack ].Push (tempStack.Pop())


let topOfTheStacksB = generateStackTopString stackList

printfn $"The stack tops for CrateMover 9000 are: \"%s{topOfTheStacksA}\"."
printfn $"The stack tops for CrateMover 9001 are: \"%s{topOfTheStacksB}\"."
