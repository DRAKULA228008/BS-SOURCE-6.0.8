using System;

public class ConsoleAttribute : Attribute
{
	public string[] commands;

	public ConsoleAttribute(params string[] value)
	{
		commands = value;
	}
}
