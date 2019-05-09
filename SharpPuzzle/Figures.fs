/// Модуль работы с фигурами из цветных кубиков
/// Для производительности многое объявлено как inline
module Figures

open System.Runtime.CompilerServices

open Points
open Colors
open Cubes

/// Фигура - набор смежных чередующихся по цвету кубиков
/// Это уже не просто запись, а класс с закрытыми полями - не все сочетания данных валидны как фигура
/// Разные повороты и отражения и смещения - это другие фигуры
/// Хранятся только точки и цвет одного из кубиков, остальные кубики рассчитываются 
///   (??? а хорошо ли это ???)
[<Struct; IsReadOnly>]
type Figure private (firstPointColor : Color, points : Point list) = 

    /// Цвет первого кубика
    member f.FirstCubeColor : Color = firstPointColor

    /// Какие смежные точки (относительные координаты) входят в фигуру
    member f.Points : Point list = points


    /// Внутренняя проверка смежности точек
    static member private ValidateAdjacement (points : Point list) =
        let cluster = SelectAdjacentCluster points id points.Head
        if cluster.Length = points.Length then points else invalidArg "points" "Points of figure must be adjacement"


    /// Создает фигуру из точек и цвета первой точки
    // Все остальные методы создания должны вызывать этот, т.к. в нем есть все нужные проверки
    static member FromPoints firstPointColor points = 
        let validatedNormalizedPoints = points |> List.distinct |> ShiftToZero |> Figure.ValidateAdjacement
        Figure(firstPointColor, validatedNormalizedPoints)


    /// Создает фигуру из кубиков
    // Проверяет, что цвета кубиков чередуются верно
    static member FromCubes (cubes: Cube list) = 
        let headOdd = IsOdd cubes.Head.Position
        let headColor = cubes.Head.Color
        let validateCube c = 
            if c = cubes.Head then c
            elif ((IsOdd c.Position) = headOdd) = (c.Color = headColor) then c
            else invalidArg "cubes" "Cubes in figure must has interleaved colors";
        
        let validatedCubes = cubes |> List.map validateCube
        Figure.FromPoints validatedCubes.Head.Color (validatedCubes |> List.map PositionOf)


    /// Создает фигуру по цвету первой точки и координатам точек
    static member FromCoords color coords = 
        let points = coords |> List.map Point.At 
        Figure.FromPoints color points


    /// Определяет цвет кубика
    /// Правильно использовать только для точек из списка фигуры, поэтому private
    member inline private f.InternalColorAt (p : Point) = 
        if (IsOdd p) = (IsOdd f.Points.Head) then f.FirstCubeColor else f.FirstCubeColor.Inverted


    /// Кубик по указанным относительным координатам или None
    // TODO: How to memoize it?
    member f.At (p: Point) : Cube option = 
        match f.ColorAt p with
        | None -> None
        | Some c -> Some (Cube.At(p, c))


    /// Цвет по указанным относительным координатам или None
    // TODO: How to memoize it?
    member f.ColorAt (p: Point) : Color option =
        if List.contains p f.Points then Some (f.InternalColorAt p) else None


    // Все кубики фигуры
    // TODO: How to memoize it?
    member f.Cubes =
        let this = f
        let cubeAt p = Cube.At(p, this.InternalColorAt p)
        f.Points |> List.map cubeAt


    /// Число кубиков в фигуре
    member inline f.CubeCount = f.Points.Length


