/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.Rendering;
using Leap.Unity.Attributes;

namespace Leap.Unity.GraphicalRenderer {

  [DisallowMultipleComponent]
  public abstract partial class LeapMeshGraphicBase : LeapGraphic {

    [EditTimeOnly]
    public Color vertexColor = Color.white;

    public Mesh mesh { get; protected set; }
    public UVChannelFlags remappableChannels { get; protected set; }

    public abstract void RefreshMeshData();

    public LeapMeshGraphicBase() {
#if UNITY_EDITOR
      editor = new MeshEditorApi(this);
#endif
    }
  }

  public class LeapMeshGraphic : LeapMeshGraphicBase {

    [EditTimeOnly]
    [SerializeField]
    private Mesh _mesh;

    [Tooltip("All channels that are allowed to be remapped into atlas coordinates.")]
    [EditTimeOnly]
    [EnumFlags]
    [SerializeField]
    private UVChannelFlags _remappableChannels = UVChannelFlags.UV0 |
                                                 UVChannelFlags.UV1 |
                                                 UVChannelFlags.UV2 |
                                                 UVChannelFlags.UV3;

    public void SetMesh(Mesh mesh) {
      if (isAttachedToGroup && !attachedGroup.addRemoveSupported) {
        Debug.LogWarning("Changing the representation of the graphic is not supported by this rendering type");
      }

      isRepresentationDirty = true;
      _mesh = mesh;
    }

    public override void RefreshMeshData() {
      mesh = _mesh;
      remappableChannels = _remappableChannels;
    }
  }
}