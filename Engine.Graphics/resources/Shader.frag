#version 330 core

in vec3 normal;

out vec4 outColor;

void main()
{
	outColor = vec4(abs(normal.x), abs(normal.y), abs(normal.z), 1.0);
}