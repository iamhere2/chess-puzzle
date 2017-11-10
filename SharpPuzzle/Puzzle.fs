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

    } with

    /// Переводит кубик или точку из координат фигуры в координаты доски
    member inline fp.ToBoard x = Shift (fp.Origin.X, fp.Origin.Y) x

    /// Все кубики фигуры в координатах доски с цветом
    // TODO: memoize???
    member fp.Cubes = 
        // Перевод точки из координат фигуры в координаты доски
        fp.Figure.Cubes |> List.map fp.ToBoard


/// Состояние доски
type BoardState =
    {
        /// Фигурки на доске с указанием положения начальной точки
        Figures : FigurePlacement list

    } with

    /// Начальная координата доски
    static member Low = 1

    /// Конечная координата доски
    static member High = 8

    /// Пустая доска
    static member Empty : BoardState = { Figures = [] }


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

    /// Размещение новой фигуры на доске -> новая доска
    member b.Place (f : Figure) (o : Point) : BoardState =
        { Figures = { Origin = o; Figure = f } :: b.Figures }

    /// Является ли состояние доски конечным
    member b.IsFinal = 
        let pointCount fp = fp.Figure.CubeCount
        let count = List.sumBy pointCount b.Figures
        let size = (BoardState.High - BoardState.Low + 1) <<< 1
        let isFinal = (count = size)
        isFinal

    /// Можно ли разместить указанную фигуру в указанной точке?
    member b.IsPlacementAllowed (fp : FigurePlacement) = 
        let isEmpty c = b.At(c.X, c.Y) = None
        fp.Cubes
        |> List.forall isEmpty
        

        








