using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class ProjectionMatrix : MonoBehaviour
{
    public GameObject projectionScreen;
    public bool estimateViewFrustum = true;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void OnPreCull()
    {

        if (null != projectionScreen && null != cam)
        {
            Vector3 pa = projectionScreen.transform.TransformPoint(new Vector3(-5.0f, 0.0f, -5.0f));
            Vector3 pb = projectionScreen.transform.TransformPoint(new Vector3(5.0f, 0.0f, -5.0f));
            Vector3 pc = projectionScreen.transform.TransformPoint(new Vector3(-5.0f, 0.0f, 5.0f));
            Vector3 pe = transform.position;

            float n = cam.nearClipPlane;
            float f = cam.farClipPlane;

            Vector3 vr = (pb - pa).normalized;
            Vector3 vu = (pc - pa).normalized;
            Vector3 vn = -Vector3.Cross(vr, vu).normalized; 

            Vector3 va = pa - pe;
            Vector3 vb = pb - pe;
            Vector3 vc = pc - pe;

            float d = -Vector3.Dot(va, vn);
            
            float l = Vector3.Dot(vr, va) * n / d;
            float r = Vector3.Dot(vr, vb) * n / d;
            float b = Vector3.Dot(vu, va) * n / d;
            float t = Vector3.Dot(vu, vc) * n / d;

            Matrix4x4 projectionMatrix = new Matrix4x4();
            projectionMatrix.m00 = 2.0f * n / (r - l);
            projectionMatrix.m01 = 0.0f;
            projectionMatrix.m02 = (r + l) / (r - l);
            projectionMatrix.m03 = 0.0f;

            projectionMatrix.m10 = 0.0f;
            projectionMatrix.m11 = 2.0f * n / (t - b);
            projectionMatrix.m12 = (t + b) / (t - b);
            projectionMatrix.m13 = 0.0f;

            projectionMatrix.m20 = 0.0f;
            projectionMatrix.m21 = 0.0f;
            projectionMatrix.m22 = -((f + n) / (f - n));
            projectionMatrix.m23 = -(2.0f * f * n / (f - n));

            projectionMatrix.m30 = 0.0f;
            projectionMatrix.m31 = 0.0f;
            projectionMatrix.m32 = -1.0f;
            projectionMatrix.m33 = 0.0f;

            Matrix4x4 rotationMatrix = new Matrix4x4();
            rotationMatrix.m00 = vr.x;
            rotationMatrix.m01 = vr.y;
            rotationMatrix.m02 = vr.z;
            rotationMatrix.m03 = 0.0f;

            rotationMatrix.m10 = vu.x;
            rotationMatrix.m11 = vu.y;
            rotationMatrix.m12 = vu.z;
            rotationMatrix.m13 = 0.0f;

            rotationMatrix.m20 = vn.x;
            rotationMatrix.m21 = vn.y;
            rotationMatrix.m22 = vn.z;
            rotationMatrix.m23 = 0.0f;

            rotationMatrix.m30 = 0.0f;
            rotationMatrix.m31 = 0.0f;
            rotationMatrix.m32 = 0.0f;
            rotationMatrix.m33 = 1.0f;

            Matrix4x4 translationMatrix = new Matrix4x4();
            translationMatrix.m00 = 1.0f;
            translationMatrix.m01 = 0.0f;
            translationMatrix.m02 = 0.0f;
            translationMatrix.m03 = -pe.x;

            translationMatrix.m10 = 0.0f;
            translationMatrix.m11 = 1.0f;
            translationMatrix.m12 = 0.0f;
            translationMatrix.m13 = -pe.y;

            translationMatrix.m20 = 0.0f;
            translationMatrix.m21 = 0.0f;
            translationMatrix.m22 = 1.0f;
            translationMatrix.m23 = -pe.z;

            translationMatrix.m30 = 0.0f;
            translationMatrix.m31 = 0.0f;
            translationMatrix.m32 = 0.0f;
            translationMatrix.m33 = 1.0f;

            cam.projectionMatrix = projectionMatrix;
            cam.worldToCameraMatrix = rotationMatrix * translationMatrix;

            if (estimateViewFrustum)
            {
                Quaternion quaternion = new Quaternion();
                quaternion.SetLookRotation((0.5f * (pb + pc) - pe), vu);
                cam.transform.rotation = quaternion;

                if (cam.aspect >= 1.0f)
                {
                    cam.fieldOfView = Mathf.Rad2Deg *
                                      Mathf.Atan(((pb - pa).magnitude +
                                                  (pc - pa).magnitude) / va.magnitude);

                }
                else
                {
                    cam.fieldOfView = Mathf.Rad2Deg / cam.aspect *
                                      Mathf.Atan(((pb - pa).magnitude +
                                                  (pc - pa).magnitude) / va.magnitude);
                }
            }
        }
    }
}
