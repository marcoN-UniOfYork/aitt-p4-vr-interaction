using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HandGrab : MonoBehaviour
{
    public GameObject collidedObject;
    public GameObject grabbedObject;
    public bool grab = false;
    public SteamVR_Input_Sources inputSource;

    public HandGrab otherHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Input.GetStateDown("GrabPinch", inputSource)) grab = true;
        if (SteamVR_Input.GetStateUp("GrabPinch", inputSource)) grab = false;

        if (grab == true)
        {
            Grab();
        }
        else
        {
            Release();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!grab && other.tag == "Grabable") {
            collidedObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!grab && collidedObject == other.gameObject) collidedObject = null;
    }

    public void Grab() {
        if (collidedObject != null)
        {
            grabbedObject = collidedObject;
            SpringJoint joint;
            if (gameObject.TryGetComponent<SpringJoint>(out joint) == false)
            {
                gameObject.AddComponent<SpringJoint>();
            }

            if(otherHand.grabbedObject != grabbedObject)
            {
                grabbedObject.GetComponent<Rigidbody>().freezeRotation = true;
                gameObject.GetComponent<SpringJoint>().breakForce = 1000;
            }
            else
            {
                grabbedObject.GetComponent<Rigidbody>().freezeRotation = false;
                gameObject.GetComponent<SpringJoint>().breakForce = 50;
            }

            gameObject.GetComponent<SpringJoint>().connectedBody = grabbedObject.GetComponent<Rigidbody>();
            gameObject.GetComponent<SpringJoint>().spring = 10000;

            //grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            //grabbedObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }
    
    public void Release()
    {
        if (grabbedObject != null)
        {
            if (otherHand.grabbedObject == grabbedObject)
            {
                grabbedObject.GetComponent<Rigidbody>().freezeRotation = true;
            }
            else
            {
                grabbedObject.GetComponent<Rigidbody>().freezeRotation = false;
            }

            SpringJoint joint;
            gameObject.TryGetComponent<SpringJoint>(out joint);
            if(joint != null) joint.connectedBody = null;
            //grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            //grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            //grabbedObject.GetComponent<Rigidbody>().velocity = gameObject.GetComponent<Hand>().trackedObject.GetVelocity();
            //grabbedObject.GetComponent<Rigidbody>().angularVelocity = gameObject.GetComponent<Hand>().trackedObject.GetAngularVelocity();
            grabbedObject = null;
        }
    }

}
