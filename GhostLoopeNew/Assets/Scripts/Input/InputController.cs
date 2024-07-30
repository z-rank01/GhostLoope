using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


public class InputController
{
    private PlayerInputControl playerInputControl;
    private InputAction.CallbackContext callbackContext;
    private InputAction moveAction;
    private InputAction fireAction;
    private InputAction interactAction;
    private InputAction dashAction;
    private InputAction swallowAndFireAction;


    // interface
    public void Init()
    {
        playerInputControl = new PlayerInputControl();
        playerInputControl.Enable();
    }

    public void Enable()
    {
        playerInputControl.Enable();
    }

    public void Disable()
    {
        playerInputControl.Disable();
    }

    // Life circle
    public void Start()
    {
        Init();
        moveAction = playerInputControl.Player.Move;
        fireAction = playerInputControl.Player.Fire;
        interactAction = playerInputControl.Player.Interact;
        dashAction = playerInputControl.Player.Dash;
        swallowAndFireAction = playerInputControl.Player.SwallowAndFire;

        MonitorMove();
        MonitorFire();
        MonitorInteract();
        MonitorDash();
        MonitorSwallowAndFire();
    }

    public void Update()
    {
        
    }

    // Input event callback
    private void MonitorMove()
    {
        playerInputControl.Player.Move.performed += (callbackContext) =>
        { GlobalInstance.GetInstance().AddStatus(E_InputStatus.moving); };

        playerInputControl.Player.Move.performed += (callbackContext) =>
        { ActInfo.GetInstance().SetActInfo(E_ActInfo.moveDirection, playerInputControl.Player.Move.ReadValue<Vector2>()); };

        playerInputControl.Player.Move.canceled += (callbackContext) =>
        { GlobalInstance.GetInstance().RemoveStatus(E_InputStatus.moving); };
    }

    private void MonitorFire()
    {
        playerInputControl.Player.Fire.performed += (callbackContext) =>
        { GlobalInstance.GetInstance().AddStatus(E_InputStatus.firing); };

        playerInputControl.Player.Fire.performed += (callbackContext) =>
        { GlobalInstance.GetInstance().StartCounter(); };
        
        playerInputControl.Player.Fire.performed += (callbackContext) =>
        { ActInfo.GetInstance().SetActInfo(E_ActInfo.bulletSpeed, GlobalSetting.GetInstance().bulletSpeed); };
        
        playerInputControl.Player.Fire.canceled += (callbackContext) =>
        { GlobalInstance.GetInstance().RemoveStatus(E_InputStatus.firing); };
    }

    private void MonitorInteract()
    {
        playerInputControl.Player.Interact.started += (callbackContext) =>
        { GlobalInstance.GetInstance().AddStatus(E_InputStatus.interacting); };

        
        playerInputControl.Player.Interact.canceled += (callbackContext) =>
        { GlobalInstance.GetInstance().RemoveStatus(E_InputStatus.interacting); };
    }

    private void MonitorDash()
    {
        playerInputControl.Player.Dash.started += (callbackContext) =>
        { GlobalInstance.GetInstance().AddStatus(E_InputStatus.dashing); };
        
        playerInputControl.Player.Dash.canceled += (callbackContext) =>
        { GlobalInstance.GetInstance().RemoveStatus(E_InputStatus.dashing); };
    }

    private void MonitorSwallowAndFire()
    {
        playerInputControl.Player.SwallowAndFire.started += (callbackContext) =>
        { GlobalInstance.GetInstance().AddStatus(E_InputStatus.swallowingAndFiring); };
        
        playerInputControl.Player.SwallowAndFire.canceled += (callbackContext) =>
        { GlobalInstance.GetInstance().RemoveStatus(E_InputStatus.swallowingAndFiring); };
    }


}