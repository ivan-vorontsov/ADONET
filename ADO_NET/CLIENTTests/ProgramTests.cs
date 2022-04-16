using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLIENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIENT.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void InsertAccountTest()
        {
            Program program = new Program();
            Random random = new Random();
            var i = random.Next(99999990);
            program.InsertAccount("Ivan", "Manager");
            var account = program.GetAccount("Ivan");
            Assert.IsNotNull(account);
        }

        [TestMethod()]
        public void SaveAccountsTest()
        {
            Program target = new Program();
            var accounts = new AdventureStoreDataSet.DimAccountDataTable();

            accounts.AddDimAccountRow(null, 0, 0, "John", "Couch", "", "", "", "");

            target.SaveAccounts(accounts);

            var account = target.GetAccount("John");
            Assert.IsNotNull(account);
        }
    }
}