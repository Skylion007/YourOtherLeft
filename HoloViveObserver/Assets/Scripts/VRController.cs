using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour {

	public float forceMultiplier = 5;

	private SteamVR_TrackedController thisController;
	private SteamVR_Controller.Device thisControllerDevice;
	private Rigidbody collidedObject = null;
	private Rigidbody connectedObject = null;
	private Transform oldObjectParent;

	// Use this for initialization
	void Start () {
		thisController = this.GetComponent<SteamVR_TrackedController> ();
		SteamVR_TrackedObject thisControllerObj = this.GetComponent<SteamVR_TrackedObject> ();
		thisControllerDevice = SteamVR_Controller.Input ((int)thisControllerObj.index);

		thisController.TriggerClicked += TriggerClicked;
		thisController.TriggerUnclicked += TriggerUnclicked;
	}

	void TriggerClicked(object sender, ClickedEventArgs e) {
		if (collidedObject != null) {
			collidedObject.isKinematic = true;
			oldObjectParent = collidedObject.transform.parent;
			collidedObject.transform.SetParent (this.transform);
			connectedObject = collidedObject;
		}
	}

	void TriggerUnclicked(object sender, ClickedEventArgs e) {
		if (connectedObject != null) {
			collidedObject.isKinematic = false;
			connectedObject.transform.SetParent (oldObjectParent);
			//Transform.TransformDirection (thisControllerDevice.velocity)
			Debug.Log(thisControllerDevice.velocity);
			//connectedObject.AddForce (new Vector3 (100, 0, 0));
			connectedObject.AddForce (thisControllerDevice.velocity * forceMultiplier);
			connectedObject = null;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (collidedObject == null) {
			collidedObject = other.gameObject.GetComponent<Rigidbody> ();
		}
	}

	void OnTriggerExit(Collider other) {
		if (collidedObject != null && other.gameObject == collidedObject.gameObject)
			collidedObject = null;
	}

}
