using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;
using GameNet;
using GameNet.LoginService;
using GameNet.Common.CommandsBase;
using GameNet.Common;
using GameNet.Common.ClientsBase;
using GameNet.Common.Operations;
using GameNet.DataBaseServise;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            //using (DataBase db = new DataBase())
            //{
            //    db.accounts.Add(new Accounts() { Login = "stqqinger125", Password = "Aslawwwn" });
            //    db.SaveChanges();

            //}

            AccountService login = AccountService.GetInstance();

            
            Console.Read();
            
         
            

        }
        


    }
}
