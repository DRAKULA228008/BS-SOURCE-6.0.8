using System;
using UnityEngine;

[Serializable]
public struct CryptoFloatFast : IFormattable, IEquatable<CryptoFloatFast>
{
	[SerializeField]
	private float defaultValue;

	[SerializeField]
	private float hiddenValue;

	[SerializeField]
	private int randomValue;

	[SerializeField]
	private bool inited;

	private CryptoFloatFast(float value)
	{
		defaultValue = value;
		randomValue = CryptoManager.staticValue;
		hiddenValue = value + (float)randomValue;
		inited = true;
	}

	public void SetValue(float value)
	{
		defaultValue = value;
		randomValue = CryptoManager.staticValue;
		hiddenValue = value + (float)randomValue;
	}

	public void CheckValue()
	{
		if (defaultValue - (hiddenValue - (float)randomValue) >= nValue.float001)
		{
			CheckManager.Detected("Controller Error 23");
		}
	}

	public float GetValue()
	{
		if (!inited)
		{
			defaultValue = 0f;
			randomValue = CryptoManager.staticValue;
			hiddenValue = defaultValue + (float)randomValue;
		}
		return defaultValue;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CryptoFloatFast))
		{
			return false;
		}
		return Equals((CryptoFloatFast)obj);
	}

	public bool Equals(CryptoFloatFast obj)
	{
		return GetValue() == obj.GetValue();
	}

	public override int GetHashCode()
	{
		return GetValue().GetHashCode();
	}

	public override string ToString()
	{
		return GetValue().ToString();
	}

	public string ToString(string format)
	{
		return GetValue().ToString(format);
	}

	public string ToString(IFormatProvider provider)
	{
		return GetValue().ToString(provider);
	}

	public string ToString(string format, IFormatProvider provider)
	{
		return GetValue().ToString(format, provider);
	}

	public static implicit operator CryptoFloatFast(float value)
	{
		return new CryptoFloatFast(value);
	}

	public static implicit operator float(CryptoFloatFast value)
	{
		return value.GetValue();
	}

	public static CryptoFloatFast operator ++(CryptoFloatFast value)
	{
		value.SetValue(value.GetValue() + 1f);
		return value;
	}

	public static CryptoFloatFast operator --(CryptoFloatFast value)
	{
		value.SetValue(value.GetValue() - 1f);
		return value;
	}
}
