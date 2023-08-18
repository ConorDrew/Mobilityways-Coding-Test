### Code test breakdown.

I would like to start off by saying that unfortunately due to how quick a turn around the interview process was, I was not able to spend long on this code test which I feel is a shame as it's a fun little project to get stuck into. Due to leaving for holiday early on the 19^th^ Aug, along with work and the fun of packing and sorting stuff before going away it was very minimal time from start to finish, but in this breakdown I would like to try to put across what I would have done differently / ways to improve this given more time.

The first thing to touch on is the performance, because this is a minimal API this performs better than a traditional controller API, as long as the end points are light weight this will have no performance issues with scaling either.

For testability, no tests have been added to this project, however due to how the endpoints have been lifted away and then in the same folder a static method, this can then be used with tests as the UserRepository is now an interface.

Reusability and Readability are hopefully covered here, in this example I have created a folder called Endpoints which is a static void passing in (this IEndpointRouteBuilderApp) this way the end points can be written as normal with App.MapGet(), but when this file needs to be created in the program.cs , you simple call app.AddUserEndpoints(); when creating more endpoint locations such as Order, Stock, routes etc. you can easily keep them separated and very clean. This can also be improved as the repo is registered as a singleton, all of this could be created into an extension method, so you would just call AddUserEndpoints, this would register the service and add the endpoints, which cleans it all up even more.

The next two points of Maintainability and Scalability should be covered here, if this was going to be a larger REST API, then I would have gone down the route of Clean architecture along with implementing CQS , the clean architecture would break the areas out into easy to read and build upon this when adding new sections, it wouldn't all be put into folders which everything has access too. CQS would also be a vast improvement for reading out of the repo, giving each command or queries a simple task to run, and then return that data.

Running this you can see the it fires up Swagger, the first endpoint to run would be to create a user, this is a simple endpoint which takes in the basic user information, it first checks to see if a user already exists, and returns the correct message if they do, if not, it will run a simple hash and salt to make the password secure and then return the GUID for the system to then do what it needs to.

Next is getting the JWT token, this takes the email and password and does a couple of quick checks to see if there is a user, and if the password matches by running it though the Hash and Salt again.\
once the user has been found this will return a JWT token to be stored.

The last part is returning a list of users, this is hidden behind an authorisation endpoint using the .RequiresAuthorization() however this was only used this way as most of the endpoints don't require authentication, but if they would I would have switched up the program.cs to make the fallback default require all endpoints require authentication, and then use the method .AllowAnnoymous() on the other endpoints mentioned before.

The token is being generated however its still giving a 401 in this demo, with a bit more time this would be working correctly, but unfortunately I have probably missed a little bit of code which would attach it correctly.

The in memory storage is using a simple List<User> this is because its quick to sort though and easy to pass the data around, another route I almost took was using a dictionary to make the GUIDS searchable, but for this simple project the List was simple enough.

The program is using a few Models, which would be broken out differently if using clean architecture, the User.cs would be the domain model, the others would be DTO's, along with this, the User.cs has the Guid as a property, but this would be pulled out as a base class that all entities would have.\
The Service response class is a simple wrapper to pass the data back from the Repo. It allows a generic type of data to come down, along with a success flag and a message, the message will be empty if everything has gone okay, but if there is an issue, this returns a handy comment to return back from the API.

The Repository is a very simple class where each method handles what it needs to, however if this was expanded to clean architecture then I would have added a base entity and an IRepository, wit simple methods such as Add, update, delete, getall, and then if this gets built upon having some safety messaures in place, as before we have dealt with an issue of children being deleted because of an update, this was more an EF, DDD issue, however a simple check added at this level then fixed it from doing a cascading deletion and removing wrongful orphans.  

Once again, I would like to apologise for not completing the task to my ability which I fully understand this task is to give better understanding of my technical knowledge, and I am hoping with this very long explanation, you get a little more insights  that I understand what this project is, and how this can be improved.

Thanks\
Conor
