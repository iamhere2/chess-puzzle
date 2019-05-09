module Colors

/// Цвет (кубика)
type Color =
    | Black
    | White

    /// Инверсия цвета
    member inline c.Inverted = if c = Black then White else Black


/// Инверсия цвета
let inline Invert (c: Color) = c.Inverted