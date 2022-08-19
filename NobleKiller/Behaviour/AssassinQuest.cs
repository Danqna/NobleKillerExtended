using Helpers;
using NobleKiller.MCM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace NobleKiller.Behaviour
{
    internal class AssassinQuest : QuestBase
    {
        // Let's create some variables for a simple quest
        // Quest will work as follows
        // Talk to a lord, ask if they have a job for you (Dialogue/Response 1)
        // We'll start with two jobs, AND CHECK IF QUEST IS ALREADY ACTIVE:
        // 1. Please kill an enemy lord (Dialogue Group 2)
        // 2. Please kill a friendly lord (Dialogue Group 2)
        // Later on we can add dialogue to flesh out why they might do it, and then even LATER add options to betray them to the noble for cash or revenge. No promises. It's late at night and I should be asleep.
        // We'll negotiate on pay with 3 options
        // This is the tricky part. Our condition needs to be set on player so they get a fail or success before they even ask so we can direct to the proper noble response.
        // 1. Barter high SUCCESS (Random Price + 50%) (condition % set by player)
        // 1.1 Barter high FAIL
        // 2. Barter hard SUCCESS (Random Price) (condition % set by player)
        // 2.1 Barter hard FAIL
        // 3. Hard pass - End dialogue        
        // SUCCESS -> Throw message to messagebox with offer, Player choice to accept or reject
        // FAIL -> try again, throw to an earlier conversation point, idc
        // 1. Accepted offer -
        //      set quest as running,
        //      add journal entry with the details,
        //      create an assassinmission to hold initiator lord, target lord and cost
        // 2. Rejected offer - treat as FAIL
        // Duplicate assassin_top_level, add conditions for if quest running and we're speaking with that lord or not so we don't just get the random "I want to kill you" dialogue, since killing the lord we're trying to quest for is probably bad
        // 1. How's that business coming along? -> End dialog
        // Speaking of, if a lord just randomly dies in battle, how will we detect and terminate the quest?  Add to MCM perhaps? Just an option to terminate the quest/fail it.
        // Next time we talk to someone it just terminates the quest if that option is checked.
        // At some point we should address relation as a reward with the lord

        //protected JournalLog AssassinationTask;

        [SaveableField(1)] 
        public int Payment;
        [SaveableField(2)]
        public static bool FailQuest;
        [SaveableField(3)]
        public static bool HideDialogue;
        [SaveableField(4)] 
        public Hero Instigator;
        [SaveableField(5)] 
        public Hero Target;
        [SaveableField(6)]
        private bool AssassinationSuccessful;
        [SaveableField(7)]
        public bool QuestRunning;

        public static bool PublicQuestRunningModifiable { get; set; }

        public AssassinQuest(Hero questGiver, int reward, Hero target, bool questrunning) : base("noblekiller_assassinations", questGiver, duration: CampaignTime.DaysFromNow(NKSettings.Instance.QuestDays), rewardGold: reward)
        {
            Payment = reward;
            Instigator = questGiver;
            Target = target;
            FailQuest = questrunning;
            AssassinationSuccessful = false;
            QuestRunning = true;
            //AddTrackedObject(Target);

            // Change hero to be highly likely to die
            //target.ProbabilityOfDeath = NKSettings.Instance.NobleDeathPercent;
            target.HitPoints = 0;
            

            AddLog(NKStartQuestLog);
        }

        public override bool IsSpecialQuest => true;

        private TextObject NKStartQuestLog
        {
            get
            {                
                TextObject assassination = new TextObject("The noble " + Hero.OneToOneConversationHero.Name + " wants me to discreetly take care of " + 
                    Target + " so my agents are out scouring the land to bring about their demise.");
                return assassination;
            }
        }

        private TextObject NKEndQuestLog
        {
            get
            {
                TextObject assassination = new TextObject("Mamaaa, just killed a man, put a gun against his head, pulled my trigger, now he's dead.");
                return assassination;
            }
        }

        private TextObject NKFailQuestLog
        {
            get
            {
                TextObject assassination = new TextObject("I just couldn't do it, why am I like this? It's a dog eat dog world!");
                return assassination;
            }
        }

        private TextObject NKCancelQuestLog
        {
            get
            {
                TextObject assassination = new TextObject("If that guy hadn't up and died of old age I was gonna send him to an early grave.");
                return assassination;
            }
        }

        public override TextObject Title
        {
            get
            {
                TextObject parent = new TextObject("Doing Dirty Work");                
                return parent;
            }
        }

        public override bool IsRemainingTimeHidden
        {
            get
            {
                return false;
            }
        }

        public void CompleteAssassinQuest()
        {
            CompleteQuestWithSuccess();
            GiveGoldAction.ApplyForQuestBetweenCharacters(Instigator, Hero.MainHero, Payment, false);
            FailQuest = false;
            QuestRunning = false;
            AssassinQuest.HideDialogue = false;
            NobleKillerDialogue.RandomSoonToBeDeadGuy = null;
            NobleKillerDialogue.PublicQuestActiveModifiable = QuestRunning;
            PublicQuestRunningModifiable = false;
            AssassinationSuccessful = true;
            AddLog(NKEndQuestLog);            
        }

        public void FailAssassinQuest()
        {
            CompleteQuestWithFail();
            QuestRunning = false;
            FailQuest = false;
            AssassinQuest.HideDialogue = false;
            NobleKillerDialogue.RandomSoonToBeDeadGuy = null;
            NobleKillerDialogue.PublicQuestActiveModifiable = QuestRunning;
            PublicQuestRunningModifiable = false;
            AddLog(NKFailQuestLog);
        }

        public void CancelQuestOldAge()
        {            
            CompleteQuestWithCancel();
            GiveGoldAction.ApplyForQuestBetweenCharacters(Instigator, Hero.MainHero, 200, false);
            FailQuest = false;
            QuestRunning = false;
            AssassinQuest.HideDialogue = false;
            NobleKillerDialogue.RandomSoonToBeDeadGuy = null;
            NobleKillerDialogue.PublicQuestActiveModifiable = QuestRunning;
            PublicQuestRunningModifiable = false;
            AssassinationSuccessful = true;
            AddLog(NKCancelQuestLog);
        }

        protected override void RegisterEvents()
        {
            PublicQuestRunningModifiable = QuestRunning;
            NobleKillerDialogue.PublicQuestActiveModifiable = QuestRunning;
            NobleKillerDialogue.PublicQuestGiverModifiable = Instigator;
            CampaignEvents.TickEvent.AddNonSerializedListener(this, Tick);
        }

        protected void Tick(float dt)
        {         
            if (Target.IsDead && Target.DeathMark == KillCharacterAction.KillCharacterActionDetail.DiedOfOldAge && NKSettings.Instance.AllowQuestCancel)
            {
                CancelQuestOldAge();
            }
            else if (Target.IsDead)
            {
                CompleteAssassinQuest();
            }            

            if (FailQuest && !AssassinationSuccessful)
            {
                // Terminate quest with fail
                FailAssassinQuest();
            }
        }

        protected override void InitializeQuestOnGameLoad()
        {
            SetDialogs();                 
        }

        protected override void SetDialogs()
        {          
        }

        // Called after the quest is finished
        protected override void OnFinalize()
        {
            base.OnFinalize();
            NobleKillerDialogue.PublicQuestActiveModifiable = false;
        }
    }
}