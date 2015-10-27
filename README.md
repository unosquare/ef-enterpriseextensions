# ef-enterpriseextensions
[![Build status](https://ci.appveyor.com/api/projects/status/6e5vk7s69ur34nd0?svg=true)](https://ci.appveyor.com/project/geoperez/ef-enterpriseextensions)

Unosquare Labs EntityFramework.EnterpriseExtensions Library contains a set of useful helpers.

NuGet Installation:
-------------------

[![NuGet version](https://badge.fury.io/nu/Unosquare.EntityFramework.EnterpriseExtensions.svg)](https://badge.fury.io/nu/Unosquare.EntityFramework.EnterpriseExtensions)
```
PM> Install-Package Unosquare.EntityFramework.EnterpriseExtensions
```

or

[![NuGet version](https://badge.fury.io/nu/Unosquare.Identity.EntityFramework.EnterpriseExtensions.svg)](https://badge.fury.io/nu/Unosquare.Identity.EntityFramework.EnterpriseExtensions)
```
PM> Install-Package Unosquare.Identity.EntityFramework.EnterpriseExtensions
```

if you are using Identity Entity Framework.

Usage
---

First you need to change your `DbContext` or `IdentityDbContext` to `BusinessDbContext` or `IdentityBusinessDbContext` respectly and you can attach Business Controllers to your DbContext in the constructor. They will execute before anytime you call `SaveChanges` method. The controllers require to specified a `BusinessRuleAttribute` to
identify what CRUD action and which Entity types will be processed.

EF Enterprise Extensions includes a `JobBase` abstract class (with a singleton extension named `SingletonJobBase`), so you can build business jobs easily. Check the Sample app.
