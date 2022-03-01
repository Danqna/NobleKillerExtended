using NobleKiller.MCM;
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
	class NobleKillerDialogue : CampaignBehaviorBase
	{
		//public static readonly noblekillerdialogue Instance = new noblekillerdialogue();

		[SaveableField(1)] public bool QuestActive;
		[SaveableField(2)] public Hero NobleKillerTarget;
		[SaveableField(3)] public string targetname;
		[SaveableField(4)] public int _asscost;
		public bool barterhighsuccess;
		public bool barterhardsuccess;
		public Hero quest_giver;
		
		

		public static Hero RandomSoonToBeDeadGuy;
		public static bool PublicQuestActiveModifiable;
		public static Hero PublicQuestGiverModifiable;

		public override void RegisterEvents()
		{
			if (NobleKillerTarget == null)
            {
				// Fill in some defaults
				NobleKillerTarget = Hero.MainHero;
				targetname = Hero.MainHero.Name.ToString();
			}
			
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
		}

		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{			
			AddDialogs(campaignGameStarter);			
		}

		protected void AddDialogs(CampaignGameStarter starter)
		{
			//Ignore for now, think this requires knowledge of XML files
			TextObject textObject = new TextObject("{=!} (Assassinate) You do not have enough gold, you need more coins");

			// NOBLE KILLER EXTENDED
			starter.AddPlayerLine("assassin_top_level_noquest", "hero_main_options", "assassin_initiate_dialogue_noquest", "You and I need to talk...", PlayerNotOnAssassinQuest, null, 100, null);
			starter.AddPlayerLine("assassin_top_level", "hero_main_options", "assassin_initiate_dialogue", "You and I need to talk...", PlayerOnAssassinQuest, null, 100, null);
			starter.AddDialogLine("assassin_initiate_dialogue_noquest", "assassin_initiate_dialogue_noquest", "assassin_dialogue_choice", "What are we discussing exactly?", null, null, 100, null);
			starter.AddDialogLine("assassin_initiate_dialogue", "assassin_initiate_dialogue", "assassin_dialogue_quest_response", "How are you going with that...business?", IsQuestGiver, null, 100, null);
			starter.AddDialogLine("assassin_initiate_dialogue", "assassin_initiate_dialogue", "assassin_dialogue_choice", "I heard you were already occupied with a task.", IsNotQuestGiver, null, 100, null);

			// NEW RESPONSES WHILE ON QUEST
			starter.AddPlayerLine("assassin_dialogue_quest_response_quit", "assassin_dialogue_quest_response", "endquestprematurely", "I need to abandon this. (move on world map after this)", null, FailAssassinQuest, 100, null);
			starter.AddPlayerLine("assassin_dialogue_quest_response_OK", "assassin_dialogue_quest_response", "assassin_working_on_it", "I'm working on it, I just need a little more time.", null, null, 100, null);
			starter.AddDialogLine("endquestprematurely", "endquestprematurely", "hero_main_options", "A disappointing result for our deal.", null, Assassin_Abandon_Quest_Immediately, 100, null);
			starter.AddDialogLine("assassin_working_on_it", "assassin_working_on_it", "hero_main_options", "Excellent news, please keep me updated if things change.", null, null, 100, null);
			// TO-DO - ADD THE COMPLETION DIALOGUE OPTION HERE
			
			// ASSASSINATION DIALOGUE - NOBLE KILLER
			starter.AddPlayerLine("assassin_target_start", "assassin_dialogue_choice", "assassin_noble_response", "(Select for Assassination) You have made a poor choice in enemies.",	new ConversationSentence.OnConditionDelegate(this.Noble_Killer_Hero_Check),this.Noble_Killer_Select, 100, null);
			starter.AddPlayerLine("assassin_start_hasgold", "assassin_dialogue_choice", "assassin_response", "(Assassinate) Will you remove a piece from the board for 5000 coins?", new ConversationSentence.OnConditionDelegate(this.Noble_Killer_Assassin_Gold_Check), Noble_Killer_Consequence);
			starter.AddPlayerLine("assassin_start_hasnogold", "assassin_dialogue_choice", "assassin_exit_response",
				textObject.ToString(), 
				new ConversationSentence.OnConditionDelegate(this.Noble_Killer_Assassin_Check), null);
			starter.AddPlayerLine("assassin_exit", "assassin_dialogue_choice", "assassin_exit_response", "Never mind...", null, null, 100, null);
			starter.AddDialogLine("assassin_noble_response", "assassin_noble_response", "hero_main_options", "I am rubber, you are glue.", null, null, 100, null);			
			starter.AddDialogLine("assassin_response", "assassin_response", "hero_main_options", "To be honest, I've always hated them. Let me consult the alchemist...", null, null, 100, null);			
			starter.AddDialogLine("assassin_exit_response", "assassin_exit_response", "hero_main_options", "Let's leave it there, then.", null, null, 100, null);

			// QUEST DIALOGUE - NOBLE KILLER EXTENDED
			starter.AddPlayerLine("assassin_dialogue_choice_quest", "assassin_dialogue_choice", "assassin_noble_startquest", "With our list of enemies growing, perhaps we should eliminate a piece or two?", PlayerNotOnAssassinQuest, null, 100, null);
			starter.AddDialogLine("assassin_noble_startquest", "assassin_noble_startquest", "assassin_noble_askquestdetails", "Yes, I have been having trouble with a lord, you might be able to sort  this out for me.", null, null, 100, null);
  
			// We need to pre-load the successes and failures so we know where the dialogue is going but the player won't. Between you and me, though, the failures use contractions but don't tell the player.
			starter.AddPlayerLine("assassin_noble_askquestdetails_high_success", "assassin_noble_askquestdetails", "assassin_noble_questhigh", "Solutions do not come cheap, and I am good at solving problems...", Assassin_High_Success, null, 100, null);
			starter.AddPlayerLine("assassin_noble_askquestdetails_high_fail", "assassin_noble_askquestdetails", "quest_start_fail", "Solutions don't come cheap, and I am good at solving problems...", Assassin_High_Fail, null, 100, null);
			starter.AddPlayerLine("assassin_noble_askquestdetails_hard_success", "assassin_noble_askquestdetails", "assassin_noble_questhard", "You are a tough negotiator, but we could try to come to some agreement...", Assassin_Hard_Success, null, 100, null);
			starter.AddPlayerLine("assassin_noble_askquestdetails_hard_fail", "assassin_noble_askquestdetails", "quest_start_fail", "You're a tough negotiator, but we could try to come to some agreement...", Assassin_Hard_Fail, null, 100, null);
			starter.AddDialogLine("assassin_noble_askquestdetails_quit", "assassin_noble_startquest", "quest_start_fail", "I just remembered I have other commitments, I'm afraid I must reconsider for now...", null, null, 100, null);
			starter.AddDialogLine("quest_start_fail", "quest_start_fail", "hero_main_options", "Hmmm, I will continue thinking through my options.", null, null, 100, null);
			starter.AddDialogLine("assassin_noble_questhigh", "assassin_noble_questhigh", "hero_main_options", "Yes, I see your point, a high price for a high favour then, my friend.", null, Assassin_Quest_High_Consequence, 100, null);
			starter.AddDialogLine("assassin_noble_questhard", "assassin_noble_questhard", "hero_main_options", "We can shake on this deal.", null, Assassin_Quest_Hard_Consequence, 100, null);
			//starter.AddPlayerLine("assassin_info", "assassin_info", "assassin_specify_noble", "Glad we could reach agreement, summarise the details for me one more time.", null, null, 100, null);
			//starter.AddDialogLine("assassin_specify_noble", "assassin_specify_noble", "hero_main_options", "I need you to use any means possible to stop " + RandomSoonToBeDeadGuy + " from interfering further in my plans.", null, null, 100, null);

			// DIALOGUE WITH LORD TO KILL THEM

			starter.AddPlayerLine("assassin_kill_lord", "hero_main_options", "assassin_lord_dies", "(Assassinate) Peace was never an option!", HeroIsTarget, null, 100, null);
			starter.AddDialogLine("assassin_lord_dies", "assassin_lord_dies", "hero_main_options", "We can shake on this deal.", null, TargetDies, 100, null);

		}

		private bool HeroIsTarget()
        {
			if (Hero.OneToOneConversationHero == RandomSoonToBeDeadGuy)
            {
				return true;
            }
			return false;
        }

		private void TargetDies()
        {
			// And now for the murder		
			KillCharacterAction.ApplyByDeathMarkForced(RandomSoonToBeDeadGuy, false);			
			GiveGoldAction.ApplyForQuestBetweenCharacters(quest_giver, Hero.MainHero, _asscost, false);
			NobleKillerTarget = null;
			Random rnd = new Random();
			int bigyeet = rnd.Next(1, 5);
			if (bigyeet == 1)
			{
				InformationManager.DisplayMessage(new InformationMessage("You execute your plan swiftly and efficiently, and the hero breathes their last."));
			}
			else if (bigyeet == 2)
            {
				InformationManager.DisplayMessage(new InformationMessage("Cleverly disguised you yell, 'Somebody call an ambulance, but not for me!' and stab the hero."));
			}
			else if (bigyeet == 3)
			{
				InformationManager.DisplayMessage(new InformationMessage("He never saw it coming, and they never saw you going."));
			}
			else if (bigyeet == 4)
			{
				InformationManager.DisplayMessage(new InformationMessage("I just like to kill, I wanted to kill."));
			}
			else if (bigyeet == 5)
			{
				InformationManager.DisplayMessage(new InformationMessage("As you overhear the hushed fearful whispers of the staff you know another job was done well."));
			}
		}

		private void Assassin_Abandon_Quest_Immediately()
        {			
			AssassinQuest.FailQuest = true;
		}

		/// <summary>
		/// Set the target variable if this option is selected. Does NOT mean the noble will die, just that they will be the target if we speak to an assassin.
		/// </summary>
		private void Noble_Killer_Select()
		{
			// Old method
			NobleKillerTarget = Hero.OneToOneConversationHero;												
			targetname = NobleKillerTarget.Name.ToString();
			int tempcost = Calculate_Assassination_Cost();
			_asscost = tempcost;
			NKSettings.Instance.CurrentCostValue = _asscost;
			NKSettings.Instance.CurrentTarget = targetname;
		}

		/// <summary>
		/// This is where the good stuff happens. Let's GET OUR MURDER ON AHAHAHAHAHAHAHAHAHA
		/// </summary>
		private void Noble_Killer_Consequence()
        {
			if (Hero.OneToOneConversationHero != null)
			{				
				KillCharacterAction.ApplyByDeathMarkForced(NobleKillerTarget, false);
				InformationManager.DisplayMessage(new InformationMessage("And so " + NobleKillerTarget.Name.ToString() + " passed into the darkness..."));
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, Hero.OneToOneConversationHero, _asscost, false);
				NobleKillerTarget = null;
			}
		}

		/// <summary>
		/// This is where we just make sure the conversation is with a Hero
		/// </summary>
		/// <returns></returns>
		private bool Noble_Killer_Hero_Check()
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
		private bool Noble_Killer_Assassin_Gold_Check()
		{			
			InformationManager.DisplayMessage(new InformationMessage("Current cost is: " + NKSettings.Instance.CurrentCostValue));			

			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero != NobleKillerTarget && NobleKillerTarget != null 
				&& Hero.MainHero.Gold >= NKSettings.Instance.CurrentCostValue && !NobleKillerTarget.IsDead && NobleKillerTarget != Hero.MainHero)
			{
				return true;
			}
			return false;
		}

		private bool Noble_Killer_Assassin_Check()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero != NobleKillerTarget && NobleKillerTarget != null 
				&& Hero.MainHero.Gold < _asscost && !NobleKillerTarget.IsDead)
			{
				return true;
			}
			return false;
		}

		private bool PlayerNotOnAssassinQuest()
		{
			
			QuestActive = PublicQuestActiveModifiable;
			quest_giver = PublicQuestGiverModifiable;

			// Basically we need to have the hero ready before we create the dialogues, so we need our random hero initiated here.
			if (RandomSoonToBeDeadGuy == null || RandomSoonToBeDeadGuy.IsDead)
			{
				INeedAHero();
			}

			// we reverse our bool coz we don't want this to fire											
			if (AssassinQuest.PublicQuestRunningModifiable || AssassinQuest.FailQuest || AssassinQuest.HideDialogue)
			{
				return false;
			}
			else if (Hero.OneToOneConversationHero.Occupation == Occupation.Lord)
			{
				return true;
			}
            else 
			{
				return false;
			}
		}

		private int Calculate_Assassination_Cost()
        {
			int cost;

			// Check if the player overrides costs
			if (NKSettings.Instance.OverrideCost)
            {
				Random rnd = new Random();
				cost = rnd.Next(NKSettings.Instance.StartCostValue, NKSettings.Instance.EndCostValue);
				NKSettings.Instance.CurrentCostValue = cost;
				return cost;
            }
			else if(Hero.MainHero.Level == 1)
            {
				cost = 5000;
				NKSettings.Instance.CurrentCostValue = cost;
				return cost;
            }
			else
            {
				// Calcuation should take into account the player level, and scale appropriately. 5000 at level 1 is fine. 100000 at level 10 might be good.  500,000 at level 20 is good.
				// Formula = Round the result of: 5000 * PlayerLevel * (PlayerLevel / 4)
				float level = Hero.MainHero.Level;
				float calc = 5000 * level * (level / 4);
				cost = Convert.ToInt32(calc);
				NKSettings.Instance.CurrentCostValue = cost;
				return cost;
			}			
        }

		private bool PlayerOnAssassinQuest()
		{
			// Debug the quest if it breaks
			if (NKSettings.Instance.QuestBug)
            {
				AssassinQuest.FailQuest = true;
				NKSettings.Instance.QuestBug = false;
				//NKSettings.Instance.NobleKillerSaveSettings();
			}

			if (QuestActive && !AssassinQuest.FailQuest)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool Assassin_High_Success()
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

		private bool Assassin_High_Fail()
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

		private bool Assassin_Hard_Success()
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

		private bool Assassin_Hard_Fail()
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

		private void Assassin_Quest_High_Consequence()
        {
			AssassinQuestStart(true);
        }

		private void Assassin_Quest_Hard_Consequence()
        {
			AssassinQuestStart(false);
		}

		private void AssassinQuestStart(bool IsHighPrice)
        {
			bool herofound = false;
			if (RandomSoonToBeDeadGuy == null)
			{
				// Try again to fill the Hero with a valid hero
				INeedAHero();
				if (RandomSoonToBeDeadGuy == null)
				{
					InformationManager.DisplayMessage(new InformationMessage("Error: Could not find a valid hero, try again later."));
					return;
				}
			}
			else
			{
				herofound = true;
			}

			if (herofound)
			{
				// Create a new quest journal entry						
				quest_giver = Hero.OneToOneConversationHero;

				// Create the price
				Random rnd = new Random();
				int reward = rnd.Next(NKSettings.Instance.StartRewardValue, NKSettings.Instance.EndRewardValue);
				if (IsHighPrice)
				{
					float calc = reward * (1 + NKSettings.Instance.BarterPercent);
					reward = Convert.ToInt32(calc);
				}

				new AssassinQuest(quest_giver, reward, RandomSoonToBeDeadGuy, false).StartQuest();
				QuestActive = true;
				PublicQuestActiveModifiable = true;
			}
		}

		private void INeedAHero()
        {
			RandomSoonToBeDeadGuy = null;
			List<Hero> heroes = Hero.AllAliveHeroes.ToList();
			int count = 0;
			while (RandomSoonToBeDeadGuy == null && count < 1000)
			{
				Random rnd = new Random();
				int ourluckyhero = rnd.Next(0, (heroes.Count() - 1));
				if (heroes[ourluckyhero].Occupation == Occupation.Lord)
				{
					RandomSoonToBeDeadGuy = heroes[ourluckyhero];
				}
				count++;
			}

			if(count > 999)
            {
				InformationManager.DisplayMessage(new InformationMessage("NobleKiller error finding valid lord. This mod is up the creek without a paddle. Flying blind mode activated."));
            }				
        }

		private bool IsQuestGiver()
        {
			if(Hero.OneToOneConversationHero == quest_giver)
            {
				return true;
            }
			return false;
        }

		private bool IsNotQuestGiver()
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
		private void FailAssassinQuest()
		{
			// Set quest to not running
			QuestActive = false;
			AssassinQuest.FailQuest = true;
			AssassinQuest.HideDialogue = true;
		}

		public override void SyncData(IDataStore dataStore)
		{
		}		
	}
}
