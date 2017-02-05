using UnityEngine;
using UnityEngine.Networking;

public class LineController : NetworkBehaviour
{
    private enum State
    {
        Static,
        Placing,
    }

    private Material staticMaterial;
    public Material placingMaterial;

    [SyncVar]
    private State state = State.Static;

    //private MeshRenderer meshRenderer;

	private LineRenderer lineRenderer;

    private Vector3 oldPosition;
    private Vector3 oldScale;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isClient){
			lineRenderer = GetComponent<LineRenderer>();
		}

        if (isServer)
        {
            //SendCurrentPosition();
        }
    }

    private void Update()
    {
        if (isServer)
        {
            //SendCurrentPosition();
        }

        if (isClient)
        {
            UpdateMaterial();
        }
    }

	[Server]
	public void AddPoint(Vector3 position) {
		//lineRenderer.numPositions++;
		//lineRenderer.SetPosition (lineRenderer.numPositions - 1, position);
		RpcAddPoint (position);
	}

    [ClientRpc]
    private void RpcAddPoint(Vector3 posn)
    {
		this.lineRenderer.numPositions++;
		this.lineRenderer.SetPosition (this.lineRenderer.numPositions - 1, posn);
    }

    [ServerCallback]
    public void StartDrawing()
    {
        if (state != State.Static)
        {
            return;
        }

        state = State.Placing;
        RpcUpdateMaterial();
    }

    [ServerCallback]
    public void FinishDrawing()
    {
        if (state != State.Placing)
        {
            return;
        }

        state = State.Static;
        RpcUpdateMaterial();
    }

    [ClientRpc]
    private void RpcUpdateMaterial()
    {
        UpdateMaterial();
    }

    [Client]
    private void UpdateMaterial() {
        /*if (!meshRenderer)
        {
            return;
        }

        switch (state)
        {
            case State.Static:
                meshRenderer.material = staticMaterial;
                break;
            case State.Placing:
                meshRenderer.material = placingMaterial;
                break;
        }*/
    }
}