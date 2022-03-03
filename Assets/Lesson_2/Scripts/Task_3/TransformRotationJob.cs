using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Lesson_2.Task_3
{
    public struct TransformRotationJob : IJobParallelForTransform
    {
        public float RotationSpeed;
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            var rotation = transform.rotation.eulerAngles;
            rotation.y += RotationSpeed * DeltaTime;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}