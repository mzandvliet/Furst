namespace Furst

open UnityEngine;
open Unity.Jobs;
open Unity.Collections;
open Unity.Burst;


type SimpleComponent() = 
    inherit MonoBehaviour()

    [<SerializeField>]
    let mutable changeSpeed = 2.0f
        
    member this.stuff = 42

    member this.Awake () =
        let mutable a = [| 1; 2; 3 |]
        Debug.Log(a.[0])
        Array.set a 0 4
        Debug.Log(a.[0])
        
    member this.Update () =
        this.transform.Translate(0.0f, 0.0f, changeSpeed * Time.deltaTime);



type MyThing(x: int) =
    let mutable X = x

    member this.Print() = 
        Debug.Log(X);

    interface IJob with
        member this.Execute () =
            Debug.Log("Ought to be illegal");

    
    
[<BurstCompile>]
type AddJob = 
    struct
        val mutable X : NativeArray<float32>
        val mutable Y : NativeArray<float32>
        val mutable Z : NativeArray<float32>

        val t : float

        new(x : NativeArray<float32>, y : NativeArray<float32>, z : NativeArray<float32>) = { X=x; Y=y; Z=z; t=0.0 }
    end

    interface IJob with
        member this.Execute () =
            for i in 0 .. this.X.Length-1 do // Hah, for-in-do range is inclusive
                this.Z.[i] <- this.X.[i] + this.Y.[i]
    