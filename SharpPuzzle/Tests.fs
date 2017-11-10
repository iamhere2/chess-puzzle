module Tests

open NUnit.Framework
open Swensen.Unquote

open Points
open Colors
open Cubes
open FigureParser

[<Test>]
let ``Проверка смежности/не смежности простых точек``() =
    let a = { X = 5; Y = 5 }
    let b = { X = 6; Y = 5 }
    let c = { X = 6; Y = 6 }
    a % b =! true
    b % c =! true
    a % c =! false

[<Test>]
let ``Проверка смежности/не смежности смешанных точек``() =
    let a = { Color = Black; X = 5; Y = 5 }
    let b = { X = 6; Y = 5 }
    let c = { Color = White; X = 6; Y = 6 }
    a % b =! true
    b % c =! true
    a % c =! false

[<Test>]
let ``Проверка четности (odd) точек``() =
    let a = { X = 5; Y = 5 }
    let b = { X = 6; Y = 5 }
    let c = { X = 6; Y = 6 }
    a.IsOdd =! false
    IsOdd b =! true
    IsOdd c =! false
    Point.Zero.IsOdd =! false
    IsOdd Point.Zero =! false

[<Test>]
let ``Парсер фигур``() =
    let s = """
            WBW WBW BWBWB
                BW  W   W
            """
    let figures = ParseFiguresPicStr s
    
    figures.Length =! 3
    
    figures.[0].Points.Length =! 3    
    figures.[1].Points.Length =! 5
    figures.[2].Points.Length =! 7

    let f0 = figures.[0]
    f0.ColorAt {X = 0; Y = 0} =! Some White
    f0.ColorAt {X = 1; Y = 0} =! Some Black
    f0.ColorAt {X = 2; Y = 0} =! Some White
