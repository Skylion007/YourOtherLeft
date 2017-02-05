using UnityEngine;
using UnityEngine.Networking;

public class LineManager : NetworkBehaviour
{
    public SteamVR_TrackedController leftController;
    public SteamVR_TrackedController rightController;
    public GameObject lineContainer;
    public GameObject lineAsset;

	private LineController lineController;

	private Vector3 lastLeftPosn = Vector3.zero;
	public float maxMovement = 10.0f;
	public float minMovement = 0.5f;

    [SyncVar]
    private bool drawingLine = false;
    private GameObject currentLine = null;

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
		if (isClient && drawingLine)
        {
            UpdateLinePoints();
        }
    }

    [ClientCallback]
    private void TriggerUnclicked(object sender, ClickedEventArgs e)
    {
		if (drawingLine)
        {
			lastLeftPosn = Vector3.zero;
            CmdFinishDrawingLine();
        }
    }

    [ClientCallback]
    private void TriggerClicked(object sender, ClickedEventArgs e)
    {
		if (!drawingLine)
        {
            CmdStartDrawingLine();
        }
    }

    [Command]
    private void CmdStartDrawingLine()
    {
        currentLine = Instantiate(lineAsset, lineContainer.transform);

		if (currentLine == null) {
			Debug.LogError ("current line is null");
		}
		lineController = currentLine.GetComponent<LineController> ();
		NetworkServer.Spawn(currentLine);
		lineController.StartDrawing();
        RpcSetLineParent(currentLine);
		UpdateLinePoints();

        drawingLine = true;
    }

    [ClientRpc]
    private void RpcSetLineParent(GameObject line)
    {
		line.transform.SetParent(lineContainer.transform, false);
    }

    [Command]
    private void CmdFinishDrawingLine()
    {
        currentLine.GetComponent<LineController>().FinishDrawing();
        currentLine = null;

		drawingLine = false;
    }

    [ClientCallback]
	private void UpdateLinePoints()
    {
        Vector3 left = leftController.transform.position;
        //Vector3 right = rightController.transform.position;
        CmdUpdateLinePoints(left);
    }

    [Command]
    private void CmdUpdateLinePoints(Vector3 position)
    { 
		if (!currentLine) return;


		float change = Vector3.Distance (lastLeftPosn, position);
		if (lastLeftPosn == Vector3.zero || (minMovement < change && change < maxMovement)) {
			lineController.AddPoint (position);

		}
    }
}