using UnityEngine;
using Mirror;
using System.Collections.Generic;

    public class PatrolPointManager : MonoBehaviour
    {
        public static PatrolPointManager Instance;

        public List<Transform> patrolPoints;

        void Awake()
        {
            Instance = this;
        }
    }
