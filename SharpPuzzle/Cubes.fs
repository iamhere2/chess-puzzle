module Cubes

open Points
open Colors

/// Кубик с относительными координатами и цветом
[<Struct>]
type Cube = 
    { 
        X: int
        Y: int
        Color : Color
    }
    with

    /// Возвращает кубик с тем же цветом, но со сдвигом
    member c.Shift (dx, dy) = 
        Cube.At(Shift (dx, dy) c.Point, c.Color)

    /// Возвращает координаты кубика в виде точки
    member c.Point = Point.At(c.X, c.Y)

    /// Проверяет, что кубик находится на указанных координатах
    member c.IsAt (x, y) = (c.X = x) && (c.Y = y)

    /// Создает кубик из координат и цвета 
    static member At(x, y, c) = { X = x; Y = y; Color = c }

    /// Создает кубик из точки и цвета 
    static member At(p, c) = Cube.At(p.X, p.Y, c)


/// Функция вытаскивает из кубика его координаты в виде точки
let public PointOf (c : Cube) = c.Point




