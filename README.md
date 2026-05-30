# AR Video Player with Transparent Playback

A premium mobile Augmented Reality (AR) application built in Unity that detects vertical walls, allows users to place a virtual video screen, and plays remote-loaded addressable videos with advanced transparency support.

---

## 🚀 Key Features Implemented

1. **Stable World-Space Anchoring**: Replaces jittery, plane-attached tracking with a dedicated `ARAnchor` mapped to the room's spatial tracking features, completely resolving screen drifting and sliding.
2. **Advanced Transparent Video Playback**:
   *   **Chroma Key Shader**: Custom URP shader (`Custom/URP/ChromaKeyVideo`) that keys out a specific background color in real-time. Features adjustable tolerance, edge softness, custom green despill edge desaturation, and alpha multiplier control.
3. **UI/UX Flow & Polish**:
   *   **Loading Screen**: An animated loading panel (rotating icon) that deactivates once references are resolved.
   *   **Interactive Controls**: Sliding top-menu panel and collapsible bottom panel containing Rotate and Scale tools.
   *   **Status Guidance Box**: Dynamic status updates guiding the user step-by-step (*"Scan the wall slowly"*, *"Wall scanned. Press to place"*, *"loading video..."*, *"playing video..."*).
4. **Vertical Plane Filtering**: Filters out small, noisy planes (e.g., clutter, laptops, or chairs) by requiring a minimum scanned size of **$50\text{ cm} \times 50\text{ cm}$** before allowing placement.

---

## 🛠️ Project Setup Steps

### 1. Requirements & Tools
*   **Unity Editor**: Unity 6 / Unity 2023+ (specifically configured for Universal Render Pipeline (URP)).
*   **Target Platform**: Android (compatible with ARCore) or iOS (compatible with ARKit).
*   **Key SDKs**: AR Foundation (v6.x), Unity Addressables.

### 2. Scene Configuration
1. Open the project in Unity.
2. Ensure you have the `Main` scene open (`Assets/DataFiles/Scenes/Main.unity`).
3. The hierarchy is structured as follows:
   *   **`XR Origin`**: Contains the AR Session components. The **AR Plane Manager** is optimized to scan **`Vertical`** planes only.
   *   **`AR Placement Controller`**: Contains the placement logic script, pointing to the `WallTestQuad` screen prefab.
   *   **`ARPlacementReticle`**: Follows your camera focus and snaps to detected walls to show placement alignment.
   *   **`Canvas`**: Holds the UI screens:
       *   `TopMenuPanel`: Slides down from the top (Next Video / Screen Controls).
       *   `BottomScreenControlPanel`: Slides up from the bottom (Scale and Rotate tools).
       *   `LoadingPanel`: Shows the initial rotating loading icon.
       *   `Status`: Houses the guidance text box.

---

## 📦 Addressables Configuration

The video assets are loaded remotely to keep the initial app installation size small.

### 1. Addressables Settings
*   **Configuration File:** [`Assets/AddressableAssetsData/AddressableAssetSettings.asset`](file:///c:/Projects/Assign_AR-VideoPlayer/Assets/AddressableAssetsData/AddressableAssetSettings.asset)
*   **Active Profile:** `Default`
*   **Profiles Load/Build Paths:**
    *   **Local Build Path:** `[UnityEngine.AddressableAssets.Addressables.BuildPath]/[BuildTarget]`
    *   **Local LoadPath:** `[UnityEngine.AddressableAssets.Addressables.RuntimePath]/[BuildTarget]`
    *   **Remote Build Path:** `ServerData/[BuildTarget]`
    *   **Remote Load Path:** `https://ar-videoplayer.web.app/ServerData/[BuildTarget]` *(Ensured to have the secure `https://` protocol prefix).*

### 2. Asset Groups
*   A remote group named **`RemoteVideos`** contains the video assets:
    *   `video_01` (WebM format)
    *   `video_02` (MP4 format)
    *   `video_03` (MP4 format)
*   The addressable keys in the groups have been trimmed (e.g. `video_01`) to match the keys defined in the `Video Addressables Loader` script on the screen prefab.

### 3. How to Build Addressable Content
Every time a video is added, modified, or renamed:
1. In the Unity Editor, open the **Addressables Groups** window (**Window > Asset Management > Addressables > Groups**).
2. Click **Build > New Build > Default Build Script**.
3. This generates the bundle files inside the local `[ProjectFolder]/ServerData/Android` directory.

---

## 🌐 Firebase Hosting / CDN Details

We use **Firebase Hosting** to serve the video AssetBundles remotely.

*   **Hosting URL Domain:** `https://ar-videoplayer.web.app`
*   **Target Folder:** `/ServerData/Android`

### Deployment Steps
1. Rebuild the Addressables in Unity using the **Default Build Script**.
2. Locate the generated files in your project directory:
   `[ProjectRoot]/ServerData/Android/` (contains `.bundle`, `.json`, and `.hash` files).
3. Copy/Move these files into your Firebase Hosting directory under the path:
   `public/ServerData/Android/`
4. Deploy the files to Firebase Hosting using your terminal:
   ```bash
   firebase deploy --only hosting
   ```
5. Once deployed, the mobile app will automatically download the correct video bundles from `https://ar-videoplayer.web.app/ServerData/Android/` when placed.

---

## 🎨 Custom Shader for Video Transparency

To use transparent videos, the video player renders onto a **Render Texture**, which is assigned to a material using our custom URP Chroma Key shader:

*   **Shader Name:** `Custom/URP/ChromaKeyVideo`
*   **File Path:** [`TransparentVideoChromaKey.shader`](file:///c:/Projects/Assign_AR-VideoPlayer/Assets/DataFiles/Shaders/TransparentVideoChromaKey.shader)
*   **Shader Properties & Settings:**
    *   `_BaseMap` (Video Texture / RenderTexture): The source video Render Texture.
    *   `_BaseColor` (Color Tint): Overall color multiplier (allows tinting and alpha opacity adjustments).
    *   `_KeyColor` (Key Color): The exact color of the background to remove (default is green `0, 1, 0, 1`).
    *   `_Tolerance` (Tolerance): Controls how close a pixel's color must be to the `Key Color` to be keyed out (range `0` to `1`).
    *   `_Softness` (Softness): Smooths out the edges of the keyed selection to prevent pixelated borders (range `0` to `1`).
    *   `_Despill` (Green Despill): Desaturates the green tint that bleeds onto the edges of your subject (spill lighting), giving clean borders (range `0` to `1`).
    *   `_AlphaMultiplier` (Alpha Multiplier): Adjusts the opacity intensity of the resulting transparent output (range `0` to `2`).
