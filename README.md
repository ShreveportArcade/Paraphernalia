# Paraphernalia 
Paraphernalia is a collection of utility scripts we use when we make stuff. We got tired of copying and pasting stuff from old projects and hunting things down on the Unify Community, so we organized them into folders, namespaced stuff, and put them in a git repo so we could just add them as a submodule when we start new projects. Everything is licensed under MIT with permission from the authors. We've done our best to document who did what. Enjoy.

## Attributes
These are property drawers for making arrays, enum masks, material indices, resource paths, and sorting layers easier to deal with.

## Editor Extensions
CubemapEditor - Adds an 'export to PNG' button to the standard cubemap inspector.

MonoScriptEditor - Allows you to toggle code display for scripts in the inspector. Adds a 'Create Asset' button to ScriptableObject inspectors.

SceneViewController - Adds buttons for changing the scene view camera.

* Look Right _Alt+1_
* Look Left _Alt+Shift+1_
* Look Forward _Alt+2_
* Look Back _Alt+Shift+2_
* Look Down _Alt+3_
* Look Up _Alt+Shift+3_
* Perspective _Alt+5_
* Orthographic _Alt+Shift+5_

ScriptableObjectUtility - Static functions for creating assets for ScriptableObjects. Used in the MonoScriptEditor.

Workflow

* Add Child _Alt+C_
* Add Parent at (0,0,0) _Ctrl/Cmd+G_
* Add Parent at Center of Selected Objects _Alt+G_
* Invert Selection _Alt+I_
* Select Children _Ctrl/Cmd+Alt+C_
* Move To Object to (0,0,0) _Alt+M_

## Modules
To use the following modules, you'll need to either namespace them properly or add the `using` directive at the top of whatever file you're using them in.

### Components
NameSingleton - This component allows for easily creating singletons based on the object name. If another GameObject exists with the same name, it immediately destroys itself. It also has a conveinence function for finding these singletons by name.

PinToViewport - Allows you to pin objects to the X component of the viewport based on a number from 0 (left edge) to 1 (right edge) where 0.5 is the center of the screen. Pinning based on the Y component is unnecessary since Unity does this for you. 

ScaleToViewport - Similarly to PinToViewport, this script scales and moves the X component of an object based on two viewport positions. This is less useful now that there's a solid UI system built in; however, you may find it useful for non-UI things.

TrackingCamera - An optionally bounded camera that tracks a transform with a given offest and interpolation type. This is not meant to be an end all, be all camera system, but it's a nice starting point when you're prototyping.

VerticalSpriteOrdering - Reorders a sprite based on its Y component. This is particulary useful for 2D games where objects can go in front of or behind another object based on Y position.

### Extensions
ArrayExtensions - generic functions for manipulating arrays

* Reverse - returns the reverse of the array
* Subarray - returns a subarray given a start point and length
* SetRange - sets the values of the array given an array and a start position

BoundsExtensions

* RandomPoint - returns a random point within bounds

ColorExtensions

* SetAlpha - functions for setting the alpha component of a single Color or an array of Colors

CubemapExtensions

* FaceToTexture2D - given a CubemapFace, returns a Texture2D
* GetColorInDirection - given a Vector3, returns a Color
* SaveToPNG - saves the Cubemap as a PNG to path given a CubeMappingType
* SaveTo4x3PNG - saves the Cubemap to path as a t-shaped 4x3 PNG
* SaveTo3x4PNG - saves the Cubemap to path as a t-shaped 3x4 PNG 
* SaveTo6x1PNG - saves the Cubemap to path as a 6x1 (long) PNG 
* SaveTo1x6PNG - saves the Cubemap to path as a 1x6 (tall) PNG 
* SaveToCylindricalPNG - saves the Cubemap to path as a cylindrically mapped PNG 
* SaveToSphericalPNG - saves the Cubemap to path as a spherically mapped PNG 
* Get4x3Texture2D - converts the Cubemap to a t-shaped 4x3 Texture2D
* Get3x4Texture2D - converts the Cubemap to a t-shaped 3x4 Texture2D 
* Get6x1Texture2D - converts the Cubemap to a 6x1 (long) Texture2D 
* Get1x6Texture2D - converts the Cubemap to a 1x6 (tall) Texture2D 
* GetCylindricalTexture2D - converts the Cubemap to a cylindrically mapped Texture2D 
* GetSphericalTexture2D - converts the Cubemap to a spherically mapped Texture2D 
* FastSaveToCylindricalPNG - saves the Cubemap to path as a cylindrically mapped PNG using a shader and the Blit function
* FastSaveToSphericalPNG - saves the Cubemap to path as a spherically mapped PNG using a shader and the Blit function
* SaveToPNGWithDirectionalMapping - saves a Cubemap to path using a directional texture, a shader, and the Blit function

GameObjectExtensions

* Instantiate - at runtime, calls GameObject.Instantiate, in the editor, calls UnityEditor.PrefabUtility.InstantiatePrefab
* GetStatic - in the editor, returns whether or not an object is marked as static
* SetStatic - allows you to mark an object as static in the editor
* DestroyChildren - destroys all children of a Transform
* DestroyChildrenAfter - destroys all children after a certain depth down the tree
* GetChildComponents - gets all child components of type T
* GetOrAddComponent - gets or adds then gets a component of type T
* DestroyComponent - destroys the first component of type T if it exists
* DestroyComponents - destroys all components of type T
* RendererBounds - returns the bounds containing all renderers attached to an object and its children
* IsVisible - returns true if any renderer attached to an object or its children is visible

RenderTextureExtensions

* ToTexture2D - returns a Texture2D representation of the RenderTexture
* SaveToPNG - saves the RenderTexture to PNG given a path

Rigidbody2DExtensions

* AddForce - replicates the AddForce function available on 3D rigidbodies

Texture2DExtensions

* RotateClockwise90 - returns the Texture2D rotated 90° clockwise
* RotateCounterclockwise90 - returns the Texture2D rotated 90° counterclockwise
* Rotate180 - returns the Texture2D rotated 180°
* SaveToPNG - saves the Texture2D to PNG given a path

Vector2Extensions

* GetPerpendicular - returns the Vector2 perpendicular to it

Vector3Extensions

* Average - returns the average of a Vector3 array
* Resample - takes a collection of unevenly spaced points on a path and resamples them into a collection of evenly spaced points on a path
* RemoveColinear - takes a collection of points on a path and removes the colinnear ones
* Winding - given a list of points on a path, returns a winding number
* ClosedWinding - similar to the Winding function but operates on a closed path
* PathLength - returns the length of a collections of points on a path
* MoveBy - given a Vector3 array and an offset, returns a Vector3 array offest by that amount
* GetBounds - returns the bounding box of a list of points
* ClipToBounds - given a list of points and a bounding box, returns a list of points constrained by those bounds

### Math
Line2D

* Distance - finds the distance from a Vector2 to the line
* Side - finds out which side of a line a point is on
* Intersect - returns the intersection of two lines

Matrix - a collection of matrix math utilities

Polygon - a collection of polygonal math utilites

### Utils
ColorUtils

* Random - returns a random color with optional alpha value or target color with weight
* HSVtoRGB - converts a Vector4 (HSV color) to a Color (RGB)
* RGBtoHSV - converts a Color (RGB) to a Vector4 (HSV color)
* HexToRGB - converts a hex string to a Color (RGB)
* RGBtoHex - converts a Color to a hex string

CubemapUtils

* GetCubeMappingType
* GetFace
* GetPlane
* GetIntersectionPoint

GameObjectUtils

* Destroy - calls GameObject.Destroy at runtime, calls GameObject.DestroyImmediate in the editor

Interpolate - a collection of easing functions and curves

MaxRectsBinPack - an efficient algorithm for packing rectangles into a square

MiniJSON - a lightweight JSON library

TextureGenerator

* SphericalMapping
* CylindricalMapping
* PerlinNoise

Triangulator - given a list of points, returns a triangulated list of indices to be used in generating a mesh