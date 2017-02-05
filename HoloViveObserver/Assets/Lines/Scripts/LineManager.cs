using UnityEngine;
using UnityEngine.Networking;

public class LineManager : NetworkBehaviour
{
	public SteamVR_TrackedController leftController;
	public SteamVR_TrackedController rightController;
	public GameObject cubeContainer;
	public GameObject cubeAsset;

	/*private Vector3 lastLeftPosn = Vector3.zero;
	public float maxMovement = 10.0f;
	public float minMovement = 0.5f;*/

	[SyncVar]
	private bool placingCube = false;
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
		if (isClient && placingCube)
		{
			UpdateCubePosition();
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
		if (!placingCube)
		{
			CmdStartPlacingCube();
		}
	}

	/*[Client]
	private bool AreBothTriggersPressed()
	{
		return leftController.triggerPressed && rightController.triggerPressed;
	}*/

	[Command]
	private void CmdStartPlacingCube()
	{
		Debug.Log ("Instantating cube");
		currentCube = Instantiate(cubeAsset, cubeContainer.transform);
		currentCube.GetComponent<LineController>().StartPlacing();
		UpdateCubePosition();
		NetworkServer.Spawn(currentCube);
		RpcSetBlockParent(currentCube);

		placingCube = true;
		Debug.Log ("Instantated cube");
	}

	[ClientRpc]
	private void RpcSetBlockParent(GameObject cube)
	{
		cube.transform.SetParent(cubeContainer.transform, false);
	}

	[Command]
	private void CmdFinishPlacingCube()
	{
		currentCube.GetComponent<LineController>().FinishPlacing();
		currentCube = null;

		placingCube = false;
	}

	[ClientCallback]
	private void UpdateCubePosition()
	{
		Debug.Log("Updating cube position");
		Vector3 left = leftController.transform.position;
		//Vector3 right = rightController.transform.position;
		CmdUpdateCubePosition(left);
		Debug.Log("Updated cube position");
	}

	[Command]
	private void CmdUpdateCubePosition(Vector3 position)
	{
		Debug.Log("CMDUpdating cube position");
		if (!currentCube) return;

		// TODO: make efficient
		LineRenderer lineRenderer = currentCube.GetComponent<LineRenderer>();
		lineRenderer.numPositions++;
		lineRenderer.SetPosition (lineRenderer.numPositions - 1, position);
		Debug.Log("CMDUpdated cube position");
	}
}