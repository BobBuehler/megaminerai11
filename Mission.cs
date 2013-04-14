using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pizza
{
    enum Objective
    {
        goTo,
        getNearestTrash,
        dumpTrashClose,
        dumpTrashFar,
        dodgeInReef,
        killTarget
    };

    class Mission
    {
        public Fish m_agent;
        //Point m_dest;
        public BitArray m_targets;
        public Objective m_obj;
        public bool m_attackAlongTheWay;


        public Mission(Fish Agent)
        {
            m_agent = Agent;
            m_obj = Objective.goTo;
            m_attackAlongTheWay = true;
        }

        public Mission(Fish Agent, Objective Obj,  BitArray Targets)
        {
            m_agent = Agent;
            m_obj = Obj;
            m_targets = Targets;
            m_attackAlongTheWay = true;
        }

        public Mission(Fish Agent, Objective Obj, BitArray Targets, bool AttackAlongTheWay)
        {
            m_agent = Agent;
            m_obj = Obj;
            m_targets = Targets;
            m_attackAlongTheWay = AttackAlongTheWay;
        }

    }
}
