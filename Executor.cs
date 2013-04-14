using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pizza
{
    class Executor
    {
        public static void Execute(AI ai, Mission mission)
        {
            switch(mission.m_obj)
            {
                case Objective.goTo:
                    GoTo(ai, mission);
                    break;
            }
            return;
        }

        public static void GoTo(AI ai, Mission mission)
        {

        }
    }
}
