using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CinemaApp.Pages;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AuthTest()
        {
            var page = new LoginPage();
            Assert.IsTrue(page.Auth("admin", "123456"));
            //Assert.IsFalse(page.Auth("user1", "12345"));
            Assert.IsFalse(page.Auth("", ""));
            //Assert.IsFalse(page.Auth(" ", " "));

        }
        [TestMethod]
        public void AuthTestSuccess()
        {
            var page = new LoginPage();
            Assert.IsTrue(page.Auth("admin", "123456"));
        }
        [TestMethod]
        public void AuthTestFail()
        {
            var page = new LoginPage();
            //Assert.IsFalse(page.Auth("manager1", "123456"));
            Assert.IsFalse(page.Auth("", ""));
        }
    }
}
