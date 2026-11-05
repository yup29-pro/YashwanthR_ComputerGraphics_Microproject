# RoboSight
### 3D Robot Workspace Visibility Analyzer

**See What Your Robot Sees**

---

## 1. Project Overview

RoboSight is a 3D robot workspace visibility analysis system developed using Unity. It determines which objects in a simulated warehouse environment are visible or hidden from a robot's camera.

The system identifies objects such as barrels, crates, and pallets and dynamically changes their materials based on their visibility status.

It demonstrates fundamental computer graphics concepts, including depth testing, hidden surface removal, camera projection, and raycasting.

## 2. Problem Statement

In a 3D robotic workspace, it is important to determine which objects are visible to the robot's camera and which are obstructed by other objects.

Without proper visibility analysis, robots may have difficulty identifying obstacles and understanding their surroundings.

RoboSight addresses this problem through real-time camera-based visibility detection and visual feedback.

## 3. Objectives

- Develop a realistic 3D warehouse environment.
- Simulate a first-person robotic camera.
- Detect barrels, crates, and pallets.
- Identify visible and occluded objects.
- Implement raycasting-based visibility analysis.
- Demonstrate depth testing using the Z-Buffer concept.
- Provide real-time visual feedback using colour-coded materials.

## 4. Technologies Used

| Technology | Purpose |
|---|---|
| Unity 6.3 LTS | 3D environment development |
| C# | Visibility detection scripts |
| Sci-Fi Warehouse Kit | 3D environment and assets |
| Unity Physics | Raycasting and collision detection |
| Z-Buffer | Depth testing and hidden surface removal |
| Universal Render Pipeline (URP) | Graphics rendering |

## 5. System Features

### Level 1 – Robot Camera View

- First-person camera simulation.
- Navigation through a 3D warehouse.
- Observation of objects from the robot's viewpoint.
- Detection of multiple warehouse objects.

### Level 2 – Visibility Status

The system assigns different materials based on object visibility.

| Colour | Meaning |
|---|---|
| Green | Visible to the robot |
| Red | Occluded or outside camera view |

Visibility is determined using camera projection and raycasting.

### Level 3 – Z-Buffer and Hidden Surface Removal

- **Viewport Projection & Frustum Culling**: Evaluates objects against camera viewport bounds ($x \in [0, 1]$, $y \in [0, 1]$, $z > 0$). Objects outside the field of view or behind the camera are immediately identified as non-visible.
- **Depth Testing (Z-Buffer Concept)**: Casts rays from the robot camera to each target object's bounding center. When multiple colliders lie along the projection path, hits are sorted by distance / depth from the camera.
- **Hidden Surface Removal**: The nearest collider along the ray determines visibility. If an obstructing obstacle is closer than the target object, the target is determined to be occluded and its material is switched to Red. If the target itself is hit first, it is determined to be visible and shaded Green.

---

## 6. Project Structure

```
Assets/
├── Scenes/
│   ├── RoboSight_Main.unity      # Main warehouse simulation scene
│   └── SampleScene.unity         # Default sample scene
├── Scripts/
│   └── VisibilityAnalyzer.cs     # Real-time visibility and depth analysis script
├── Hidden_Red.mat                # Material applied to occluded/hidden objects
├── Visible_Green.mat             # Material applied to visible objects
└── SciFi Warehouse Kit/          # 3D models, textures, materials, and warehouse environment
```

## 7. How to Run

1. Open the project in Unity Editor (Unity 2022.3 LTS / Unity 6 compatible).
2. Navigate to `Assets/Scenes/` in the Project window and open `RoboSight_Main.unity`.
3. Press the **Play** button at the top of the Unity Editor.
4. Navigate the robotic camera through the warehouse to observe dynamic real-time visibility detection and color updates on crates, barrels, and pallets.

