using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
namespace DNWS
{
    class TwitterApiPlugin : TwitterPlugin, IPlugin
    {
        private string action;
        private string method;
        
        public List<User> GetUsers()
        {
            using (var context = new TweetContext())
            {
                try
                {
                    List<User> userlist = context.Users.Where(b => true).Include(b => b.Following).ToList();
                    return userlist;
                }
                catch (Exception)
                {
                    return null;
                }
            } 
           
        }
        
        public List<Following> GetFollowings(string name)
        {
            using (var context = new TweetContext())
            {
                try
                {
                    List<User> Followlist = context.Users.Where(b => b.Name.Equals(name)).Include(b => b.Following).ToList();
                    return Followlist[0].Following;
                }
                catch (Exception)
                {
                    return null;
                }
            }


        }

        public new HTTPResponse GetResponse(HTTPRequest request)
        {
            HTTPResponse response = new HTTPResponse(200);
            string user = request.getRequestByKey("user");
            string password = request.getRequestByKey("Password");
            string following = request.getRequestByKey("follow");
            string message = request.getRequestByKey("message");
            StringBuilder sb = new StringBuilder();
            string[] url = request.Filename.Split("?");
            if (url[0] == "user")
            {
                if(request.Method == "GET")//get list's user
                {
                    string Json = JsonConvert.SerializeObject(GetUsers());
                    response.body = Encoding.UTF8.GetBytes(Json);
                }
                else if(request.Method == "POST") //create new user
                {
                    try { 
                        Twitter.AddUser(user, password);
                    }
                    catch (Exception)
                    {
                        sb.Append("<h1>Error</h1>");
                    }
         
                }
                else if(request.Method == "Delete") //remove user
                {
                    try
                    {
                        Twitter.DeleteUser(user,password);
                    }
                    catch (Exception)
                    {
                        sb.Append("<h1>Error</h1>");
                    }

                }
            }
            else if(url[0]=="Follow")
            {
                Twitter twitter = new Twitter(user);
                if(request.Method == "GET")
                {
                   string Json = JsonConvert.SerializeObject(GetFollowings(user));
                    response.body = Encoding.UTF8.GetBytes(Json);
                }
                else if(request.Method == "POST")
                {
                    try
                    {
                        twitter.AddFollowing(following);
                    }
                    catch (Exception)
                    {
                        response.status = 404;
                        response.body = Encoding.UTF8.GetBytes("404");
                    }
                }
                else if (request.Method =="DELETE")
                {
                    try
                    {
                        twitter.RemoveFollowing(following);
                    }
                    catch (Exception)
                    {

                    }
                }

            }
            else if (url[0] == "tweet")
            {
                Twitter twitter = new Twitter(user);
                if (request.Method == "GET")
                {
                    try
                    {
                        string timeline = request.getRequestByKey("timeline");
                        if (timeline == "follow")
                        {
                            string Json = JsonConvert.SerializeObject(twitter.GetFollowingTimeline());
                            response.body = Encoding.UTF8.GetBytes(Json);
                        }
                        else
                        {
                            string Json = JsonConvert.SerializeObject(twitter.GetUserTimeline());
                            response.body = Encoding.UTF8.GetBytes(Json);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else if(request.Method == "POST")
                {
                    try
                    {
                        twitter.PostTweet(message);
                        response.body = Encoding.UTF8.GetBytes("200 OK");
                    }
                    catch (Exception)
                    {

                    }
                }
            }


            response.type = "application/json";
            return response;
        }
        
    }
}
