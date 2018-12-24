using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public class FSharpTest : MonoBehaviour {
    
    private void Awake() {
        int count = 1024;
        var a = new NativeArray<float>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        var b = new NativeArray<float>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        var c = new NativeArray<float>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);

        for (int i = 0; i < count; i++) {
            a[i] = 0.33333f;
            b[i] = i;
        }

        var j = new Furst.AddJob(a, b, c);
        var h = j.Schedule();
        h.Complete();

        for (int i = 0; i < a.Length; i++) {
            Debug.Log(c[i]);
        }

        // var aj = new Furst.AddJob();
        //aj.Execute() // Todo: Where did it go?

        // new Furst.MyThing(3).Print();

        a.Dispose();
        b.Dispose();
        c.Dispose();
    }
}