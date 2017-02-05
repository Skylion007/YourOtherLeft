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

	public override void OnStartClient()
	{
		base.OnStartClient();

		if (isClient)
		{
			/*meshRenderer = GetComponent<MeshRenderer>();
			staticMaterial = meshRenderer.material;
			UpdateMaterial();*/
		}

		if (isServer)
		{
			SendCurrentPosition();
		}
	}

	private void Update()
	{
		if (isServer)
		{
			SendCurrentPosition();
		}

		if (isClient)
		{
			//UpdateMaterial();
		}
	}

	[Server]
	void SendCurrentPosition()
	{
		/*var position = transform.localPosition;
		var scale = transform.localScale;*/

		LineRenderer lineRenderer = this.GetComponent<LineRenderer>();
		Vector3 position = lineRenderer.GetPosition (lineRenderer.numPositions - 1);

		RpcUpdatePosition(position);
	}

	[ClientRpc]
	void RpcUpdatePosition(Vector3 position)
	{
		/*this.transform.localPosition = position;
		this.transform.localScale = scale;*/

		LineRenderer lineRenderer = this.GetComponent<LineRenderer>();
		lineRenderer.numPositions++;
		lineRenderer.SetPosition (lineRenderer.numPositions - 1, position);
	}

	[ServerCallback]
	public void StartPlacing()
	{
		if (state != State.Static)
		{
			return;
		}

		state = State.Placing;
		RpcUpdateMaterial();
	}

	[ServerCallback]
	public void FinishPlacing()
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
		//UpdateMaterial();
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