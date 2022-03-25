//Coded by Joshua using tutorials from 2-Bit coding on youtube(https://www.youtube.com/channel/UCsCBEU7sWvEMlTzP3jE0olg)
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Threading;
namespace OpenTKGraphics {
    class Program {
        public class Game:GameWindow{
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;
        private float frames = 0;
        public Game() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
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
            //                              Background Color
            GL.ClearColor(Color4.GreenYellow);
            Serverlog("Background loaded");
            float[] vertices = new float[]
            {
                0.0f, 0.5f, 0f,
                0.5f, -0.5f, 0f,
                -0.5f, -0.5f, 0f,
            };
            this.vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Serverlog("Vertex Buffer loaded");
            Thread.Sleep(500);
            this.vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7*sizeof(float), 3*sizeof(float));
            Serverlog("Vertex Array loaded");
            Thread.Sleep(500);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);

            string vertexShaderCode = 
                @"
                #version 330 core
                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec4 aColor;
                out vec4 vColor;
                void main()
                {
                    vColor = aColor;
                    gl_Position = vec4(aPosition, 1f);
                }
                ";
            string pixelShaderCode =
                @"
                #version 330 core
                int vec4 vColor;
                out vec4 pixelColor;
                void main()
                {
                    pixelColor = vColor;
                }
                ";
            int VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShaderHandle, vertexShaderCode);
            GL.CompileShader(VertexShaderHandle);

            int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);

            this.shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(this.shaderProgramHandle, VertexShaderHandle);
            GL.AttachShader(this.shaderProgramHandle, pixelShaderHandle);
            GL.LinkProgram(shaderProgramHandle);
            GL.DetachShader(this.shaderProgramHandle, VertexShaderHandle);
            GL.DetachShader(this.shaderProgramHandle, pixelShaderHandle);
            GL.DeleteShader(VertexShaderHandle);
            GL.DeleteShader(pixelShaderHandle);
            base.OnLoad();
        }
        protected override void OnUnload()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(this.vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(this.shaderProgramHandle);
            GL.UseProgram(0);
            GL.DeleteProgram(this.shaderProgramHandle);
            base.OnUnload();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            frames++;
            Updatelog("Frames:"+frames);
            if (KeyboardState.IsKeyDown(key:OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            {
                Errorlog("Shutting down. . .");
                Thread.Sleep(1000);
                Environment.Exit(60);
                Close();
            }
            GL.Clear(ClearBufferMask.ColorBufferBit);
            base.OnUpdateFrame(args);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(this.shaderProgramHandle);
            GL.BindVertexArray(this.vertexArrayHandle);
            try{
                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);// Erase comment for Filled triangle
                //GL.DrawArrays(PrimitiveType.LineLoop, 0, 3);// Erase comment for Outlined triangle
            }catch(System.Exception e){
                Errorlog("Program failed to draw shape");
            }
            this.Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
        static void Main(string[] args)
        {
            Serverlog("Program has started");
            Thread.Sleep(2000);
            using(Game game = new Game())
            {
                game.Run();
                game.Title = "OpenTK simple triangle";
            }
        }
        public static void Serverlog(string text){
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("SERVER");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("]:"+text);
        }
        public static void Errorlog(string text){
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("]:"+text);
        }
        public static void Updatelog(string text){
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("UPDATE");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("]:"+text);
        }
    }
}
