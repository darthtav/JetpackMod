using UnityEngine;
using ThunderRoad;
using OVR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jetpack
{
    public class ItemJetpack : MonoBehaviour
    {
        Rigidbody Playerbody;   //ref to the body
        Item item;              //ref to the item
        bool hover;
        bool equipped;
        bool flyloop;

        AudioSource liftOff;
        AudioSource landing;
        AudioSource flying;

        

        public void Awake()
        {
            Playerbody = Player.local.locomotion.rb;        //This is how you assign the body to the variable
            item = GetComponent<Item>();                    //This is how you assign the item itself (Jetpack in this case) to a variable
            hover = false;
            equipped = false;
            flyloop = false;

            item.OnSnapEvent += equipShoulder;              //These two lines are events that trigger when the item is equipped, everytime the item is equipped to the shoulder these are called.
            item.OnUnSnapEvent += removeShoulder;

            liftOff = item.GetCustomReference("JetpackLift").GetComponent<AudioSource>();       //Audio calls, custom references created in the unity engine. Will not work unless you create the custom reference 
            landing = item.GetCustomReference("JetpackLanding").GetComponent<AudioSource>();    // and drag audio source into the transform section under the main prefab.
            flying = item.GetCustomReference("JetpackLoopLong").GetComponent<AudioSource>();

        }

        public void Update()        //called every frame
        {
            OVRInput.Update();
            if (Player.local.locomotion.isGrounded)     //on ground keeping gravity on
            {
                hitGround();
            }

            if (hover && !flyloop && !Player.local.locomotion.isGrounded)       //playing jetpack sound loop while in air
            {
                flyloop = true;
                flying.Play();
            }

            if (canFly())
            {
                toggleHover();              // Toggle for jetpack on and off
            }
            else if (hover)
            {
                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
                {
                    thrustUp();
                }
                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown))
                {
                    thrustDown();
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
            return (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick) && !Player.local.locomotion.isGrounded && equipped);
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
