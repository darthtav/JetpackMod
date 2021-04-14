using UnityEngine;
using UnityEngine.XR;
using ThunderRoad;
using OVR;
using Valve.VR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jetpack
{
    public class ItemJetpack : MonoBehaviour
    {
        Rigidbody Playerbody; //ref to the body
        Item item;              //ref to the item
        bool hover;
        bool equipped;
        bool flyloop;
        bool jumpButton;
        bool isSteam;

        AudioSource liftOff;
        AudioSource landing;
        AudioSource flying;

        Vector2 rightStick;

        UnityEngine.XR.InputDevice device;

        public void Awake()
        {
            var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

            if (SteamVR.active)
            {
                isSteam = true;
            }
            else
            {
                isSteam = false;
            }
         
            if (rightHandDevices.Count == 1)
            {
                device = rightHandDevices[0];
                Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
                Debug.Log("Device assigned.");
            }
            else if (rightHandDevices.Count > 1)
            {
                Debug.Log("Found more than one right hand!");
            }
            else
            {
                Debug.Log("Nothing Found");
            }

            Playerbody = Player.local.locomotion.rb;        //This is how you assign the body to the variable
            item = GetComponent<Item>();                    //This is how you assign the item itself (Jetpack in this case) to a variable
            hover = false;
            equipped = false;
            flyloop = false;
            jumpButton = false;

            item.OnSnapEvent += equipShoulder;              //These two lines are events that trigger when the item is equipped, everytime the item is equipped to the shoulder these are called.
            item.OnUnSnapEvent += removeShoulder;

            liftOff = item.GetCustomReference("JetpackLift").GetComponent<AudioSource>();       //Audio calls, custom references created in the unity engine. Will not work unless you create the custom reference 
            landing = item.GetCustomReference("JetpackLanding").GetComponent<AudioSource>();    // and drag audio source into the transform section under the main prefab.
            flying = item.GetCustomReference("JetpackLoopLong").GetComponent<AudioSource>();
        }

        public void Update()        //called every frame
        {
            if (isSteam)
            {
                steamMode();
            }
            else
            {
                oculusMode();
            }
        }

        public void steamMode()
        {
            if (Player.local.locomotion.isGrounded)     //on ground keeping gravity on
            {
                hitGround();
            }

            if (hover && !flyloop && !Player.local.locomotion.isGrounded)       //playing jetpack sound loop while in air
            {
                flyloop = true;
                flying.Play();
            }

            if (equipped && !hover)
            {
                if (SteamVR_Actions.default_Turn.axis.y >= .85)
                {
                    liftOff.Play();
                    toggleHover();
                    thrustUp();
                }
            }
            else if (equipped && hover)
            {
                if (SteamVR_Actions.default_Turn.axis.y >= .85)
                {
                    thrustUp();
                }
                if (SteamVR_Actions.default_Turn.axis.y <= -.85) 
                {
                    thrustDown();
                }
                if (SteamVR_Actions.default_Jump.stateDown) 
                {
                    toggleHover();
                }
            }
        }

        public void oculusMode()
        {
            device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out jumpButton);
            device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out rightStick);

            if (Player.local.locomotion.isGrounded)     //on ground keeping gravity on
            {
                hitGround();
            }

            if (hover && !flyloop && !Player.local.locomotion.isGrounded)       //playing jetpack sound loop while in air
            {
                flyloop = true;
                flying.Play();
            }

            if (equipped && !hover)
            {
                if (rightStick.y >= .85) 
                {
                    liftOff.Play();
                    toggleHover();
                    thrustUp();
                }
            }
            else if (equipped && hover)
            {
                if (rightStick.y >= .85) 
                {
                    thrustUp();
                }
                if (rightStick.y <= -.85)
                {
                    thrustDown();
                }
                if (jumpButton)
                {
                    toggleHover();
                }

            }
        }

        public void equipShoulder(Holder holder)        // proper positioning of jetpack when on shoulders
        {
            if(holder.ToString() == "BackLeft (ThunderRoad.Holder)")
            {
                item.transform.localPosition += new Vector3(0.16f, 0, 0);
                item.transform.Rotate(0, 0, 14);
            }
            else
            {
                item.transform.localPosition += new Vector3(0.16f, 0, 0);
                item.transform.Rotate(0, 0, -14);
            }

            equipped = true;
        }

        public void removeShoulder(Holder holder)
        {
            equipped = false;
        }

        public void toggleHover()   
        {
            if (hover)
            {
                flying.Stop();
                flyloop = false;
                landing.Play();
                hover = false;
                Playerbody.useGravity = true;
            }
            else
            {                               
                liftOff.Play();
                hover = true;
                Playerbody.useGravity = false;
            }
        }

        public bool canFly()
        {
            return (jumpButton && !Player.local.locomotion.isGrounded && equipped);
        }

        public void thrustUp()
        {
            Playerbody.AddForce(transform.up * 500f); 
        }

        public void thrustDown()
        {
            Playerbody.AddForce(transform.up * -500f);
        }

        public void hitGround()
        {
            if (hover)
            {
                flying.Stop();
                flyloop = false;
                landing.Play();
            }
            hover = false;
            Playerbody.useGravity = true;
        }
    }
}
