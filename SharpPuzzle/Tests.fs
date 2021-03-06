﻿module Tests

open System

open NUnit.Framework
open Swensen.Unquote

open Points
open Colors
open Cubes
open Figures
open FigureParser
open Boards


[<Test>]
let ``Проверка смежности/не смежности простых точек работает``() =
    // Arrange
    let a = { X = 5; Y = 5 }
    let b = { X = 6; Y = 5 }
    let c = { X = 6; Y = 6 }

    // Assert
    a % b =! true
    b % c =! true
    a % c =! false


[<Test>]
let ``Четность (odd) точек работает``() =
    // Arrange
    let a = { X = 5; Y = 5 }
    let b = { X = 6; Y = 5 }
    let c = { X = 6; Y = 6 }

    // Assert
    a.IsOdd =! false
    IsOdd b =! true
    IsOdd c =! false
    Point.Zero.IsOdd =! false
    IsOdd Point.Zero =! false


[<Test>]
let ``Создание простой фигуры из готовых кубиков работает, координаты нормализуются``() =
    // Arrange
    let cubes = [ 
        { Position = { X = 10; Y = 10}; Color = Black };
        { Position = { X = 10; Y = 11}; Color = White };
        { Position = { X = 11; Y = 10}; Color = White };
        { Position = { X = 11; Y = 11}; Color = Black }
    ]

    // Act
    let f = Figure.FromCubes cubes 

    // Assert
    f.CubeCount =! 4
    let maxX = f.Points |> List.map (fun p -> p.X) |> List.max
    maxX =! 1
    let maxY = f.Points |> List.map (fun p -> p.Y) |> List.max
    maxY =! 1


[<Test>]
let ``Нельзя создать фигуру с несмежными точками``() =

    // Assert
    raises<ArgumentException> <@ Figure.FromCoords Black [ (1, 1); (2, 2) ] @>
    

[<Test>]
let ``Нельзя создать фигуру с нечередующимися цветами кубиков``() =
    // Arrange
    let cubes = [ 
        { Position = { X = 11; Y = 10}; Color = White };
        { Position = { X = 11; Y = 11}; Color = White }
    ]

    // Act, Assert
    raises<ArgumentException> <@ Figure.FromCubes cubes  @>


[<Test>]
let ``Парсер фигур парсит фигуры``() =
    // Arrange
    let s = """
            WBW WBW BWBWB     W
                BW  W   W  WBWBW
            """
    // Act
    let figures = ParseFiguresPicStr s
    
    // Assert
    figures.Length =! 4
    
    figures.[0].Points.Length =! 3    
    figures.[1].Points.Length =! 5
    figures.[2].Points.Length =! 7
    figures.[3].Points.Length =! 6

    let f0 = figures.[0]
    f0.ColorAt { X = 0; Y = 0 } =! Some White
    f0.ColorAt { X = 1; Y = 0 } =! Some Black
    f0.ColorAt { X = 2; Y = 0 } =! Some White


[<Test>]
let ``Цвет кубиков фигуры корректно определяется для случая первой точки не в (0,0) и точек не по порядку и смещенных``() =

    // Arrange
    let dx = 10;
    let dy = 20;
    let coords = [ (1, 1); (1, 2); (1, 3); (0, 3); (1, 0) ] |> List.map (fun (x, y) -> (x + dx, y + dy));
    let f = Figure.FromCoords Black coords

    // Act

    // Assert
    f.ColorAt { X = 1; Y = 1; } =! Some Black
    f.ColorAt { X = 0; Y = 3; } =! Some White
    f.ColorAt { X = 9; Y = 9; } =! None



[<Test>]
let ``Расчет кластера смежности работает на краевом случае двух 8-смежных точек``() =

    // Arrange
    let points = [ { X = 1; Y = 1 }; { X = 2; Y = 2 } ]

    // Act
    let cluster = SelectAdjacentCluster points id points.Head

    // Assert
    cluster =! [ { X = 1; Y = 1 } ]



[<Test>]
let ``Нельзя создать доску с наложением фигур``() =
    // Arrange
    let s = """
            WB
            BW  
            """
    let figure = Seq.exactlyOne (ParseFiguresPicStr s)
    let placements = 
        [
            { Figure = figure; Origin = PointAt(2, 2) };
            { Figure = figure; Origin = PointAt(2, 2) }
        ]

    // Act, Assert
    raises<ArgumentException> <@ BoardState.WithFigures placements @>


[<Test>]
let ``Нельзя создать доску с выходом фигуры за границу``() =
    // Arrange
    let s = """WBWBWB"""
    let figure = Seq.exactlyOne (ParseFiguresPicStr s)
    let placements = [ { Figure = figure; Origin = PointAt(7, 7) } ]

    // Act, Assert
    raises<ArgumentException> <@ BoardState.WithFigures placements @>


[<Test>]
let ``Добавление фигур по одной работает, пока не будет наложения``() =
    // Arrange
    let s = """
            WB
            BW  
            """
    let figure = Seq.exactlyOne (ParseFiguresPicStr s)

    // Act
    let s0 = BoardState.Empty
    let s1 = s0.Place figure (PointAt (1, 1));
    let s2 = s1.Place figure (PointAt (3, 3));
    let s3 = s2.Place figure (PointAt (5, 5));
    let s4 = s3.Place figure (PointAt (7, 7));
    
    // Assert
    raises<ArgumentException> <@ s4.Place figure (PointAt (2, 2)) @>


[<Test>]
let ``Хеш-код не зависит от порядка формирования и сравнение работает``() =
    // Arrange
    let s = """
            W
            """
    let figure = Seq.exactlyOne (ParseFiguresPicStr s)

    let steps = [1 .. 1 .. 3];
    let origins = steps |> List.map (fun s -> PointAt(s, s))
    let placements = origins |> List.mapi (fun ndx o -> { Figure = figure; Origin = o; FigureIndex = ndx })
    let placer (s: BoardState) placement = s.Place placement

    // Act
    let statesForward = origins |> List.scan placer BoardState.Empty
    let statesBackward = origins |> List.rev |> List.scan placer BoardState.Empty

    let lastForward = statesForward |> List.last
    let lastBackward = statesBackward |> List.last

    let interStatesForward = statesForward.GetSlice(Some 0, Some (statesForward.Length - 1))
    let interStatesBackward = statesBackward.GetSlice(Some 0, Some (statesBackward.Length - 1))

    // Assert

    // Последние состояния равны
    statesForward.Length =! statesBackward.Length
    lastForward.GetHashCode() =! lastBackward.GetHashCode()
    lastForward =! lastBackward

    // Промежуточные - отличаются
    List.iter2 
        (fun (s1:BoardState) (s2:BoardState) -> 
            s1 <>! s2
            s1.GetHashCode() <>! s2.GetHashCode()
            s1.CubeMap.Count =! s2.CubeMap.Count
        )
        interStatesForward
        interStatesBackward


(*    
[<Test>]
let ``Memoization работает (проверка отладчиком)``() =

    // Arrange
    let f = Figure.FromCoords Black [ (1, 1); (1, 2); ]

    // Act
    let points = f.Points
    let reversePoints =  List.rev points
    let colors = points |> List.map f.ColorAt
    let reverseColors = reversePoints |> List.map f.ColorAt

    // Assert
    // ... и проверяем также под отладчиком, что зашли в расчет цвета точки 
    // только один раз на каждую координату, а не дважды
    colors =! (List.rev reverseColors)
*)


    
