module Program

open System

open Puzzle
open BoardPrinter
open FigureParser

let allFigiresPicStr = """
                                 B                         B    BW    W       WBWB   
   BW   BWBW  BWB  BWB           W      B     BWB   BWB   BWB    BW   BWB     B      
   WB   W      B   W W  BWBWB  BWB    WBWB   BW      BW    B      B    B             

   """

[<EntryPoint>]
let main _ = 
    
    let figures = ParseFiguresPicStr(allFigiresPicStr);

    Console.WriteLine(String.Format("Figures ({0}):", figures.Length))

    let printFigure f = 
        let b = BoardState.WithFigures([ 
                    { Origin = { X = 2; Y = 2 }; Figure = f } 
                ]) 
        Print b
        Console.WriteLine()

    figures |> List.iter printFigure 

    Console.ReadLine() |> ignore

    0 // return an integer exit code
