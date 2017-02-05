﻿using UnityEngine;
using UnityEngine.Networking;

public class BlockManager : NetworkBehaviour
{
    public SteamVR_TrackedController leftController;
    public SteamVR_TrackedController rightController;
    public GameObject cubeContainer;
    public GameObject cubeAsset;

    [SyncVar]
    private bool placingCube = false;
    private GameObject currentCube = null;

    public override void OnStartClient()
    {
        base.OnStartClient();

        leftController.TriggerClicked += TriggerClicked;
        rightController.TriggerClicked += TriggerClicked;
        leftController.TriggerUnclicked += TriggerUnclicked;
        rightController.TriggerUnclicked += TriggerUnclicked;
    }
    
    void Update()
    {
        if (isClient && placingCube)
        {
			CmdPlaceCube();
        }
    }

    [ClientCallback]
    private void TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        if (placingCube)
        {
            CmdFinishPlacingCube();
        }
    }

    [ClientCallback]
    private void TriggerClicked(object sender, ClickedEventArgs e)
    {
        if (AreBothTriggersPressed() && !placingCube)
        {
            CmdPlaceCube();
        }
    }
    
    [Client]
    private bool AreBothTriggersPressed()
    {
        return leftController.triggerPressed && rightController.triggerPressed;
    }

    [Command]
    private void CmdPlaceCube()
    {
        currentCube = Instantiate(cubeAsset, cubeContainer.transform);
        //currentCube.GetComponent<BlockController>().StartPlacing();
        UpdateCubePosition();
        NetworkServer.Spawn(currentCube);
        RpcSetBlockParent(currentCube);

        placingCube = true;
    }

    [ClientRpc]
    private void RpcSetBlockParent(GameObject cube)
    {
        cube.transform.SetParent(cubeContainer.transform, false);
    }

    [Command]
    private void CmdFinishPlacingCube()
    {
        //currentCube.GetComponent<BlockController>().FinishPlacing();
        currentCube = null;

        placingCube = false;
    }

    [ClientCallback]
    private void UpdateCubePosition()
    {
        Vector3 left = leftController.transform.position;
        //Vector3 right = rightController.transform.position;
        CmdUpdateCubePosition(left);
    }

    [Command]
    private void CmdUpdateCubePosition(Vector3 position)
    {
        if (!currentCube) return;
        currentCube.transform.position = position;
		//currentCube.transform.localScale = new Vector3(0.01f,0.01f,0.01f);
    }
}