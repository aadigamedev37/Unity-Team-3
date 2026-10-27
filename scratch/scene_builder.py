import os
import sys
import uuid
import math
from datetime import datetime

# Ensure directories exist
SCENES_DIR = r"Assets/Scenes/LevelBlockoutReference"
DOCS_DIR = r"Assets/Documentation/LevelDesign"
os.makedirs(SCENES_DIR, exist_ok=True)
os.makedirs(DOCS_DIR, exist_ok=True)

# Shared GUIDs from Unity project
GUIDS = {
    'PlayerPrefab': 'fcf79d213ca14884587cbf5b2f2ff42e',
    'PlayerRootTransform': '4292794577101088139',
    
    'ShadowPrefab': '6fa31faf1ed7fea4d9574d53ef3b69eb',
    'ShadowRootTransform': '2563460784940571933',
    
    'DoorPrefab': '81425b68d46108d42882abfe6c5b9ce0',
    'DoorRootTransform': '3725110221008668686',
    
    'DrawbridgePrefab': 'db45a3b9b534e30419b923e0195dc4b8',
    'DrawbridgeRootTransform': '9195304213588873793',
    
    'PlatformPrefab': 'e551f24042b42b246b5079981de0a0db',
    'PlatformRootTransform': '486826206205589185',
    
    'DangerLightScript': 'f9b113b7d3da4b44aab81c2d18c18f0b',
    'SweepingHazardScript': '62e469d2a05e6fc4e8054f49372329ff',
    'TrapLightScript': '9d05ca06d59417d4bbd94977eed2d607',
    'ExitTriggerScript': '7b6c7e1c58076834380a1271a12c1bdd',
    
    'Mat_FloorGrid': '2b029dad924613341a2b569e8650353e',
    'Mat_WallPillar': '6de2beb7deaa1c644977a008a127a22b',
    'Mat_DoorSolid': 'f539d16efd7263d42bd3354361ae3243',
    'Mat_Drawbridge': 'e93dca697be07a843abc9508a363336e',
    'Mat_Platform': '52537103bb1a1834686abf93ab87e676',
    'Mat_DangerLightRed': '7c3930e7798a38c4b9ae43b81f95dc84',
    'Mat_TrapLightYellow': '16e2feb851ea5f144ba38d1580a278e5',
    'Mat_ExitZoneGreen': '42b4beb51767f4e48a5d4c93e71dd392',
    
    'TextMeshProScript': '9541d86e2fd84c1d9990edf0852d74ab',
    'TMP_FontAsset': '8f586378b4e144a9851e7b34d9b748ee',
}

class SceneBuilder:
    def __init__(self, code, name, bounds_w, bounds_l):
        self.code = code
        self.name = name
        self.bounds_w = bounds_w
        self.bounds_l = bounds_l
        self.id_counter = 100
        self.blocks = []
        self.root_children = []
        self.env_children = []
        self.gameplay_children = []
        self.labels_children = []
        self.floors_children = []
        self.walls_children = []
        self.obstacles_children = []
        self.player_children = []
        self.stealables_children = []
        self.hazards_children = []
        self.exit_children = []
        
        # Records for CSV
        self.records = []

    def get_id(self):
        self.id_counter += 1
        return self.id_counter

    def add_primitive_cube(self, name, parent_children_list, pos, rot, scale, mat_guid, category, value_source, notes):
        go_id = self.get_id()
        trans_id = self.get_id()
        mf_id = self.get_id()
        mr_id = self.get_id()
        col_id = self.get_id()
        
        parent_children_list.append(trans_id)

        # GameObject
        self.blocks.append(f"""--- !u!1 &{go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {trans_id}}}
  - component: {{fileID: {mf_id}}}
  - component: {{fileID: {mr_id}}}
  - component: {{fileID: {col_id}}}
  m_Layer: 0
  m_Name: {name}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1""")

        # MeshFilter (Cube 10202)
        self.blocks.append(f"""--- !u!33 &{mf_id}
MeshFilter:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Mesh: {{fileID: 10202, guid: 0000000000000000e000000000000000, type: 0}}""")

        # MeshRenderer
        self.blocks.append(f"""--- !u!23 &{mr_id}
MeshRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_CastShadows: 1
  m_ReceiveShadows: 1
  m_DynamicOccludee: 1
  m_StaticShadowCaster: 0
  m_MotionVectors: 1
  m_LightProbeUsage: 1
  m_ReflectionProbeUsage: 1
  m_RayTracingMode: 2
  m_RayTraceProcedural: 0
  m_RayTracingAccelStructBuildFlags: -1
  m_SmallMeshCulling: 1
  m_RenderingLayerMask: 1
  m_RendererPriority: 0
  m_Materials:
  - {{fileID: 2100000, guid: {mat_guid}, type: 2}}
  m_StaticBatchInfo:
    firstSubMesh: 0
    subMeshCount: 0
  m_StaticBatchRoot: {{fileID: 0}}
  m_ProbeAnchor: {{fileID: 0}}
  m_LightProbeVolumeOverride: {{fileID: 0}}
  m_ScaleInLightmap: 1
  m_ReceiveGI: 1
  m_PreserveUVs: 0
  m_IgnoreNormalsForChartDetection: 0
  m_ImportantGI: 0
  m_StitchLightmapSeams: 1
  m_SelectedEditorRenderState: 3
  m_MinimumChartSize: 4
  m_AutoUVMaxDistance: 0.5
  m_AutoUVMaxAngle: 89
  m_LightmapParameters: {{fileID: 0}}
  m_SortingLayerID: 0
  m_SortingLayer: 0
  m_SortingOrder: 0
  m_AdditionalVertexStreams: {{fileID: 0}}""")

        # BoxCollider
        self.blocks.append(f"""--- !u!65 &{col_id}
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Material: {{fileID: 0}}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 0
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {{x: 1, y: 1, z: 1}}
  m_Center: {{x: 0, y: 0, z: 0}}""")

        # Transform (placeholder, will be finalized with parent)
        self.blocks.append((trans_id, go_id, pos, rot, scale))
        
        self.records.append({
            'Level': self.code,
            'ObjectName': name,
            'Category': category,
            'Pos': pos,
            'Rot': rot,
            'Scale': scale,
            'ColType': 'BoxCollider',
            'ColSize': scale,
            'Source': value_source,
            'Notes': notes
        })

    def add_prefab_instance(self, name, parent_children_list, prefab_guid, root_transform_id, pos, rot, scale, category, value_source, notes):
        pref_id = self.get_id()
        # Add to parent list
        parent_children_list.append(pref_id)

        # PrefabInstance
        self.blocks.append(f"""--- !u!1001 &{pref_id}
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {{fileID: 0}}
    m_Modifications:
    - target: {{fileID: {root_transform_id}, guid: {prefab_guid}, type: 3}}
      propertyPath: m_LocalPosition.x
      value: {pos[0]}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {root_transform_id}, guid: {prefab_guid}, type: 3}}
      propertyPath: m_LocalPosition.y
      value: {pos[1]}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {root_transform_id}, guid: {prefab_guid}, type: 3}}
      propertyPath: m_LocalPosition.z
      value: {pos[2]}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {root_transform_id}, guid: {prefab_guid}, type: 3}}
      propertyPath: m_LocalRotation.x
      value: {rot[0]}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {root_transform_id}, guid: {prefab_guid}, type: 3}}
      propertyPath: m_LocalRotation.y
      value: {rot[1]}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {root_transform_id}, guid: {prefab_guid}, type: 3}}
      propertyPath: m_LocalRotation.z
      value: {rot[2]}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {root_transform_id}, guid: {prefab_guid}, type: 3}}
      propertyPath: m_LocalScale.x
      value: {scale[0]}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {root_transform_id}, guid: {prefab_guid}, type: 3}}
      propertyPath: m_LocalScale.y
      value: {scale[1]}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {root_transform_id}, guid: {prefab_guid}, type: 3}}
      propertyPath: m_LocalScale.z
      value: {scale[2]}
      objectReference: {{fileID: 0}}
    m_SourcePrefab: {{fileID: 100100000, guid: {prefab_guid}, type: 3}}""")

        self.records.append({
            'Level': self.code,
            'ObjectName': name,
            'Category': category,
            'Pos': pos,
            'Rot': rot,
            'Scale': scale,
            'ColType': 'PrefabCollider',
            'ColSize': scale,
            'Source': value_source,
            'Notes': notes
        })

    def add_danger_light(self, name, pos, trigger_size, is_sweeping=False, sweep_angle=35.0, sweep_speed=30.0, is_pulsing=False, notes=""):
        go_id = self.get_id()
        trans_id = self.get_id()
        col_id = self.get_id()
        dl_script_id = self.get_id()
        sweep_script_id = self.get_id() if is_sweeping else None
        
        self.hazards_children.append(trans_id)

        # Children of Hazard: Spotlight and FloorIndicator
        spot_go_id = self.get_id()
        spot_trans_id = self.get_id()
        spot_light_id = self.get_id()
        
        quad_go_id = self.get_id()
        quad_trans_id = self.get_id()
        quad_mf_id = self.get_id()
        quad_mr_id = self.get_id()

        components = [
            f"- component: {{fileID: {trans_id}}}",
            f"- component: {{fileID: {col_id}}}",
            f"- component: {{fileID: {dl_script_id}}}"
        ]
        if is_sweeping:
            components.append(f"- component: {{fileID: {sweep_script_id}}}")

        comp_str = "\n  ".join(components)

        # GameObject
        self.blocks.append(f"""--- !u!1 &{go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  {comp_str}
  m_Layer: 0
  m_Name: {name}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1""")

        # DangerLight MonoBehaviour
        self.blocks.append(f"""--- !u!114 &{dl_script_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUIDS['DangerLightScript']}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::DangerLight
  isActive: 1""")

        if is_sweeping:
            self.blocks.append(f"""--- !u!114 &{sweep_script_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUIDS['SweepingHazardScript']}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::SweepingHazard
  sweepEnabled: 1
  rotationAxis: {{x: 0, y: 1, z: 0}}
  sweepAngle: {sweep_angle}
  sweepSpeed: {sweep_speed}""")

        # BoxCollider trigger
        self.blocks.append(f"""--- !u!65 &{col_id}
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Material: {{fileID: 0}}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 1
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {{x: {trigger_size[0]}, y: {trigger_size[1]}, z: {trigger_size[2]}}}
  m_Center: {{x: 0, y: 0, z: 0}}""")

        # Hazard Transform with children
        self.blocks.append(f"""--- !u!4 &{trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: {spot_trans_id}}}
  - {{fileID: {quad_trans_id}}}
  m_Father: {{fileID: HAZARDS_ROOT_ID}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""")

        # Spotlight child
        self.blocks.append(f"""--- !u!1 &{spot_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {spot_trans_id}}}
  - component: {{fileID: {spot_light_id}}}
  m_Layer: 0
  m_Name: Spotlight
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!108 &{spot_light_id}
Light:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {spot_go_id}}}
  m_Enabled: 1
  serializedVersion: 11
  m_Type: 0
  m_Color: {{r: 1, g: 0.2, b: 0.1, a: 1}}
  m_Intensity: 4
  m_Range: 8
  m_SpotAngle: 65
  m_InnerSpotAngle: 45
  m_CookieSize: 10
  m_Shadows:
    m_Type: 0
--- !u!4 &{spot_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {spot_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0.7071068, y: 0, z: 0, w: 0.7071068}}
  m_LocalPosition: {{x: 0, y: {trigger_size[1] * 0.5 + 1.5}, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {trans_id}}}
  m_LocalEulerAnglesHint: {{x: 90, y: 0, z: 0}}""")

        # Quad Floor Indicator child
        self.blocks.append(f"""--- !u!1 &{quad_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {quad_trans_id}}}
  - component: {{fileID: {quad_mf_id}}}
  - component: {{fileID: {quad_mr_id}}}
  m_Layer: 0
  m_Name: DangerFootprint
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!33 &{quad_mf_id}
MeshFilter:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {quad_go_id}}}
  m_Mesh: {{fileID: 10210, guid: 0000000000000000e000000000000000, type: 0}}
--- !u!23 &{quad_mr_id}
MeshRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {quad_go_id}}}
  m_Enabled: 1
  m_Materials:
  - {{fileID: 2100000, guid: {GUIDS['Mat_DangerLightRed']}, type: 2}}
--- !u!4 &{quad_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {quad_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0.7071068, y: 0, z: 0, w: 0.7071068}}
  m_LocalPosition: {{x: 0, y: 0.05, z: 0}}
  m_LocalScale: {{x: {trigger_size[0]}, y: {trigger_size[2]}, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {trans_id}}}
  m_LocalEulerAnglesHint: {{x: 90, y: 0, z: 0}}""")

        self.records.append({
            'Level': self.code,
            'ObjectName': name,
            'Category': 'Hazard',
            'Pos': pos,
            'Rot': (0, 0, 0),
            'Scale': (1, 1, 1),
            'ColType': 'BoxCollider (Trigger)',
            'ColSize': trigger_size,
            'Source': 'DOCUMENTED',
            'Notes': notes or ('Sweeping Danger Light' if is_sweeping else ('Pulsing Danger Light' if is_pulsing else 'Static Danger Light'))
        })

    def add_trap_light(self, name, pos, trigger_size, notes=""):
        go_id = self.get_id()
        trans_id = self.get_id()
        col_id = self.get_id()
        tl_script_id = self.get_id()
        
        self.hazards_children.append(trans_id)

        spot_go_id = self.get_id()
        spot_trans_id = self.get_id()
        spot_light_id = self.get_id()
        
        quad_go_id = self.get_id()
        quad_trans_id = self.get_id()
        quad_mf_id = self.get_id()
        quad_mr_id = self.get_id()

        # GameObject
        self.blocks.append(f"""--- !u!1 &{go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {trans_id}}}
  - component: {{fileID: {col_id}}}
  - component: {{fileID: {tl_script_id}}}
  m_Layer: 0
  m_Name: {name}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1""")

        # TrapLight MonoBehaviour
        self.blocks.append(f"""--- !u!114 &{tl_script_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUIDS['TrapLightScript']}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::TrapLight
  isActive: 1""")

        # BoxCollider trigger
        self.blocks.append(f"""--- !u!65 &{col_id}
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Material: {{fileID: 0}}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 1
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {{x: {trigger_size[0]}, y: {trigger_size[1]}, z: {trigger_size[2]}}}
  m_Center: {{x: 0, y: 0, z: 0}}""")

        # Transform
        self.blocks.append(f"""--- !u!4 &{trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: {spot_trans_id}}}
  - {{fileID: {quad_trans_id}}}
  m_Father: {{fileID: HAZARDS_ROOT_ID}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""")

        # Purple Spotlight child
        self.blocks.append(f"""--- !u!1 &{spot_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {spot_trans_id}}}
  - component: {{fileID: {spot_light_id}}}
  m_Layer: 0
  m_Name: Spotlight
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!108 &{spot_light_id}
Light:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {spot_go_id}}}
  m_Enabled: 1
  serializedVersion: 11
  m_Type: 0
  m_Color: {{r: 0.75, g: 0.2, b: 1, a: 1}}
  m_Intensity: 4
  m_Range: 8
  m_SpotAngle: 65
  m_InnerSpotAngle: 45
  m_Shadows:
    m_Type: 0
--- !u!4 &{spot_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {spot_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0.7071068, y: 0, z: 0, w: 0.7071068}}
  m_LocalPosition: {{x: 0, y: {trigger_size[1] * 0.5 + 1.5}, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {trans_id}}}
  m_LocalEulerAnglesHint: {{x: 90, y: 0, z: 0}}""")

        # Quad TrapFootprint child
        self.blocks.append(f"""--- !u!1 &{quad_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {quad_trans_id}}}
  - component: {{fileID: {quad_mf_id}}}
  - component: {{fileID: {quad_mr_id}}}
  m_Layer: 0
  m_Name: TrapFootprint
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!33 &{quad_mf_id}
MeshFilter:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {quad_go_id}}}
  m_Mesh: {{fileID: 10210, guid: 0000000000000000e000000000000000, type: 0}}
--- !u!23 &{quad_mr_id}
MeshRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {quad_go_id}}}
  m_Enabled: 1
  m_Materials:
  - {{fileID: 2100000, guid: {GUIDS['Mat_TrapLightYellow']}, type: 2}}
--- !u!4 &{quad_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {quad_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0.7071068, y: 0, z: 0, w: 0.7071068}}
  m_LocalPosition: {{x: 0, y: 0.05, z: 0}}
  m_LocalScale: {{x: {trigger_size[0]}, y: {trigger_size[2]}, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {trans_id}}}
  m_LocalEulerAnglesHint: {{x: 90, y: 0, z: 0}}""")

        self.records.append({
            'Level': self.code,
            'ObjectName': name,
            'Category': 'Hazard',
            'Pos': pos,
            'Rot': (0, 0, 0),
            'Scale': (1, 1, 1),
            'ColType': 'BoxCollider (Trigger)',
            'ColSize': trigger_size,
            'Source': 'DOCUMENTED',
            'Notes': notes or 'Static Trap Light (Snare - requires 3 RECOVER pulses)'
        })

    def add_exit_zone(self, pos, trigger_size, notes=""):
        go_id = self.get_id()
        trans_id = self.get_id()
        col_id = self.get_id()
        exit_script_id = self.get_id()
        
        self.exit_children.append(trans_id)

        light_go_id = self.get_id()
        light_trans_id = self.get_id()
        light_comp_id = self.get_id()
        
        pad_go_id = self.get_id()
        pad_trans_id = self.get_id()
        pad_mf_id = self.get_id()
        pad_mr_id = self.get_id()

        # GameObject
        self.blocks.append(f"""--- !u!1 &{go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {trans_id}}}
  - component: {{fileID: {col_id}}}
  - component: {{fileID: {exit_script_id}}}
  m_Layer: 0
  m_Name: ExitZone
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1""")

        # ExitTrigger MonoBehaviour
        self.blocks.append(f"""--- !u!114 &{exit_script_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUIDS['ExitTriggerScript']}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::ExitTrigger""")

        # BoxCollider trigger
        self.blocks.append(f"""--- !u!65 &{col_id}
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Material: {{fileID: 0}}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 1
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {{x: {trigger_size[0]}, y: {trigger_size[1]}, z: {trigger_size[2]}}}
  m_Center: {{x: 0, y: {trigger_size[1] * 0.5}, z: 0}}""")

        # ExitZone Transform
        self.blocks.append(f"""--- !u!4 &{trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: {light_trans_id}}}
  - {{fileID: {pad_trans_id}}}
  m_Father: {{fileID: EXIT_ROOT_ID}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""")

        # Soft green light child
        self.blocks.append(f"""--- !u!1 &{light_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {light_trans_id}}}
  - component: {{fileID: {light_comp_id}}}
  m_Layer: 0
  m_Name: ExitLight
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!108 &{light_comp_id}
Light:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {light_go_id}}}
  m_Enabled: 1
  serializedVersion: 11
  m_Type: 2
  m_Color: {{r: 0.2, g: 1, b: 0.4, a: 1}}
  m_Intensity: 2.5
  m_Range: 6
--- !u!4 &{light_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {light_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 2, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""")

        # Green exit floor pad
        self.blocks.append(f"""--- !u!1 &{pad_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {pad_trans_id}}}
  - component: {{fileID: {pad_mf_id}}}
  - component: {{fileID: {pad_mr_id}}}
  m_Layer: 0
  m_Name: ExitPad
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!33 &{pad_mf_id}
MeshFilter:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {pad_go_id}}}
  m_Mesh: {{fileID: 10210, guid: 0000000000000000e000000000000000, type: 0}}
--- !u!23 &{pad_mr_id}
MeshRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {pad_go_id}}}
  m_Enabled: 1
  m_Materials:
  - {{fileID: 2100000, guid: {GUIDS['Mat_ExitZoneGreen']}, type: 2}}
--- !u!4 &{pad_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {pad_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0.7071068, y: 0, z: 0, w: 0.7071068}}
  m_LocalPosition: {{x: 0, y: 0.05, z: 0}}
  m_LocalScale: {{x: {trigger_size[0]}, y: {trigger_size[2]}, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {trans_id}}}
  m_LocalEulerAnglesHint: {{x: 90, y: 0, z: 0}}""")

        self.records.append({
            'Level': self.code,
            'ObjectName': 'ExitZone',
            'Category': 'Exit',
            'Pos': pos,
            'Rot': (0, 0, 0),
            'Scale': (1, 1, 1),
            'ColType': 'BoxCollider (Trigger)',
            'ColSize': trigger_size,
            'Source': 'DOCUMENTED',
            'Notes': notes or 'Level exit completion trigger pad'
        })

    def add_world_label(self, name, text, pos, color_hex="FFFFFFFF", font_size=3.5, notes=""):
        go_id = self.get_id()
        trans_id = self.get_id()
        mr_id = self.get_id()
        tmp_id = self.get_id()
        
        self.labels_children.append(trans_id)

        clean_text = text.replace('\n', '\\n')

        # GameObject
        self.blocks.append(f"""--- !u!1 &{go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {trans_id}}}
  - component: {{fileID: {mr_id}}}
  - component: {{fileID: {tmp_id}}}
  m_Layer: 0
  m_Name: {name}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1""")

        # MeshRenderer for TMP
        self.blocks.append(f"""--- !u!23 &{mr_id}
MeshRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_CastShadows: 0
  m_ReceiveShadows: 0
  m_Materials:
  - {{fileID: 2180264, guid: {GUIDS['TMP_FontAsset']}, type: 2}}""")

        # TextMeshPro component
        self.blocks.append(f"""--- !u!114 &{tmp_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUIDS['TextMeshProScript']}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: Unity.TextMeshPro::TMPro.TextMeshPro
  m_Material: {{fileID: 0}}
  m_Color: {{r: 1, g: 1, b: 1, a: 1}}
  m_text: '<mark=#000000B0 padding="14,14,6,6"><b><color=#{color_hex}>{clean_text}</color></b></mark>'
  m_fontSize: {font_size}
  m_fontSizeBase: {font_size}
  m_alignment: 514
  m_isRightToLeft: 0
  m_fontAsset: {{fileID: 11400000, guid: {GUIDS['TMP_FontAsset']}, type: 2}}
  m_sharedMaterial: {{fileID: 2180264, guid: {GUIDS['TMP_FontAsset']}, type: 2}}""")

        # Transform (angled 50 deg to face overview camera)
        self.blocks.append(f"""--- !u!4 &{trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0.4226183, y: 0, z: 0, w: 0.9063078}}
  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: LABELS_ROOT_ID}}
  m_LocalEulerAnglesHint: {{x: 50, y: 0, z: 0}}""")

        self.records.append({
            'Level': self.code,
            'ObjectName': name,
            'Category': 'Label',
            'Pos': pos,
            'Rot': (50, 0, 0),
            'Scale': (1, 1, 1),
            'ColType': 'None',
            'ColSize': (0, 0, 0),
            'Source': 'IMPLEMENTATION_ASSUMPTION',
            'Notes': f"Reference label: {clean_text}"
        })

    def build_yaml(self):
        root_go_id = self.get_id()
        root_trans_id = self.get_id()
        
        dir_light_go_id = self.get_id()
        dir_light_trans_id = self.get_id()
        dir_light_comp_id = self.get_id()
        
        cam_go_id = self.get_id()
        cam_trans_id = self.get_id()
        cam_comp_id = self.get_id()
        
        env_go_id = self.get_id()
        env_trans_id = self.get_id()
        floors_go_id = self.get_id()
        floors_trans_id = self.get_id()
        walls_go_id = self.get_id()
        walls_trans_id = self.get_id()
        obstacles_go_id = self.get_id()
        obstacles_trans_id = self.get_id()

        gameplay_go_id = self.get_id()
        gameplay_trans_id = self.get_id()
        player_root_go_id = self.get_id()
        player_root_trans_id = self.get_id()
        stealables_root_go_id = self.get_id()
        stealables_root_trans_id = self.get_id()
        hazards_root_go_id = self.get_id()
        hazards_root_trans_id = self.get_id()
        exit_root_go_id = self.get_id()
        exit_root_trans_id = self.get_id()

        labels_go_id = self.get_id()
        labels_trans_id = self.get_id()

        # Camera position based on room dimensions
        cam_z = self.bounds_l * 0.45
        cam_y = max(self.bounds_l * 0.7, 15.0)

        header = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestHole: 0.25
    smallestOccluder: 5
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {{fileID: 0}}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 10
  m_Fog: 0
  m_FogColor: {{r: 0.5, g: 0.5, b: 0.5, a: 1}}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {{r: 0.212, g: 0.227, b: 0.259, a: 1}}
  m_AmbientEquatorColor: {{r: 0.114, g: 0.125, b: 0.133, a: 1}}
  m_AmbientGroundColor: {{r: 0.047, g: 0.043, b: 0.035, a: 1}}
  m_AmbientIntensity: 1
  m_AmbientMode: 0
  m_SubtractiveShadowColor: {{r: 0.42, g: 0.478, b: 0.627, a: 1}}
  m_SkyboxMaterial: {{fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {{fileID: 0}}
  m_SpotCookie: {{fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {{fileID: 0}}
  m_Sun: {{fileID: {dir_light_comp_id}}}
  m_IndirectSpecularColor: {{r: 0.18028378, g: 0.22571412, b: 0.30692285, a: 1}}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 13
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 1
    m_EnableRealtimeLightmaps: 0
  m_LightmapEditorSettings:
    serializedVersion: 12
  m_LightingDataAsset: {{fileID: 0}}
  m_LightingSettings: {{fileID: 0}}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 3
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    buildHeightMesh: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {{fileID: 0}}"""

        # Scene Roots
        hierarchy_blocks = [
            # Directional Light
            f"""--- !u!1 &{dir_light_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {dir_light_trans_id}}}
  - component: {{fileID: {dir_light_comp_id}}}
  m_Layer: 0
  m_Name: Directional Light
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!108 &{dir_light_comp_id}
Light:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {dir_light_go_id}}}
  m_Enabled: 1
  serializedVersion: 11
  m_Type: 1
  m_Color: {{r: 1, g: 0.95686275, b: 0.8392157, a: 1}}
  m_Intensity: 1
  m_Range: 10
  m_SpotAngle: 30
  m_InnerSpotAngle: 21.80208
  m_CookieSize: 10
  m_Shadows:
    m_Type: 2
    m_Resolution: -1
    m_CustomResolution: -1
    m_Strength: 1
    m_Bias: 0.05
    m_NormalBias: 0.4
    m_NearPlane: 0.2
    m_CullingMatrixOverride:
      e00: 1
      e01: 0
      e02: 0
      e03: 0
      e10: 0
      e11: 1
      e12: 0
      e13: 0
      e20: 0
      e21: 0
      e22: 1
      e23: 0
      e30: 0
      e31: 0
      e32: 0
      e33: 1
    m_UseCullingMatrixOverride: 0
--- !u!4 &{dir_light_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {dir_light_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0.40821788, y: -0.23456968, z: 0.10938163, w: 0.8754261}}
  m_LocalPosition: {{x: 0, y: 15, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {root_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 50, y: -30, z: 0}}""",

            # Overview Camera
            f"""--- !u!1 &{cam_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {cam_trans_id}}}
  - component: {{fileID: {cam_comp_id}}}
  m_Layer: 0
  m_Name: OverviewCamera
  m_TagString: MainCamera
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!20 &{cam_comp_id}
Camera:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {cam_go_id}}}
  m_Enabled: 1
  serializedVersion: 2
  m_ClearFlags: 1
  m_BackGroundColor: {{r: 0.19215687, g: 0.3019608, b: 0.4745098, a: 0}}
  m_projectionMatrixMode: 1
  m_GateFitMode: 2
  m_FOVAxisMode: 0
  m_Iso: 200
  m_ShutterSpeed: 0.005
  m_Aperture: 16
  m_FocusDistance: 10
  m_FocalLength: 50
  m_BladeCount: 5
  m_Curvature: {{x: 2, y: 11}}
  m_BarrelClipping: 0.25
  m_Anamorphism: 0
  m_SensorSize: {{x: 36, y: 24}}
  m_LensShift: {{x: 0, y: 0}}
  m_NormalizedViewPortRect:
    serializedVersion: 2
    x: 0
    y: 0
    width: 1
    height: 1
  near clip plane: 0.3
  far clip plane: 1000
  field of view: 60
  orthographic: 0
  orthographic size: 5
  m_Depth: -1
  m_CullingMask:
    serializedVersion: 2
    m_Bits: 4294967295
  m_RenderingPath: -1
  m_TargetTexture: {{fileID: 0}}
  m_TargetDisplay: 0
  m_TargetEye: 3
  m_HDR: 1
  m_AllowMSAA: 1
  m_AllowDynamicResolution: 0
  m_ForceIntoRT: 0
  m_OcclusionCulling: 1
  m_StereoConvergence: 10
  m_StereoSeparation: 0.022
--- !u!4 &{cam_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {cam_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0.4617486, y: 0, z: 0, w: 0.8870108}}
  m_LocalPosition: {{x: 0, y: {cam_y}, z: {cam_z}}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {root_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 55, y: 0, z: 0}}""",

            # Root GameObject
            f"""--- !u!1 &{root_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {root_trans_id}}}
  m_Layer: 0
  m_Name: _LEVEL_{self.code}_ROOT
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{root_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {root_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: {dir_light_trans_id}}}
  - {{fileID: {cam_trans_id}}}
  - {{fileID: {env_trans_id}}}
  - {{fileID: {gameplay_trans_id}}}
  - {{fileID: {labels_trans_id}}}
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            # Environment Root & Sub-roots
            f"""--- !u!1 &{env_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {env_trans_id}}}
  m_Layer: 0
  m_Name: Environment
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{env_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {env_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: {floors_trans_id}}}
  - {{fileID: {walls_trans_id}}}
  - {{fileID: {obstacles_trans_id}}}
  m_Father: {{fileID: {root_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            f"""--- !u!1 &{floors_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {floors_trans_id}}}
  m_Layer: 0
  m_Name: Floors
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{floors_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {floors_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
{chr(10).join([f"  - {{{{fileID: {cid}}}}}" for cid in self.floors_children])}
  m_Father: {{fileID: {env_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            f"""--- !u!1 &{walls_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {walls_trans_id}}}
  m_Layer: 0
  m_Name: Walls
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{walls_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {walls_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
{chr(10).join([f"  - {{{{fileID: {cid}}}}}" for cid in self.walls_children])}
  m_Father: {{fileID: {env_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            f"""--- !u!1 &{obstacles_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {obstacles_trans_id}}}
  m_Layer: 0
  m_Name: Obstacles
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{obstacles_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {obstacles_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
{chr(10).join([f"  - {{{{fileID: {cid}}}}}" for cid in self.obstacles_children])}
  m_Father: {{fileID: {env_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            # Gameplay Root & Sub-roots
            f"""--- !u!1 &{gameplay_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {gameplay_trans_id}}}
  m_Layer: 0
  m_Name: Gameplay
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{gameplay_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {gameplay_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: {player_root_trans_id}}}
  - {{fileID: {stealables_root_trans_id}}}
  - {{fileID: {hazards_root_trans_id}}}
  - {{fileID: {exit_root_trans_id}}}
  m_Father: {{fileID: {root_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            f"""--- !u!1 &{player_root_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {player_root_trans_id}}}
  m_Layer: 0
  m_Name: PlayerSetup
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{player_root_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {player_root_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
{chr(10).join([f"  - {{{{fileID: {cid}}}}}" for cid in self.player_children])}
  m_Father: {{fileID: {gameplay_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            f"""--- !u!1 &{stealables_root_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {stealables_root_trans_id}}}
  m_Layer: 0
  m_Name: Stealables
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{stealables_root_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {stealables_root_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
{chr(10).join([f"  - {{{{fileID: {cid}}}}}" for cid in self.stealables_children])}
  m_Father: {{fileID: {gameplay_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            f"""--- !u!1 &{hazards_root_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {hazards_root_trans_id}}}
  m_Layer: 0
  m_Name: Hazards
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{hazards_root_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {hazards_root_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
{chr(10).join([f"  - {{{{fileID: {cid}}}}}" for cid in self.hazards_children])}
  m_Father: {{fileID: {gameplay_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            f"""--- !u!1 &{exit_root_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {exit_root_trans_id}}}
  m_Layer: 0
  m_Name: Exit
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{exit_root_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {exit_root_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
{chr(10).join([f"  - {{{{fileID: {cid}}}}}" for cid in self.exit_children])}
  m_Father: {{fileID: {gameplay_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}""",

            # ReferenceLabels Root
            f"""--- !u!1 &{labels_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {labels_trans_id}}}
  m_Layer: 0
  m_Name: ReferenceLabels
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{labels_trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {labels_go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
{chr(10).join([f"  - {{{{fileID: {cid}}}}}" for cid in self.labels_children])}
  m_Father: {{fileID: {root_trans_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}"""
        ]

        # Convert remaining blocks into YAML strings
        rendered_blocks = []
        for b in self.blocks:
            if isinstance(b, tuple):
                # Transform tuple: (trans_id, go_id, pos, rot, scale)
                trans_id, go_id, pos, rot, scale = b
                # Determine father ID
                if trans_id in self.floors_children: father_id = floors_trans_id
                elif trans_id in self.walls_children: father_id = walls_trans_id
                elif trans_id in self.obstacles_children: father_id = obstacles_trans_id
                elif trans_id in self.player_children: father_id = player_root_trans_id
                elif trans_id in self.stealables_children: father_id = stealables_root_trans_id
                elif trans_id in self.hazards_children: father_id = hazards_root_trans_id
                elif trans_id in self.exit_children: father_id = exit_root_trans_id
                elif trans_id in self.labels_children: father_id = labels_trans_id
                else: father_id = root_trans_id

                t_str = f"""--- !u!4 &{trans_id}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  serializedVersion: 2
  m_LocalRotation: {{x: {rot[0]}, y: {rot[1]}, z: {rot[2]}, w: 1}}
  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}
  m_LocalScale: {{x: {scale[0]}, y: {scale[1]}, z: {scale[2]}}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {father_id}}}
  m_LocalEulerAnglesHint: {{x: {rot[0]}, y: {rot[1]}, z: {rot[2]}}}"""
                rendered_blocks.append(t_str)
            elif isinstance(b, str):
                # Replace placeholder parent IDs
                s = b.replace('HAZARDS_ROOT_ID', str(hazards_root_trans_id))
                s = s.replace('EXIT_ROOT_ID', str(exit_root_trans_id))
                s = s.replace('LABELS_ROOT_ID', str(labels_trans_id))
                rendered_blocks.append(s)

        full_content = header + "\n" + "\n".join(hierarchy_blocks) + "\n" + "\n".join(rendered_blocks) + "\n"
        return full_content

print("SceneBuilder engine initialized.")
