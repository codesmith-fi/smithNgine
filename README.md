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

Example for using the 3D renderer:

        testPolygonTexture1 = Content.Load<Texture2D>("Textures/pillar_texture");
        testPolygonTexture2 = Content.Load<Texture2D>("Textures/dogs1");
        renderer3D = new Renderer3D(GraphicsDevice);

        // Create cube with texture, size and position
        testObject1 = CreateCube(testPolygonTexture1, 50.0f, new Vector3(0, 0, 200));
        testObject2 = CreateCube(testPolygonTexture1, 100.0f, new Vector3(100, 100, 200));
        testObject3 = CreateCube(testPolygonTexture2, 75.0f, new Vector3(-100, 0, 200));

        var camera3D = new Camera3D(
            new Vector3(0, 0, -100),
            new Vector3(0, 0, 0),
            Vector3.Up,
            MathHelper.ToRadians(45f), // Field of view
            GraphicsDevice.Viewport.AspectRatio, // Aspect ratio
            1f, // Near plane
            1000f // Far plane
        );
        scene3D = new Scene3D(camera3D);

        scene3D.AddObject(testObject1);
        scene3D.AddObject(testObject2);
        scene3D.AddObject(testObject3);

        PointLight pointLight = new PointLight(new Vector3(0, 0, -50), Color.White, 1.0f)
        {
            ConstantAttenuation = 1.0f,
            LinearAttenuation = 0.1f,
            QuadraticAttenuation = 0.01f
        };
        scene3D.AddLight(pointLight);

        // Register supported effects for the 3D renderer
        renderer3D.RegisterEffect<BasicEffectParameters>(
            EffectType.Basic,
            LoadEffectFromFile("Content/Shaders/Basic.mgfxo"));
        renderer3D.RegisterEffect<BasicTextureEffectParameters>(
            EffectType.BasicTexture,
            LoadEffectFromFile("Content/Shaders/BasicTexture.mgfxo"));
        renderer3D.RegisterEffect<LitTextureAmbientDiffuseEffectParameters>(
            EffectType.LitTextureAmbientDiffuse,
            LoadEffectFromFile("Content/Shaders/LitTextureAmbientDiffuse.mgfxo"));

        testObject1.SetEffect(EffectType.BasicTexture);
        testObject2.SetEffect(EffectType.LitTextureAmbientDiffuse);
        testObject3.SetEffect(EffectType.Basic);

        // Set parameters for registered effects
        basicEffectParameters = new BasicEffectParameters() { };
        renderer3D.ApplyParameters(EffectType.Basic, basicEffectParameters);
        basicTextureEffectParameters = new BasicTextureEffectParameters();
        renderer3D.ApplyParameters(EffectType.BasicTexture, basicTextureEffectParameters);
        litAmbientDiffuseEffectParameters = new LitTextureAmbientDiffuseEffectParameters()
        {
            AmbientColor = Color.White,
            AmbientIntensity = 0.50f,
            DiffuseColor = Color.White,
            DiffuseIntensity = 1.0f,
            LightDirection = new Vector3(0f, 0f, 1f)
        };
        renderer3D.ApplyParameters(EffectType.LitTextureAmbientDiffuse, litAmbientDiffuseEffectParameters);

  