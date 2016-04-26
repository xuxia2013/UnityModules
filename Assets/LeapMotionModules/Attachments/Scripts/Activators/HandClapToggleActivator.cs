﻿using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity;

public class HandClapToggleActivator : Activator {
  private LeapProvider provider = null;
  private bool velocityThresholdExceeded = false;

  public float Proximity = 0.1f; //meters
  public float VelocityThreshold = 0.1f; //meters/s
  public float PalmAngleLimit = 75; //degrees

  #if UNITY_EDITOR
  //For debugging --set Inspector to debug mode
  private float currentAngle = 0;
  private float currentVelocityVectorAngle = 0;
  private float currentDistance = 0;
  private float currentVelocity = 0;
  #endif

  void Start () {
    provider = GetComponentInParent<LeapServiceProvider>();
  }

  void Update(){
    Hand thisHand;
    Hand thatHand;
    Frame frame = provider.CurrentFrame;
    if(frame != null && frame.Hands.Count >= 2){
      thisHand = frame.Hands[0];
      thatHand = frame.Hands[1];
      if(thisHand != null && thatHand != null){
        Vector velocityDirection = thisHand.PalmVelocity.Normalized;
        Vector otherhandDirection = (thisHand.PalmPosition - thatHand.PalmPosition).Normalized;

        #if UNITY_EDITOR
        //for debugging
        Debug.DrawRay(thisHand.PalmPosition.ToVector3(), velocityDirection.ToVector3());
        Debug.DrawRay(thatHand.PalmPosition.ToVector3(), otherhandDirection.ToVector3());
        currentAngle = thisHand.PalmNormal.AngleTo(thatHand.PalmNormal) * Constants.RAD_TO_DEG;
        currentDistance = thisHand.PalmPosition.DistanceTo(thatHand.PalmPosition);
        currentVelocity = thisHand.PalmVelocity.MagnitudeSquared + thatHand.PalmVelocity.MagnitudeSquared;
        currentVelocityVectorAngle = velocityDirection.AngleTo(otherhandDirection) * Constants.RAD_TO_DEG;
        #endif

        if( thisHand.PalmVelocity.MagnitudeSquared + thatHand.PalmVelocity.MagnitudeSquared > VelocityThreshold &&
          velocityDirection.AngleTo(otherhandDirection) >= (180 - PalmAngleLimit) * Constants.DEG_TO_RAD){
          velocityThresholdExceeded = true;
        }
      }
    }
  }

  void OnEnable () {
    StartCoroutine(clapWatcher());
  }

  void OnDisable () {
    StopCoroutine(clapWatcher());
    IsActive = false;
  }

  IEnumerator clapWatcher() {
    Hand thisHand;
    Hand thatHand;
    bool clapped = false;
    while(true){
      if(provider){
        Frame frame = provider.CurrentFrame;
        if(frame != null && frame.Hands.Count >= 2){
          thisHand = frame.Hands[0];
          thatHand = frame.Hands[1];
          if(thisHand != null && thatHand != null){
            //decide if clapped
            clapped = velocityThresholdExceeded && //went fast enough
                      thisHand.PalmPosition.DistanceTo(thatHand.PalmPosition) < Proximity && // and got close 
                      thisHand.PalmNormal.AngleTo(thatHand.PalmNormal) >= (180 - PalmAngleLimit) * Constants.DEG_TO_RAD; //while facing each other
  
            if(clapped & !IsActive){
              Activate();
            } else if(clapped & IsActive){
              Deactivate();
            }
          }
        }
      }
      velocityThresholdExceeded = false;
      yield return new WaitForSeconds(Period);
    }
  }
}
