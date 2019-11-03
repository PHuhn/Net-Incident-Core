using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
//
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv_Tests
{
    [TestClass]
    public class Research_UnitTests
    {
        //
        //
        public Research_UnitTests()
        {
            //
        }
        //
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        //
        [TestMethod]
        public void RemoveAll_Test()
        {
            List<SelectListItem> _list = new List<SelectListItem>()
            {
                new SelectListItem(){ Value= "1", Text = "Bob Smith" },
                new SelectListItem(){ Value= "2", Text = "Fred Jones" },
                new SelectListItem(){ Value= "3", Text = "Brian Brains" },
                new SelectListItem(){ Value= "4", Text = "Billy TheKid" },
            };
            string[] _valToRemove = new string[] { "1", "3" };
            _list.RemoveAll(x => _valToRemove.Contains(x.Value));
            _list.ForEach(sl => Console.WriteLine(sl.Value + " " + sl.Text));
        }
        //
    }
}
