using System;
using System.Collections.Generic;

[Serializable]
public struct YandexAppMetricaPreloadInfo
{
	public string TrackingId { get; private set; }

	public Dictionary<string, string> AdditionalInfo { get; private set; }

	public YandexAppMetricaPreloadInfo(string trackingId)
	{
		TrackingId = trackingId;
		AdditionalInfo = new Dictionary<string, string>();
	}
}
