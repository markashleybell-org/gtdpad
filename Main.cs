using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;

namespace gtdpad
{
    public class Main : NancyModule
    {
        public Main()
        {
            var db = new Repository();

            Get("/", args => {
                this.RequiresAuthentication();
                return View["index.html"];
            });

            Get("/login", args => {
                return View["login.html"];
            });

            Post("/login", args => {
                var id = db.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
                return this.LoginAndRedirect(id.Value, null);
            });

            Get("/logout", args => {
                return this.LogoutAndRedirect("~/");
            });
        }
    }
}