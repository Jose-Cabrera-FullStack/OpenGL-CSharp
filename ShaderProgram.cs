using System;
using OpenTK.Graphics.OpenGL;

namespace Physic_Engine
{
    public sealed class ShaderProgram : IDisposable
    {
        private bool disposed;
        public readonly int ShaderProgramHandle;
        public readonly int VertexShaderHandle;
        public readonly int PixelShaderHandle;

        public ShaderProgram(string vertexShaderCode, string pixelShaderCode)
        {
            this.disposed = false;

            if (!ShaderProgram.CompileVertexShader(vertexShaderCode, out this.VertexShaderHandle, out string vertexShaderCompileError))
            {
                throw new ArgumentException(vertexShaderCompileError);
            }

            if (!ShaderProgram.CompilePixelShader(pixelShaderCode, out this.PixelShaderHandle, out string pixelShaderCompileError))
            {
                throw new ArgumentException(pixelShaderCompileError);
            }

            this.ShaderProgramHandle = ShaderProgram.CreateLinkProgram(this.VertexShaderHandle, this.PixelShaderHandle);
        }

        ~ShaderProgram()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            GL.DeleteShader(this.VertexShaderHandle);
            GL.DeleteShader(this.PixelShaderHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(this.ShaderProgramHandle);

            this.disposed = true;
            GC.SuppressFinalize(this);
        }

        public static bool CompileVertexShader(string vertexShaderCode, out int vertexShaderHandle, out string errorMessage)
        {
            errorMessage = string.Empty;
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);

            // Check for errors in the vertex shader
            string vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderHandle);
            if (vertexShaderInfo != string.Empty)
            {
                errorMessage = vertexShaderInfo;
                return false;
            }
            return true;
        }

        public static bool CompilePixelShader(string pixelShaderCode, out int pixelShaderHandle, out string errorMessage)
        {
            errorMessage = string.Empty;

            pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);

            // Check for errors in the pixel shader
            string pixelShaderInfo = GL.GetShaderInfoLog(pixelShaderHandle);
            if (pixelShaderInfo != string.Empty)
            {
                errorMessage = pixelShaderInfo;
                return false;
            }

            return true;

        }

        public static int CreateLinkProgram(int vertexShaderHandle, int pixelShaderHandle)
        {
            int shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, pixelShaderHandle);

            GL.LinkProgram(shaderProgramHandle);

            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, pixelShaderHandle);

            return shaderProgramHandle;
        }
    }
}