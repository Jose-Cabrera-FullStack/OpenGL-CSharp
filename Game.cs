
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Physic_Engine
{
    public class Game : GameWindow
    {
        private VertexBuffer vertexBuffer;
        private int indexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;

        public Game(int width = 1280, int height = 768, string title = "Physic Engine") : base(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                Title = title,
                Size = new Vector2i(width, height),
                WindowBorder = WindowBorder.Fixed,
                StartVisible = false,
                StartFocused = true,
                API = ContextAPI.OpenGL,
                Profile = ContextProfile.Core,
                APIVersion = new Version(3, 3),
            })
        {
            this.CenterWindow();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {

            this.IsVisible = true;
            GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1.0f));


            float x = 384f;
            float y = 400f;
            float w = 512f;
            float h = 256f;

            VertexPositionColor[] vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector2(x, y + h), new Color4(1f, 0f, 0f, 1f)),
                new VertexPositionColor(new Vector2(x + w, y + h), new Color4(0f, 1f, 0f, 1f)),
                new VertexPositionColor(new Vector2(x + w, y ), new Color4(0f, 0f, 1f, 1f)),
                new VertexPositionColor(new Vector2(x, y), new Color4(1f, 1f, 0f, 1f)),
            };

            int[] indeces = new int[] {
                0, 1, 2, 0, 2, 3
            };


            this.vertexBuffer = new VertexBuffer(VertexPositionColor.VertexInfo, vertices.Length, false);
            this.vertexBuffer.SetData(vertices, vertices.Length);

            int vertexSizeInBytes = VertexPositionColor.VertexInfo.SizeInBytes;

            this.indexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indeces.Length * sizeof(int), indeces, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            this.vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer.VertexBufferHandle);

            VertexAttribute attr0 = VertexPositionColor.VertexInfo.VertexAttributes[0];
            VertexAttribute attr1 = VertexPositionColor.VertexInfo.VertexAttributes[1];

            GL.VertexAttribPointer(attr0.Index, attr0.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, attr0.Offset);
            GL.VertexAttribPointer(attr1.Index, attr1.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, attr1.Offset);

            GL.EnableVertexAttribArray(attr0.Index);
            GL.EnableVertexAttribArray(attr1.Index);

            GL.BindVertexArray(0);

            string vertexShaderCode =
            @"
            #version 330 core

            uniform vec2 ViewportSize;

            layout (location = 0) in vec2 aPosition;
            layout (location = 1) in vec4 aColor;

            out vec4 vColor;

            void main()
            {
                float nx = aPosition.x / ViewportSize.x * 2 - 1;
                float ny = aPosition.y / ViewportSize.y * 2 - 1;
                gl_Position = vec4(nx, ny, 0, 1);

                vColor = aColor;
            }
            ";

            string pixelShaderCode =
            @"
            #version 330 core

            in vec4 vColor;

            out vec4 pixelColor;
            void main()
            {
                pixelColor = vColor;
            }";


            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);

            // Check for errors in the vertex shader
            string vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderHandle);
            if (vertexShaderInfo != string.Empty) Console.WriteLine(vertexShaderInfo);

            int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);

            // Check for errors in the pixel shader
            string pixelShaderInfo = GL.GetShaderInfoLog(pixelShaderHandle);
            if (pixelShaderInfo != string.Empty) Console.WriteLine(pixelShaderInfo);

            this.shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(this.shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(this.shaderProgramHandle, pixelShaderHandle);

            GL.LinkProgram(this.shaderProgramHandle);

            GL.DetachShader(this.shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(this.shaderProgramHandle, pixelShaderHandle);

            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(pixelShaderHandle);

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            GL.UseProgram(this.shaderProgramHandle);
            int ViewportSizeUniformLocation = GL.GetUniformLocation(this.shaderProgramHandle, "ViewportSize");
            GL.Uniform2(ViewportSizeUniformLocation, (float)viewport[2], (float)viewport[3]);
            GL.UseProgram(0);


            base.OnLoad();
        }

        protected override void OnUnload()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(this.vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(this.indexBufferHandle);


            this.vertexBuffer?.Dispose();

            GL.UseProgram(0);
            GL.DeleteProgram(this.shaderProgramHandle);

            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(this.shaderProgramHandle);
            GL.BindVertexArray(this.vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            this.Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }

    public readonly struct VertexPositionTexture
    {
        public readonly Vector2 Position;
        public readonly Vector2 TexCoord;

        public static readonly VertexInfo VertexInfo = new VertexInfo(
            typeof(VertexPositionTexture),
            new VertexAttribute("Position", 0, 2, 0),
            new VertexAttribute("TexCoord", 1, 2, 2 * sizeof(float))
        );

        public VertexPositionTexture(Vector2 position, Vector2 texCoord)
        {
            this.Position = position;
            this.TexCoord = texCoord;
        }
    }
}