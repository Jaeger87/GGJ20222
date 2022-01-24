using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class InputSystem: MonoBehaviour
    {

        public enum EAction
        {
            MoveLeft,
            MoveRight,
            MoveDown,
            MoveUp,
            Jump,
            Dash
        }

        public static Action OnJump;

        private static InputSystem Instance;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(Instance);
            }
        }

        private void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        public static bool GetAction(EAction i_Action)
        {
            bool result = false;
            
            // Instance.m_ActionStatus.TryGetValue(i_Action, out result);
            
            return result;
        }
    }
}