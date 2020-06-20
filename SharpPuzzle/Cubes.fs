/// Модуль работы с цветными кубиками
/// Для производительности многое объявлено как inline
module Cubes

open System.Runtime.CompilerServices

open Points
open Colors

/// Кубик с относительными координатами и цветом
/// Структура-запись, любые комбинации значений полей валидны 
[<Struct; IsReadOnly>]
type Cube = 
    { 
        Position: Point
        Color: Color
    }

    /// Возвращает кубик с тем же цветом, но со сдвигом
    member inline c.Shift (dx, dy) = Cube.At(Shift (dx, dy) c.Position, c.Color)

    /// Проверяет, что кубик находится на указанных координатах
    member inline c.IsAt (x, y) = c.Position.IsAt(x, y)

    /// Создает кубик из координат и цвета 
    static member inline At(x, y, c) = Cube.At(PointAt(x, y), c)

    /// Создает кубик из точки и цвета 
    static member inline At(p, c) = { Position = p; Color = c }


/// Функция вытаскивает из кубика его координаты в виде точки
let inline PositionOf (c: Cube) = c.Position






