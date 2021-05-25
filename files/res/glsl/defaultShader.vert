#version 330

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 textureCoords;

out vec2 pass_textureCoords;

uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;

void main ()
{
	gl_Position = ProjectionMatrix * ViewMatrix * ModelMatrix * vec4(position, 1);
	pass_textureCoords = textureCoords;
}