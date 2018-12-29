namespace Furst

open UnityEngine;
open Unity.Jobs;
open Unity.Collections;
open Unity.Burst;
open Unity.Mathematics;

(*
    We can try using the |> pipe operator to compose numerical computation pipelines like Tensorflow.
    Also, weaving coroutines should be much faster. We finally get lots of ways to clean it up and compose them.
*)


// This is basically the typedef I was looking for in C#
type FixedQ15_16 =
    | Q15_16 of int32

    //static let Scale = 16 How? Is this because of single-choice type?

    static member Create(n:int) = 
        Q15_16(n <<< 16)

    static member Create(n:float32) =
        Q15_16((int)(n * (float32)(1 <<< 16)))

    static member (+) (Q15_16 a, Q15_16 b) = FixedQ15_16.Create(a + b);

module MyMath =
    let add(x,y) = x+y
    let addTyped(x:int, y:int) : int = x+y

    let evalAndAdd2 func = func 5 + 2 // evaluates function with 5, then adds 2 to result
    let evalWith5AsFloat (fn:int->float) = fn 5 // Explicitly type the input function

    let adderGenerator numberToAdd = (+) numberToAdd

    let inline sqr x = x * x // Inlining is dead-easy


type FixedPointTest() = 
    inherit MonoBehaviour()

    member this.Awake () =
        this.TestIntAdd()
        this.TestFixedAdd()

    member this.TestIntAdd () =
        let a = 2
        let b = 3
        let mutable c = 0

        let watch = System.Diagnostics.Stopwatch.StartNew()
        watch.Start()

        for i in 0 .. 9999999 do
            c <- a + b

        watch.Stop()
        sprintf "%i" watch.Elapsed.Ticks |> Debug.Log 

    member this.TestFixedAdd () =
        // Todo: this function deadlocks the editor for some reason
        let a = FixedQ15_16.Create 2
        let b = FixedQ15_16.Create 3
        let mutable c = FixedQ15_16.Create 0

        let watch = System.Diagnostics.Stopwatch.StartNew()
        watch.Start()

        for i in 0 .. 9999999 do
            c <- a + b

        watch.Stop()
        sprintf "%i" watch.Elapsed.Ticks |> Debug.Log 

    
[<BurstCompile>]
type AddJob = 
    struct
        val mutable X : NativeArray<float32>
        val mutable Y : NativeArray<float32>
        val mutable Z : NativeArray<float32>

        new(x : NativeArray<float32>, y : NativeArray<float32>, z : NativeArray<float32>) = {
            X=x; Y=y; Z=z;
        }
    end

    interface IJob with
        member this.Execute () =
            for i in 0 .. this.X.Length-1 do // Hah, for-in-do range is inclusive
                this.Z.[i] <- this.X.[i] + this.Y.[i]
   

type SimpleComponent() = 
    inherit MonoBehaviour()

    [<SerializeField>]
    let mutable changeSpeed = 2.0f
        
    member this.Transform : Transform = null

    member this.Update () =
        this.Transform.Translate(0.0f, 0.0f, changeSpeed * Time.deltaTime)