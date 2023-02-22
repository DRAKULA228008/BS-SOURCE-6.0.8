== Description ==

GuardingPearSoftwares Obfuscator was developed to increase your software 
and game security, especially for applications built with Unity.

Its main objective is to conceal your own proprietary source code, and 
third party compiled dotNet assemblies as well. We support all known platforms, 
whether standalone or embedded.

== How To ==

The Obfuscator runs, when activated (is the default), automatically at
build time. Directly after Unity generates the build target specific 
assemblies (*.dll), generated at Library\ScriptAssemblies, the Obfuscator
applies to them.

== Settings ==

The Obfuscator comes with a "Settings" window allowing to specify exactly and
user friendly which assemblies should be obfuscated and what features should apply
to them. You can find it in the Unity Editor Menu OPS->Obfuscator->Settings.

== Error Stack Trace ==

To still be able to debug or understand obfuscated error logs, the Obfuscator
comes with a "Error Stack Trace" window. Here you can load a mapping file (you
have to activate it in the settings) and enter a obfuscated stack trace.
Pressing "Deobfuscate" the obfuscator will try to deobfuscate the stack trace.
You can find it in the Unity Editor Menu OPS->Obfuscator->Error Stack Trace.
