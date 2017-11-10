module Points


/// Функция проверяет что две структуры с координатами (любого типа) являются 4-смежными
let inline public IsAdjacent a b =
        let a_X = (^a : (member X : int) a)
        let a_Y = (^a : (member Y : int) a)
        let b_X = (^b : (member X : int) b)
        let b_Y = (^b : (member Y : int) b)
        let dx = abs (a_X - b_X)
        let dy = abs (a_Y - b_Y)
        dx + dy <= 1 


/// Бинарный оператор проверяет что две структуры с координатами (любого типа) являются 4-смежными
let inline public (%) a b = IsAdjacent a b 


/// Функция определяет, четная или нечетная сумма координат - для "шашечек"
let inline public IsOdd x =
        let p_X = (^p : (member X : int) x)
        let p_Y = (^p : (member Y : int) x)
        ((p_X + p_Y) &&& 1) = 1


/// Функция смещает структуру с координатами (любого типа) 
let inline public Shift (dx, dy) x =
        (^p : (member Shift : int * int -> ^p) (x, dx, dy))


/// Функция смещает набор структур с координатами (любого типа) к началу координат (минимизирует значения координат)
let inline public ShiftToZero pl =
        // TODO: Определить minX и minY за один проход
        let minX = pl |> List.map (fun p -> (^p : (member X : int) p)) |> List.min
        let minY = pl |> List.map (fun p -> (^p : (member Y : int) p)) |> List.min
        let shiftToMin p = Shift (-minX, -minY) p
        pl |> List.map shiftToMin


/// Просто точка с относительными координатами и больше без ничего
[<Struct>]
type Point = 
    { 
        X: int 
        Y: int 
    } 
    with

    /// Нулевая точка
    static member Zero = { X = 0; Y = 0 }

    /// Преобразование из координат int * int в точку
    static member At(x, y) = { X = x; Y = y }

    /// Проверка положения точки
    member p.IsAt(x, y) = (p.X = x) && (p.Y = y)

    /// Возвращает (новую) точку со сдвигом
    member p.Shift (dx, dy) = { p with X = p.X + dx; Y = p.Y + dy }

    /// Проверка четности
    member p.IsOdd = IsOdd p

