// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Internal.Utilities.Library

open System
open System.Threading
open System.Collections.Generic
open System.Runtime.CompilerServices

[<Class>]
type InterruptibleLazy<'T> =
    new: valueFactory: (unit -> 'T) -> InterruptibleLazy<'T>

    member IsValueCreated: bool

    member Value: 'T
    member Force: unit -> 'T

    static member FromValue: value: 'T -> InterruptibleLazy<'T>

module InterruptibleLazy =
    val force: InterruptibleLazy<'T> -> 'T

[<AutoOpen>]
module internal PervasiveAutoOpens =
    /// Logical shift right treating int32 as unsigned integer.
    /// Code that uses this should probably be adjusted to use unsigned integer types.
    val (>>>&): x: int32 -> n: int32 -> int32

    val notlazy: v: 'a -> InterruptibleLazy<'a>

    val (|InterruptibleLazy|): l: InterruptibleLazy<'T> -> 'T

    val (|RecoverableException|_|): exn: Exception -> Exception voption

    val inline isNil: l: 'a list -> bool

    /// Returns true if the list has less than 2 elements. Otherwise false.
    val inline isNilOrSingleton: l: 'a list -> bool

    /// Returns true if the list contains exactly 1 element. Otherwise false.
    val inline isSingleton: l: 'a list -> bool

    val inline (===): x: 'a -> y: 'a -> bool when 'a: not struct

    /// Per the docs the threshold for the Large Object Heap is 85000 bytes: https://learn.microsoft.com/dotnet/standard/garbage-collection/large-object-heap#how-an-object-ends-up-on-the-large-object-heap-and-how-gc-handles-them
    /// We set the limit to be 80k to account for larger pointer sizes for when F# is running 64-bit.
    val LOH_SIZE_THRESHOLD_BYTES: int

    val reportTime: (string -> unit)

    /// Get an initialization hole
    val getHole: r: 'a option ref -> 'a

    type String with

        member inline StartsWithOrdinal: value: string -> bool

        member inline EndsWithOrdinal: value: string -> bool

        member inline EndsWithOrdinalIgnoreCase: value: string -> bool

        member inline IndexOfOrdinal: value: string -> int

        member inline IndexOfOrdinal: value: string * startIndex: int -> int

        member inline IndexOfOrdinal: value: string * startIndex: int * count: int -> int

    type Async with

        /// Runs the computation synchronously, always starting on the current thread.
        static member RunImmediate: computation: Async<'T> * ?cancellationToken: CancellationToken -> 'T

    val foldOn: p: ('a -> 'b) -> f: ('c -> 'b -> 'd) -> z: 'c -> x: 'a -> 'd

    val notFound: unit -> 'a

[<AbstractClass>]
type DelayInitArrayMap<'T, 'TDictKey, 'TDictValue> =
    new: f: (unit -> 'T[]) -> DelayInitArrayMap<'T, 'TDictKey, 'TDictValue>

    member GetArray: unit -> 'T[]
    member GetDictionary: unit -> IDictionary<'TDictKey, 'TDictValue>

    abstract CreateDictionary: 'T[] -> IDictionary<'TDictKey, 'TDictValue>

module internal Order =

    val orderBy: p: ('T -> 'U) -> IComparer<'T> when 'U: comparison and 'T: not null and 'T: not struct

    val orderOn: p: ('T -> 'U) -> pxOrder: IComparer<'U> -> IComparer<'T> when 'T: not null and 'T: not struct

    val toFunction: pxOrder: IComparer<'U> -> x: 'U -> y: 'U -> int

module internal Array =

    val mapq: f: ('a -> 'a) -> inp: 'a[] -> 'a[] when 'a: not struct

    val lengthsEqAndForall2: p: ('a -> 'b -> bool) -> l1: 'a[] -> l2: 'b[] -> bool

    val order: eltOrder: IComparer<'T> -> IComparer<'T array>

    val existsOne: p: ('a -> bool) -> l: 'a[] -> bool

    val existsTrue: arr: bool[] -> bool

    val findFirstIndexWhereTrue: arr: 'a[] -> p: ('a -> bool) -> int

    /// pass an array byref to reverse it in place
    val revInPlace: array: 'T[] -> unit

    /// Async implementation of Array.map.
    val mapAsync: mapping: ('T -> Async<'U>) -> array: 'T[] -> Async<'U[]>

    /// Returns a new array with an element replaced with a given value.
    val replace: index: int -> value: 'a -> array: 'a[] -> 'a[]

    /// Optimized arrays equality. ~100x faster than `array1 = array2` on strings.
    /// ~2x faster for floats
    /// ~0.8x slower for ints
    val inline areEqual: xs: 'T[] -> ys: 'T[] -> bool when 'T: equality

    /// Returns all heads of a given array.
    val heads: array: 'T[] -> 'T[][]

    /// Check if subArray is found in the wholeArray starting at the provided index
    val inline isSubArray: subArray: 'T[] -> wholeArray: 'T[] -> index: int -> bool when 'T: equality

    /// Returns true if one array has another as its subset from index 0.
    val startsWith: prefix: 'a[] -> whole: 'a[] -> bool when 'a: equality

    /// Returns true if one array has trailing elements equal to another's.
    val endsWith: suffix: 'a[] -> whole: 'a[] -> bool when 'a: equality

    val prepend: item: 'T -> array: 'T[] -> 'T[]

module internal Option =

    val mapFold: f: ('a -> 'b -> 'c * 'a) -> s: 'a -> opt: 'b option -> 'c option * 'a

    val attempt: f: (unit -> 'T) -> 'T option

module internal List =

    val sortWithOrder: c: IComparer<'T> -> elements: 'T list -> 'T list

    val splitAfter: n: int -> l: 'a list -> 'a list * 'a list

    val existsi: f: (int -> 'a -> bool) -> xs: 'a list -> bool

    val lengthsEqAndForall2: p: ('a -> 'b -> bool) -> l1: 'a list -> l2: 'b list -> bool

    val findi: n: int -> f: ('a -> bool) -> l: 'a list -> ('a * int) option

    val splitChoose: select: ('a -> Choice<'b, 'c>) -> l: 'a list -> 'b list * 'c list

    val checkq: l1: 'a list -> l2: 'a list -> bool when 'a: not struct

    val mapq: f: ('T -> 'T) -> inp: 'T list -> 'T list when 'T: not struct

    val frontAndBack: l: 'a list -> 'a list * 'a

    val tryFrontAndBack: l: 'a list -> ('a list * 'a) option

    val tryRemove: f: ('a -> bool) -> inp: 'a list -> ('a * 'a list) option

    val zip4: l1: 'a list -> l2: 'b list -> l3: 'c list -> l4: 'd list -> ('a * 'b * 'c * 'd) list

    val unzip4: l: ('a * 'b * 'c * 'd) list -> 'a list * 'b list * 'c list * 'd list

    val iter3: f: ('a -> 'b -> 'c -> unit) -> l1: 'a list -> l2: 'b list -> l3: 'c list -> unit

    val takeUntil: p: ('a -> bool) -> l: 'a list -> 'a list * 'a list

    val order: eltOrder: IComparer<'T> -> IComparer<'T list>

    val indexNotFound: unit -> 'a

    val assoc: x: 'a -> l: ('a * 'b) list -> 'b when 'a: equality

    val memAssoc: x: 'a -> l: ('a * 'b) list -> bool when 'a: equality

    val memq: x: 'a -> l: 'a list -> bool when 'a: not struct

    val mapNth: n: int -> f: ('a -> 'a) -> xs: 'a list -> 'a list

    val count: pred: ('a -> bool) -> xs: 'a list -> int

    val headAndTail: l: 'a list -> 'a * 'a list

    // WARNING: not tail-recursive
    val mapHeadTail: fhead: ('a -> 'b) -> ftail: ('a -> 'b) -> _arg1: 'a list -> 'b list

    val collectFold: f: ('a -> 'b -> 'c list * 'a) -> s: 'a -> l: 'b list -> 'c list * 'a

    val collect2: f: ('a -> 'b -> 'c list) -> xs: 'a list -> ys: 'b list -> 'c list

    val toArraySquared: xss: 'a list list -> 'a[][]

    val iterSquared: f: ('a -> unit) -> xss: 'a list list -> unit

    val collectSquared: f: ('a -> 'b list) -> xss: 'a list list -> 'b list

    val mapSquared: f: ('a -> 'b) -> xss: 'a list list -> 'b list list

    val mapFoldSquared: f: ('a -> 'b -> 'c * 'a) -> z: 'a -> xss: 'b list list -> 'c list list * 'a

    val forallSquared: f: ('a -> bool) -> xss: 'a list list -> bool

    val mapiSquared: f: (int -> int -> 'a -> 'b) -> xss: 'a list list -> 'b list list

    val existsSquared: f: ('a -> bool) -> xss: 'a list list -> bool

    val mapiFoldSquared: f: ('a -> int * int * 'b -> 'c * 'a) -> z: 'a -> xss: 'b list list -> 'c list list * 'a

    val duplicates: xs: 'T list -> 'T list when 'T: equality

    val internal allEqual: xs: 'T list -> bool when 'T: equality

    val isSingleton: xs: 'T list -> bool

    val prependIfSome: x: 'a option -> l: 'a list -> 'a list

    val vMapFold<'T, 'State, 'Result> :
        mapping: ('State -> 'T -> struct ('Result * 'State)) ->
        state: 'State ->
        list: 'T list ->
            struct ('Result list * 'State)

module internal ResizeArray =

    /// Split a ResizeArray into an array of smaller chunks.
    /// This requires `items/chunkSize` Array copies of length `chunkSize` if `items/chunkSize % 0 = 0`,
    /// otherwise `items/chunkSize + 1` Array copies.
    val chunkBySize: chunkSize: int -> f: ('t -> 'a) -> items: ResizeArray<'t> -> 'a[][]

    /// Split a large ResizeArray into a series of array chunks that are each under the Large Object Heap limit.
    /// This is done to help prevent a stop-the-world collection of the single large array, instead allowing for a greater
    /// probability of smaller collections. Stop-the-world is still possible, just less likely.
    val mapToSmallArrayChunks: f: ('t -> 'a) -> inp: ResizeArray<'t> -> 'a[][]

module internal Span =
    val inline exists: predicate: ('T -> bool) -> span: Span<'T> -> bool

module internal String =

    val make: n: int -> c: char -> string

    val get: str: string -> i: int -> char

    val sub: s: string -> start: int -> len: int -> string

    val contains: s: string -> c: char -> bool

    val order: IComparer<string>

    val lowercase: s: string -> string

    val uppercase: s: string -> string

    val isLeadingIdentifierCharacterUpperCase: s: string -> bool

    val capitalize: s: string -> string

    val uncapitalize: s: string -> string

    val dropPrefix: s: string -> t: string -> string

    val dropSuffix: s: string -> t: string -> string

    val inline toCharArray: str: string -> char[]

    val lowerCaseFirstChar: str: string -> string

    val extractTrailingIndex: str: string -> string * int option

    /// Splits a string into substrings based on the strings in the array separators
    val split: options: StringSplitOptions -> separator: string[] -> value: string -> string[]

    val (|StartsWith|_|): pattern: string -> value: string -> unit option

    val (|Contains|_|): pattern: string -> value: string | null -> unit option

    val getLines: str: string -> string[]

module internal Dictionary =
    val inline newWithSize: size: int -> Dictionary<'a, 'b> when 'a: equality
    val inline ofList: xs: ('Key * 'Value) list -> Dictionary<'Key, 'Value> when 'Key: equality

[<Extension; Class>]
type internal DictionaryExtensions =

    [<Extension>]
    static member inline BagAdd: dic: Dictionary<'key, 'value list> * key: 'key * value: 'value -> unit

    [<Extension>]
    static member inline BagExistsValueForKey:
        dic: Dictionary<'key, 'value list> * key: 'key * f: ('value -> bool) -> bool

module internal Lazy =
    val force: x: Lazy<'T> -> 'T

/// Represents a permission active at this point in execution
type internal ExecutionToken = interface end

/// Represents a token that indicates execution on the compilation thread, i.e.
///   - we have full access to the (partially mutable) TAST and TcImports data structures
///   - compiler execution may result in type provider invocations when resolving types and members
///   - we can access various caches in the SourceCodeServices
///
/// Like other execution tokens this should be passed via argument passing and not captured/stored beyond
/// the lifetime of stack-based calls. This is not checked, it is a discipline within the compiler code.
[<Sealed>]
type internal CompilationThreadToken =

    interface ExecutionToken
    new: unit -> CompilationThreadToken

/// Represents a token that indicates execution on any of several potential user threads calling the F# compiler services.
[<Sealed>]
type internal AnyCallerThreadToken =

    interface ExecutionToken
    new: unit -> AnyCallerThreadToken

/// A base type for various types of tokens that must be passed when a lock is taken.
/// Each different static lock should declare a new subtype of this type.
type internal LockToken =
    interface
        inherit ExecutionToken
    end

/// Encapsulates a lock associated with a particular token-type representing the acquisition of that lock.
type internal Lock<'LockTokenType when 'LockTokenType :> LockToken> =

    new: unit -> Lock<'LockTokenType>
    member AcquireLock: f: ('LockTokenType -> 'a) -> 'a

[<AutoOpen>]
module internal LockAutoOpens =
    /// Represents a place where we are stating that execution on the compilation thread is required. The
    /// reason why will be documented in a comment in the code at the callsite.
    val RequireCompilationThread: _ctok: CompilationThreadToken -> unit

    /// Represents a place in the compiler codebase where we are passed a CompilationThreadToken unnecessarily.
    /// This represents code that may potentially not need to be executed on the compilation thread.
    val DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent: _ctok: CompilationThreadToken -> unit

    /// Represents a place in the compiler codebase where we assume we are executing on a compilation thread
    val AssumeCompilationThreadWithoutEvidence: unit -> CompilationThreadToken

    val AnyCallerThread: AnyCallerThreadToken

    val AssumeLockWithoutEvidence: unit -> #LockToken

module internal Map =
    val tryFindMulti: k: 'a -> map: Map<'a, 'b list> -> 'b list when 'a: comparison

[<Struct>]
type internal ResultOrException<'TResult> =
    | Result of result: 'TResult
    | Exception of ``exception``: Exception

module internal ResultOrException =

    val success: a: 'a -> ResultOrException<'a>

    val raze: b: exn -> ResultOrException<'a>

    val (|?>): res: ResultOrException<'a> -> f: ('a -> 'b) -> ResultOrException<'b>

    val ForceRaise: res: ResultOrException<'a> -> 'a

    val otherwise: f: (unit -> ResultOrException<'a>) -> x: ResultOrException<'a> -> ResultOrException<'a>

/// Generates unique stamps
type internal UniqueStampGenerator<'T when 'T: equality and 'T: not null> =

    new: unit -> UniqueStampGenerator<'T>

    member Encode: str: 'T -> int

    member Table: ICollection<'T>

/// Memoize tables (all entries cached, never collected unless whole table is collected)
type internal MemoizationTable<'T, 'U when 'T: not null> =

    new:
        compute: ('T -> 'U) * keyComparer: IEqualityComparer<'T> * ?canMemoize: ('T -> bool) -> MemoizationTable<'T, 'U>

    member Apply: x: 'T -> 'U

/// A thread-safe lookup table which is assigning an auto-increment stamp with each insert
type internal StampedDictionary<'T, 'U when 'T: not null> =

    new: keyComparer: IEqualityComparer<'T> -> StampedDictionary<'T, 'U>

    member Add: key: 'T * value: 'U -> unit

    member UpdateIfExists: key: 'T * valueReplaceFunc: ('U -> 'U option) -> unit

    member GetAll: unit -> seq<'T * (int * 'U)>

exception internal UndefinedException

type internal LazyWithContextFailure =

    new: exn: exn -> LazyWithContextFailure

    member Exception: exn

    static member Undefined: LazyWithContextFailure

/// Just like "Lazy" but EVERY forcer must provide an instance of "ctxt", e.g. to help track errors
/// on forcing back to at least one sensible user location
[<Sealed>]
type internal LazyWithContext<'T, 'ctxt> =
    static member Create: f: ('ctxt -> 'T) * findOriginalException: (exn -> exn) -> LazyWithContext<'T, 'ctxt>
    static member NotLazy: x: 'T -> LazyWithContext<'T, 'ctxt>
    member Force: ctxt: 'ctxt -> 'T
    member UnsynchronizedForce: ctxt: 'ctxt -> 'T
    member IsDelayed: bool
    member IsForced: bool

/// Intern tables to save space.
module internal Tables =
#if NET8_0_OR_GREATER
    val memoize: f: ('a -> 'b) -> ('a -> 'b) when 'a: equality and 'a: not null
#else
    val memoize: f: ('a -> 'b) -> ('a -> 'b) when 'a: equality
#endif

/// Interface that defines methods for comparing objects using partial equality relation
type internal IPartialEqualityComparer<'T> =
    inherit IEqualityComparer<'T>
    abstract InEqualityRelation: 'T -> bool

/// Interface that defines methods for comparing objects using partial equality relation
module internal IPartialEqualityComparer =
    val On:
        f: ('a -> 'b) -> c: IPartialEqualityComparer<'b> -> IPartialEqualityComparer<'a>
            when 'a: not null and 'a: not struct

    /// Like Seq.distinctBy but only filters out duplicates for some of the elements
    val partialDistinctBy: per: IPartialEqualityComparer<'T> -> seq: 'T list -> 'T list

type internal NameMap<'T> = Map<string, 'T>

type internal NameMultiMap<'T> = NameMap<'T list>

type internal MultiMap<'T, 'U when 'T: comparison> = Map<'T, 'U list>

module internal NameMap =

    val empty: Map<'a, 'b> when 'a: comparison

    val range: m: Map<'a, 'b> -> 'b list when 'a: comparison

    val foldBack: f: (string -> 'T -> 'a -> 'a) -> m: NameMap<'T> -> z: 'a -> 'a

    val forall: f: ('a -> 'b -> bool) -> m: Map<'a, 'b> -> bool when 'a: comparison

    val exists: f: ('a -> 'b -> bool) -> m: Map<'a, 'b> -> bool when 'a: comparison

    val ofKeyedList: f: ('a -> 'b) -> l: 'a list -> Map<'b, 'a> when 'b: comparison

    val ofList: l: (string * 'T) list -> NameMap<'T>

    val ofSeq: l: seq<string * 'T> -> NameMap<'T>

    val toList: l: NameMap<'T> -> (string * 'T) list

    val layer: m1: NameMap<'T> -> m2: Map<string, 'T> -> Map<string, 'T>

    /// Not a very useful function - only called in one place - should be changed
    val layerAdditive:
        addf: ('a list -> 'b -> 'a list) -> m1: Map<'c, 'b> -> m2: Map<'c, 'a list> -> Map<'c, 'a list>
            when 'c: comparison

    /// Union entries by identical key, using the provided function to union sets of values
    val union: unionf: (seq<'a> -> 'b) -> ms: seq<NameMap<'a>> -> Map<string, 'b>

    /// For every entry in m2 find an entry in m1 and fold
    val subfold2:
        errf: ('a -> 'b -> 'c) -> f: ('a -> 'd -> 'b -> 'c -> 'c) -> m1: Map<'a, 'd> -> m2: Map<'a, 'b> -> acc: 'c -> 'c
            when 'a: comparison

    val suball2:
        errf: ('a -> 'b -> bool) -> p: ('c -> 'b -> bool) -> m1: Map<'a, 'c> -> m2: Map<'a, 'b> -> bool
            when 'a: comparison

    val mapFold: f: ('a -> string -> 'T -> 'b * 'a) -> s: 'a -> l: NameMap<'T> -> Map<string, 'b> * 'a

    val foldBackRange: f: ('T -> 'a -> 'a) -> l: NameMap<'T> -> acc: 'a -> 'a

    val filterRange: f: ('T -> bool) -> l: NameMap<'T> -> Map<string, 'T>

    val mapFilter: f: ('T -> 'a option) -> l: NameMap<'T> -> Map<string, 'a>

    val map: f: ('T -> 'a) -> l: NameMap<'T> -> Map<string, 'a>

    val iter: f: ('T -> unit) -> l: NameMap<'T> -> unit

    val partition: f: ('T -> bool) -> l: NameMap<'T> -> Map<string, 'T> * Map<string, 'T>

    val mem: v: string -> m: NameMap<'T> -> bool

    val find: v: string -> m: NameMap<'T> -> 'T

    val tryFind: v: string -> m: NameMap<'T> -> 'T option

    val add: v: string -> x: 'T -> m: NameMap<'T> -> Map<string, 'T>

    val isEmpty: m: NameMap<'T> -> bool

    val existsInRange: p: ('a -> bool) -> m: Map<'b, 'a> -> bool when 'b: comparison

    val tryFindInRange: p: ('a -> bool) -> m: Map<'b, 'a> -> 'a option when 'b: comparison

module internal NameMultiMap =

    val existsInRange: f: ('T -> bool) -> m: NameMultiMap<'T> -> bool

    val find: v: string -> m: NameMultiMap<'T> -> 'T list

    val add: v: string -> x: 'T -> m: NameMultiMap<'T> -> Map<string, 'T list>

    val range: m: NameMultiMap<'T> -> 'T list

    val rangeReversingEachBucket: m: NameMultiMap<'T> -> 'T list

    val chooseRange: f: ('T -> 'a option) -> m: NameMultiMap<'T> -> 'a list

    val map: f: ('T -> 'a) -> m: NameMultiMap<'T> -> Map<string, 'a list>

    val empty: NameMultiMap<'T>

    val initBy: f: ('T -> string) -> xs: seq<'T> -> NameMultiMap<'T>

    val ofList: xs: (string * 'T) list -> NameMultiMap<'T>

module internal MultiMap =

    val existsInRange: f: ('a -> bool) -> m: MultiMap<'b, 'a> -> bool when 'b: comparison

    val find: v: 'a -> m: MultiMap<'a, 'b> -> 'b list when 'a: comparison

    val add: v: 'a -> x: 'b -> m: MultiMap<'a, 'b> -> Map<'a, 'b list> when 'a: comparison

    val range: m: MultiMap<'a, 'b> -> 'b list when 'a: comparison

    val empty: MultiMap<'a, 'b> when 'a: comparison

    val initBy: f: ('a -> 'b) -> xs: seq<'a> -> MultiMap<'b, 'a> when 'b: comparison

    val ofList: xs: ('a * 'b) list -> MultiMap<'a, 'b> when 'a: comparison

type internal LayeredMap<'Key, 'Value when 'Key: comparison> = Map<'Key, 'Value>

[<AutoOpen>]
module internal MapAutoOpens =
    type internal Map<'Key, 'Value when 'Key: comparison> with

        static member Empty: Map<'Key, 'Value> when 'Key: comparison

#if FSHARPCORE_USE_PACKAGE
        member Values: 'Value list
#endif

        member AddMany: kvs: KeyValuePair<'Key, 'Value>[] -> Map<'Key, 'Value> when 'Key: comparison

        member AddOrModify: key: 'Key * f: ('Value option -> 'Value) -> Map<'Key, 'Value> when 'Key: comparison

/// Immutable map collection, with explicit flattening to a backing dictionary
[<Sealed>]
type internal LayeredMultiMap<'Key, 'Value when 'Key: comparison> =

    new: contents: LayeredMap<'Key, 'Value list> -> LayeredMultiMap<'Key, 'Value>

    member Add: k: 'Key * v: 'Value -> LayeredMultiMap<'Key, 'Value>

    member AddMany: kvs: KeyValuePair<'Key, 'Value>[] -> LayeredMultiMap<'Key, 'Value>

    member TryFind: k: 'Key -> 'Value list option

    member TryGetValue: k: 'Key -> bool * 'Value list

    member Item: k: 'Key -> 'Value list with get

    member Values: 'Value list

    static member Empty: LayeredMultiMap<'Key, 'Value>
