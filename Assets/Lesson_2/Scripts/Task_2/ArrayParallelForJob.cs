using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

namespace Lesson_2.Task_2
{
    public struct ArrayParallelForJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> Positions;

        [ReadOnly]
        public NativeArray<Vector3> Velocities;

        [WriteOnly]
        public NativeArray<Vector3> FinalPositions;

        public void Execute(int index)
        {
            FinalPositions[index] = Positions[index] + Velocities[index];
        }
    }
}