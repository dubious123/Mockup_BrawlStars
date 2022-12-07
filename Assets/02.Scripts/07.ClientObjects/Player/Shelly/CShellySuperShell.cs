using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CShellySuperShell : MonoBehaviour, ICBaseSkill
{
	public bool Performing { get; set; }
	public bool Active { get; set; }
	private NShellySuperShell _netSuperShell;

	public void Init(CPlayerShelly shelly)
	{
		_netSuperShell = (shelly.NPlayer as NCharacterShelly).BuckShot as NShellySuperShell;
	}

	public void HandleOneFrame()
	{

	}
}
