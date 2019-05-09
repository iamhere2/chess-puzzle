/// Модуль работы с точками (парами координат)
/// Для производительности многое объявлено как inline
module Points

open System.Runtime.CompilerServices

/// Просто точка с относительными координатами и больше без ничего
/// Структура-запись, все комбинации значений полей - валидны
[<Struct; IsReadOnly>]
type Point = 
    { 
        X: int 
        Y: int 
    } 

    /// Нулевая точка
    static member inline Zero = { X = 0; Y = 0 }

    /// Преобразование из координат int * int в точку
    static member inline At(x, y) = { X = x; Y = y }

    /// Проверка положения точки
    member inline p.IsAt(x, y) = (p.X = x) && (p.Y = y)

    /// Возвращает (новую) точку со сдвигом
    member inline p.Shift (dx, dy) = { X = p.X + dx; Y = p.Y + dy }

    /// Проверка четности суммы координат
    member inline p.IsOdd = ((p.X + p.Y) &&& 1) = 1


/// Проверка того, что две точки 4-смежны (и не совпадают)
let inline IsAdjacent a b =
    let dx = abs (a.X - b.X)
    let dy = abs (a.Y - b.Y)
    dx + dy = 1


/// Бинарный оператор проверки смежности точек
let inline (%) a b = IsAdjacent a b 


/// Определяет, четная или нечетная сумма координат - для "шашечек"
let inline IsOdd (p : Point) = p.IsOdd


/// Сдвигает точку
let inline Shift (dx, dy) (p : Point) = p.Shift (dx, dy)


/// Смещает набор точек к началу координат (минимизирует значения координат)
let inline ShiftToZero points =
        // TODO: Определить minX и minY за один проход
        let minX = points |> List.map (fun p -> p.X) |> List.min
        let minY = points |> List.map (fun p -> p.Y) |> List.min
        let shiftToMin p = Shift (-minX, -minY) p
        points |> List.map shiftToMin


/// Отбирает точки, смежные с данной (но не совпадающие с ней)
let SelectAdjacentPoints points origin =
    points |> List.filter (IsAdjacent origin)


/// Отбирает позиционированные элементы, смежные с данным
let SelectAdjacent (items : 'a list) (pos : 'a -> Point) origin =
    let originPos = pos origin
    items |> List.filter (fun x -> (pos x) % originPos)


/// Отбирает кластер смежности - позиционированные элементы, 
/// смежные друг с другом, начиная с указанного элемента в указанном списке
let SelectAdjacentCluster (items : 'a list) (pos : 'a -> Point) (origin : 'a) =
    let rec nextLayersRec (rest : 'a list) (prevLayer : 'a list) =
        if List.isEmpty rest || List.isEmpty prevLayer then [] 
        else
            let nextLayer = 
                prevLayer 
                |> List.collect (SelectAdjacent rest pos) 
                |> List.except prevLayer 
                |> List.distinct
            let nextRest = rest |> List.except nextLayer
            nextLayer @ nextLayersRec nextRest nextLayer
    origin :: nextLayersRec (items |> List.except [origin]) [origin]


/// Кластеризация списка любых элементов, размещенных по координатам
let rec ClusterizeByPosition (items : 'a list) (pos : 'a -> Point) =
    if List.isEmpty items then []
    else
        // Берем первый элемент и наращиваем до смежного кластера - это первый кластер
        // потом выкидываем элементы этого кластера из списка 
        // и повторяем, пока список не пуст
        let cluster = SelectAdjacentCluster items pos items.Head
        let rest = items |> List.except cluster
        [cluster] @ ClusterizeByPosition rest pos

