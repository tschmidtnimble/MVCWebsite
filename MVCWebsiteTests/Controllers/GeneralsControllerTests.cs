﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCWebsite.Controllers;
using MVCWebsite.Models;
using System.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net;

namespace MVCWebsite.Controllers.Tests
{
    [TestClass()]
    public class GeneralsControllerTests
    {
        //TODO refactor the code 
        GenDBContext db = new GenDBContext();
        [TestMethod()]
        public void IndexTest()
        {
            TestDefault();
            TestChangedSort();
            TestSearchAll();
            TestNonExistentString();
            TestSortingAndSearch();
        }
        [TestMethod()]
        public void IDTest()
        {
            GeneralsController genController = new GeneralsController();
            Random gen = new Random();
            List<General> genList = db.Generals.ToList();
            int testID = gen.Next(genList.Count);
            ViewResult v = genController.Details(genList[testID].ID) as ViewResult;
            General g = v.Model as General;
            Assert.AreEqual(g, genList[testID], "Correct id is not being returned");

            //null check 
            genController = new GeneralsController();
            HttpStatusCodeResult correctStatus = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ActionResult details = genController.Details(null);
            HttpStatusCodeResult resultStatus = genController.Details(null) is HttpStatusCodeResult ? details as HttpStatusCodeResult : null;
            if (resultStatus == null)
            {
                Assert.Fail("not responding to nulls correctly");
            }
            else {
                Assert.AreEqual(correctStatus.StatusCode
                    , resultStatus.StatusCode, "Null check wrong status code");
            }

            //null general check
            genController = new GeneralsController();
            var nullGendetails = genController.Details(-1);
            var nullGenDetailsResult = nullGendetails is HttpNotFoundResult? nullGendetails as HttpNotFoundResult:null;
            if (nullGenDetailsResult == null)
            {
                Assert.Fail("did not handle non existent general properly");
            }
            else
            {

                var r = new HttpNotFoundResult();
                Assert.AreEqual(nullGenDetailsResult.StatusCode , r.StatusCode, "Did not handle non existent general");
            }
        }
        /// <summary>
        /// Tests on empty lists
        /// </summary>
        private void TestDefault()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "asc");
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index() as ViewResult;
            List<General> resultList = (List<General>)view.ViewData.Model;
            CollectionAssert.AreEqual(correctResult, resultList, "The Default list is not correct ");
        }
        private void TestChangedSort()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "desc");
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(sort:"Desc") as ViewResult;
            List<General> resultList = (List<General>)view.ViewData.Model;
            CollectionAssert.AreEqual(correctResult, resultList, "The descending list is not correct ");
        }
        private void TestSearchAll()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "asc");
            string search = "Barca";
            defaultQuery = defaultQuery.Where("Name.contains(@0) or Country.contains(@0) or Comments.contains(@0)", search);
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(SearchString: search,SearchName:  true,SearchCountry:  true,SearchComments:  true) as ViewResult;

            List<General> resultList = (List<General>)view.ViewData.Model;
            Assert.IsTrue(resultList.All(x =>
            x.Country.Contains(search) || x.Name.Contains(search) || x.Comments.Contains(search)));
            General g = resultList.Find(x => !(x.Country.Contains(search) || x.Name.Contains(search) || x.Comments.Contains(search)));
            if(g != null)
            {
                Assert.Fail(g.Name);
            }
            CollectionAssert.AreEqual(correctResult, resultList, "The searchall list is not correct ");
        }
        private void TestNonExistentString()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "asc");
            string search = "l;kl;jop";
            defaultQuery = defaultQuery.Where("Name.contains(@0) or Country.contains(@0) or Comments.contains(@0)", search);
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(SearchString: search, SearchName: true, SearchCountry: true, SearchComments: true) as ViewResult;
            List<General> resultList = (List<General>)view.ViewData.Model; ;
            //if true may need a new string
            Assert.IsTrue(correctResult.Count == 0,"string is now part of the databse found "+ correctResult.Count +"result");
            CollectionAssert.AreEqual(correctResult, resultList, "The SearchNonexistent list is not correct ");
        }
        private void TestSortingAndSearch()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "desc");
            string search = "l;kl;jop";
            defaultQuery = defaultQuery.Where("Name.contains(@0) or Country.contains(@0) or Comments.contains(@0)", search);
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(sort:  "desc",SearchString: search, SearchName: true, SearchCountry: true, SearchComments: true) as ViewResult;
            List<General> resultList = (List<General>)view.ViewData.Model; ;
            //if true may need a new string
            Assert.IsTrue(correctResult.Count == 0, "string is now part of the databse found " + correctResult.Count + "result");
            CollectionAssert.AreEqual(correctResult, resultList, "The search while sorting list is not correct ");

        }
        private void TestOnlyMatchName()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "asc");
            string search = "Barca";
            defaultQuery = defaultQuery.Where("Name.contains(@0)", search);
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(SearchString: search, SearchName: true) as ViewResult;

            List<General> resultList = (List<General>)view.ViewData.Model;
            Assert.IsTrue(resultList.All(x =>
            x.Name.Contains(search)));
            CollectionAssert.AreEqual(correctResult, resultList, "The Search only name" +
                " list is not correct ");

        }
    }
}