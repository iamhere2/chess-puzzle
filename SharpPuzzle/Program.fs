module Program

open Colors
open Figures
open Puzzle
open BoardPrinter
open System
open FigureParser
open System.Reflection


let startFigures : Figure list = 
    [ 
        Figure.FromCoords Black [ (1, 0); (2, 0); (3, 0); (4, 0) ]
        Figure.FromCoords White [ (1, 0); (1, 1); (0, 1) ]
        Figure.FromCoords Black [ (1, 0); (0, 1); (-1, 0); (0, -1) ]
    ]

let allFigiresPicStr = """
   BW   BWBW  WBW  WBWB    W      B
   WB   W      W   B  W    BWBWBWBW
   """


[<EntryPoint>]
let main _ = 
    
    let b:BoardState = 
        { 
            Figures = 
            [ 
                { Origin = { X = 2; Y = 2 }; Figure = startFigures.[0] } 
                { Origin = { X = 2; Y = 4 }; Figure = startFigures.[1] } 
                { Origin = { X = 6; Y = 5 }; Figure = startFigures.[2] } 
            ] 
        }

    Print b

    Console.WriteLine()

    let newFigures = ParseFiguresPicStr(allFigiresPicStr);

    let printFigure f = 
        let b:BoardState = 
            {
                Figures = 
                [ 
                    { Origin = { X = 2; Y = 2 }; Figure = f } 
                ] 
            }
        Print b
        Console.WriteLine()

    newFigures |> List.iter printFigure 



    Console.ReadLine() |> ignore

    0 // return an integer exit code
