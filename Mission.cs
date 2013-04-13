using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pizza
{
    enum Objective
    {
        goTo,
        getTrash,
        dropTrash,
        kill
    };

    class Mission
    {
        Fish m_agent;
        Point m_dest;
        Objective m_obj;


        public Mission(Fish Agent)
        {
            m_agent = Agent;
            m_dest = new Point();
            m_obj = Objective.goTo;
        }

        public Mission(Fish Agent, Objective Obj, Point Dest)
        {
            m_agent = Agent;
            m_obj = Obj;
            m_dest = Dest;
        }


    }
}
