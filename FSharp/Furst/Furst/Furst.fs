namespace Furst

open UnityEngine;
open Unity.Jobs;
open Unity.Collections;
open Unity.Burst;
open Unity.Mathematics;

// This is basically the typedef I was looking for in C#
type Q15_16 =
    Q15_16 of int32

module MyMath =
    let add(x,y) = x+y
    let addTyped(x:int, y:int) : int = x+y


type SimpleComponent() = 
    inherit MonoBehaviour()

    [<SerializeField>]
    let mutable changeSpeed = 2.0f
        
    member this.stuff = 42
    member this.Transform : Transform = null

    member this.Awake () =
        let mutable a = [| 1; 2; 3 |]
        Debug.Log(a.[0])
        Array.set a 0 4
        Debug.Log(a.[0])

        let watch = System.Diagnostics.Stopwatch.StartNew()
        watch.Start()

        let b = MyMath.add(1,2)
        Debug.Log(b)

        watch.Stop()
        Debug.Log watch.Elapsed

        this.Transform = this.gameObject.GetComponent<Transform>()

    member this.Update () =
        this.Transform.Translate(0.0f, 0.0f, changeSpeed * Time.deltaTime)



type MyThing(x: int) =
    let mutable X = x

    member this.print() = 
        Debug.Log(X);

    
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
   