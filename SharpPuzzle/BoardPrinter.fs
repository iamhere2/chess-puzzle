module BoardPrinter

open System

open Colors
open Points
open Boards

/// Печатает состояние доски на консоли
let Print (b : BoardState) =
    let writeCell (cell : CubeOnBoard option) =
        let (bk, fg, s) = 
            match cell with
            | None                                    -> (ConsoleColor.DarkGray, ConsoleColor.DarkGray, "  ")
            | Some { Color = White; FigureIndex = n } -> (ConsoleColor.White,    ConsoleColor.DarkGray, sprintf "%2d" n)
            | Some { Color = Black; FigureIndex = n } -> (ConsoleColor.Black,    ConsoleColor.Gray,     sprintf "%2d" n)

        Console.BackgroundColor <- bk;
        Console.ForegroundColor <- fg;
        Console.Write(s)
    
    for y in [ BoardState.Low .. BoardState.High ] do
    (
        for x in [ BoardState.Low .. BoardState.High ] do
            writeCell(b.At(Point.At(x, y)))

        Console.WriteLine()
    )


