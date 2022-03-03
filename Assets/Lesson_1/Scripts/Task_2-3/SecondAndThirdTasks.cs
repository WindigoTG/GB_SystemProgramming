using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Lesson_1.Task_2_3
{
    public class SecondAndThirdTasks : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Button _startTaskButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _startWhatFasterTaskButton;
        [SerializeField] private Button _cancelWhatFasterButton;
        [Space]
        [SerializeField] private float _timeToWait = 1f;
        [SerializeField] private int _framesToWait = 60;

        private bool _areTasksRunning;
        private bool _areWhatFasterTasksRunning;

        private CancellationTokenSource _whatFasterCts;

        #endregion


        #region UnityMethods

        void Start()
        {
            _startTaskButton?.onClick.AddListener(PerformTasksAsync);
            _startWhatFasterTaskButton?.onClick.AddListener(WhatTaskFasterAsync);
        }

        private void OnDestroy()
        {
            _startTaskButton?.onClick.RemoveAllListeners();
            _startWhatFasterTaskButton?.onClick.RemoveAllListeners();
            _whatFasterCts?.Dispose();
        }

        #endregion


        #region Methods

        private async void PerformTasksAsync()
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

        private async void WhatTaskFasterAsync()
        {
            if (_areWhatFasterTasksRunning)
                return;

            _whatFasterCts = new CancellationTokenSource();

            Debug.Log("WhatFaster tasks started\n" +
                "Task 1 - Time Based  |  Task 2 = Frame Based");

            _areWhatFasterTasksRunning = true;
            var ct = _whatFasterCts.Token;

            Task task1 = TimeBasedTask(ct); 
            Task task2 = FrameBasedTask(ct);
            bool finishedTask = await WhatTaskFasterAsync(ct, task1, task2);

            Debug.Log($"Task {(finishedTask ? 1 : 2)} was faster");

            _areWhatFasterTasksRunning = false;

            _whatFasterCts?.Dispose();
        }

        public async Task<bool> WhatTaskFasterAsync(CancellationToken ct, Task task1, Task task2)
        {
            
            Task finishedTask = await Task.WhenAny(task1, task2);
            _whatFasterCts.Cancel();

            return finishedTask == task1;
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
            return;
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
            return;
        }

        #endregion
    }
}