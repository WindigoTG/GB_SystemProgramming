using Unity.Jobs;
using Unity.Collections;

namespace Lesson_2.Task_1
{
    public struct ArrayJob : IJob
    {
        public NativeArray<int> Values;

        public void Execute()
        {
            for (int i = 0; i < Values.Length; i++)
                if (Values[i] > 10)
                    Values[i] = 0;
        }
    }
}