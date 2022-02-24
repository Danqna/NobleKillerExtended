using MCM.Abstractions.FluentBuilder;
using MCM.Abstractions.Ref;
using MCM.Abstractions.Settings.Base.Global;
using NobleKiller.Behaviour;
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

            var builder = BaseSettingsBuilder.Create("NobleKiller", "Assassination Options")!
                .SetFormat("xml")
                .SetFolderName(MySubModule.ModuleFolderName)
                .SetSubFolder(MySubModule.ModName)
                .CreateGroup("1. General Settings", groupBuilder => groupBuilder
                    .AddBool("overridedefaultocost", "Override calculated costs?", new ProxyRef<bool>(() => OverrideCost, o => OverrideCost = o), boolBuilder => boolBuilder
                        .SetHintText("Set own cost")
                        .SetRequireRestart(false))
                    .AddInteger("daysquest", "Days before quest fails", 1, 200, new ProxyRef<int>(() => QuestDays, o => QuestDays = o), integerBuilder => integerBuilder
                        .SetHintText("Days until quest fails"))
                    )
                .CreateGroup("2. Balancing Cost and Rewards", groupBuilder => groupBuilder
                    .AddInteger("Lowestcost", "1. Lowest cost of assassination if overriding:", 1, 100000, new ProxyRef<int>(() => StartCostValue, o => StartCostValue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values"))
                    .AddInteger("Highestcost", "2. Highest cost of assassination if overriding:", 1, 100000, new ProxyRef<int>(() => EndCostValue, o => EndCostValue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values"))
                    .AddFloatingInteger("barterhigh", "3. Chance to get a high reward:", 1, 100, new ProxyRef<float>(() => BarterHigh, o => BarterHigh = o), floatingBuilder => floatingBuilder
                        .SetHintText("During conversations to start assassination quest"))
                    .AddFloatingInteger("barterhard", "4. Chance to win a hard barter:", 1, 100, new ProxyRef<float>(() => BarterHard, o => BarterHard = o), floatingBuilder => floatingBuilder
                        .SetHintText("During conversations to start assassination quest"))
                    .AddInteger("_startrewardvalue", "5. Lowest reward value:", 1, 100000, new ProxyRef<int>(() => StartRewardValue, o => StartRewardValue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values"))
                    .AddInteger("_endrewardvalue", "6. Highest reward value:", 1, 100000, new ProxyRef<int>(() => EndRewardValue, o => EndRewardValue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values"))
                    .AddFloatingInteger("_barterpercent", "7. Multiplier for a high reward:", 1, 100, new ProxyRef<float>(() => BarterPercent, o => BarterPercent = o), floatingBuilder => floatingBuilder
                        .SetHintText("During conversations to start assassination quest"))
                    )
                .CreateGroup("3. READ ONLY Target Info", groupBuilder => groupBuilder
                    .AddText("currenttarget", "Current target is: " + CurrentTarget, new ProxyRef<string>(() => CurrentTarget, o => CurrentTarget = o), null)
                    .AddInteger("Currentcost", "Current Cost", CurrentCostValue, CurrentCostValue, new ProxyRef<int>(() => CurrentCostValue, o => CurrentCostValue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values"))                    
                    .AddBool("assassinquestrunning", "Assassin quest is running:", new ProxyRef<bool>(() => AssassinQuest.FailQuest, o => AssassinQuest.FailQuest = o), boolBuilder => boolBuilder
                        .SetHintText("Information on whether the game is running the assassin quest - for debugging")
                        .SetRequireRestart(false))
                    .AddBool("assassindonotshowdialogue", "Dialogue is hidden:", new ProxyRef<bool>(() => AssassinQuest.HideDialogue, o => AssassinQuest.HideDialogue = o), boolBuilder => boolBuilder
                        .SetHintText("Information on whether the game is showing dialogue for the assassin quest - for debugging")
                        .SetRequireRestart(false))
                    )
                .CreateGroup("4. Debug", groupBuilder => groupBuilder
                    .AddBool("debugger", "Debugging enabled:", new ProxyRef<bool>(() => DebugMessages, o => DebugMessages = o), boolBuilder => boolBuilder
                        .SetHintText("Adds messages sometimes")
                        .SetRequireRestart(false))
                    .AddBool("questbug", "Fix Quest Launch:", new ProxyRef<bool>(() => QuestBug, o => QuestBug = o), boolBuilder => boolBuilder
                        .SetHintText("If the quest option stops showing")
                        .SetRequireRestart(false))
                    )
                .CreateGroup("5. Reset to defaults", groupBuilder => groupBuilder
                    .AddBool("firstrun", "Uncheck this to reset all settings to defaults - requires restart", new ProxyRef<bool>(() => FirstRunDone, o => FirstRunDone = o), boolBuilder => boolBuilder
                    .SetHintText("This is for setting defaults")
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
        }

        public void Dispose()
        {
            //globalSettings.Unregister();
        }
    }
}