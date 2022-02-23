using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace NobleKiller.Modules
{
    class Assassinations
    {
		[SaveableProperty(1)]
		public int Cost { get; set; }

		[SaveableProperty(2)]
		public Hero target { get; set; }
	}
}
