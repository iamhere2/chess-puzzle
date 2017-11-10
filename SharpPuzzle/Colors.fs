module Colors

/// Цвет кубика
type Color =
    | Black
    | White
    with
    /// Инверсия цвета
    member c.Inverted = if c = Black then White else Black


