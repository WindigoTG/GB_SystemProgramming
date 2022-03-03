using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using System.Text;

namespace Lesson_2.Task_2
{
    public class SecondTask : MonoBehaviour
    {
        #region Fields

        [SerializeField] private int _arraySize;

        #endregion


        #region Unity Methods

        void Start()
        {
            Vector3[] positions = new Vector3[_arraySize];
            Vector3[] velocities = new Vector3[_arraySize];
            Vector3[] finalPositions = new Vector3[_arraySize];

            for (int i = 0; i < positions.Length; i++)
                positions[i] = Random.insideUnitSphere;

            for (int i = 0; i < velocities.Length; i++)
                velocities[i] = Random.insideUnitSphere;

            Debug.Log("Positions:");
            PrintArray(positions);
            Debug.Log("Velocities:");
            PrintArray(velocities);

            NativeArray<Vector3> nativePositions = new NativeArray<Vector3>(positions, Allocator.TempJob);
            NativeArray<Vector3> nativeVelocities = new NativeArray<Vector3>(velocities, Allocator.TempJob);
            NativeArray<Vector3> nativeFinalPositions = new NativeArray<Vector3>(finalPositions, Allocator.TempJob);

            var job = new ArrayParallelForJob
            {
                Positions = nativePositions,
                Velocities = nativeVelocities,
                FinalPositions = nativeFinalPositions
            };

            var jobHandle = job.Schedule(_arraySize, 0);

            jobHandle.Complete();

            nativeFinalPositions.CopyTo(finalPositions);

            Debug.Log("Final Positions:");
            PrintArray(finalPositions);

            nativeFinalPositions.Dispose();
            nativeVelocities.Dispose();
            nativeFinalPositions.Dispose();
        }

        #endregion


        #region Methods

        private void PrintArray(Vector3[] array)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var value in array)
                sb.Append($"{value}    ");

            Debug.Log(sb);
        }

        #endregion
    }
}