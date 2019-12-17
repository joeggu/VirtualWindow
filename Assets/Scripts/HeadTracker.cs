using System.Collections;
using System.Collections.Generic;
using nuitrack;
using UnityEngine;
using Joint = nuitrack.Joint;
using Vector3 = UnityEngine.Vector3;

public class HeadTracker : MonoBehaviour
{
    public JointType TrackeJointType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentUserTracker.CurrentUser != 0)
        {
            Skeleton skeleton = CurrentUserTracker.CurrentSkeleton;

            Joint joint = skeleton.GetJoint(TrackeJointType);
            Vector3 jointPos = 0.01f * joint.ToVector3();
            CorrectPosition(ref jointPos);
            gameObject.transform.localPosition = jointPos;
        }
    }

    private void CorrectPosition(ref Vector3 jointPos)
    {
        jointPos.x = -1.0f * jointPos.x;
        //jointPos.y = -1.0f * jointPos.y;
        jointPos.z = -1.0f * jointPos.z;
    }
}
