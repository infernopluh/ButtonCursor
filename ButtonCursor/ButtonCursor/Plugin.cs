using BepInEx;
using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace ButtonCursor
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public bool stopSpammingErrorsPlease;

        GameObject pointer;
        GameObject tc;
        Vector3 pos;

        public void Awake()
        {
            stopSpammingErrorsPlease = true;
        }

        public void Update()
        {
            if (stopSpammingErrorsPlease)
            {
                if (pointer == null)
                {
                    tc = GorillaTagger.Instance.rightHandTriggerCollider;
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    pointer.transform.localScale = GorillaTagger.Instance.rightHandTriggerCollider.transform.localScale / 15;
                    pointer.layer = LayerMask.NameToLayer("TransparentFX");
                    Destroy(pointer.GetComponent<SphereCollider>());
                }
                else
                {
                    if (!XRSettings.isDeviceActive)
                    {
                        pointer.GetComponent<Renderer>().enabled = false;
                        Vector2 wp = Mouse.current.position.ReadValue();
                        Ray ray = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>().ScreenPointToRay(wp);
                        if (Physics.Raycast(ray, out RaycastHit info, 500, GTPlayer.Instance.locomotionEnabledLayers))
                        {
                            pos = info.point;
                            pointer.transform.position = pos;
                        }
                        if (Mouse.current.leftButton.isPressed)
                        {
                            if (tc.GetComponent<TransformFollow>().enabled == true)
                            {
                                tc.GetComponent<TransformFollow>().enabled = false;
                                pointer.GetComponent<Renderer>().material.color = Color.green;
                            }
                            tc.transform.position = pos;
                        }
                        else
                        {
                            if (tc.GetComponent<TransformFollow>().enabled == false)
                            {
                                pointer.GetComponent<Renderer>().material.color = Color.white;
                                tc.GetComponent<TransformFollow>().enabled = true;
                            }
                        }
                    }
                    else
                    {
                        if (GorillaTagger.Instance.offlineVRRig.rightMiddle.gripValue > 0.2f)
                        {
                            if (Physics.Raycast(GTPlayer.Instance.rightControllerTransform.position, GTPlayer.Instance.rightControllerTransform.forward, out RaycastHit info, 500, GTPlayer.Instance.locomotionEnabledLayers))
                            {
                                pos = info.point;
                                pointer.transform.position = pos;
                            }
                            if (GorillaTagger.Instance.offlineVRRig.rightIndex.triggerValue > 0.3f)
                            {
                                if (tc.GetComponent<TransformFollow>().enabled == true)
                                {
                                    tc.GetComponent<TransformFollow>().enabled = false;
                                    pointer.GetComponent<Renderer>().material.color = Color.green;
                                }
                                tc.transform.position = pos;
                            }
                            else
                            {
                                if (tc.GetComponent<TransformFollow>().enabled == false)
                                {
                                    pointer.GetComponent<Renderer>().material.color = Color.white;
                                    tc.GetComponent<TransformFollow>().enabled = true;
                                }
                            }
                        }
                        else
                        {
                            DestroyPointer();
                        }
                    }
                }
            }
        }

        void OnDisable()
        {
            DestroyPointer();
        }
        
        void DestroyPointer()
        {
            if (pointer != null)
            {
                pointer.GetComponent<Renderer>().material.color = Color.white;
                tc.GetComponent<TransformFollow>().enabled = true;
                GameObject.Destroy(pointer);
                pointer = null;
            }
        }
    }
}
