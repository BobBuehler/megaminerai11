using System;
using System.Runtime.InteropServices;


public class main
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            string defaultHost = "r99acm.device.mst.edu";
            System.Console.Write("Game Id: ");
            string gameId = System.Console.ReadLine();
            args = new string[] { defaultHost, gameId };
        }

        IntPtr connection = Client.createConnection();

        AI ai = new AI(connection);
        if ((Client.serverConnect(connection, args[0], "19000")) == 0)
        {
            System.Console.WriteLine("Unable to connect to server");
            return;
        }
        if ((Client.serverLogin(connection, ai.username(), ai.password())) == 0)
        {
            return;
        }

        if (args.Length < 2)
        {
            Client.createGame(connection);
        }
        else
        {
            Client.joinGame(connection, Int32.Parse(args[1]), "player");
        }
        while (Client.networkLoop(connection) != 0)
        {
            if (ai.startTurn())
            {
                Client.endTurn(connection);
            }
            else
            {
                Client.getStatus(connection);
            }
        }
        Client.networkLoop(connection); //Grab end game state
        Client.networkLoop(connection); //Grab log
        ai.end();
        return;
    }
}
