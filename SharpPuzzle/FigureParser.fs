/// Модуль парсера фигур из текстового представления
module FigureParser

open Colors
open Points
open Cubes
open Figures

/// Парсит строку-картинку с фигурами,
/// возвращает список фигур
let ParseFiguresPicStr (ps : string) : Figure list = 

    // Так парсим одну строку в кубики
    let parseLine line = 
        let (y : int, s : string) = line
        let parseChar (i, c) = 
            match c with
            | 'W' -> Some (i, White)
            | 'B' -> Some (i, Black)
            | _   -> None
        let res = 
            s.ToCharArray()
            |> List.ofArray
            |> List.indexed
            |> List.choose parseChar
            |> List.map (fun (x, c) -> Cube.At(x, y, c))
        res

    // Парсим набор строк в набор кубиков
    let lines = ps.Split('\r', '\n')
    let cubes =
        lines
        |> List.ofArray 
        |> List.filter (fun (s) -> s.Length > 0)
        |> List.indexed
        |> List.collect parseLine

    // Кластеризуем набор кубиков 
    let clusters = ClusterizeByPosition cubes PositionOf

    // Преобразуем в фигуры
    clusters |> List.map Figure.FromCubes


