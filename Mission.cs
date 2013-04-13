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
        int m_x = 0;
        int m_y = 0;
        Objective m_obj = Objective.goTo;


        Mission(Fish Agent)
        {
            m_agent = Agent;

        }

        Mission(Fish Agent, Objective O)
        {
            m_agent = Agent;
            m_obj = O;
        }


    }
}
