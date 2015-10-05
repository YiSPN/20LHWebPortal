using System;
using System.Collections.Generic;
using System.Web;

namespace _20LHWebPortal.Models
{
	public class UserGravatarModel
	{
        public string GetGravatarUrl(object key) {
            // string email = (string)DataBinder.Eval(key, "email");
            string email = "mllwnf@gmail.com";
            string hash = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(email.Trim(), "MD5");
            hash = hash.Trim().ToLower();

            return string.Format("http://www.gravatar.com/avatar.php?gravatar_id={0}&rating=G&size=60", hash);
        }
    }
}
