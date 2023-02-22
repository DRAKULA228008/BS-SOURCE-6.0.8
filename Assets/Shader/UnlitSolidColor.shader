Shader "ProBuilder/Unlit Solid Color" {
Properties {
 _Color ("Color Tint", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "IGNOREPROJECTOR"="true" "RenderType"="Geometry" }
 Pass {
  Tags { "IGNOREPROJECTOR"="true" "RenderType"="Geometry" }
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater 0.25
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
uniform highp mat4 glstate_matrix_mvp;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Color;
void main ()
{
  mediump vec4 tmpvar_1;
  tmpvar_1 = _Color;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
uniform highp mat4 glstate_matrix_mvp;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Color;
void main ()
{
  mediump vec4 tmpvar_1;
  tmpvar_1 = _Color;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
SubProgram "gles3 " {
"!!GLES3"
}
}
 }
}
}