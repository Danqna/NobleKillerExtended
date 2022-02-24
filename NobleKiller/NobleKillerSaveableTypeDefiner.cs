using NobleKiller.Behaviour;
using TaleWorlds.SaveSystem;

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
            AddClassDefinition(typeof(noblekillerdialogue), 5634132);
        }
    }
}
