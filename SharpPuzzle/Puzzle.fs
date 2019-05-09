module Puzzle

open System

open Points
open Cubes
open Figures

            
/// Расположение фигуры на доске
type FigurePlacement = 
    { 
        /// Начальная точка фигуры в координатах доски
        Origin : Point; 

        /// Фигура
        Figure : Figure; 
    } 

    /// Переводит точку из координат фигуры в координаты доски
    member inline fp.ToBoard p = Shift(fp.Origin.X, fp.Origin.Y) p

    /// Переводит кубик из координат фигуры в координаты доски сохраняя цвет
    member inline fp.ToBoard c = Cube.At(fp.ToBoard c.Position, c.Color)

    /// Все кубики фигуры в координатах доски
    // TODO: memoize???
    member fp.Cubes = 
        // Перевод точки из координат фигуры в координаты доски
        fp.Figure.Cubes |> List.map fp.ToBoard


/// Состояние доски - не просто запись, а класс, т.к. не любая комбинация положений фигур - валидна
type BoardState private (figures) =

    /// Фигурки на доске с указанием положения начальной точки
    member b.Figures : FigurePlacement list = figures

    /// Начальная координата доски
    static member Low = 1

    /// Конечная координата доски
    static member High = 8

    /// Внутренний конструктор
    // Все остальные должны вызывать его
    // Не делает проверок для эффективности создания новых состояний
    static member private InternalWithFigures figures = 
        BoardState(figures)

    /// Пустая доска
    static member Empty = BoardState []

    /// Пустая доска
    new() = BoardState []

    /// Открытый конструктор
    // TODO: Проверки корректности!!!
    static member WithFigures figures = 
        BoardState.InternalWithFigures figures

    /// Все кубики фигур с цветами, пронумерованные номерами фигур
    // TODO: memoize???
    member b.AllCubesIndexed =  
        /// Маркирует каждый элемент последовательности одинаковым значением
        let markEach mark s = Seq.map (fun x -> mark, x) s
        /// Раскрывает индексированное размещение фигуры в ее кубики в координатах доски с цветом
        let expandCubes (ndx : int, fp : FigurePlacement) = markEach ndx fp.Cubes

        b.Figures                   // : FigurePlacement list
        |> Seq.indexed              // : seq<int * FigurePlacement> 
        |> Seq.collect expandCubes  // : seq<int * Cube>
        |> Seq.toList


    /// Определяет, что находится в заданной точке доски : Cube с номером или None
    // TODO: memoize
    member b.At (x, y) =
        if x < BoardState.Low || x > BoardState.High then raise (ArgumentOutOfRangeException("x"))
        if y < BoardState.Low || y > BoardState.High then raise (ArgumentOutOfRangeException("y"))

        let isAt (c : Cube) = c.IsAt(x, y)
        b.AllCubesIndexed |> List.tryFind (snd >> isAt)

    /// Определяет, что находится в заданной точке доски : Cube с номером или None
    // TODO: memoize
    member b.At p = b.At(p.X, p.Y)


    /// Размещение новой фигуры на доске -> новая доска
    member b.Place (f : Figure) (o : Point) : BoardState =
        let newPlacement = { Origin = o; Figure = f }
        if b.IsPlacementAllowed newPlacement 
            then BoardState.InternalWithFigures (newPlacement :: b.Figures)
            else invalidArg "f" "Figure can't be placed at specified point on board"


    /// Является ли состояние доски конечным
    member b.IsFinal = 
        let pointCount fp = fp.Figure.CubeCount
        let count = List.sumBy pointCount b.Figures
        let size = (BoardState.High - BoardState.Low + 1) <<< 1
        let isFinal = (count = size)
        isFinal


    /// Можно ли разместить указанную фигуру в указанной точке?
    member b.IsPlacementAllowed (fp : FigurePlacement) = 
        let isEmpty c = b.At(c.Position) = None
        fp.Cubes
        |> List.forall isEmpty


    override b.GetHashCode() = hash(b.Figures)

    override b.Equals(o) = 
        match o with
        | :? BoardState as other -> b.Figures = other.Figures
        |_ -> false


        

        








