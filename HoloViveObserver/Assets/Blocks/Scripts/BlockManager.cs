using UnityEngine;
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

    private Vector3 oldLeftPosition;
    private Vector3 oldRightPosition;
    private float minimumDistance = 0.2f;

    public override void OnStartClient()
    {
        base.OnStartClient();

        /*
        leftController.TriggerClicked += TriggerClicked;
        rightController.TriggerClicked += TriggerClicked;
        leftController.TriggerUnclicked += TriggerUnclicked;
        rightController.TriggerUnclicked += TriggerUnclicked;
        */
        //Using the touchpad instead
        //Using the touchpad instead
        leftController.PadClicked += TriggerClicked;
        rightController.PadClicked += TriggerClicked;
        leftController.PadUnclicked += TriggerUnclicked;
        rightController.PadUnclicked += TriggerUnclicked;
        
        oldLeftPosition = leftController.transform.position;
        oldRightPosition = rightController.transform.position;

    }

        private Vector3 leftMovement()
    {
        return leftController.transform.position - oldLeftPosition;
    }

    private float leftDistance()
    {
        return leftMovement().magnitude;
    }

    private Quaternion leftDirection()
    {
        return Quaternion.LookRotation(leftMovement(), Vector3.up);
    }

    private Vector3 rightMovement()
    {
        return rightController.transform.position - oldRightPosition;
    }
    private float rightDistance()
    {
        return rightMovement().magnitude;
    }

    private Quaternion rightDirection()
    {
        return Quaternion.LookRotation(rightMovement(), Vector3.up);
    }


    void Update()
    {
        if (isClient && placingCube && leftDistance() > minimumDistance)

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
        oldLeftPosition = leftController.transform.position;


        placingCube = false;
    }

    [ClientCallback]
    private void UpdateCubePosition()
    {
        Quaternion leftAim = leftDirection();
        float length = leftDistance();


        Vector3 left = leftController.transform.position;
        //Vector3 right = rightController.transform.position;
        CmdUpdateCubePosition(left, leftAim, leftDistance());
    }

    [Command]
    private void CmdUpdateCubePosition(Vector3 position, Quaternion newDirection, float distance)
    {
        if (!currentCube) return;
        currentCube.transform.position = position;
        currentCube.transform.localRotation = newDirection;
        currentCube.transform.localScale = new Vector3(0.01f, distance, 0.01f);

    }
}