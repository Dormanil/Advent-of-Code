open System
open System.IO

let filename: string = "input.txt"


let file: FileInfo = new FileInfo(filename)

let games: string[] =
    using (file.OpenText()) (fun (reader: StreamReader) -> reader.ReadToEnd().Split ("\n", StringSplitOptions.RemoveEmptyEntries))

/// <summary>Calculates the score for a game of A,X=rock, B,Y=paper, C,Z=scissors</summary>
/// <param name="playerOne">Opponent's choice</param>
/// <param name="playerTwo">Coder's choice</param>
/// <returns>The sum of the value of the Coder's choice and the value of the result of the game</returns>
let gameScoreOne (playerOne: char) (playerTwo: char) =
    let selectionScore =
        match playerTwo with
        | 'X' -> 1
        | 'Y' -> 2
        | 'Z' -> 3
        | _ -> 0

    let loseDrawWin =
        match (playerOne, playerTwo) with
        | ('A', 'X') -> 3
        | ('A', 'Y') -> 6
        | ('A', 'Z') -> 0
        | ('B', 'X') -> 0
        | ('B', 'Y') -> 3
        | ('B', 'Z') -> 6
        | ('C', 'X') -> 6
        | ('C', 'Y') -> 0
        | ('C', 'Z') -> 3
        | _ -> 0

    selectionScore + loseDrawWin

/// <summary>Calculates the score for a game of A=rock, B=paper, C=scissors, X=Lose, Y=Draw, Z=Win</summary>
/// <param name="playerOne">Opponent's choice</param>
/// <param name="playerTwo">Coder's choice</param>
/// <returns>The sum of the value of the Coder's choice and the value of the result of the game</returns>
let gameScoreTwo (playerOne: char) (playerTwo: char) =
    let loseDrawWin =
        match playerTwo with
        | 'X' -> 0
        | 'Y' -> 3
        | 'Z' -> 6
        | _ -> 0

    let selectionScore =
        match (playerOne, playerTwo) with
        | ('A', 'X') -> 3 // Rock     -> Scissors
        | ('A', 'Y') -> 1 // Rock     -> Rock
        | ('A', 'Z') -> 2 // Rock     -> Paper
        | ('B', 'X') -> 1 // Paper    -> Rock
        | ('B', 'Y') -> 2 // Paper    -> Paper
        | ('B', 'Z') -> 3 // Paper    -> Scissors
        | ('C', 'X') -> 2 // Scissors -> Paper
        | ('C', 'Y') -> 3 // Scissors -> Scissors
        | ('C', 'Z') -> 1 // Scissors -> Rock
        | _ -> 0

    selectionScore + loseDrawWin

let scores = games |> Array.map (fun game -> gameScoreOne game[0] game[2])

let adversarialScores = games |> Array.map (fun game -> gameScoreTwo game[0] game[2])

let totalScore = Array.sum scores
let totalAdversarialScore = Array.sum adversarialScores

printfn $"Total score: %d{totalScore}."
printfn $"Total reactive score: %d{totalAdversarialScore}."
