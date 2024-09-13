#version 330 core

layout (location = 0) in int data;

const vec3 NORMALS[6] = vec3[6]
(
  vec3( 0.0,  0.0, -1.0), //z-
  vec3( 0.0,  0.0,  1.0), //z+
  vec3(-1.0,  0.0,  0.0), //x-
  vec3( 1.0,  0.0,  0.0), //x+
  vec3( 0.0, -1.0,  0.0), //y-
  vec3( 0.0,  1.0,  0.0)  //y+
);

uniform mat4 mvp;
uniform vec3 worldPos;

out vec3 normal;

void main()
{
	int x = data & 63; //0b111111
	int y = (data >> 6) & 63;
	int z = (data >> 12) & 63;
	
	vec3 pos = vec3(x, y, z);
	int normalIndex = (data >> 18) & 7;

	gl_Position = mvp * vec4(pos + worldPos, 1.0);
	normal = NORMALS[normalIndex];
}