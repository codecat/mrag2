using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Mrag2
{
  public class _Model
  {
    public string Asset;
    public Model Model;
    public Texture2D[] Textures;

    // Heavily modified version of www.toymaker.info/Games/XNA/html/xna_bounding_box.html
    public BoundingBox[] CalculateBoundingBoxes()
    {
      return this.CalculateBoundingBoxes(1f);
    }
    public BoundingBox[] CalculateBoundingBoxes(float overrideScale)
    {
      Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
      Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

      Matrix[] transforms = new Matrix[this.Model.Bones.Count];
      this.Model.CopyAbsoluteBoneTransformsTo(transforms);

      List<BoundingBox> ret = new List<BoundingBox>();

      foreach (ModelMesh mesh in this.Model.Meshes) {
        Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        Matrix meshTransform = transforms[mesh.ParentBone.Index];

        foreach (ModelMeshPart part in mesh.MeshParts) {
          int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

          Vector3[] vertices = new Vector3[part.NumVertices];
          part.VertexBuffer.GetData<Vector3>(part.VertexOffset * stride, vertices, 0, part.NumVertices, stride);

          for (int i = 0; i < vertices.Length; i++) {
            Vector3 pos = Vector3.Transform(vertices[i], meshTransform * Matrix.CreateRotationX((float)System.Math.PI / 2f)) * overrideScale;

            meshMin = Vector3.Min(meshMin, pos);
            meshMax = Vector3.Max(meshMax, pos);
          }
        }

        modelMin = Vector3.Min(modelMin, meshMin);
        modelMax = Vector3.Max(modelMax, meshMax);

        ret.Add(BoundingBox.CreateFromPoints(new Vector3[] { modelMin, modelMax }));
      }

      return ret.ToArray();
    }
  }

  public class _Texture
  {
    public Texture2D[] Overrides = new Texture2D[0];
    public Texture2D[] NormalMaps = new Texture2D[0];
    public Vector2 Offset;
    public Vector2 Scale = Vector2.One;

    public static _Texture Normal;
  }

  public class _3D
  {
    public struct VertexPositionColorNormal
    {
      public Vector3 Position;
      public Vector2 TextureCoord;
      public Vector3 Normal;

      public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
          new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
          new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
          new VertexElement(sizeof(float) * (3 + 2), VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
      );
    }

    private static bool Initialized = false;

    private static GraphicsDevice graphicsDevice;

    private static Vector3 _CameraPosition;
    private static Vector3 _CameraTarget;

    private static float _FOV = 45f;
    private static float _AspectRatio;
    private static float _MinDistance = 1f;
    private static float _MaxDistance = 10000f; // TODO: Change default value of this

    public static Vector3 CameraPosition
    {
      get { return _CameraPosition; }
      set
      {
        _CameraPosition = value;
        CalculateView();
      }
    }
    public static Vector3 CameraTarget
    {
      get { return _CameraTarget; }
      set
      {
        _CameraTarget = value;
        CalculateView();
      }
    }

    public static float FOV
    {
      get { return _FOV; }
      set
      {
        _FOV = value;
        CalculateProjection();
      }
    }
    public static float AspectRatio
    {
      get { return _AspectRatio; }
      set
      {
        _AspectRatio = value;
        CalculateProjection();
      }
    }
    public static float MinDistance
    {
      get { return _MinDistance; }
      set
      {
        _MinDistance = value;
        CalculateProjection();
      }
    }
    public static float MaxDistance
    {
      get { return _MaxDistance; }
      set
      {
        _MaxDistance = value;
        CalculateProjection();
      }
    }

    public static Matrix View;
    public static Matrix Projection;

    public static bool EnableLighting = false;

    private static Effect lineEffect;

    public static List<PointLight> Lights = new List<PointLight>();
    public class PointLight
    {
      internal bool initialized = false;

      public Vector3 Position = Vector3.Zero;
      public float Power = 1f;
      public float FallOff = 20f;
      public Color Color = Color.White;

      public PointLight()
      {
        this.initialized = true;
      }
    }

    public static void Initialize(GraphicsDevice graphics)
    {
      graphicsDevice = graphics;

      _CameraPosition = new Vector3(0, 0, 0);
      _CameraTarget = new Vector3(0, 0, 0);

      _AspectRatio = graphics.Viewport.AspectRatio;

      lineEffect = new BasicEffect(graphicsDevice);
      _Texture.Normal = new _Texture();

      CalculateView();
      CalculateProjection();

      Initialized = true;
    }

    public static void EnableZBuffer()
    {
      graphicsDevice.DepthStencilState = new DepthStencilState() {
        DepthBufferEnable = true,
        DepthBufferFunction = CompareFunction.LessEqual,
        DepthBufferWriteEnable = true
      };
    }

    public static void DisableZBuffer()
    {
      graphicsDevice.DepthStencilState = new DepthStencilState();
      graphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1f, 0);
    }

    private static void setEffectMatrices(Effect effect, Matrix world, Matrix view, Matrix projection)
    {
      if (effect.Parameters["World"] != null) effect.Parameters["World"].SetValue(world);
      if (effect.Parameters["View"] != null) effect.Parameters["View"].SetValue(view);
      if (effect.Parameters["Projection"] != null) effect.Parameters["Projection"].SetValue(projection);
      if (effect.Parameters["WorldViewProj"] != null) effect.Parameters["WorldViewProj"].SetValue(world * view * projection);
    }

    private static void setEffectDiffuseColor(Effect effect, Color diffuseColor)
    {
      Vector4 diffColor = diffuseColor.ToVector4();
      if (effect.Parameters["DiffuseColor"] != null) effect.Parameters["DiffuseColor"].SetValue(diffColor);
    }

    public static void Begin()
    {
      CheckInitialized();
      EnableZBuffer();
    }

    private static void CheckInitialized()
    {
      if (!Initialized)
        throw new Exception("MrAG._3D.Initialize was never called!");
    }

    public static void CalculateView()
    {
      View = Matrix.CreateLookAt(CameraPosition, CameraTarget, Vector3.Up);
    }

    public static void CalculateProjection()
    {
      Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), AspectRatio, MinDistance, MaxDistance);
    }

    // TODO: Make less overloads, this is confusing as hell
    public static void DrawModel(_Model model, Vector3 position)
    {
      DrawModel(model, position, Vector3.Zero, Vector3.One, Color.White, _Texture.Normal);
    }
    public static void DrawModel(_Model model, Vector3 position, _Texture textureInfo)
    {
      DrawModel(model, position, Vector3.Zero, Vector3.One, Color.White, textureInfo);
    }
    public static void DrawModel(_Model model, Vector3 position, Color color)
    {
      DrawModel(model, position, Vector3.Zero, Vector3.One, color, _Texture.Normal);
    }
    public static void DrawModel(_Model model, Vector3 position, Color color, _Texture textureInfo)
    {
      DrawModel(model, position, Vector3.Zero, Vector3.One, color, textureInfo);
    }
    public static void DrawModel(_Model model, Vector3 position, Vector3 rotation)
    {
      DrawModel(model, position, rotation, Vector3.One, Color.White, _Texture.Normal);
    }
    public static void DrawModel(_Model model, Vector3 position, Vector3 rotation, _Texture textureInfo)
    {
      DrawModel(model, position, rotation, Vector3.One, Color.White, textureInfo);
    }
    public static void DrawModel(_Model model, Vector3 position, Vector3 rotation, Color color)
    {
      DrawModel(model, position, rotation, Vector3.One, color, _Texture.Normal);
    }
    public static void DrawModel(_Model model, Vector3 position, Vector3 rotation, Color color, _Texture textureInfo)
    {
      DrawModel(model, position, rotation, Vector3.One, color, textureInfo);
    }
    public static void DrawModel(_Model model, Vector3 position, Vector3 rotation, Vector3 scale)
    {
      DrawModel(model, position, rotation, scale, Color.White, _Texture.Normal);
    }
    public static void DrawModel(_Model model, Vector3 position, Vector3 rotation, Vector3 scale, Color color)
    {
      DrawModel(model, position, rotation, scale, color, _Texture.Normal);
    }
    public static void DrawModel(_Model model, Vector3 position, Vector3 rotation, Vector3 scale, Color color, _Texture textureInfo)
    {
      CheckInitialized();

      if (model == null) return;

      Matrix[] transforms = new Matrix[model.Model.Bones.Count];
      model.Model.CopyAbsoluteBoneTransformsTo(transforms);

      Matrix m = Matrix.CreateRotationZ(rotation.Y) * Matrix.CreateRotationY(rotation.X) * Matrix.CreateRotationX(rotation.Z) *
                 Matrix.CreateScale(scale.X, scale.Z, scale.Y) * Matrix.CreateTranslation(new Vector3(position.X, position.Z, position.Y));

      int i = 0;
      foreach (ModelMesh mesh in model.Model.Meshes) {
        foreach (Effect effect in mesh.Effects) {
          setEffectMatrices(effect, transforms[mesh.ParentBone.Index] * m, View, Projection);

          if (effect.GetType() == typeof(BasicEffect)) {
            if (EnableLighting) (effect as BasicEffect).EnableDefaultLighting();
            (effect as BasicEffect).DiffuseColor = color.ToVector3();
            continue;
          }

          setEffectDiffuseColor(effect, color);

          bool textureDataSet = false;
          if (i < textureInfo.Overrides.Length) {
            textureDataSet = true;
            if (effect.Parameters["xTexture"] != null) effect.Parameters["xTexture"].SetValue(textureInfo.Overrides[i]);
          } else {
            if (i < model.Textures.Length) {
              textureDataSet = true;
              if (effect.Techniques["TechniqueTextured"] != null) effect.CurrentTechnique = effect.Techniques["TechniqueTextured"];
              if (effect.Parameters["xTexture"] != null) effect.Parameters["xTexture"].SetValue(model.Textures[i]);
            } else
              if (effect.Techniques["TechniqueNonTextured"] != null) effect.CurrentTechnique = effect.Techniques["TechniqueNonTextured"];
          }

          if (textureDataSet) {
            if (i < textureInfo.NormalMaps.Length) {
              if (effect.Techniques["TechniqueTexturedNormalMap"] != null) effect.CurrentTechnique = effect.Techniques["TechniqueTexturedNormalMap"];
              if (effect.Parameters["xNormalMap"] != null) effect.Parameters["xNormalMap"].SetValue(textureInfo.NormalMaps[i]);
            }

            if (effect.Parameters["xTextureOffset"] != null) effect.Parameters["xTextureOffset"].SetValue(textureInfo.Offset);
            if (effect.Parameters["xTextureScale"] != null) effect.Parameters["xTextureScale"].SetValue(textureInfo.Scale);

            i++;
          }

          if (EnableLighting) {
            if (effect.Parameters["xLights"] != null) {
              int maxLightCount = effect.Parameters["xLights"].Elements.Count;
              PointLight[] rangeLights = (from light in Lights
                                          let dist = (position - light.Position).Length()
                                          where dist < light.FallOff
                                          orderby dist ascending
                                          select light).Take(maxLightCount).ToArray();
              for (int j = 0; j < maxLightCount; j++) {
                EffectParameter effParam = effect.Parameters["xLights"].Elements[j];

                if (j >= rangeLights.Length) {
                  for (int k = j; k < maxLightCount; k++)
                    effParam.StructureMembers["Initialized"].SetValue(false);
                  break;
                }

                PointLight light = rangeLights[j];

                effParam.StructureMembers["Initialized"].SetValue(light.initialized);
                effParam.StructureMembers["Position"].SetValue(light.Position);
                effParam.StructureMembers["Power"].SetValue(light.Power);
                effParam.StructureMembers["FallOff"].SetValue(light.FallOff);
                effParam.StructureMembers["Color"].SetValue(light.Color.ToVector4());
              }
            }
          }
        }

        mesh.Draw();
      }
    }

    private static short[] primitiveIndexData(short count)
    {
      short[] ret = new short[count];
      for (short i = 0; i < count; i++)
        ret[i] = i;
      return ret;
    }

    public static void DrawLine(Vector3 pos1, Vector3 pos2)
    {
      DrawLines(new Vector3[] { pos1, pos2 }, Color.White);
    }
    public static void DrawLine(Vector3 pos1, Vector3 pos2, Color color)
    {
      DrawLines(new Vector3[] { pos1, pos2 }, color);
    }

    public static void DrawLines(Vector3[] verts)
    {
      DrawLines(verts, Color.White);
    }
    public static void DrawLines(Vector3[] verts, Color color)
    {
      VertexPositionColor[] vertices = new VertexPositionColor[verts.Length];
      for (int i = 0; i < verts.Length; i++)
        vertices[i] = new VertexPositionColor(new Vector3(verts[i].X, verts[i].Z, verts[i].Y), color);

      setEffectDiffuseColor(lineEffect, color);
      setEffectMatrices(lineEffect, Matrix.CreateRotationZ(0) * Matrix.CreateRotationY(0) * Matrix.CreateRotationX(0) * Matrix.CreateScale(1) * Matrix.CreateTranslation(Vector3.Zero), View, Projection);

      foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes) pass.Apply();

      graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, vertices.Length, primitiveIndexData((short)vertices.Length), 0, vertices.Length - 1);
    }

    public static void DrawBoxOutline(Vector3 min, Vector3 max)
    {
      DrawBoxOutline(new BoundingBox(min, max), Color.White);
    }
    public static void DrawBoxOutline(Vector3 min, Vector3 max, Color color)
    {
      DrawBoxOutline(new BoundingBox(min, max), color);
    }
    public static void DrawBoxOutline(BoundingBox bbox)
    {
      DrawBoxOutline(bbox, Color.White);
    }
    public static void DrawBoxOutline(BoundingBox bbox, Color color)
    {
      Vector3[] corners = bbox.GetCorners();

      Mrag2._3D.DrawLines(new Vector3[] { corners[0], corners[1], corners[2], corners[3], corners[0] }, color);
      Mrag2._3D.DrawLines(new Vector3[] { corners[4], corners[5], corners[6], corners[7], corners[4] }, color);
      Mrag2._3D.DrawLine(corners[0], corners[4], color);
      Mrag2._3D.DrawLine(corners[1], corners[5], color);
      Mrag2._3D.DrawLine(corners[2], corners[6], color);
      Mrag2._3D.DrawLine(corners[3], corners[7], color);
    }

    public static Ray RayCast(int X, int Y)
    {
      Vector3 nearSource = new Vector3(X, Y, 0f);
      Vector3 farSource = new Vector3(X, Y, 1f);

      Matrix world = Matrix.CreateScale(1) * Matrix.CreateTranslation(Vector3.Zero);
      Vector3 nearPoint = graphicsDevice.Viewport.Unproject(nearSource, Projection, View, world);
      Vector3 farPoint = graphicsDevice.Viewport.Unproject(farSource, Projection, View, world);

      Vector3 dir = farPoint - nearPoint;
      dir.Normalize();

      return new Ray(new Vector3(nearPoint.X, nearPoint.Z, nearPoint.Y), new Vector3(dir.X, dir.Z, dir.Y));
    }
  }
}
