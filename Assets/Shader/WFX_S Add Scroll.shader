Shader "WFX/Scroll/Additive" {
Properties {
 _MainTex ("Looped Texture + Alpha Mask", 2D) = "white" {}
 _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
 _ScrollSpeed ("Scroll Speed", Float) = 2
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
  ColorMask RGB
Program "vp" {
SubProgram "gles " {
Keywords { "SOFTPARTICLES_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _MainTex_ST;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = _glesColor;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform sampler2D _MainTex;
uniform highp float _ScrollSpeed;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0;
  mediump vec4 prev_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = vec4((texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w));
  prev_3 = tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_Time * _ScrollSpeed);
  highp vec4 tmpvar_6;
  tmpvar_6 = fract(abs(tmpvar_5));
  highp float tmpvar_7;
  if ((tmpvar_5.x >= 0.0)) {
    tmpvar_7 = tmpvar_6.x;
  } else {
    tmpvar_7 = -(tmpvar_6.x);
  };
  tmpvar_2.y = (xlv_TEXCOORD0.y - tmpvar_7);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, tmpvar_2);
  prev_3.xyz = (prev_3.xyz * (tmpvar_8.xyz * xlv_COLOR.xyz));
  tmpvar_1 = prev_3;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SOFTPARTICLES_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _MainTex_ST;
out lowp vec4 xlv_COLOR;
out highp vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = _glesColor;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform sampler2D _MainTex;
uniform highp float _ScrollSpeed;
in lowp vec4 xlv_COLOR;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD0;
  mediump vec4 prev_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = vec4((texture (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w));
  prev_3 = tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_Time * _ScrollSpeed);
  highp vec4 tmpvar_6;
  tmpvar_6 = fract(abs(tmpvar_5));
  highp float tmpvar_7;
  if ((tmpvar_5.x >= 0.0)) {
    tmpvar_7 = tmpvar_6.x;
  } else {
    tmpvar_7 = -(tmpvar_6.x);
  };
  tmpvar_2.y = (xlv_TEXCOORD0.y - tmpvar_7);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture (_MainTex, tmpvar_2);
  prev_3.xyz = (prev_3.xyz * (tmpvar_8.xyz * xlv_COLOR.xyz));
  tmpvar_1 = prev_3;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SOFTPARTICLES_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp vec4 _MainTex_ST;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (tmpvar_2 * 0.5);
  highp vec2 tmpvar_5;
  tmpvar_5.x = tmpvar_4.x;
  tmpvar_5.y = (tmpvar_4.y * _ProjectionParams.x);
  o_3.xy = (tmpvar_5 + tmpvar_4.w);
  o_3.zw = tmpvar_2.zw;
  tmpvar_1.xyw = o_3.xyw;
  tmpvar_1.z = -((glstate_matrix_modelview0 * _glesVertex).z);
  gl_Position = tmpvar_2;
  xlv_COLOR = _glesColor;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp float _InvFade;
uniform highp float _ScrollSpeed;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_2.xyz = xlv_COLOR.xyz;
  tmpvar_3 = xlv_TEXCOORD0;
  mediump vec4 prev_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture2DProj (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_6;
  z_6 = tmpvar_5.x;
  highp float tmpvar_7;
  tmpvar_7 = (xlv_COLOR.w * clamp ((_InvFade * 
    ((1.0/(((_ZBufferParams.z * z_6) + _ZBufferParams.w))) - xlv_TEXCOORD1.z)
  ), 0.0, 1.0));
  tmpvar_2.w = tmpvar_7;
  lowp vec4 tmpvar_8;
  tmpvar_8 = vec4((texture2D (_MainTex, xlv_TEXCOORD0).w * tmpvar_2.w));
  prev_4 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (_Time * _ScrollSpeed);
  highp vec4 tmpvar_10;
  tmpvar_10 = fract(abs(tmpvar_9));
  highp float tmpvar_11;
  if ((tmpvar_9.x >= 0.0)) {
    tmpvar_11 = tmpvar_10.x;
  } else {
    tmpvar_11 = -(tmpvar_10.x);
  };
  tmpvar_3.y = (xlv_TEXCOORD0.y - tmpvar_11);
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_MainTex, tmpvar_3);
  prev_4.xyz = (prev_4.xyz * (tmpvar_12.xyz * xlv_COLOR.xyz));
  tmpvar_1 = prev_4;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SOFTPARTICLES_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp vec4 _MainTex_ST;
out lowp vec4 xlv_COLOR;
out highp vec2 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (tmpvar_2 * 0.5);
  highp vec2 tmpvar_5;
  tmpvar_5.x = tmpvar_4.x;
  tmpvar_5.y = (tmpvar_4.y * _ProjectionParams.x);
  o_3.xy = (tmpvar_5 + tmpvar_4.w);
  o_3.zw = tmpvar_2.zw;
  tmpvar_1.xyw = o_3.xyw;
  tmpvar_1.z = -((glstate_matrix_modelview0 * _glesVertex).z);
  gl_Position = tmpvar_2;
  xlv_COLOR = _glesColor;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp float _InvFade;
uniform highp float _ScrollSpeed;
in lowp vec4 xlv_COLOR;
in highp vec2 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  highp vec2 tmpvar_3;
  tmpvar_2.xyz = xlv_COLOR.xyz;
  tmpvar_3 = xlv_TEXCOORD0;
  mediump vec4 prev_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureProj (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_6;
  z_6 = tmpvar_5.x;
  highp float tmpvar_7;
  tmpvar_7 = (xlv_COLOR.w * clamp ((_InvFade * 
    ((1.0/(((_ZBufferParams.z * z_6) + _ZBufferParams.w))) - xlv_TEXCOORD1.z)
  ), 0.0, 1.0));
  tmpvar_2.w = tmpvar_7;
  lowp vec4 tmpvar_8;
  tmpvar_8 = vec4((texture (_MainTex, xlv_TEXCOORD0).w * tmpvar_2.w));
  prev_4 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (_Time * _ScrollSpeed);
  highp vec4 tmpvar_10;
  tmpvar_10 = fract(abs(tmpvar_9));
  highp float tmpvar_11;
  if ((tmpvar_9.x >= 0.0)) {
    tmpvar_11 = tmpvar_10.x;
  } else {
    tmpvar_11 = -(tmpvar_10.x);
  };
  tmpvar_3.y = (xlv_TEXCOORD0.y - tmpvar_11);
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture (_MainTex, tmpvar_3);
  prev_4.xyz = (prev_4.xyz * (tmpvar_12.xyz * xlv_COLOR.xyz));
  tmpvar_1 = prev_4;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "SOFTPARTICLES_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SOFTPARTICLES_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SOFTPARTICLES_ON" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SOFTPARTICLES_ON" }
"!!GLES3"
}
}
 }
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
  ColorMask RGB
  SetTexture [_MainTex] { combine texture * primary }
  SetTexture [_MainTex] { combine previous * previous alpha, previous alpha }
 }
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
  ColorMask RGB
  SetTexture [_MainTex] { combine texture * texture alpha, texture alpha }
 }
}
}