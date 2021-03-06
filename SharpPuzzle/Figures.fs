﻿/// Модуль работы с фигурами из цветных кубиков
/// Для производительности многое объявлено как inline
module Figures

open System.Runtime.CompilerServices

open Points
open Colors
open Cubes

/// Фигура - набор смежных чередующихся по цвету кубиков
/// Это уже не просто запись, а класс с закрытыми полями - не все сочетания данных валидны как фигура
/// Разные повороты и отражения и смещения - это другие фигуры
[<Struct; IsReadOnly>]
type Figure private (cubeMap: Map<Point, Cube>) = 

    /// Карта фигуры
    /// Это все, что хранится, а остальное - вычисляется
    /// Какие кубики по каким относительным координатам входят в фигуру
    member f.CubeMap: Map<Point, Cube> = cubeMap

    /// Число кубиков в фигуре
    member inline f.CubeCount = f.CubeMap.Count

    /// Кубики фигуры
    member inline f.Cubes = f.CubeMap |> Map.toList |> List.map snd

    /// Точки фигуры
    member inline f.Points = f.CubeMap |> Map.toList |> List.map fst


    /// Создает фигуру из кубиков
    // Все остальные методы создания должны вызывать этот, т.к. в нем есть все нужные проверки
    // Проверяет, что цвета кубиков чередуются верно, и что кубики смежны
    static member FromCubes (sourceCubes: Cube list) = 
        
        // Сдвинем к началу координат
        let minX = sourceCubes |> List.map (fun p -> p.Position.X) |> List.min
        let minY = sourceCubes |> List.map (fun p -> p.Position.Y) |> List.min
        let shiftToMin c = { Color = c.Color; Position = Shift (-minX, -minY) c.Position }
        let figureCubes = sourceCubes |> List.map shiftToMin

        // Проверим на смежность
        let cluster = SelectAdjacentCluster figureCubes PositionOf figureCubes.Head
        if cluster.Length <> figureCubes.Length then invalidArg "cubes" "Cubes of figure must be adjacement"

        // Проверим на цвета
        let headOdd = IsOdd figureCubes.Head.Position
        let headColor = figureCubes.Head.Color
        let validateCubeColor c = 
            if c = figureCubes.Head then c
            elif ((IsOdd c.Position) = headOdd) = (c.Color = headColor) then c
            else invalidArg "cubes" "Cubes in figure must has interleaved colors";
        
        let validatedCubes = figureCubes |> List.map validateCubeColor
        let cubeMap = validatedCubes |> List.map (fun c -> (c.Position, c)) |> Map.ofList

        Figure(cubeMap)


    /// Создает фигуру из точек и цвета первой точки
    static member FromPoints firstPointColor (points: Point list) = 
        let headOdd = IsOdd points.Head
        let cubeFromPoint p = { Position = p; Color = if p.IsOdd = headOdd then firstPointColor else firstPointColor.Inverted  } 
        let cubes = points |> List.map cubeFromPoint 
        Figure.FromCubes cubes


    /// Создает фигуру по цвету первой точки и координатам точек
    static member FromCoords color coords = 
        let points = coords |> List.map Point.At 
        Figure.FromPoints color points

    
    /// Цвет по указанным относительным координатам или None
    // TODO: Кажется, это деалется проще с монадными функциями типа bind, поизучать
    member inline f.ColorAt (p: Point) : Color option =
        let c = f.CubeMap.TryFind p
        match c with 
            | Some cube -> Some cube.Color
            | None -> None


    /// Кубик по указанным относительным координатам или None
    // TODO: How to memoize it? Видимо, явно включить в структуру заготовленный Map
    member inline f.CubeAt (p: Point) : Cube option = f.CubeMap.TryFind p


