Shader "Hidden/ProBuilder/HideVertices" {
SubShader { 
 Tags { "IGNOREPROJECTOR"="true" "RenderType"="Geometry" }
 Pass {
  Tags { "IGNOREPROJECTOR"="true" "RenderType"="Geometry" }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

void main ()
{
  gl_Position = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif
#ifdef FRAGMENT

void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


void main ()
{
  gl_Position = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
void main ()
{
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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