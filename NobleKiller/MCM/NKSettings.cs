using MCM.Abstractions.FluentBuilder;
using MCM.Abstractions.Ref;
using MCM.Abstractions.Settings.Base.Global;
using NobleKiller.Behaviour;
using TaleWorlds.Localization;
using System;

namespace NobleKiller.MCM
{
    public class NKSettings : IDisposable
    {
        private static NKSettings _instance;

        private FluentGlobalSettings globalSettings;

        public bool OverrideCost { get; set; }
        public int QuestDays { get; set; }
        public int StartCostValue { get; set; }
        public int EndCostValue { get; set; }
        public float BarterHigh { get; set; }
        public float BarterHard { get; set; }
        public int StartRewardValue { get; set; }
        public int EndRewardValue { get; set; }
        public float BarterPercent { get; set; }
        public string CurrentTarget { get; set; }
        public int CurrentCostValue { get; set; }
        public bool DebugMessages { get; set; }
        public bool QuestBug { get; set; }        
        public string OurTarget { get; set; }
        public bool FirstRunDone { get; set; }      
        public int NobleDeathPercent { get; set; }
        public bool AllowQuestCancel { get; set; }

        public static NKSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NKSettings();
                }
                return _instance;
            }
        }

        public void Settings()
        {                
            // Can't have nulls
            OurTarget = "default value";
            CurrentTarget = "default value";
            NobleDeathPercent = 200;

            TextObject NKTITLE00 = new TextObject("NobleKiller");
            TextObject NKSETTING = new TextObject("Assassination Options");
            TextObject NKCSTOVRD = new TextObject("1.Override calculated costs ? ");
            TextObject NKCSTHNT0 = new TextObject("Set own cost");
            TextObject NKDQFAIL0 = new TextObject("2. Days before quest fails");
            TextObject NKDQHINT0 = new TextObject("Days until quest fails");
            TextObject NKNDEATHP = new TextObject("3. Modify chance of noble dying (old age and battle)");
            TextObject NKNDPHINT = new TextObject("Adjust how much chance the noble has of dying from all possible events");
            TextObject NKNDCANCL = new TextObject("4.Do you want the quest to cancel if target dies of old age?");
            TextObject NKNDCHINT = new TextObject("Allow Quest Cancellations");
            TextObject NKBCAR000 = new TextObject("2. Balancing Cost and Rewards");
            TextObject NKLCAOVRD = new TextObject("1. Lowest cost of assassination if overriding:");
            TextObject NKLCV0000 = new TextObject("Cost Values");
            TextObject NKHCAOVRD = new TextObject("2. Highest cost of assassination if overriding:");
            TextObject NKLCV0001 = new TextObject("Cost Values");
            TextObject NKCGHR000 = new TextObject("3. Chance to get a high reward:");
            TextObject NKDCSAQ00 = new TextObject("During conversations to start assassination quest");
            TextObject NKCWHB000 = new TextObject("4.Chance to win a hard barter: ");
            TextObject NKDCSAQ01 = new TextObject("During conversations to start assassination quest");
            TextObject NKLRWDVAL = new TextObject("5. Lowest reward value:");
            TextObject NKLCV0002 = new TextObject("Cost values");
            TextObject NKHRWDVAL = new TextObject("6. Highest reward value:");
            TextObject NKLCV0003 = new TextObject("Cost values");
            TextObject NKMFAHRWD = new TextObject("7. Multiplier for a high reward:");
            TextObject NKDCSAQ02 = new TextObject("During conversations to start assassination quest");
            TextObject NKREADONL = new TextObject("3. READ ONLY Target Info");
            TextObject NKCURRTAR = new TextObject("1. Current target is: ");
            TextObject NKCURRCOS = new TextObject("2. Current Cost");
            TextObject NKLCV0004 = new TextObject("Cost values");
            TextObject NKASSQRUN = new TextObject("3.Assassin quest is running:");
            TextObject NKASSDEBU = new TextObject("Information on whether the game is running the assassin quest - for debugging");
            TextObject NKDIALHID = new TextObject("4. Dialogue is hidden:");
            TextObject NKASSDHID = new TextObject("Information on whether the game is showing dialogue for the assassin quest - for debugging");
            TextObject NKDEBUG00 = new TextObject("Debug");
            TextObject NKDBGENAB = new TextObject("1. Debugging enabled:");
            TextObject NKDBGHNTT = new TextObject("Adds messages sometimes");
            TextObject NKFIXQLAU = new TextObject("2. Fix Quest Launch:");
            TextObject NKFIXQHNT = new TextObject("If the quest option stops showing");
            TextObject NKRESETDF = new TextObject("5. Reset to defaults");
            TextObject NKRESET00 = new TextObject("1. Uncheck this to reset all settings to defaults - requires restart");
            TextObject NKRESETHN = new TextObject("This is for setting defaults");

            var builder = BaseSettingsBuilder.Create(NKTITLE00.ToString(), NKSETTING.ToString())!
                .SetFormat("xml")
                .SetFolderName(MySubModule.ModuleFolderName)
                .SetSubFolder(MySubModule.ModName)
                .CreateGroup("1. General Settings", groupBuilder => groupBuilder
                    .AddBool("overridedefaultocost", NKCSTOVRD.ToString(), new ProxyRef<bool>(() => OverrideCost, o => OverrideCost = o), boolBuilder => boolBuilder
                        .SetHintText(NKCSTHNT0.ToString())
                        .SetRequireRestart(false))
                    .AddInteger("daysquest", NKDQFAIL0.ToString(), 1, 200, new ProxyRef<int>(() => QuestDays, o => QuestDays = o), integerBuilder => integerBuilder
                        .SetHintText(NKDQHINT0.ToString()))
                    .AddInteger("deathpercent", NKNDEATHP.ToString(), 1, 200, new ProxyRef<int>(() => NobleDeathPercent, o => NobleDeathPercent = o), integerBuilder => integerBuilder
                        .SetHintText(NKNDPHINT.ToString()))
                    .AddBool("allowquestcancellation", NKNDCANCL.ToString(), new ProxyRef<bool>(() => AllowQuestCancel, o => AllowQuestCancel = o), boolBuilder => boolBuilder
                        .SetHintText(NKNDCHINT.ToString())
                        .SetRequireRestart(false))
                    )
                .CreateGroup(NKBCAR000.ToString(), groupBuilder => groupBuilder
                    .AddInteger("Lowestcost", NKLCAOVRD.ToString(), 1, 100000, new ProxyRef<int>(() => StartCostValue, o => StartCostValue = o), integerBuilder => integerBuilder
                        .SetHintText(NKLCV0000.ToString()))
                    .AddInteger("Highestcost", NKHCAOVRD.ToString(), 1, 100000, new ProxyRef<int>(() => EndCostValue, o => EndCostValue = o), integerBuilder => integerBuilder
                        .SetHintText(NKLCV0001.ToString()))
                    .AddFloatingInteger("barterhigh", NKCGHR000.ToString(), 1, 100, new ProxyRef<float>(() => BarterHigh, o => BarterHigh = o), floatingBuilder => floatingBuilder
                        .SetHintText(NKDCSAQ00.ToString()))
                    .AddFloatingInteger("barterhard", NKCWHB000.ToString(), 1, 100, new ProxyRef<float>(() => BarterHard, o => BarterHard = o), floatingBuilder => floatingBuilder
                        .SetHintText(NKDCSAQ01.ToString()))
                    .AddInteger("_startrewardvalue", NKLRWDVAL.ToString(), 1, 100000, new ProxyRef<int>(() => StartRewardValue, o => StartRewardValue = o), integerBuilder => integerBuilder
                        .SetHintText(NKLCV0002.ToString()))
                    .AddInteger("_endrewardvalue", NKHRWDVAL.ToString(), 1, 100000, new ProxyRef<int>(() => EndRewardValue, o => EndRewardValue = o), integerBuilder => integerBuilder
                        .SetHintText(NKLCV0003.ToString()))
                    .AddFloatingInteger("_barterpercent", NKMFAHRWD.ToString(), 1, 100, new ProxyRef<float>(() => BarterPercent, o => BarterPercent = o), floatingBuilder => floatingBuilder
                        .SetHintText(NKDCSAQ02.ToString()))
                    )
                .CreateGroup(NKREADONL.ToString(), groupBuilder => groupBuilder
                    .AddText("currenttarget", NKCURRTAR.ToString() + CurrentTarget, new ProxyRef<string>(() => CurrentTarget, o => CurrentTarget = o), null)
                    .AddInteger("Currentcost", NKCURRCOS.ToString(), CurrentCostValue, CurrentCostValue, new ProxyRef<int>(() => CurrentCostValue, o => CurrentCostValue = o), integerBuilder => integerBuilder
                        .SetHintText(NKLCV0004.ToString()))
                    .AddBool("assassinquestrunning", NKASSQRUN.ToString(), new ProxyRef<bool>(() => AssassinQuest.FailQuest, o => AssassinQuest.FailQuest = o), boolBuilder => boolBuilder
                        .SetHintText(NKASSDEBU.ToString())
                        .SetRequireRestart(false))
                    .AddBool("assassindonotshowdialogue", NKDIALHID.ToString(), new ProxyRef<bool>(() => AssassinQuest.HideDialogue, o => AssassinQuest.HideDialogue = o), boolBuilder => boolBuilder
                        .SetHintText(NKASSDHID.ToString())
                        .SetRequireRestart(false))
                    )
                .CreateGroup(NKDEBUG00.ToString(), groupBuilder => groupBuilder
                    .AddBool("debugger", NKDBGENAB.ToString(), new ProxyRef<bool>(() => DebugMessages, o => DebugMessages = o), boolBuilder => boolBuilder
                        .SetHintText(NKDBGHNTT.ToString())
                        .SetRequireRestart(false))
                    .AddBool("questbug", NKFIXQLAU.ToString(), new ProxyRef<bool>(() => QuestBug, o => QuestBug = o), boolBuilder => boolBuilder
                        .SetHintText(NKFIXQHNT.ToString())
                        .SetRequireRestart(false))
                    )
                .CreateGroup(NKRESETDF.ToString(), groupBuilder => groupBuilder
                    .AddBool("firstrun", NKRESET00.ToString(), new ProxyRef<bool>(() => FirstRunDone, o => FirstRunDone = o), boolBuilder => boolBuilder
                    .SetHintText(NKRESETHN.ToString())
                    .SetRequireRestart(true)));



            globalSettings = builder.BuildAsGlobal();
            globalSettings.Register();

            if (!FirstRunDone)
            {
                Perform_First_Time_Setup();
            }
        }

        private void Perform_First_Time_Setup()
        {
            NKSettings.Instance.OverrideCost = false;
            NKSettings.Instance.QuestDays = 30;
            NKSettings.Instance.StartCostValue = 5000;
            NKSettings.Instance.EndCostValue = 500000;
            NKSettings.Instance.DebugMessages = false;
            NKSettings.Instance.BarterHigh = 50;
            NKSettings.Instance.BarterHard = 50;
            NKSettings.Instance.StartRewardValue = 5000;
            NKSettings.Instance.EndRewardValue = 20000;
            NKSettings.Instance.BarterPercent = .5f;
            NKSettings.Instance.FirstRunDone = true;
            NKSettings.Instance.NobleDeathPercent = 200;
        }

        public void Dispose()
        {
            //NKSettings.Unregister();
        }
    }
}