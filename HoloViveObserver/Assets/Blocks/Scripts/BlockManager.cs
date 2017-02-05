using UnityEngine;
using UnityEngine.Networking;

public class BlockManager : NetworkBehaviour
{
    public SteamVR_TrackedController leftController;
    public SteamVR_TrackedController rightController;
    public GameObject cubeContainer;
    public GameObject cubeAsset;

    [SyncVar]
    private bool trigger = false;
    private GameObject currentCube = null;

    public override void OnStartClient()
    {
        base.OnStartClient();

        leftController.TriggerClicked += TriggerClicked;
        //rightController.TriggerClicked += TriggerClicked;
        leftController.TriggerUnclicked += TriggerUnclicked;
        //rightController.TriggerUnclicked += TriggerUnclicked;
    }
    
    void Update()
    {
		if (isClient && trigger)
        {
            //UpdateCubePosition();
			CmdPlaceCube();
        }
    }

    [ClientCallback]
    private void TriggerUnclicked(object sender, ClickedEventArgs e)
    {
		trigger = false;
        /*if (trigger)
        {
            CmdFinishPlacingCube();
        }*/
    }

    [ClientCallback]
    private void TriggerClicked(object sender, ClickedEventArgs e)
    {
		trigger = true;
        /*if (AreBothTriggersPressed() && !trigger)
        {
            CmdStartPlacingCube();
        }*/
    }
    
    [Client]
    private bool AreBothTriggersPressed()
    {
        return leftController.triggerPressed && rightController.triggerPressed;
    }
	 
    [Command]
	private void CmdPlaceCube()
    {
		//Vector3 left = leftController.transform.position;
		currentCube = Instantiate(cubeAsset);
		currentCube.transform.position = leftController.transform.position;
        //currentCube.GetComponent<BlockController>().StartPlacing();
        //UpdateCubePosition();
        NetworkServer.Spawn(currentCube);
        RpcSetBlockParent(currentCube);

        trigger = true;
    }

    [ClientRpc]
    private void RpcSetBlockParent(GameObject cube)
    {
        cube.transform.SetParent(cubeContainer.transform, false);
    }

    /*[Command]
    private void CmdFinishPlacingCube()
    {
        currentCube.GetComponent<BlockController>().FinishPlacing();
        currentCube = null;

        trigger = false;
    }

    [ClientCallback]
    private void UpdateCubePosition()
    {
        Vector3 left = leftController.transform.position;
        Vector3 right = rightController.transform.position;
        CmdUpdateCubePosition((left + right) / 2, right - left);
    }

    [Command]
    private void CmdUpdateCubePosition(Vector3 position, Vector3 scale)
    {
        if (!currentCube) return;
        currentCube.transform.position = position;
        currentCube.transform.localScale = scale;
    }*/
}