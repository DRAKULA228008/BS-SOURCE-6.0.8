using System;

[Flags]
public enum PropertyTypeFlag : byte
{
	None = 0x0,
	Game = 0x1,
	Actor = 0x2,
	GameAndActor = 0x3
}
