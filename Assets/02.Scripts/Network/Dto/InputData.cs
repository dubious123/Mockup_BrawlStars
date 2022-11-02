using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public readonly struct InputData
	{
		public readonly sVector3 MoveInput { get; init; }
		public readonly sVector3 LookInput { get; init; }
		public readonly ushort ButtonInput { get; init; }
	}
}
