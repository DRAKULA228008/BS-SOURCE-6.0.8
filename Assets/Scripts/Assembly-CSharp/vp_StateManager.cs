using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_StateManager
{
	private vp_Component m_Component;

	private List<vp_State> m_States;

	private Dictionary<string, int> m_StateIds;

	private static string m_DefaultStateNoDisableMessage = "Warning: The 'Default' state cannot be disabled.";

	private int m_DefaultId;

	private int m_TargetId;

	public vp_StateManager(vp_Component component, List<vp_State> states)
	{
		m_States = states;
		m_Component = component;
		m_Component.RefreshDefaultState();
		m_StateIds = new Dictionary<string, int>(StringComparer.CurrentCulture);
		foreach (vp_State state in m_States)
		{
			state.StateManager = this;
			if (!m_StateIds.ContainsKey(state.Name))
			{
				m_StateIds.Add(state.Name, m_States.IndexOf(state));
			}
			else
			{
				Debug.LogWarning(string.Concat("Warning: ", m_Component.GetType(), " on '", m_Component.name, "' has more than one state named: '", state.Name, "'. Only the topmost one will be used."));
				m_States[m_DefaultId].StatesToBlock.Add(m_States.IndexOf(state));
			}
			if (state.Preset == null)
			{
				state.Preset = new vp_ComponentPreset();
			}
			if (state.TextAsset != null)
			{
				state.Preset.LoadFromTextAsset(state.TextAsset);
			}
		}
		m_DefaultId = m_States.Count - 1;
	}

	public void ImposeBlockingList(vp_State blocker)
	{
		if (blocker == null || blocker.StatesToBlock == null || m_States == null)
		{
			return;
		}
		foreach (int item in blocker.StatesToBlock)
		{
			m_States[item].AddBlocker(blocker);
		}
	}

	public void RelaxBlockingList(vp_State blocker)
	{
		if (blocker == null || blocker.StatesToBlock == null || m_States == null)
		{
			return;
		}
		foreach (int item in blocker.StatesToBlock)
		{
			m_States[item].RemoveBlocker(blocker);
		}
	}

	public void SetState(string state, bool setEnabled = true)
	{
		if (AppPlaying() && m_StateIds.TryGetValue(state, out m_TargetId))
		{
			if (m_TargetId == m_DefaultId && !setEnabled)
			{
				Debug.LogWarning(m_DefaultStateNoDisableMessage);
				return;
			}
			m_States[m_TargetId].Enabled = setEnabled;
			CombineStates();
			m_Component.Refresh();
		}
	}

	public void Reset()
	{
		if (!AppPlaying())
		{
			return;
		}
		foreach (vp_State state in m_States)
		{
			state.Enabled = false;
		}
		m_States[m_DefaultId].Enabled = true;
		m_TargetId = m_DefaultId;
		CombineStates();
	}

	public void CombineStates()
	{
		for (int num = m_States.Count - 1; num > -1; num--)
		{
			if ((num == m_DefaultId || (m_States[num].Enabled && !m_States[num].Blocked && !(m_States[num].TextAsset == null))) && m_States[num].Preset != null && m_States[num].Preset.ComponentType != null)
			{
				vp_ComponentPreset.Apply(m_Component, m_States[num].Preset);
			}
		}
	}

	public bool IsEnabled(string state)
	{
		if (!AppPlaying())
		{
			return false;
		}
		if (m_StateIds.TryGetValue(state, out m_TargetId))
		{
			return m_States[m_TargetId].Enabled;
		}
		return false;
	}

	private static bool AppPlaying()
	{
		return true;
	}
}
