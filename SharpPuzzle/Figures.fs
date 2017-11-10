module Figures

open Points
open Colors
open Cubes

/// Фигура
//  Разные повороты и отражения - это другие фигуры
type Figure = 
    {
        /// Цвет начальной точки (0, 0)
        OriginColor: Color         

        /// Какие смежные точки (относительные координаты) входят в фигуру
        Points: Point list    

    } with

    /// Обычный конструктор
    // TODO: Валидация смежности, нормализация координат и порядка точек
    static member FromPoints originColor points = 
        { OriginColor = originColor; Points = Point.Zero :: points |> List.distinct } 

    /// Компактный конструктор из координат
    static member FromCoords color coords = 
        let points = coords |> List.map Point.At 
        Figure.FromPoints color points

    /// Определяет цвет кубика
    /// Правильно использовать только для точек из списка фигуры
    member private f.InternalColorAt (p : Point) = 
        if IsOdd p then f.OriginColor.Inverted else f.OriginColor

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
        let cubeAt p = Cube.At(p, f.InternalColorAt p)
        f.Points |> List.map cubeAt

    /// Число кубиков в фигуре
    member f.CubeCount = f.Points.Length


