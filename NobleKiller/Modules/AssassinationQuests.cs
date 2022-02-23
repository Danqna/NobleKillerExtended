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
    class AssassinationQuests
	{ 
		[SaveableProperty(1)]
		public string QuestName { get; set; }

		[SaveableProperty(2)]
		public Hero target { get; set; }

		[SaveableProperty(3)]
		public int reward { get; set; }

		[SaveableProperty(4)]
		public string description { get; set; }
	}
}
