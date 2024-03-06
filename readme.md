# Physic-Engine

This project is a 2D physics engine that renders rectangles using OpenTK. It's designed to provide a simple yet effective demonstration of 2D physics simulations within a graphical window.

## System Requirements

- .NET Core 3.1.0 or higher

## Getting Started

Before running the project, ensure you have .NET Core 3.1.0 or higher installed on your machine. You can check your current .NET version by running the following command in your terminal:

```bash
dotnet --version
```

If you need to install or update .NET Core, visit the official .NET download page for guidance.

Running the Project
To execute the physics engine, navigate to the project's root directory in your terminal and run:

```bash
dotnet run
```

## Project Structure

The project consists of several key files:

**FlatPhysics.cs**: Contains the core physics calculations and logic.

**Game.cs**: Manages the game loop and rendering.

**IndexBuffer.cs**, VertexBuffer.cs, VertexArray.cs: Handle graphics rendering details.

**ShaderProgram.cs**: Manages the shaders for rendering.

**VertexDefinitions.cs**: Defines the vertex data structures used in rendering.

**Program.cs**: The entry point of the application.

**Physic-Engine.csproj**: The project file for .NET Core.

## License

This project is open-source and available under the MIT License. By contributing to the Physic-Engine, you agree that your contributions will be licensed under its MIT License.