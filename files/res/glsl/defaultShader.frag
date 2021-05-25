#version 330

in vec2 pass_textureCoords;

out vec4 color;

uniform float Alpha;
uniform sampler2D TextureSampler;

void main()
{
	color = texture(TextureSampler, pass_textureCoords);
}