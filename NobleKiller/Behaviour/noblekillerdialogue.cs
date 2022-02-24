using NobleKiller.MCM;
using NobleKiller.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace NobleKiller.Behaviour
{
	class noblekillerdialogue : CampaignBehaviorBase
	{
		public static readonly noblekillerdialogue Instance = new noblekillerdialogue();

		// Saveable Fields
		[SaveableProperty(1)]
		public Hero RandomSoonToBeDeadGuy { get; set; }

		[SaveableField(2)]
		public bool playeronassassinationalready;
		
		[SaveableProperty(3)]
		public static Hero NobleKillerTarget { get; set; }

		[SaveableProperty(4)]
		public static Hero NobleKillerAssassin { get; set; }

		[SaveableField(5)]
		public string targetname = string.Empty;

		[SaveableField(6)]
		bool barterhighsuccess;

		[SaveableField(7)]
		bool barterhardsuccess;	

		[SaveableProperty(8)]
		public Hero quest_giver { get; set; }
		

		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
		}

		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{			
			AddDialogs(campaignGameStarter);			
		}

		protected void AddDialogs(CampaignGameStarter starter)
		{
			// Basically we need to have the hero ready before we create the dialogues, so we need our random hero initiated here.
			if (RandomSoonToBeDeadGuy == null || RandomSoonToBeDeadGuy.IsDead)
			{
				INeedAHero();
			}

			//Ignore for now, think this requires knowledge of XML files
			TextObject textObject = new TextObject("{=!} (Assassinate) You do not have enough gold, you need more coins");

			// NOBLE KILLER EXTENDED
			starter.AddPlayerLine("assassin_top_level_noquest", "hero_main_options", "assassin_initiate_dialogue_noquest", "You and I need to talk...", playernotonassassinquest, null, 100, null);
			starter.AddPlayerLine("assassin_top_level", "hero_main_options", "assassin_initiate_dialogue", "You and I need to talk...", playeronassassinquest, null, 100, null);
			starter.AddDialogLine("assassin_initiate_dialogue_noquest", "assassin_initiate_dialogue_noquest", "assassin_dialogue_choice", "What are we discussing exactly?", null, null, 100, null);
			starter.AddDialogLine("assassin_initiate_dialogue", "assassin_initiate_dialogue", "assassin_dialogue_quest_response", "How are you going with that...business?", isquestgiver, null, 100, null);
			starter.AddDialogLine("assassin_initiate_dialogue", "assassin_initiate_dialogue", "assassin_dialogue_choice", "I heard you were already occupied with a task.", isnotquestgiver, null, 100, null);

			// NEW RESPONSES WHILE ON QUEST
			starter.AddPlayerLine("assassin_dialogue_quest_response_quit", "assassin_dialogue_quest_response", "endquestprematurely", "I need to abandon this. (move on world map after this)", null, failassassinquest, 100, null);
			starter.AddPlayerLine("assassin_dialogue_quest_response_OK", "assassin_dialogue_quest_response", "assassin_working_on_it", "I'm working on it, I just need a little more time.", null, null, 100, null);
			starter.AddDialogLine("endquestprematurely", "endquestprematurely", "hero_main_options", "A disappointing result for our deal.", null, assassin_abandon_quest_immediately, 100, null);
			starter.AddDialogLine("assassin_working_on_it", "assassin_working_on_it", "hero_main_options", "Excellent news, please keep me updated if things change.", null, null, 100, null);
			// TO-DO - ADD THE COMPLETION DIALOGUE OPTION HERE
			
			// ASSASSINATION DIALOGUE - NOBLE KILLER
			starter.AddPlayerLine("assassin_target_start", "assassin_dialogue_choice", "assassin_noble_response", "(Select for Assassination) You have made a poor choice in enemies.",	new ConversationSentence.OnConditionDelegate(this.noble_killer_hero_check),this.noble_killer_select, 100, null);
			starter.AddPlayerLine("assassin_start_hasgold", "assassin_dialogue_choice", "assassin_response", "(Assassinate) Will you remove a piece from the board for 5000 coins?", new ConversationSentence.OnConditionDelegate(this.noble_killer_assassin_gold_check), noble_killer_consequence);
			starter.AddPlayerLine("assassin_start_hasnogold", "assassin_dialogue_choice", "assassin_exit_response",
				textObject.ToString(), 
				new ConversationSentence.OnConditionDelegate(this.noble_killer_assassin_check), null);
			starter.AddPlayerLine("assassin_exit", "assassin_dialogue_choice", "assassin_exit_response", "Never mind...", null, null, 100, null);
			starter.AddDialogLine("assassin_noble_response", "assassin_noble_response", "hero_main_options", "I am rubber, you are glue.", null, null, 100, null);			
			starter.AddDialogLine("assassin_response", "assassin_response", "hero_main_options", "To be honest, I've always hated them. Let me consult the alchemist...", null, null, 100, null);			
			starter.AddDialogLine("assassin_exit_response", "assassin_exit_response", "hero_main_options", "Let's leave it there, then.", null, null, 100, null);

			// QUEST DIALOGUE - NOBLE KILLER EXTENDED
			starter.AddPlayerLine("assassin_dialogue_choice_quest", "assassin_dialogue_choice", "assassin_noble_startquest", "With our list of enemies growing, perhaps we should eliminate a piece or two?", playernotonassassinquest, null, 100, null);
			starter.AddDialogLine("assassin_noble_startquest", "assassin_noble_startquest", "assassin_noble_askquestdetails", "Yes, I have been having trouble with a lord, you might be able to sort  this out for me.", null, null, 100, null);
  
			// We need to pre-load the successes and failures so we know where the dialogue is going but the player won't. Between you and me, though, the failures use contractions but don't tell the player.
			starter.AddPlayerLine("assassin_noble_askquestdetails_high_success", "assassin_noble_askquestdetails", "assassin_noble_questhigh", "Solutions do not come cheap, and I am good at solving problems...", assassin_high_success, null, 100, null);
			starter.AddPlayerLine("assassin_noble_askquestdetails_high_fail", "assassin_noble_askquestdetails", "quest_start_fail", "Solutions don't come cheap, and I am good at solving problems...", assassin_high_fail, null, 100, null);
			starter.AddPlayerLine("assassin_noble_askquestdetails_hard_success", "assassin_noble_askquestdetails", "assassin_noble_questhard", "You are a tough negotiator, but we could try to come to some agreement...", assassin_hard_success, null, 100, null);
			starter.AddPlayerLine("assassin_noble_askquestdetails_hard_fail", "assassin_noble_askquestdetails", "quest_start_fail", "You're a tough negotiator, but we could try to come to some agreement...", assassin_hard_fail, null, 100, null);
			starter.AddDialogLine("assassin_noble_askquestdetails_quit", "assassin_noble_startquest", "quest_start_fail", "I just remembered I have other commitments, I'm afraid I must reconsider for now...", null, null, 100, null);
			starter.AddDialogLine("quest_start_fail", "quest_start_fail", "hero_main_options", "Hmmm, I will continue thinking through my options.", null, null, 100, null);
			starter.AddDialogLine("assassin_noble_questhigh", "assassin_noble_questhigh", "hero_main_options", "Yes, I see your point, a high price for a high favour then, my friend.", null, assassin_quest_high_consequence, 100, null);
			starter.AddDialogLine("assassin_noble_questhard", "assassin_noble_questhard", "hero_main_options", "We can shake on this deal.", null, assassin_quest_hard_consequence, 100, null);
			//starter.AddPlayerLine("assassin_info", "assassin_info", "assassin_specify_noble", "Glad we could reach agreement, summarise the details for me one more time.", null, null, 100, null);
			//starter.AddDialogLine("assassin_specify_noble", "assassin_specify_noble", "hero_main_options", "I need you to use any means possible to stop " + RandomSoonToBeDeadGuy + " from interfering further in my plans.", null, null, 100, null);

			// DIALOGUE WITH LORD TO KILL THEM

			starter.AddPlayerLine("assassin_kill_lord", "hero_main_options", "assassin_lord_dies", "(Assassinate) Peace was never an option!", heroistarget, null, 100, null);
			starter.AddDialogLine("assassin_lord_dies", "assassin_lord_dies", "hero_main_options", "We can shake on this deal.", null, targetdies, 100, null);

		}

		private bool heroistarget()
        {
			if (Hero.OneToOneConversationHero == RandomSoonToBeDeadGuy)
            {
				return true;
            }
			return false;
        }

		private void targetdies()
        {
			// And now for the murder
			List<string> ourHero = new List<string>();
			ourHero.Add(NobleKillerTarget.Name.ToString());
			
			KillCharacterAction.ApplyByDeathMarkForced(RandomSoonToBeDeadGuy, false);			
			GiveGoldAction.ApplyForQuestBetweenCharacters(quest_giver, Hero.MainHero, NKSettings.Instance._currentcostvalue, false);
			NobleKillerTarget = null;

		}

		private void assassin_abandon_quest_immediately()
        {
			playeronassassinationalready = false;

		}

		/// <summary>
		/// Set the target variable if this option is selected. Does NOT mean the noble will die, just that they will be the target if we speak to an assassin.
		/// </summary>
		private void noble_killer_select()
		{
			// Old method
			NobleKillerTarget = Hero.OneToOneConversationHero;												
			targetname = NobleKillerTarget.Name.ToString();
			NKSettings.Instance._currentcostvalue = calculate_assassination_cost();
			NKSettings.Instance._currenttarget = targetname;
		}

		/// <summary>
		/// This is where the good stuff happens. Let's GET OUR MURDER ON AHAHAHAHAHAHAHAHAHA
		/// </summary>
		private void noble_killer_consequence()
        {
			if (Hero.OneToOneConversationHero != null)
			{
				// And now for the murder
				List<string> ourHero = new List<string>();
				ourHero.Add(NobleKillerTarget.Name.ToString());
				// CampaignCheats.KillHero(ourHero); Only works with cheats enabled
				KillCharacterAction.ApplyByDeathMarkForced(NobleKillerTarget, false);
				InformationManager.DisplayMessage(new InformationMessage("And so " + NobleKillerTarget.Name.ToString() + " passed into the darkness..."));
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, Hero.OneToOneConversationHero, NKSettings.Instance._currentcostvalue, false);
				NobleKillerTarget = null;
			}
		}

		/// <summary>
		/// This is where we just make sure the conversation is with a Hero
		/// </summary>
		/// <returns></returns>
		private bool noble_killer_hero_check()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero != NobleKillerTarget)
			{				
				return true;
			}
			return false;
		}

		/// <summary>
		/// Here we need to check that the player is speaking with another noble, but not the same noble. We're not asking the noble to kill themselves (as much as we might want to).
		/// </summary>
		/// <returns></returns>
		private bool noble_killer_assassin_gold_check()
		{
			if (NKSettings.Instance._debugmessages)               
			{
				InformationManager.DisplayMessage(new InformationMessage("Current cost is: " + NKSettings.Instance._currentcostvalue));
			}

			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero != NobleKillerTarget && NobleKillerTarget != null 
				&& Hero.MainHero.Gold >= NKSettings.Instance._currentcostvalue && !NobleKillerTarget.IsDead)
			{
				return true;
			}
			return false;
		}

		private bool noble_killer_assassin_check()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero != NobleKillerTarget && NobleKillerTarget != null 
				&& Hero.MainHero.Gold < NKSettings.Instance._currentcostvalue && !NobleKillerTarget.IsDead)
			{
				return true;
			}
			return false;
		}

		private bool playernotonassassinquest()
		{
			// Basically we need to have the hero ready before we create the dialogues, so we need our random hero initiated here.
			if (RandomSoonToBeDeadGuy == null || RandomSoonToBeDeadGuy.IsDead)
			{
				INeedAHero();
			}

			// we reverse our bool coz we don't want this to fire
			if (playeronassassinationalready || AssassinQuest.isActive)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private int calculate_assassination_cost()
        {
			int cost = 5000;

			// Check if the player overrides costs
			if (NKSettings.Instance._overridecost)
            {
				Random rnd = new Random();
				cost = rnd.Next(NKSettings.Instance._startcostvalue, NKSettings.Instance._endcostvalue);
				return cost;
            }
			else if(Hero.MainHero.Level == 1)
            {
				cost = 5000;
				return cost;
            }
			else
            {
				// Calcuation should take into account the player level, and scale appropriately. 5000 at level 1 is fine. 100000 at level 10 might be good.  500,000 at level 20 is good.
				// Formula = Round the result of: 5000 * PlayerLevel * (PlayerLevel / 4)
				float level = Hero.MainHero.Level;
				float calc = 5000 * level * (level / 4);
				cost = Convert.ToInt32(calc);
				return cost;
			}			
        }

		private bool playeronassassinquest()
		{
			// Debug the quest if it breaks
			if (NKSettings.Instance._questbug)
            {
				playeronassassinationalready = false;
				AssassinQuest.isActive = false;
				NKSettings.Instance._questbug = false;
			}

			if (playeronassassinationalready)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool assassin_high_success()
        {
			// we're going to assume this code always runs and set our barterhighsuccess here
			Random rnd = new Random();
			int result = rnd.Next(1, 100);
			if (result > 50)
			{
				barterhighsuccess = true;
			}
			else
            {
				barterhighsuccess = false;

			}
			return barterhighsuccess;
		}

		private bool assassin_high_fail()
        {
			if (barterhighsuccess)
            {
				return false;
            }
			else
            {
				return true;
			}
        }

		private bool assassin_hard_success()
		{
			// we're going to assume this code always runs and set our barterhighsuccess here
			Random rnd = new Random();
			int result = rnd.Next(1, 100);
			if (result > 50)
			{
				barterhardsuccess = true;
			}
			else
			{
				barterhardsuccess = false;

			}
			return barterhardsuccess;
		}

		private bool assassin_hard_fail()
		{
			if (barterhardsuccess)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private void assassin_quest_high_consequence()
        {
			assassinqueststart(true);
        }

		private void assassin_quest_hard_consequence()
        {
			assassinqueststart(false);
		}

		private void assassinqueststart(bool IsHighPrice)
        {
			playeronassassinationalready = true;

			// Create a new quest journal entry						
			quest_giver = Hero.OneToOneConversationHero;

			// Create the price
			Random rnd = new Random();
			int reward = rnd.Next(NKSettings.Instance._startrewardvalue, NKSettings.Instance._endrewardvalue);
			if (IsHighPrice)
            {
				float calc = reward;
				calc = calc * (1 + NKSettings.Instance._barterpercent);
				reward = Convert.ToInt32(calc);
            }

			new AssassinQuest(quest_giver, reward, RandomSoonToBeDeadGuy).StartQuest();
		}

		private void INeedAHero()
        {
			List<Hero> heroes = Hero.AllAliveHeroes.ToList();
			int count = 0;
			while (RandomSoonToBeDeadGuy == null && count < 100)
			{
				Random rnd = new Random();
				int ourluckyhero = rnd.Next(0, (heroes.Count() - 1));
				if (heroes[ourluckyhero].CanLeadParty())
				{
					RandomSoonToBeDeadGuy = heroes[ourluckyhero];
				}
				count = count + 1;
			}

			if(count > 98)
            {
				InformationManager.DisplayMessage(new InformationMessage("NobleKiller error finding valid lord. This mod is up the creek without a paddle. Flying blind mode activated."));
            }				
        }

		private bool isquestgiver()
        {
			if(Hero.OneToOneConversationHero == quest_giver)
            {
				return true;
            }
			return false;
        }

		private bool isnotquestgiver()
        {
			if (Hero.OneToOneConversationHero != quest_giver)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// If the player decides to end the quest or takes too long or lord dies another way
		/// </summary>
		private void failassassinquest()
		{
			// Set quest to not running
			playeronassassinationalready = false;
		}

		public override void SyncData(IDataStore dataStore)
		{
		}		
	}
}
