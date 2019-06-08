module Boards

open System

open System.Runtime.CompilerServices

open Utils
open Colors
open Points
open Cubes
open Figures

            
/// Расположение фигуры на доске
/// Структура-запись, все комбинации значений полей - валидны
[<Struct; IsReadOnly>]
type FigurePlacement = 
    { 
        /// Фигура
        Figure: Figure; 

        /// Где находится первая точка фигуры (в координатах доски)
        Origin: Point; 
    } 

    /// Переводит точку из координат фигуры в координаты доски
    member inline fp.ToBoard p = Shift(fp.Origin.X, fp.Origin.Y) p

    /// Переводит кубик из координат фигуры в координаты доски сохраняя цвет
    member inline fp.ToBoard c = Cube.At(fp.ToBoard c.Position, c.Color)

    /// Все кубики фигуры в координатах доски
    // TODO: memoize???
    member inline fp.Cubes = 
        // Перевод кубика из координат фигуры в координаты доски
        fp.Figure.Cubes |> List.map fp.ToBoard

    /// Все точки фигуры в координатах доски
    // TODO: memoize???
    member inline fp.Points = 
        fp.Cubes |> List.map PositionOf

/// Все кубики фигуры в координатах доски
let inline CubesOf (fp: FigurePlacement) = fp.Cubes

/// Все точки фигуры в координатах доски
let inline PointsOf (fp: FigurePlacement) = fp.Points


/// Кубик на доске
/// Структура-запись, все комбинации значений полей - валидны
[<Struct; IsReadOnly>]
type CubeOnBoard =
    {
        /// Местоположение
        Position: Point;

        /// Цвет
        Color: Color;

        /// Номер фигуры, к которой принадлежит
        FigureIndex: int;
    }

let WithIndex index (c: Cube) = { Position = c.Position; Color = c.Color; FigureIndex = index }

let CubeMap cubeOnBoardList = 
    cubeOnBoardList 
    |> List.map (fun (cob: CubeOnBoard) -> cob.Position, cob) 
    |> Map.ofList

/// Состояние доски - не просто запись, а класс, т.к. не любая комбинация положений фигур - валидна
type BoardState private (figures, cubeMap) =

    /// Начальная координата доски
    static member Low = 1

    /// Конечная координата доски
    static member High = 8


    /// Удостоверяется, что точка в координатах доски (иначе - исключение)
    static member inline EnsurePointIsValid p =
        if p.X < BoardState.Low || p.X > BoardState.High then raise (ArgumentOutOfRangeException("p.X"))
        if p.Y < BoardState.Low || p.Y > BoardState.High then raise (ArgumentOutOfRangeException("p.Y"))
        p


    /// Фигурки на доске с указанием положения начальной точки
    member b.Figures: FigurePlacement list = figures

    /// Хранимая карта всех кубиков на доске
    member b.CubeMap: Map<Point, CubeOnBoard> = cubeMap

    /// Хеш-код состояния (не гарантирует равенства)
    member b.Hash : int = hash(cubeMap)


    /// Внутренний конструктор
    // Все остальные должны вызывать его
    // Не делает проверок для эффективности создания новых состояний
    static member private InternalCreateNew (figures, cubeMap) = 
        BoardState(figures, cubeMap)


    /// Пустая доска
    static member Empty = BoardState.InternalCreateNew (List.empty, Map.empty)


    /// Является ли состояние доски конечным
    member b.IsFinal = 
        b.CubeMap.Count = (BoardState.High - BoardState.Low + 1) * 2



    /// Открытый конструктор с набором фигурок
    static member WithFigures figurePlacements = 

        // Проверим все точки фигур на пересечение и корректность координат
        let allPoints = figurePlacements |> List.collect PointsOf |> List.map BoardState.EnsurePointIsValid
        if (List.distinct allPoints).Length <> allPoints.Length then raise (ArgumentException("Figures ovelaps", "figurePlacements"))
        
        // Построим CubeMap
        
        // Раскрывает индексированное размещение фигуры в ее кубики на доске
        let expandCubes (ndx : int, fp : FigurePlacement) = fp.Cubes |> List.map (WithIndex ndx)

        // Преобразуем все фигуры в кубики и карту
        let cubeMap = 
            figurePlacements             // : FigurePlacement list
            |> List.indexed              // : (int * FigurePlacement) list
            |> List.collect expandCubes  // : CubeOnBoard lit
            |> CubeMap

        BoardState.InternalCreateNew (figurePlacements, cubeMap)


    /// Определяет, что находится в заданной точке доски : CubeOnBoard или None
    member inline b.At p =
        b.CubeMap.TryFind (BoardState.EnsurePointIsValid p)


    /// Пуста ли указанная точка доски?
    member inline b.IsEmptyAt p = b.At(p) = None


    /// Можно ли разместить указанную фигуру в указанной точке?
    member b.IsPlacementAllowed (fp : FigurePlacement) = 
        fp.Points
        |> List.forall b.IsEmptyAt


    /// Размещение новой фигуры на доске -> новое состояние доски
    member b.Place (f: Figure) (o: Point) : BoardState =

        // Проверим
        let newPlacement = { Origin = o; Figure = f }
        if not (b.IsPlacementAllowed newPlacement) then invalidArg "f" "Figure can't be placed at specified point on board"

        // Добавим
        let newFigureIndex = b.Figures.Length + 1
        let newCubeMap = 
            newPlacement.Cubes 
            |> List.map (WithIndex newFigureIndex)
            |> CubeMap
        BoardState.InternalCreateNew (newPlacement :: b.Figures, JoinMaps b.CubeMap newCubeMap) 


    override b.GetHashCode() = b.Hash

    override b.Equals(o) = 
        match o with
        | :? BoardState as other -> b.Hash = other.Hash && b.CubeMap = other.CubeMap
        |_ -> false


        

        








