
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Physic_Engine
{
    public class Game : GameWindow
    {
        private IndexBuffer indexBuffer;
        private VertexBuffer vertexBuffer;
        private VertexArray vertexArray;
        private int shaderProgramHandle;

        private int vertexCount;
        private int IndexCount;

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
            // GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1.0f));
            GL.ClearColor(0.8f, 0.8f, 0.8f, 1.0f); // Background color

            Random rand = new Random();

            int windowWidth = this.ClientSize.X;
            int windowHeight = this.ClientSize.Y;

            int boxCount = 1000;

            VertexPositionColor[] vertices = new VertexPositionColor[boxCount * 4];
            this.vertexCount = 0;

            for (int i = 0; i < boxCount; i++)
            {
                int w = rand.Next(32, 128);
                int h = rand.Next(32, 128);
                int x = rand.Next(0, windowWidth - w);
                int y = rand.Next(0, windowHeight - h);

                float r = (float)rand.NextDouble();
                float g = (float)rand.NextDouble();
                float b = (float)rand.NextDouble();

                vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x, y + h), new Color4(r, g, b, 1f));
                vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x + w, y + h), new Color4(r, g, b, 1f));
                vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x + w, y), new Color4(r, g, b, 1f));
                vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x, y), new Color4(r, g, b, 1f));

            }

            int[] indeces = new int[boxCount * 6];
            this.IndexCount = 0;
            this.vertexCount = 0;

            for (int i = 0; i < boxCount; i++)
            {
                indeces[this.IndexCount++] = 0 + this.vertexCount;
                indeces[this.IndexCount++] = 1 + this.vertexCount;
                indeces[this.IndexCount++] = 2 + this.vertexCount;
                indeces[this.IndexCount++] = 0 + this.vertexCount;
                indeces[this.IndexCount++] = 2 + this.vertexCount;
                indeces[this.IndexCount++] = 3 + this.vertexCount;

                this.vertexCount += 4;
            }


            this.vertexBuffer = new VertexBuffer(VertexPositionColor.VertexInfo, vertices.Length, true);
            this.vertexBuffer.SetData(vertices, vertices.Length);

            this.indexBuffer = new IndexBuffer(indeces.Length, true);
            this.indexBuffer.SetData(indeces, indeces.Length);

            this.vertexArray = new VertexArray(this.vertexBuffer);


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
            this.vertexArray?.Dispose();
            this.indexBuffer?.Dispose();
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
            GL.BindVertexArray(this.vertexArray.VertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer.IndexBufferHandle);
            GL.DrawElements(PrimitiveType.Triangles, this.IndexCount, DrawElementsType.UnsignedInt, 0);

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