using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CinemaApp.Pages;

namespace UnitTestProject2
{
    //Reg(name, email, login, pass, pass2);

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RegTestSuccess()
        {
            var page = new RegisterPage();
            Assert.IsTrue(page.Reg("user_new1", "", "user_new1", "User123", "User123"));
        }
        [TestMethod]
        public void RegTestFail()
        {
            var page = new RegisterPage();
            Assert.IsFalse(page.Reg("admin", "", "admin", "Admin123", "1"));
        }

    }
}
