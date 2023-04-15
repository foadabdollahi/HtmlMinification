# HTML Minification middleware

HTML minification middleware is a process of removing unnecessary or redundant data from HTML without affecting how the resource is processed by the browser. This can include removing code comments and formatting, removing unused code, using shorter variable and function names, and so on. Minifying HTML can help reduce the size of web pages, resulting in faster page load times and improved user experience.


#### ASP.NET Core 3.x ~ 7.x 

Remove All whiteSpace from rendered html

*New update: Fixed Image 500 error.*



####  Usage:
in Startup.cs Or Program.cs 




`app.UseHtmlMinification(); // <-- call it before  StaticFiles middleware
`
`
app.UseStaticFiles();
`


