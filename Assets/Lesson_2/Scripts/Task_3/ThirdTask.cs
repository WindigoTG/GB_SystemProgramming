using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace Lesson_2.Task_3
{
    public class ThirdTask : MonoBehaviour
    {
        #region Fields

        [SerializeField] private int _amount;
        [SerializeField] private float _rotationSpeed;

        [SerializeField]
        private GameObject celestialBodyPrefab;

        private TransformAccessArray _transformAccessArray;

        #endregion


        #region Unity Methods

        void Start()
        {
            Transform[] transforms = new Transform[_amount];
            for (int i = 0; i < _amount; i++)
            {
                transforms[i] = Instantiate(celestialBodyPrefab, Random.insideUnitSphere * _amount, Quaternion.identity).transform;
            }

            _transformAccessArray = new TransformAccessArray(transforms);
        }

        private void Update()
        {
            var job = new TransformRotationJob
            {
                RotationSpeed = _rotationSpeed,
                DeltaTime = Time.deltaTime
            };

            var jobHandle = job.Schedule(_transformAccessArray);

            jobHandle.Complete();
        }

        private void OnDestroy()
        {
            _transformAccessArray.Dispose();
        }

        #endregion

    }
}