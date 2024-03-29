﻿using DB_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DB_Project.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Console()
        {
            List<Order> AllOrders = OrderCRUD.GetAllOrders();
            List<Request> AllRequests = RequestCRUD.GetRequest((int)Session["UserID"]); //pass admin id to get all requests

            AllOrders.RemoveAll(item => item.OrderStatus == "Delivered"); //only keep undelievered orders
            AllRequests.RemoveAll(item => item.RequestStatus == "Resolved"); //only keep unresolved requests

            //display pending work to the admin docking page
            return View(new Tuple<List<Order>, List<Request>>(AllOrders, AllRequests));
        }

        public ActionResult Users()
        {
            return View(AccountCRUD.GetAllUsers());
        }

        public ActionResult UserDetail(int id)
        {
            return PartialView("_UserDetail", AccountCRUD.GetAccount(id));
        }

        public ActionResult ChangeAccess(int id)
        {
            return PartialView("_changeAccess", id);
        }

        [HttpPost]
        public ActionResult UpdateUserPriviledges(FormCollection collection)
        {
            int id = Int32.Parse(collection["UserID"]);
            int value = Int32.Parse(collection["AccessStatus"]);

            if (AccountCRUD.ChangePriviledges(id, value == 1 ? "Admin" : "User"))
                return Content("<script>alert('User Status has been Updated Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('User Status could not be Updated.');window.location.href=document.referrer</script>");
        }

        public ActionResult RemoveUsers(int id)
        {

            List<Order> orders = OrderCRUD.GetUserOrders(id);

            //if any order exists that hasnt been delievered then cannot delete account
            if (orders.FindIndex(item => item.OrderStatus != "Delivered") < 0)
            {
                AccountCRUD.RemoveUser(id);
                return Redirect("Console");
            }
            else
                return Content("<script>alert('This User still has pending Orders.');window.location.href=document.referrer</script>");


        }

        // GET: All Books
        public ActionResult Books()
        {
            return View(BookCRUD.GetAllBooks());
        }

        public ActionResult BookDetails(int id)
        {
            return View(BookCRUD.GetBookReviews(id, 0));
        }

        public ActionResult EditBook(int id)
        {
            Book mybook = BookCRUD.GetBook(id);
            return PartialView("_EditBook", mybook);
        }

        [HttpPost]
        public ActionResult UpdateBook(FormCollection collection)
        {
            Book newBook = new Book();

            newBook.BookID = Int32.Parse(collection["BookID"]);
            newBook.Title = collection["Title"];
            newBook.Synopsis = collection["Synopsis"];
            newBook.Publisher = collection["Publisher"];
            newBook.Category = collection["Category"];
            newBook.Price = Int32.Parse(collection["Price"]);
            newBook.Stock = Int32.Parse(collection["Stock"]);
            newBook.Discount = (double.Parse(collection["Discount"])) / 100;
            newBook.SubStatus = Convert.ToBoolean(collection["SubStatus"]);
            newBook.Authors = collection["Authors"].Split(',').ToList();
            newBook.Genres = collection["Genres"].Split(',').ToList();


            if (BookCRUD.UpdateBook(newBook))
                return Content("<script>alert('Book has been Updated Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Book could not be Update.');window.location.href=document.referrer</script>");
        }

        [HttpPost]
        public ActionResult AddBook(FormCollection collection)
        {
            Book newBook = new Book();

            newBook.Title = collection["Title"];
            newBook.Synopsis = collection["Synopsis"];
            newBook.Publisher = collection["Publisher"];
            newBook.Category = collection["Category"];
            newBook.Price = Int32.Parse(collection["Price"]);
            newBook.Stock = Int32.Parse(collection["Stock"]);
            newBook.SubStatus = Convert.ToBoolean(collection["SubStatus"]);
            newBook.Authors = collection["Authors"].Split(',').ToList();
            newBook.Genres = collection["Genres"].Split(',').ToList();


            if (BookCRUD.CreateBook(newBook))
                return Content("<script>alert('Book has been added Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Book could not be added.');window.location.href=document.referrer</script>");
        }

        public ActionResult RemoveBook(int id)
        {
            if (BookCRUD.DeleteBook(id))
                return Content("<script>alert('Book Deleted Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Book could not be found.');window.location.href=document.referrer</script>");
        }

        [HttpPost]
        public ActionResult ReviewBook(FormCollection collection)
        {
            Review newReview = new Review();
            newReview.UserID = (int)Session["UserID"];
            newReview.UserName = (string)Session["UserName"];
            newReview.BookID = Int32.Parse(collection["BookID"]);
            newReview.Description = collection["ReviewText"];
            newReview.Rating = Int32.Parse(collection["Rating"]);

            if (ReviewCRUD.CreateReview(newReview))
                return Content("<script>alert('Review has been added Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Review Failed.');window.location.href=document.referrer;</script>");
        }

        public ActionResult Order()
        {
            List<Order> AllOrders = OrderCRUD.GetAllOrders();
            return View(AllOrders);
        }

        public ActionResult RemoveOrder(int id)
        {
            if (OrderCRUD.DeleteOrder(id))
                return Content("<script>alert('Order Deleted Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Order could not be found.');window.location.href=document.referrer</script>");
        }

        public ActionResult OrderStatus(int id)
        {
            return PartialView("_OrderStatus", id);
        }

        [HttpPost]
        public ActionResult UpdateStatus(FormCollection collection)
        {
            int oid = Int32.Parse(collection["OrderID"]);
            string status = collection["OrderStatus"];

            if (OrderCRUD.UpdateStatus(oid, status))
                return Content("<script>alert('Order Status has been Updated Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Order Status could not be Updated.');window.location.href=document.referrer</script>");
        }

        public ActionResult OrderDetails(int id)
        {
            return PartialView("_OrderDetails", OrderCRUD.GetOrderItems(id));
        }

        public ActionResult Requests()
        {
            List<Request> AllRequests = RequestCRUD.GetRequest((int)Session["UserID"]); //pass admin id to get all requests
            AllRequests.RemoveAll(item => item.RequestStatus != "Resolved");
            return View(AllRequests);
        }

        public ActionResult RequestStatus(int id)
        {
            return PartialView("_RequestStatus", id);
        }

        [HttpPost]
        public ActionResult UpdateRStatus(FormCollection collection)
        {
            int rid = Int32.Parse(collection["RequestID"]);
            string status = collection["RequestStatus"];

            if (RequestCRUD.UpdateRequest(rid, status))
                return Content("<script>alert('Request Status has been Updated Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Request Status could not be Updated.');window.location.href=document.referrer</script>");
        }

        public ActionResult BookSubscriptions(int id)
        {
            return View(SubscriptionCRUD.GetSubscribers(id));
        }

        public ActionResult UnSubscribe(int id)
        {
            if (SubscriptionCRUD.UnSubscribe(id, (int)Session["UserID"]))
                return Content("<script>alert('Unsubscribed Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Operation Failed.');window.location.href=document.referrer;</script>");
        }

        //Admin Account related methods
        public ActionResult ProfileInfo()
        {
            return View(AccountCRUD.GetAccount((int)Session["UserID"]));
        }

        public ActionResult PasswordChange()
        {
            return PartialView("_PasswordChange");
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection collection)
        {
            //int id = 1;
            string newPass = collection["Password"];

            if (AccountCRUD.ChangePassword((int)Session["UserID"], newPass))
                return Content("<script>alert('Password Changed Successfully Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Password could not be Changed');window.location.href=document.referrer</script>");
        }

        public ActionResult EditInfo(int id)
        {
            return PartialView("_EditInfo", AccountCRUD.GetAccount(id));
        }

        [HttpPost]
        public ActionResult ChangeInfo(FormCollection collection)
        {
            Account myacc = new Account();
            myacc.UserID = Int32.Parse(collection["UserID"]);
            myacc.Email = collection["Email"];
            myacc.Username = collection["Username"];
            myacc.ContactNo = collection["ContactNo"];
            myacc.Address = collection["Address"];
            myacc.AccStatus = (string)Session["Priviledges"];
            myacc.Gender = Convert.ToChar(collection["Gender"]);
            if (AccountCRUD.UpdateUser(myacc))
                return Content("<script>alert('Profile Edited Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Profile Could not be Updated');window.location.href=document.referrer</script>");
        }

    }
}