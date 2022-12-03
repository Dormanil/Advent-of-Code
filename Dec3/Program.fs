open System
open System.IO

let filename: string = "input.txt"

let file: FileInfo = new FileInfo(filename)

let rucksacks: string[] =
    using (file.OpenText()) (fun (reader: StreamReader) ->
        reader.ReadToEnd().Split("\n", StringSplitOptions.RemoveEmptyEntries))

/// <summary>Separates the items in a rucksack into the two compartments</summary>
/// <param name="rucksack">A rucksack containing an even number of items</param>
/// <returns>A tuple containing the two compartments</returns>
let halveItems (rucksack: string) =
    (rucksack[.. rucksack.Length / 2 - 1], rucksack[rucksack.Length / 2 ..])

let compartments: (string * string)[] = rucksacks |> Array.map halveItems

let groups: (string * string * string)[] =
    Array.chunkBySize 3 rucksacks
    |> Array.map (fun (group: string[]) -> (group[0], group[1], group[2]))

/// <summary>Calculates the priority of the item</summary>
/// <param name="item">An item to calculate the priority for.</param>
/// <returns>The calculated priority.</returns>
let getPriority (item: char) =
    if Char.IsUpper item then
        (int item) - 38
    else
        (int item) - 96

/// <summary>Finds the common item in two compartments.</summary>
/// <param name="compartment_a">The first compartment.</param>
/// <param name="compartment_b">The second compartment.</param>
/// <returns>The common item.</returns>
let getCommonItem (compartment_a: string) (compartment_b: string) =
    let mutable commonItem: char = '0'

    for item: char in compartment_a do
        if compartment_b.Contains item then
            commonItem <- item

    commonItem

let getCommonItem3 (rucksack_a: string) (rucksack_b: string) (rucksack_c: string) =
    let mutable commonItem: char = '0'

    for item: char in rucksack_a do
        if rucksack_b.Contains item then
            if rucksack_c.Contains item then
                commonItem <- item
    
    commonItem

let prioritiesOfCommonItemsInCompartments: int[] =
    compartments
    |> Array.map (fun (a: string, b: string) -> getCommonItem a b |> getPriority)

let prioritiesOfCommonItemsInGroups: int[] =
    groups
    |> Array.map (fun (a: string, b: string, c: string) -> getCommonItem3 a b c |> getPriority)

printfn "%d" <| Array.sum prioritiesOfCommonItemsInCompartments
printfn "%d" <| Array.sum prioritiesOfCommonItemsInGroups
