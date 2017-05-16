/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using Leap;
using Leap.Unity.Attributes;
using Leap.Unity.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction.Internal {

  /// <summary>
  /// IInteractionBehaviour is the interface that defines all Interaction objects,
  /// specifying the minimum set of functionality required to make objects interactable.
  /// </summary>
  public interface IInteractionBehaviour {

    // Properties from MonoBehaviour.
    string              name       { get; } // (subclass MonoBehaviour to satisfy)
    GameObject          gameObject { get; } // ^
    Transform           transform  { get; } // ^

    // Properties for interaction.
    InteractionManager  manager    { get; }
    Rigidbody           rigidbody  { get; }
    ISpaceComponent     space      { get; } // OK to return null if this object is not in curved space.

    // Interaction overrides.
    IgnoreHoverMode ignoreHoverMode    { get; }
    bool            ignorePrimaryHover { get; }
    bool            ignoreContact      { get; }
    bool            ignoreGrasping     { get; }

    // Interaction settings.
    bool allowMultiGrasp { get; }
    
    // Called by the Interaction Manager manually
    // every fixed (physics) frame.
    void FixedUpdateObject();

    // Interaction types:
    // - Hover
    //   -- Primary Hover
    // - Contact
    // - Grasping
    //   -- Suspension

    // Hover
    float GetHoverDistance(Vector3 worldPosition);
    void BeginHover(List<InteractionControllerBase> beganHovering);
    void EndHover(List<InteractionControllerBase> endedHovering);
    void StayHovered(List<InteractionControllerBase> currentlyHovering);

    // Primary hover
    void BeginPrimaryHover(List<InteractionControllerBase> beganPrimaryHovering);
    void EndPrimaryHover(List<InteractionControllerBase> endedPrimaryHovering);
    void StayPrimaryHovered(List<InteractionControllerBase> currentlyPrimaryHovering);

    // Contact
    void BeginContact(List<InteractionControllerBase> beganContact);
    void EndContact(List<InteractionControllerBase> endedContact);
    void StayContacted(List<InteractionControllerBase> currentlyContacting);

    // Grasping
    bool isGrasped { get; }
    void BeginGrasp(List<InteractionControllerBase> beganGrasping);
    void EndGrasp(List<InteractionControllerBase> endedGrasping);
    void StayGrasped(List<InteractionControllerBase> currentlyGrasping);

    // Suspension
    bool isSuspended { get; }
    void BeginSuspension(InteractionControllerBase beganSuspending);
    void EndSuspension(InteractionControllerBase endedSuspending);

  }

}
