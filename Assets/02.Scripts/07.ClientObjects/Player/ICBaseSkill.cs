using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Game;

public interface ICBaseSkill
{
	public bool Performing { get; set; }
	public bool Active { get; set; }
	public void HandleOneFrame();
}
