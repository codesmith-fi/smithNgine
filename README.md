# smithNgineLib
smithNgine Library and Core for MonoGame
Code and design by Erno Pakarinen (alias Moonlite)

This is a port from my original game engine based on XNA Framework by Microsoft, ported to MonoGame which is a open source port of XNA.

Original snithNgine features
- Sprites with transformations (rotate, move, scale etc.) with Events like (click, drag etc.)
- Animated sprites and SpriteAtlases 
- Full 2D Camera support with Position, Rotation and Scaling with parallax layers
- Evemts for basic entities
- Basic UI elements (Buttons, MenuEntries)
- Game states with layers and canvases
  -> Allows separating the game to separate sub states handled and rendered separately
  -> Switching between states
- Fully OO designed ParticleSystem with Effects, Generators and Emitters, designed to be easily extendable
- Events for primities
- General useful utility classes and methods
  -> Several ready made (time based or other) Interpolations, Constants, random number generator math helpers
- 2D physics engine support implemented with FarseerPhysics (see FarseerPhysics/Readme.txt for more info and licensing details)

2025 new features
- Basic 3D rendering pipeline which supports textured Object3D objects rendered using meshes.
- Scene and ligting being designed and implemented

