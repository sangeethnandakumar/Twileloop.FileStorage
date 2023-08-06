<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/sangeethnandakumar/Twileloop.UOW">
    <img src="https://iili.io/HPIj6ss.png" alt="Logo" width="80" height="80">
  </a>

  <h1 align="center"> Twileloop.UOW</h1>
  <h4 align="center"> LiteDB | MongoDB </h4>
</div>

## About
A lightweight and ready-made implementation of unit of work pattern + NoSQL database. 

Twileloop.UOW is a package that ships a plug and play model predefined repository, unit of work pattern on top of 2 popular NoSQL databases.
There are 2 varients of Twileloop.UOW for LiteDB and MongoDB support

## License
> Twileloop.UOW.LiteDB & Twileloop.UOW.MongoDB - are licensed under the MIT License. See the LICENSE file for more details.

#### This library is absolutely free. If it gives you a smile, A small coffee would be a great way to support my work. Thank you for considering it!
[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/sangeethnanda)

## Usage
***To get started, You have to select which package to install:***

- If you prefer to use file-based database, Install Twileloop.UOW with LiteDB support. Install `Twileloop.UOW.LiteDB` package
- If you prefer to use a centrally deployed MongoDB database, Install Twileloop.UOW with MongoDB support. Install `Twileloop.UOW.MongoDB` package

<hr/>


## 2. Install Package

> Choose the installation that suites your need

| Driver | To Use | Install Package   
| :---: | :---:   | :---:
| <img src="https://iili.io/HPIj6ss.png" alt="Logo" height="30"> | LiteDB | `dotnet add package Twileloop.UOW.LiteDB`  
| <img src="https://iili.io/HPIj6ss.png" alt="Logo" height="30"> | MongoDB | `dotnet add package Twileloop.UOW.MongoDB`  

### Supported Features

| Feature     | LiteDB | MongoDB
| ---      | ---       | ---
| Create | ✅ | ✅
| Read | ✅ | ✅
| Update | ✅ | ✅
| Delete | ✅ | ✅
| Full Repository Access | ✅ | ✅
| Multiple Databases | ✅ | ✅
| Database Level Transactions | ✅ | ❌


✅ - Available &nbsp;&nbsp;&nbsp; 
🚧 - Work In Progress &nbsp;&nbsp;&nbsp; 
❌ - Not Available


## 1. Register all databases (ASP.NET dependency injection)
```csharp
//LiteDB
builder.Services.AddUnitOfWork((uow) => {
    uow.Connections = new List<LiteDBConnection>
    {
        new LiteDBConnection("DatabaseA", "Filename=DatabaseA.db; Mode=Shared; Password=****;"),
        new LiteDBConnection("DatabaseB", "Filename=DatabaseB.db; Mode=Shared; Password=****;")
    };
});

//MongoDB
builder.Services.AddUnitOfWork((uow) => {
    uow.Connections = new List<MongoDBConnection>
    {
        new MongoDBConnection("DatabaseA", "mongodb+srv://Uername:****@Cluster"),
        new MongoDBConnection("DatabaseB", "mongodb+srv://Uername:****@Cluster")
    };
});
```

## 2. For Non Dependency Injection Setup (Like Console apps)
```csharp
//LiteDB
var context = LiteDB.Support.Extensions.BuildDbContext(option =>
    {
        option.Connections = new List<LiteDBConnection>
        {
            new LiteDBConnection("DatabaseA", "Filename=DatabaseA.db; Mode=Shared; Password=****;"),
            new LiteDBConnection("DatabaseB", "Filename=DatabaseB.db; Mode=Shared; Password=****;")
        };
    });
var uow = new LiteDB.Core.UnitOfWork(context);

//MongoDB
var context = MongoDB.Support.Extensions.BuildDbContext(option =>
    {
        option.Connections = new List<MongoDBConnection>
        {
            new MongoDBConnection("DatabaseA", "mongodb+srv://Username:****@Cluster"),
            new MongoDBConnection("DatabaseB", "mongodb+srv://Username:****@Cluster")
        };
    });
var uow = new MongoDB.Core.UnitOfWork(context);
```

### PLEASE NOTE
❌ - BSON Serialization will work only on serializable properties. Objects like `DataTable` etc.. are non-generic which can't be stored as in DB

## 3. Inject and Use as required
```csharp
    [ApiController]
    public class HomeController : ControllerBase 
    {
        private readonly UnitOfWork uow;

        public HomeController(UnitOfWork uow)
        {
            this.uow = uow;
        }

        [HttpGet]
        public IActionResult Get() 
        {            
            try
            {
                // Step 1: Point to a database
                uow.UseDatabase("<DB_NAME>");

                //Step 2: Get a repository for your model 'Dogs'
                var dogRepo = uow.GetRepository<Dogs>();

                //Step 3: Do some fetch
                allDogs = dogRepo.GetAll().ToList();

                //Step 4: Or any CRUD operations you like
                uow.BeginTransaction();
                dogRepo.Add(new Dog());
                uow.Commit();

                return Ok(allDogs);
            }
            catch(Exception)
            {
                uow.Rollback();
            }            
        }

    }
```
