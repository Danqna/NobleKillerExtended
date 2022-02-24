using HarmonyLib;
using MCM;
using MCM.Implementation;
using NobleKiller.Behaviour;
using NobleKiller.MCM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;


namespace NobleKiller
{
	class MySubModule : MBSubModuleBase
	{
		// MCM Menu settings
		public static readonly string ModuleFolderName = "NobleKiller";
		public static readonly string ModName = "NobleKiller";

		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();			
		}

		// MCM Settings
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
			NKSettings.Instance.Settings();			

		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			try
			{
				base.OnGameStart(game, gameStarterObject);
				if (!(game.GameType is Campaign))
				{
					return;
				}
				AddBehaviors(gameStarterObject as CampaignGameStarter);				
			}
			catch { }
		}
		
		private void AddBehaviors(CampaignGameStarter gameStarterObject)
		{
			if (gameStarterObject != null)
			{
				gameStarterObject.AddBehavior( new NobleKillerDialogue());
			}			
		}
	}
}