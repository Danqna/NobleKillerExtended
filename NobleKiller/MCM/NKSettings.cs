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
        public bool _firstrundone { get; set; }
        public bool _overridecost { get; set; }
        public bool _debugmessages { get; set; }
        public int _startcostvalue { get; set; }
        public int _endcostvalue { get; set; }
        public int _startrewardvalue { get; set; }
        public int _endrewardvalue { get; set; }
        public int _currentcostvalue { get; set; }
        public float _barterhigh { get; set; }
        public float _barterhard { get; set; }
        public float _barterpercent { get; set; }
        public int _questdays { get; set; }


        public string _currenttarget = noblekillerdialogue.Instance.targetname;

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
            // Update this value here
            _currenttarget = noblekillerdialogue.Instance.targetname;
            string idiotproofprobably = _currenttarget;

            var builder = BaseSettingsBuilder.Create("NobleKiller", "Assassination Options")!
                .SetFormat("xml")
                .SetFolderName(MySubModule.ModuleFolderName)
                .SetSubFolder(MySubModule.ModName)
                .CreateGroup("1. General Settings", groupBuilder => groupBuilder                    
                    .AddBool("overridedefaultocost", "Override calculated costs?", new ProxyRef<bool>(() => _overridecost, o => _overridecost = o), boolBuilder => boolBuilder
                        .SetHintText("Set own cost")
                        .SetRequireRestart(false))
                    .AddInteger("daysquest", "Days before quest fails", 1, 200, new ProxyRef<int>(() => _questdays, o => _questdays = o), integerBuilder => integerBuilder
                        .SetHintText("Days until quest fails"))
                    )
                .CreateGroup("2. Balancing Cost and Rewards", groupBuilder => groupBuilder
                    .AddInteger("Lowestcost", "1. Lowest cost of assassination if overriding:", 1, 100000, new ProxyRef<int>(() => _startcostvalue, o => _startcostvalue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values"))
                    .AddInteger("Highestcost", "2. Highest cost of assassination if overriding:", 1, 100000, new ProxyRef<int>(() => _endcostvalue, o => _endcostvalue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values"))
                    .AddFloatingInteger("barterhigh", "3. Chance to get a high reward:", 1, 100, new ProxyRef<float>(() => _barterhigh, o => _barterhigh = o), floatingBuilder => floatingBuilder
                        .SetHintText("During conversations to start assassination quest"))
                    .AddFloatingInteger("barterhard", "4. Chance to win a hard barter:", 1, 100, new ProxyRef<float>(() => _barterhard, o => _barterhard = o), floatingBuilder => floatingBuilder
                        .SetHintText("During conversations to start assassination quest"))
                    .AddInteger("_startrewardvalue", "5. Lowest reward value:", 1, 100000, new ProxyRef<int>(() => _startrewardvalue, o => _startrewardvalue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values"))
                    .AddInteger("_endrewardvalue", "6. Highest reward value:", 1, 100000, new ProxyRef<int>(() => _endrewardvalue, o => _endrewardvalue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values"))
                    .AddFloatingInteger("_barterpercent", "7. Multiplier for a high reward:", 1, 100, new ProxyRef<float>(() => _barterpercent, o => _barterpercent = o), floatingBuilder => floatingBuilder
                        .SetHintText("During conversations to start assassination quest"))
                    )
                .CreateGroup("3. READ ONLY Target Info", groupBuilder => groupBuilder
                    .AddText("currenttarget", "Current target is: " + idiotproofprobably, new ProxyRef<string>(() => idiotproofprobably, o => idiotproofprobably = o), null)
                    .AddInteger("Currentcost", "Current Cost", _currentcostvalue, _currentcostvalue, new ProxyRef<int>(() => _currentcostvalue, o => _currentcostvalue = o), integerBuilder => integerBuilder
                        .SetHintText("Cost values")))
                .CreateGroup("4. Debug", groupBuilder => groupBuilder
                    .AddBool("debugger", "Debugging enabled", new ProxyRef<bool>(() => _debugmessages, o => _debugmessages = o), boolBuilder => boolBuilder
                        .SetHintText("Adds messages sometimes")
                        .SetRequireRestart(false)))
                .CreateGroup("5. Reset to defaults", groupBuilder => groupBuilder
                    .AddBool("firstrun", "Uncheck this to reset all settings to defaults - requires restart", new ProxyRef<bool>(() => _firstrundone, o => _firstrundone = o), boolBuilder => boolBuilder
                    .SetHintText("This is for setting defaults")
                    .SetRequireRestart(true)));



            globalSettings = builder.BuildAsGlobal();
            globalSettings.Register();
            if (!_firstrundone)
            {
                perform_first_time_setup();
            }
        }

        private void perform_first_time_setup()
        {
            NKSettings.Instance._overridecost = false;
            NKSettings.Instance._questdays = 30;
            NKSettings.Instance._startcostvalue = 5000;
            NKSettings.Instance._endcostvalue = 500000;
            NKSettings.Instance._debugmessages = false;
            NKSettings.Instance._barterhigh = 50;
            NKSettings.Instance._barterhard = 50;
            NKSettings.Instance._startrewardvalue = 5000;
            NKSettings.Instance._endrewardvalue = 20000;
            NKSettings.Instance._barterpercent = .5f;
            NKSettings.Instance._firstrundone = true;
        }

        public void Dispose()
        {
            //globalSettings.Unregister();
        }
    }
}