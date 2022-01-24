using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class InputSystem: MonoBehaviour
    {

        public enum EMoveDirection
        {
            Left,
            Right
        }

        public enum EAction
        {
            Move,
            Jump,
            Dash
        }
        
        public enum EActionStatus
        {
            Enter,
            Update,
            Exit,
            None
        }

        public static Action OnJumpEnter;
        public static Action OnJumpUpdate;
        public static Action OnJumpExit;
        
        public static Action OnDashEnter;
        public static Action OnDashUpdate;
        public static Action OnDashExit;
        
        public static Action<EMoveDirection> OnMoveEnter;
        public static Action<EMoveDirection> OnMoveUpdate;
        public static Action<EMoveDirection> OnMoveExit;

        private static Dictionary<EAction, EActionStatus> m_ActionStatus = new Dictionary<EAction, EActionStatus>()
        {
            { EAction.Jump, EActionStatus.None },
            { EAction.Move, EActionStatus.None },
            { EAction.Dash, EActionStatus.None }
        };

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
            HandleJump();
            HandleDash();
            HandleMove();
        }

        private void HandleDash()
        {
            switch (m_ActionStatus[EAction.Dash])
            {
                case EActionStatus.Enter:
                case EActionStatus.Update:
                    if (Input.GetAxisRaw("Dash") != 0)
                    {
                        m_ActionStatus[EAction.Dash] = EActionStatus.Update;
                        OnDashUpdate?.Invoke();
                    }
                    else
                    {
                        m_ActionStatus[EAction.Dash] = EActionStatus.Exit;
                        OnDashExit?.Invoke();
                    }
                    break;
                case EActionStatus.Exit:
                    m_ActionStatus[EAction.Dash] = EActionStatus.None;
                    break;
                case EActionStatus.None:
                    if (Input.GetAxisRaw("Dash") != 0)
                    {
                        m_ActionStatus[EAction.Dash] = EActionStatus.Enter;
                        OnDashEnter?.Invoke();
                    }
                    break;
                default:
                    m_ActionStatus[EAction.Jump] = EActionStatus.None; 
                    break;
            }
        }

        private void HandleMove()
        {
            float axis = Input.GetAxisRaw("Horizontal");
            bool bMoving = axis != 0f;
            EMoveDirection dir = axis < 0 ? EMoveDirection.Left : EMoveDirection.Right;
            
            switch (m_ActionStatus[EAction.Move])
            {
                case EActionStatus.Enter:
                case EActionStatus.Update:
                    if (bMoving)
                    {
                        m_ActionStatus[EAction.Move] = EActionStatus.Update;
                        OnMoveUpdate?.Invoke(dir);
                    }
                    else
                    {
                        m_ActionStatus[EAction.Move] = EActionStatus.Exit;
                        OnMoveExit?.Invoke(dir);
                    }
                    break;
                case EActionStatus.Exit:
                    m_ActionStatus[EAction.Move] = EActionStatus.None;
                    break;
                case EActionStatus.None:
                    if (bMoving)
                    {
                        m_ActionStatus[EAction.Move] = EActionStatus.Enter;
                        OnMoveEnter?.Invoke(dir);
                    }
                    break;
                default:
                    m_ActionStatus[EAction.Move] = EActionStatus.None; 
                    break;
            }
        }

        private void HandleJump()
        {
            switch (m_ActionStatus[EAction.Jump])
            {
                case EActionStatus.Enter:
                case EActionStatus.Update:
                    if (Input.GetAxisRaw("Jump") != 0)
                    {
                        m_ActionStatus[EAction.Jump] = EActionStatus.Update;
                        OnJumpUpdate?.Invoke();
                    }
                    else
                    {
                        m_ActionStatus[EAction.Jump] = EActionStatus.Exit;
                        OnJumpExit?.Invoke();
                    }
                    break;
                case EActionStatus.Exit:
                    m_ActionStatus[EAction.Jump] = EActionStatus.None;
                    break;
                case EActionStatus.None:
                    if (Input.GetAxisRaw("Jump") != 0)
                    {
                        m_ActionStatus[EAction.Jump] = EActionStatus.Enter;
                        OnJumpEnter?.Invoke();
                    }
                    break;
                default:
                    m_ActionStatus[EAction.Jump] = EActionStatus.None; break;
            }
        }
    }
}