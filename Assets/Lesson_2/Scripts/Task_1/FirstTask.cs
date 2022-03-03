using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using System.Text;

namespace Lesson_2.Task_1
{
    public class FirstTask : MonoBehaviour
    {
        #region Fields

        [SerializeField] private int _arraySize;

        #endregion


        #region Unity Methods

        void Start()
        {
            int[] array = new int[_arraySize];

            for (int i = 0; i < array.Length; i++)
                array[i] = Random.Range(1, 21);

            PrintArray(array);

            NativeArray<int> nativeArray = new NativeArray<int>(array, Allocator.TempJob);

            var job = new ArrayJob { Values = nativeArray };
            var jobHandle = job.Schedule();
            jobHandle.Complete();

            nativeArray.CopyTo(array);

            nativeArray.Dispose();

            PrintArray(array);
        }

        #endregion


        #region Methods

        private void PrintArray(int[] array)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var value in array)
                sb.Append($"{value}    ");

            Debug.Log(sb);
        }

        #endregion
    }
}