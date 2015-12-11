TomiSoft WebServer
==================
A simple webserver to control your application right in the browser.

How it works
------------
The webserver requires classes called controllers. In the controller classes
you will need action methods. These actions will be called when the user calls
them in the browser. For example:
```
http://localhost/demo/index
```
will call the Index action method in the Demo class. Controller classes have to
be marked with WebController attribute, and action methods have to be marked with
WebAction attribute.

Simple tutorial
---------------
Create controller classes with action methods in them:
```
using TomiSoft.Web.HttpServer;
using System.Collections.Generic;

[WebController]
public class Demo {
  //To communicate with the browser, the webserver will pass a WebClient
  //instance to the constructor
  private WebClient client;
  
  public Demo(WebClient c) {
    this.client = c;
  }

  //This is an action method that can be called in the browser
  [WebAction]
  public void Index(Dictionary<string, string> Parameters) {
    //Tip: GET parameters are stored in Parameters. If there are no GET parameters,
    //this Dictionary will be empty, but it won't be null.
    
    //A simple way to communicate with your application is to use the static
    //WebServer.Parameters[]
    //BE CAREFUL! It uses the dynamic type.
  
    //You need to create the response header
    HttpHeader h = new HttpHeader(HttpStatus.Ok, ProtocolVersion.Http1_1);
    //Tell the browser what type of content will we send
    h.SetParameter("Content-Type", "text/html; charset=utf-8");
    
    //Send the header and the content to the browser.
    //You can send a string, a binary data or a System.IO.Stream.
    //The Content-Length header will be added automatically.
    this.client.Send(h, "<h1>It works!</h1>");
  }
}
```

Start the webserver. It will create it's own thread, so it won't interrupt the execution of your code.
```
using TomiSoft.Web.HttpServer;
using System.Reflection;

class Program {
  public static void Main(string[] args) {
    //You need to define a default controller with a default action in it.
    //Any calls to http://localhost will be redirected to this action.
    string DefaultController = "demo";
    string DefaultAction = "index";
    
    //You also need to tell the assembly where the controllers are:
    Assembly ProgramAssembly = typeof(Demo).Assembly;
    
    //It's launch time. Server will be listening on port 80.
    WebServer ws = new WebServer(ProgramAssembly, DefaultController, DefaultAction);
    
    //You may specify a different port:
    //int Port = 25414;
    //new WebServer(ProgramAssembly, DefaultController, DefaultAction, Port);
  }
}
```

Now launch a browser and navigate to http://localhost.
You will be redirected to http://localhost/demo/index and you can see the result.
