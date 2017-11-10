module BoardPrinter

open System

open Colors
open Cubes
open Puzzle

let Print (b:BoardState) =
    let writeCell (cell : (int * Cube) option) =
        let (bk, fg, s) = 
            match cell with
            | None                      -> (ConsoleColor.DarkGray, ConsoleColor.DarkGray, "  ")
            | Some (n, {Color = White}) -> (ConsoleColor.White,    ConsoleColor.Gray,     sprintf "%2d" n)
            | Some (n, {Color = Black}) -> (ConsoleColor.Black,    ConsoleColor.Gray,     sprintf "%2d" n)

        Console.BackgroundColor <- bk;
        Console.ForegroundColor <- fg;
        Console.Write(s)
    
    for y in [ BoardState.Low .. BoardState.High ] do
        for x in [ BoardState.Low .. BoardState.High ] do
            writeCell(b.At(x, y))
        Console.WriteLine()


