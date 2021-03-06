﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCWebsite.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using MVCWebsite.Models;
using System.Web.Mvc;

namespace MVCWebsite.Controllers.Tests
{
    [TestClass()]
    public class DatesControllerTests
    {
        
        private DateDBContext db = new DateDBContext();
        DatesController datesController = new DatesController();
        /// <summary>
        /// Test asc sort works
        /// </summary>
        [TestMethod()]
        public void TestBasicSort()
        {
            var dates = from d in db.Dates
                        orderby d.Time
                        select d;

            var view = datesController.Index("", 1, 25) as ViewResult;
            PagedList<Date> resultPage = view.Model as PagedList<Date>;
            List<Date> resultList = resultPage.ToList();
            List<Date> correctList = dates.ToPagedList(1, 25).ToList();
            CollectionAssert.AreEqual(resultList, correctList, "Basic sorting is not working correctly");
        }
        /// <summary>
        /// Test desc sort works
        /// </summary>
        [TestMethod()]
        public void TestDescSort()
        {
            var dates = from d in db.Dates
                        orderby d.Time
                        select d;
            dates = dates.OrderByDescending(d => d.Time);

            var view = datesController.Index("desc", 1, 25) as ViewResult;
            PagedList<Date> resultPage = view.Model as PagedList<Date>;
            List<Date> resultList = resultPage.ToList();
            List<Date> correctList = dates.ToPagedList(1, 25).ToList();

            CollectionAssert.AreEqual(resultList, correctList, "Desc sorting is not working correctly");
        }
        //TODO test against pagination

    }
}