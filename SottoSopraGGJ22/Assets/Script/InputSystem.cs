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
            Right,
            Up,
            Down
        }

        public enum EAction
        {
            MoveHorizontal,
            MoveVertical,
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
        
        public static Action<EMoveDirection, float> OnMoveHorizontalEnter;
        public static Action<EMoveDirection, float> OnMoveHorizontalUpdate;
        public static Action<EMoveDirection, float> OnMoveHorizontalExit;
        
        public static Action<EMoveDirection, float> OnMoveVerticalEnter;
        public static Action<EMoveDirection, float> OnMoveVerticalUpdate;
        public static Action<EMoveDirection, float> OnMoveVerticalExit;

        private static Dictionary<EAction, EActionStatus> m_ActionStatus = new Dictionary<EAction, EActionStatus>()
        {
            { EAction.Jump, EActionStatus.None },
            { EAction.MoveHorizontal, EActionStatus.None },
            { EAction.MoveVertical, EActionStatus.None },
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
            HandleMoveHorizontal();
            HandleMoveVertical();
        }

        private void HandleDash()
        {
            switch (m_ActionStatus[EAction.Dash])
            {
                case EActionStatus.Enter: case EActionStatus.Update:
                    HandleDashCaseEnterUpdate();
                    break;
                case EActionStatus.Exit:
                    m_ActionStatus[EAction.Dash] = EActionStatus.None;
                    break;
                case EActionStatus.None:
                    HandleDashCaseNone();
                    break;
                default:
                    m_ActionStatus[EAction.Jump] = EActionStatus.None; 
                    break;
            }
        }


        private void HandleDashCaseEnterUpdate()
        {
            if (Input.GetAxisRaw("Fire2") != 0)
            {
                m_ActionStatus[EAction.Dash] = EActionStatus.Update;
                OnDashUpdate?.Invoke();
            }
            else
            {
                m_ActionStatus[EAction.Dash] = EActionStatus.Exit;
                OnDashExit?.Invoke();
            }
        }

        private void HandleDashCaseNone()
        {
            if (Input.GetAxisRaw("Fire2") != 0)
            {
                m_ActionStatus[EAction.Dash] = EActionStatus.Enter;
                OnDashEnter?.Invoke();
            }
        }
        private void HandleMoveHorizontal()
        {
            float Axis = Input.GetAxisRaw("Horizontal");
            bool bMoving = Axis != 0f;
            EMoveDirection Direction = Axis < 0 ? EMoveDirection.Left : EMoveDirection.Right;
            HandleMoveHorizontalSwitch(bMoving, Direction, Axis);
        }

        private void HandleMoveHorizontalSwitch(bool bMoving, EMoveDirection Direction, float Axis)
        {
            switch (m_ActionStatus[EAction.MoveHorizontal]) {
                case EActionStatus.Enter:
                case EActionStatus.Update:
                    HandleMoveHorizontalEnterUpdate(bMoving, Direction, Axis);
                    break;
                case EActionStatus.Exit:
                    m_ActionStatus[EAction.MoveHorizontal] = EActionStatus.None;
                    break;
                case EActionStatus.None:
                    HandleMoveHorizontalNone(bMoving, Direction, Axis);
                    break;
                default:
                    m_ActionStatus[EAction.MoveHorizontal] = EActionStatus.None; 
                    break;
            }
        }

        private void HandleMoveHorizontalEnterUpdate(bool bMoving, EMoveDirection Direction, float Axis)
        {
            if (bMoving) {
                m_ActionStatus[EAction.MoveHorizontal] = EActionStatus.Update;
                OnMoveHorizontalUpdate?.Invoke(Direction, Axis);
            }
            else {
                m_ActionStatus[EAction.MoveHorizontal] = EActionStatus.Exit;
                OnMoveHorizontalExit?.Invoke(Direction, Axis);
            }
        }

        private void HandleMoveHorizontalNone(bool bMoving, EMoveDirection Direction, float Axis)
        {
            if (bMoving) {
                m_ActionStatus[EAction.MoveHorizontal] = EActionStatus.Enter;
                OnMoveHorizontalEnter?.Invoke(Direction, Axis);
            }
        }
        private void HandleMoveVertical()
        {
            float Axis = Input.GetAxisRaw("Vertical");
            bool bMoving = Axis != 0f;
            EMoveDirection Direction = Axis < 0 ? EMoveDirection.Down : EMoveDirection.Up;
            HandleMoveVerticalSwitch(bMoving, Direction, Axis);
        }

        private void HandleMoveVerticalSwitch(bool bMoving, EMoveDirection Direction, float Axis)
        {
            switch (m_ActionStatus[EAction.MoveVertical]) {
                case EActionStatus.Enter: case EActionStatus.Update:
                    HandleMoveVerticalEnterUpdate(bMoving, Direction, Axis);
                    break;
                case EActionStatus.Exit:
                    m_ActionStatus[EAction.MoveVertical] = EActionStatus.None;
                    break;
                case EActionStatus.None:
                    HandleMoveVerticalNone(bMoving, Direction, Axis);
                    break;
                default:
                    m_ActionStatus[EAction.MoveVertical] = EActionStatus.None; 
                    break;
            }
        }

        private void HandleMoveVerticalEnterUpdate(bool bMoving, EMoveDirection Direction, float Axis)
        {
            if (bMoving) {
                m_ActionStatus[EAction.MoveVertical] = EActionStatus.Update;
                OnMoveVerticalUpdate?.Invoke(Direction, Axis);
            }
            else {
                m_ActionStatus[EAction.MoveVertical] = EActionStatus.Exit;
                OnMoveVerticalExit?.Invoke(Direction, Axis);
            }
        }

        private void HandleMoveVerticalNone(bool bMoving, EMoveDirection Direction, float Axis)
        {
            if (bMoving)
            {
                m_ActionStatus[EAction.MoveVertical] = EActionStatus.Enter;
                OnMoveVerticalEnter?.Invoke(Direction, Axis);
            }
        }

        private void HandleJump()
        {
            switch (m_ActionStatus[EAction.Jump]) {
                case EActionStatus.Enter: case EActionStatus.Update:
                    HandleJumpEnterUpdate();
                    break;
                case EActionStatus.Exit:
                    m_ActionStatus[EAction.Jump] = EActionStatus.None;
                    break;
                case EActionStatus.None:
                    HandleJumpNone();
                    break;
                default:
                    m_ActionStatus[EAction.Jump] = EActionStatus.None; break;
            }
        }

        private void HandleJumpEnterUpdate()
        {
            if (Input.GetAxisRaw("Fire1") != 0) {
                m_ActionStatus[EAction.Jump] = EActionStatus.Update;
                OnJumpUpdate?.Invoke();
            }
            else {
                m_ActionStatus[EAction.Jump] = EActionStatus.Exit;
                OnJumpExit?.Invoke();
            }
        }

        private void HandleJumpNone()
        {
            if (Input.GetAxisRaw("Fire1") != 0) {
                m_ActionStatus[EAction.Jump] = EActionStatus.Enter;
                OnJumpEnter?.Invoke();
            }
        }
    }
}