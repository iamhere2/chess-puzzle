﻿module FigureParser

open System

open Colors
open Points
open Cubes
open Figures

/// Парсит строку-картинку с фигурами,
/// возаращает список фигур
let ParseFiguresPicStr (ps : string) : Figure list = 

    // Сперва строки превратим в список точек

    // Так парсим одну строку
    let parseLine line = 
        let (y : int, s : string) = line
        let parseChar (i,c) = 
            match c with
            | 'W' -> Some (i,White)
            | 'B' -> Some (i,Black)
            | _   -> None
        let res = 
            s.ToCharArray()
            |> List.ofArray
            |> List.indexed
            |> List.choose parseChar
            |> List.map (fun (x,c) -> { X = x; Y = y; Color = c })
        res

    // Парсим набор строк в набор точек
    let seps = "\r\n".ToCharArray() 
    let lines = ps.Split(seps, StringSplitOptions.RemoveEmptyEntries)
    let pointList =
        lines
        |> List.ofArray 
        |> List.indexed
        |> List.map parseLine
        |> List.fold List.append [] 

    // Теперь кластеризуем набор точек на фигуры
    // по признаку смежности
    // (рекурсивно расширяем от каждой точки, потом выбрасываем дубли)
    let pointClusters : Cube list list =
        let isAdjacentToAny cpl cp =
            cpl |> List.exists (fun x -> x % cp)

        let expandToAdjacent cls = 
            let findNextAdjacentLayer prev =
                pointList 
                |> List.except prev
                |> List.filter (fun cp -> isAdjacentToAny prev cp)
            
            let rec expandRec prev =
                let next = findNextAdjacentLayer prev
                if List.isEmpty next then prev else expandRec (prev @ next)

            expandRec cls

        // Берем зачаток кластера от каждой точки
        // Расширяем каждый кластер до пределов смежности
        // Приводим к единообразному виду
        // Отбрасываем копии, начавшиеся от разных точек
        pointList 
        |> List.map (List.singleton >> expandToAdjacent >> List.sort)
        |> List.distinct          

    // Преобразуем в фигуры, попутно минимизируя координаты

    // ...и проверяя корректность указания цвета
    let validateColors pcl : Cube list = 
        // Корректный цвет совпадает с первой точкой при четной разнице координат
        // и противоположен при нечетной
        let headPoint = List.head pcl
        let headColor = headPoint.Color
        let isValid (pc : Cube) = 
            let isOdd = (abs(pc.X - headPoint.X) + abs(pc.Y - headPoint.Y)) &&& 1 = 1;
            let res = if isOdd then pc.Color = headColor.Inverted else pc.Color = headColor
            res
        let validated pc =
            if isValid pc then pc 
            else failwithf "Invalid colors in figure points: %A" pcl
        
        pcl |> List.map validated

    // ...получаем результирующий список фигур
    let createFigure (cls:Cube list) : Figure = 
        Figure.FromPoints cls.Head.Color (cls |> List.map (fun pc -> pc.Point))
    let figures =
        pointClusters |> List.map (ShiftToZero >> validateColors >> createFigure)
    figures


