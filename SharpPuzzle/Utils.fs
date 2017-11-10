﻿module Utils

/// Кеширующая обертка над f: 'T -> 'U
//
//  see https://blogs.msdn.microsoft.com/dsyme/2007/05/31/a-sample-of-the-memoization-pattern-in-f/
//
let memoize f =
    let cache = ref Map.empty
    fun x ->
        match (!cache).TryFind(x) with
        | Some res -> res
        | None ->
             let res = f x
             cache := (!cache).Add(x,res)
             res