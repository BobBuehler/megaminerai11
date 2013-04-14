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
        getTrash,
        dumpTrash,
        coverWithTrash,
        surviveWithInTarget,
        attackTarget,
        dontsuicide
    };

    class Mission
    {
        public Fish m_agent;
        public Func<BitArray> m_targets;
        public Objective m_obj;
        public bool m_attackAlongTheWay;
        public bool m_overrideDropMission;

        public Mission(Fish Agent, Objective Obj, Func<BitArray> Targets, bool OverrideDropMission = false, bool AttackAlongTheWay = true)
        {
            m_agent = Agent;
            m_obj = Obj;
            m_targets = Targets;
            m_attackAlongTheWay = AttackAlongTheWay;
            m_overrideDropMission = OverrideDropMission;
        }
    }
}
