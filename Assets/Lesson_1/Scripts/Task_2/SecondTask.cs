using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Lesson_1.Task_2
{
    public class SecondTask : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Button _startTaskButton;
        [SerializeField] private Button _cancelButton;
        [Space]
        [SerializeField] private float _timeToWait = 1f;
        [SerializeField] private int _framesToWait = 60;

        private bool _areTasksRunning;

        #endregion


        #region UnityMethods

        void Start()
        {
            _startTaskButton?.onClick.AddListener(PerformTasks);
        }

        private void OnDestroy()
        {
            _startTaskButton?.onClick.RemoveAllListeners();
        }

        #endregion


        #region Methods

        private async void PerformTasks()
        {
            if (_areTasksRunning)
                return;

            Debug.Log("Tasks started");
            
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                _areTasksRunning = true;
                var ct = cts.Token;
                _cancelButton?.onClick.AddListener(() => { cts.Cancel(); });

                Task task1 = TimeBasedTask(ct);
                Task task2 = FrameBasedTask(ct);
                await Task.WhenAll(task1, task2);

                _cancelButton?.onClick.RemoveAllListeners();
                _areTasksRunning = false;
            }
        }

        private async Task TimeBasedTask(CancellationToken ct)
        {
            var time = 0f;

            while (time < _timeToWait)
            {
                if (ct.IsCancellationRequested)
                {
                    Debug.Log("Time Based Task aborted");
                    return;
                }

                await Task.Yield();
                time += Time.deltaTime;
            }

            Debug.Log("Time Based Task is done");
        }

        private async Task FrameBasedTask(CancellationToken ct)
        {
            var frames = 0;

            while (frames < _framesToWait)
            {
                if (ct.IsCancellationRequested)
                {
                    Debug.Log("Frame Based Task aborted");
                    return;
                }

                await Task.Yield();
                frames++;
            }

            Debug.Log("Frame Based Task is done");
        }

        #endregion
    }
}