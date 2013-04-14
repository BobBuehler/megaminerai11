using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pizza
{
    enum Objective
    {
        goTo,//ie: Bb.OurReef
        getTrash,//ie: Bb.OurTrashMap
        dumpTrash,//ie: Bb.TheirReef, 
        surviveWithInTarget,// implement last. bounds i want the fish to dodge in  ie:Bb.TheirReef 
        attackTarget// move to nearest target in input and attack ie: Bb.TheirFishMap
    };

    class Mission
    {
        public Fish m_agent;
        public Func<BitArray> m_targets;
        public Objective m_obj;
        public bool m_attackAlongTheWay;
        public bool m_moveIffAcheivable;

        public Mission(Fish Agent, Objective Obj, Func<BitArray> Targets, bool MoveIffAcheivable = false, bool AttackAlongTheWay = true)
        {
            m_agent = Agent;
            m_obj = Obj;
            m_targets = Targets;
            m_attackAlongTheWay = AttackAlongTheWay;
        }

    }
}
