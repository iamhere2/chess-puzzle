module Program

open System

open Boards
open BoardPrinter
open FigureParser

let allFigiresPicStr = """
                                 B                         B    BW    W       WBWB  WBWB W B WB WB
   BW   BWBW  BWB  BWB           W      B     BWB   BWB   BWB    BW   BWB     B     B  W         W
   WB   W      B   W W  BWBWB  BWB    WBWB   BW      BW    B      B    B            WBWB 

   """

[<EntryPoint>]
let main _ = 
    
    Console.WriteLine("Empty:")
    Console.WriteLine()
    Print BoardState.Empty
    Console.WriteLine("Hash: 0x{0:X8}", BoardState.Empty.GetHashCode())
    Console.WriteLine()

    let figures = ParseFiguresPicStr(allFigiresPicStr);

    let printFigure f = 
        let b = BoardState.WithFigures([ 
                    { Origin = { X = 2; Y = 2 }; Figure = f } 
                ]) 
        Print b
        Console.WriteLine("Hash: 0x{0:X8}", f.GetHashCode())
        Console.WriteLine()

    Console.WriteLine("Figures ({0}):", figures.Length)
    Console.WriteLine()

    figures |> List.iter printFigure 

    Console.ReadLine() |> ignore

    0 // exit code
