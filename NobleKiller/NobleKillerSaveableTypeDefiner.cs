using NobleKiller.Behaviour;
using TaleWorlds.SaveSystem;
using TaleWorlds.CampaignSystem;

namespace NobleKiller
{
    public class NobleKillerSaveableTypeDefiner : SaveableTypeDefiner
    {
        public NobleKillerSaveableTypeDefiner() : base(5634131)
        {
        }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(AssassinQuest), 5634131);
            AddClassDefinition(typeof(NobleKillerDialogue), 56341312);
        }
    }
}
