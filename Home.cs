using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;

namespace gtdpad
{
    public class Home : NancyModule
    {
        public Home()
        {
            var db = new Repository();

            Get("/", args => {
                return "HOME";
            });

            Get("/secured", args => {
                this.RequiresAuthentication();
                return "SECURE";
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